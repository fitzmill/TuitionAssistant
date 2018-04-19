﻿using Core.DTOs;
using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
    /// <summary>
    /// Accessor for getting transactions from the database
    /// </summary>
    public interface IGetTransactionAccessor
    {
        /// <summary>
        /// Gets a list of all transactions from the database for a user based on their userID
        /// </summary>
        /// <param name="userID">The user id associated with the desired transaction information</param>
        /// <returns>A list of transactions for the user</returns>
        IList<Transaction> GetAllTransactionsForUser(int userID);

        /// <summary>
        /// Gets a list of transactions from the database for all users that were charged in a given date range
        /// </summary>
        /// <param name="startTime">The oldest date allowed for retrieved transactions</param>
        /// <param name="endTime">The newest date allowed for retrieved transactions</param>
        /// <returns>List of transactions that occurred in the date range</returns>
        IList<TransactionWithUserInfoDTO> GetTransactionsForDateRange(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets all transactions that have not been successfully charged
        /// </summary>
        /// <returns>List of all RETRYING and NOT_YET_CHARGED transactions</returns>
        IList<Transaction> GetAllUnsettledTransactions();

        /// <summary>
        /// Gets all transactions marked as FAILED from the database
        /// </summary>
        /// <returns>List of all FAILED transactions</returns>
        IList<Transaction> GetAllFailedTransactions();
    }
}