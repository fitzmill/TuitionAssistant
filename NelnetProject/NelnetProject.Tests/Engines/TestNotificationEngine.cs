﻿using Core;
using Core.Models;
using Engines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NelnetProject.Tests.Engines.MockedAccessors;
using System;
using System.Collections.Generic;

namespace NelnetProject.Tests.Engines
{
    [TestClass]
    public class TestNotificationEngine
    {
        private readonly NotificationEngine _notificationEngine;
        private readonly MockEmailAccessor _mockEmailAccessor;
        private readonly MockUserAccessor _mockUserAccessor;

        private List<User> userDB = new List<User>()
        {
            new User()
            {
                UserID = 1,
                FirstName = "Bob",
                LastName = "Bobbort",
                Email = "bob@email.com"
            },
            new User()
            {
                UserID = 2,
                FirstName = "Jimmeh",
                LastName = "Jim",
                Email = "jimmmmmms@eh.jim"
            }
        };

        public TestNotificationEngine()
        {
            _mockEmailAccessor = new MockEmailAccessor();
            _mockUserAccessor = new MockUserAccessor(userDB);
            _notificationEngine = new NotificationEngine(_mockEmailAccessor, _mockUserAccessor);
        }

        [TestMethod]
        public void TestSendEmailNotifications()
        {
            List<EmailNotification> expectedEmails = new List<EmailNotification>()
            {
                new EmailNotification()
                {
                    To = "bob@email.com",
                    Subject = "Alert from Tuition Assistant: Upcoming Payment",
                    Body = "Hi Bob,<br><br>You have an upcoming payment.<br><br>Date: May 5 2018<br>Amount: $37.34<br><br>You don't need to worry about anything. " +
                    "We'll charge your card automatically on this date.<br>Please contact us if you have any questions.<br><br><br>Powered by Tuition Assistant<br>"
                },
                new EmailNotification()
                {
                    To = "jimmmmmms@eh.jim",
                    Subject = "Alert from Tuition Assistant: Payment Failed",
                    Body = "Hi Jimmeh,<br><br>Your payment of $234.12 that was due on May 5 2018 failed for 7 days and has been deferred." +
                    "<br><br>The amount will be added to your next payment, along with a late fee of $25.75.<br>Please contact us if you have any questions." +
                    "<br><br><br>Powered by Tuition Assistant<br>"
                }
            };

            List<Transaction> transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    AmountCharged = 37.34,
                    UserID = 1,
                    DateDue = new DateTime(2018, 5, 5),
                    ProcessState = ProcessState.NOT_YET_CHARGED
                },
                new Transaction()
                {
                    AmountCharged = 234.12,
                    UserID = 2,
                    DateDue = new DateTime(2018, 5, 5),
                    DateCharged = new DateTime(2018, 5, 12),
                    ProcessState = ProcessState.FAILED,
                    ReasonFailed = "Insufficient Funds"
                },
            };

            _notificationEngine.SendTransactionNotifications(transactions);
            List<EmailNotification> actualEmails = _mockEmailAccessor.emails;

            CollectionAssert.AreEqual(expectedEmails, actualEmails);
        }

        [TestMethod]
        public void TestSendAccountUpdateNotification()
        {
            var email = "hi@me.com";
            var firstName = "Sean";
            var expected = new EmailNotification()
            {
                To = email,
                Subject = "Alert from Tuition Assistant: Account Information Updated",
                Body = "Hi Sean,<br><br>Your personal information was updated on your account. If you did not make this change, " +
                "please contact your administrator.<br>Please contact us if you have any questions." +
                "<br><br><br>Powered by Tuition Assistant<br>"
            };

            _notificationEngine.SendAccountUpdateNotification(email, firstName, "personal");
            var result = _mockEmailAccessor.emails.Find(x => x.To == email);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Email cannot be empty")]
        public void TestSendAccountUpdateNotificationNullTo()
        {
            var firstName = "Sean";
            _notificationEngine.SendAccountUpdateNotification(null, firstName, "personal");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "first name cannot be empty")]
        public void TestSendAccountUpdatedNotificationNullFirstName()
        {
            var email = "hi@me.com";
            _notificationEngine.SendAccountUpdateNotification(email, null, "personal");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "information type cannot be empty")]
        public void TestSendAccountUpdatedNotificationNullInfoType()
        {
            var email = "hi@me.com";
            var firstName = "Sean";
            _notificationEngine.SendAccountUpdateNotification(email, firstName, null);
        }

        [TestMethod]
        public void TestSendAccountCreationNotification()
        {
            User user = new User()
            {
                FirstName = "Joe",
                Email = "joe@joe.r.accountant",
                Plan = PaymentPlan.MONTHLY,
                Students = new List<Student>()
                {
                    new Student()
                    {
                        Grade = 8
                    }
                }
            };
            Transaction nextTransaction = new Transaction()
            {
                DateDue = new DateTime(2018, 9, 5),
                AmountCharged = 3125
            };

            var expected = new List<EmailNotification>()
            {
                new EmailNotification()
                {
                    To = user.Email,
                    Subject = "Welcome to Tuition Assistant!",
                    Body = "Hi Joe,<br><br>Thank you creating an account with Tuition Assistant. We're glad you're here!<br><br>" +
                    "Your next automatic payment will be on August 5 2018, for an amount of $386.25.<br>" +
                    "We'll let you know five days before we charge your account.<br>" +
                    "Please contact us if you have any questions.<br><br><br>Powered by Tuition Assistant<br>"
                }
            };
            
            _notificationEngine.SendAccountCreationNotification(user, new DateTime(2018, 7, 14));

            CollectionAssert.AreEqual(expected, _mockEmailAccessor.emails);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "user cannot be null")]
        public void TestSendAccountCreationNotificationNullUser()
        {
            _notificationEngine.SendAccountCreationNotification(null, default(DateTime));
        }

        [TestMethod]
        public void TestAccountDeletedNotification()
        {
            User user = new User()
            {
                FirstName = "Joe",
                Email = "joe@joe.r.accountant"
            };

            var expected = new List<EmailNotification>()
            {
                new EmailNotification()
                {
                    To = user.Email,
                    Subject = "Alert from Tuition Assistant: Account Deleted",
                    Body = "Hi Joe,<br><br>The account associated with your email address has been deleted.<br>" +
                    "Please contact us if you have any questions.<br><br><br>Powered by Tuition Assistant<br>"
                }
            };

            _notificationEngine.SendAccountDeletionNotification(user);

            CollectionAssert.AreEqual(expected, _mockEmailAccessor.emails);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "user cannot be null")]
        public void TestAccountDeletedNotificationNullUser()
        {
            _notificationEngine.SendAccountDeletionNotification(null);
        }

    }
}
