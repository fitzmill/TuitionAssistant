﻿using Core.DTOs;
using Core.Interfaces.Engines;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace Web.Controllers
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        private readonly IUserEngine _userEngine;

        public LoginController(IUserEngine userEngine)
        {
            _userEngine = userEngine;
        }

        [HttpGet]
        [Route("ValidateLoginInfo")]
        [AllowAnonymous]
        public IHttpActionResult ValidateLoginInfo()
        {
            AuthenticationHeaderValue authenticationHeader = Request.Headers.Authorization;

            if (authenticationHeader == null || !authenticationHeader.Scheme.StartsWith("Basic"))
            {
                return BadRequest();
            }

            var encodedUsernamePassword = authenticationHeader.Parameter;
            var encoding = Encoding.GetEncoding("iso-8859-1");
            string emailPassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            var seperatorIndex = emailPassword.IndexOf(':');

            string email = emailPassword.Substring(0, seperatorIndex);
            string password = emailPassword.Substring(seperatorIndex + 1);

            var user = _userEngine.ValidateLoginInfo(email, password);
            if (user != null)
            {
                return Ok(new LoginDTO()
                {
                    JwtToken = JwtManager.GenerateToken(user),
                    UserType = user.UserType
                });

            }

            return Ok();
        }
    }
}
