﻿using Core;
using Core.Interfaces.Accessors;
using Core.Interfaces.Engines;
using Engines.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Engines
{
    public class UserEngine : IUserEngine
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IStudentAccessor _studentAccessor;
        private readonly RNGCryptoServiceProvider _cryptoServiceProvider;

        //used for the generation of salt for password hashing
        private readonly int _saltLength = 20;

        public UserEngine(IUserAccessor userAccessor, 
            IStudentAccessor studentAccessor,
            RNGCryptoServiceProvider cryptoServiceProvider)
        {
            _userAccessor = userAccessor;
            _studentAccessor = studentAccessor;
            _cryptoServiceProvider = cryptoServiceProvider;
        }

        public void DeletePersonalInfo(int userID)
        {
            EngineArgumentValidation.ArgumentIsNonNegative(userID, "UserID");
            _userAccessor.DeletePersonalInfo(userID);
        }

        public bool EmailExists(string email)
        {
            EngineArgumentValidation.StringIsNotEmpty(email, "Email");
            return _userAccessor.EmailExists(email);
        }

        public IEnumerable<User> GetAllUsers()
        {
            var users = _userAccessor.GetAllActiveUsers();
            var students = _studentAccessor.GetAllStudents();

            foreach (User user in users)
            {
                user.Students = students.Where(x => x.UserID == user.UserID);
            }

            return users;
        }

        public User GetUserInfoByID(int userID)
        {
            EngineArgumentValidation.ArgumentIsNonNegative(userID, "UserID");

            var user = _userAccessor.GetUserInfoByID(userID);
            if (user != null)
            {
                user.Students = _studentAccessor.GetStudentInfoByUserID(userID);
            }
            return user;
        }

        public void InsertPersonalInfo(User user, string password)
        {
            EngineArgumentValidation.ArgumentIsNotNull(user, "User");
            EngineArgumentValidation.StringIsNotEmpty(password, "Password");

            var randomBytes = new byte[_saltLength];
            _cryptoServiceProvider.GetBytes(randomBytes);
            user.Salt = Convert.ToBase64String(randomBytes);

            user.Hashed = PasswordUtils.HashPasswords(password, user.Salt);

            _userAccessor.InsertPersonalInfo(user);
        }

        public void UpdatePersonalInfo(User user)
        {
            EngineArgumentValidation.ArgumentIsNotNull(user, "User");
            _userAccessor.UpdatePersonalInfo(user);
        }

        public User ValidateLoginInfo(string email, string password)
        {
            EngineArgumentValidation.StringIsNotEmpty(email, "Email");
            EngineArgumentValidation.StringIsNotEmpty(password, "Password");

            var user = _userAccessor.GetUserInfoByEmail(email);
            if (user == null)
            {
                //if no user with that email is found
                return null;
            }

            string hashedGivenPassword = PasswordUtils.HashPasswords(password, user.Salt);
            if (hashedGivenPassword.Equals(user.Hashed))
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
