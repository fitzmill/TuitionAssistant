﻿using Core;
using Core.DTOs;
using Core.Interfaces.Engines;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using Web.Filters;

namespace Web.Controllers
{
    /// <summary>
    /// Controller for get/set operations relating to users' information.
    /// </summary>
    [RoutePrefix("api/userinfo")]
    [SqlRowNotAffectedFilter]
    public class UserInfoController : ApiController
    {
        private readonly IUserEngine _userEngine;
        private readonly IStudentEngine _studentEngine;
        private readonly IPaymentEngine _paymentEngine;
        private readonly INotificationEngine _notificationEngine;

        public UserInfoController(IUserEngine userEngine,
            IStudentEngine studentEngine,
            IPaymentEngine paymentEngine,
            INotificationEngine notificationEngine)
        {
            _userEngine = userEngine;
            _studentEngine = studentEngine;
            _paymentEngine = paymentEngine;
            _notificationEngine = notificationEngine;
        }

        /// <summary>
        /// Gets a user from the database.
        /// </summary>
        /// <returns>the authenticated user</returns>
        [HttpGet]
        [Route("GetUserInfo")]
        [JwtAuthentication]
        public IHttpActionResult GetUserInfo()
        {
            var user = (ClaimsIdentity) User.Identity;
            string userID = ControllerUtils.httpGetUserID(user);
            if (userID == null || !int.TryParse(userID, out int parsedUserID))
            {
                return Unauthorized();
            }

            return Ok(_userEngine.GetUserInfoByID(parsedUserID));
        }

        /// <summary>
        /// Checks whether the provided email is already associated with an active account.
        /// </summary>
        /// <param name="email">the email</param>
        /// <returns>a boolean, whether the email already exists</returns>
        [HttpPost]
        [Route("EmailExists")]
        [AllowAnonymous]
        public IHttpActionResult EmailExists([FromBody] string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return BadRequest("Email was not supplied");
            }

            return Ok(_userEngine.EmailExists(email));
        }

        /// <summary>
        /// Updates the user's information in the database.
        /// </summary>
        /// <param name="user">the user</param>
        /// <returns>the updated user token</returns>
        [HttpPost]
        [Route("UpdatePersonalInfo")]
        [JwtAuthentication]
        public IHttpActionResult UpdatePersonalInfo(User user)
        {
            if (user == null || !ModelState.IsValid)
            {
                return BadRequest("One or more required objects was not included in the request body.");
            }

            _userEngine.UpdatePersonalInfo(user);

            var newToken = JwtManager.GenerateToken(user);

            var claimsIdentity = (ClaimsIdentity) User.Identity;
            _notificationEngine.SendAccountUpdateNotification(user.Email, user.FirstName, "personal");

            return Ok(newToken);
        }

