﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Core.Interfaces
{
    /// <summary>
    /// Interface for engine that generates and sends notifications.
    /// </summary>
    public interface INotificationEngine
    {
        /// <summary>
        /// Generates and sends notifications based on the given transactions.
        /// </summary>
        /// <param name="transactions">List of one or more transactions</param>
        void SendTransactionNotifications(List<Transaction> transactions);

        /// <summary>
        /// Generates and sends a notification to a user telling them the information type has been changed on their account.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="informationType"></param>
        void SendAccountUpdateNotification(string email, string firstName, string informationType);
    }
}