        /// <summary>
        /// Updates the students for a user in the database.
        /// </summary>
        /// <param name="updatedInfo">the student update information</param>
        /// <returns>a success response</returns>
        [HttpPost]
        [Route("UpdateStudentInfo")]
        [JwtAuthentication]
        public IHttpActionResult UpdateStudentInfo(UpdateStudentInfoDTO updatedInfo)
        {
            var user = (ClaimsIdentity)User.Identity;
            string userID = ControllerUtils.httpGetUserID(user);
            if (updatedInfo == null || !ModelState.IsValid || userID == null || !int.TryParse(userID, out int parsedUserID))
            {
                return BadRequest("One or more required objects was not included in the request body.");
            }

            _studentEngine.UpdateStudentInfo(updatedInfo.UpdatedStudents);
            _studentEngine.InsertStudentInfo(parsedUserID, updatedInfo.AddedStudents);
            _studentEngine.DeleteStudentInfo(updatedInfo.DeletedStudentIDs);

            _notificationEngine.SendAccountUpdateNotification(ControllerUtils.httpGetEmail(user), user.Name, "student");
            return Ok();
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="user">the user to be deleted</param>
        /// <returns>a success response</returns>
        [HttpPost]
        [Route("DeleteUser")]
        [JwtAuthentication]
        public IHttpActionResult DeleteUser(User user)
        {
            if (user == null || !ModelState.IsValid)
            {
                return BadRequest("One or more required objects was not included in the request body");
            }

            _paymentEngine.DeletePaymentInfo(user.CustomerID);
            _studentEngine.DeleteStudentInfo(user.Students.Select(x => x.StudentID));
            _userEngine.DeletePersonalInfo(user.UserID);
            _notificationEngine.SendAccountDeletionNotification(user);

            return Ok();
        }

        /// <summary>
        /// Inserts a new user into the database and PaymentSpring.
        /// </summary>
        /// <param name="accountCreationInfo">the new user info</param>
        /// <returns>the new user token</returns>
        [HttpPost]
        [Route("InsertUser")]
        [AllowAnonymous]
        public IHttpActionResult InsertUser([FromBody] AccountCreationDTO accountCreationInfo)
        {
            if (accountCreationInfo == null || !ModelState.IsValid)
            {
                return BadRequest("One or more required objects was not included in the request body.");
            }

            UserPaymentInfoDTO paymentInfo = CreatePaymentInfo(accountCreationInfo);
            string customerID = _paymentEngine.InsertPaymentInfo(paymentInfo);

            //check if payment info is valid, if not return bad request
            if (customerID == null)
            {
                return BadRequest("Payment information is invalid.");
            }

            User user = CreateUser(accountCreationInfo, customerID);

            _userEngine.InsertPersonalInfo(user, accountCreationInfo.Password);
            _studentEngine.InsertStudentInfo(user.UserID, user.Students);
            _notificationEngine.SendAccountCreationNotification(user, DateTime.Today);

            var token = JwtManager.GenerateToken(user);

            return Ok(token);
        }

        /// <summary>
        /// Helper method for creating a UserPaymentInfoDTO from an AccountCreationDTO.
        /// </summary>
        /// <param name="accountCreationInfo">the AccountCreationDTO</param>
        /// <returns>a UserPaymentInfoDTO</returns>
        private static UserPaymentInfoDTO CreatePaymentInfo(AccountCreationDTO accountCreationInfo)
        {
            return new UserPaymentInfoDTO
            {
                CustomerID = "", //This isn't neccessary for the creation of a customer in payment spring
                FirstName = accountCreationInfo.CardholderFirstName,
                LastName = accountCreationInfo.CardholderLastName,
                StreetAddress1 = accountCreationInfo.StreetAddress1,
                StreetAddress2 = accountCreationInfo.StreetAddress2,
                City = accountCreationInfo.City,
                State = accountCreationInfo.State,
                Zip = accountCreationInfo.Zip,
                CardNumber = accountCreationInfo.CardNumber,
                ExpirationYear = accountCreationInfo.ExpirationYear,
                ExpirationMonth = accountCreationInfo.ExpirationMonth,
                CardType = "" //This also isn't neccessary for the creation of a customer in payment spring
            };
        }

        /// <summary>
        /// Helper method for creating a User from an AccountCreationDTO and customer ID.
        /// </summary>
        /// <param name="accountCreationInfo">the AccountCreationDTO</param>
        /// <param name="customerID">the PaymentSpring customer ID</param>
        /// <returns>a user</returns>
        private static User CreateUser(AccountCreationDTO accountCreationInfo, string customerID)
        {
            return new User
            {
                UserID = 0,
                FirstName = accountCreationInfo.FirstName,
                LastName = accountCreationInfo.LastName,
                Email = accountCreationInfo.Email,
                Hashed = "", //This should be set when the account is created
                Salt = "", //This should also be set when the account is created
                Plan = accountCreationInfo.Plan,
                UserType = accountCreationInfo.UserType,
                CustomerID = customerID,
                Students = accountCreationInfo.Students
            };
        }

    }
}