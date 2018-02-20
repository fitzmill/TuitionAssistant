﻿using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Core;
using Core.DTOs;

namespace Engines
{
    public class GetTransactionEngine : IGetTransactionEngine
    {
        IGetTransactionAccessor getTransactionAccessor;
        public GetTransactionEngine(IGetTransactionAccessor getTransactionAccessor)
        {
            this.getTransactionAccessor = getTransactionAccessor;
        }

        //Calls identical method in IGetTransactionAccessor
        public IList<Transaction> GetAllTransactionsForUser(int userID)
        {
            return getTransactionAccessor.GetAllTransactionsForUser(userID);
        }

        //Calls identical method in IGetTransactionAccessor
        public IList<Transaction> GetAllUnsettledTransactions()
        {
            return getTransactionAccessor.GetAllUnsettledTransactions();
        }

        //Calls identical method in IGetTransactionAccessor
        public Transaction GetMostRecentTransactionForUser(int userID)
        {
            return getTransactionAccessor.GetMostRecentTransactionForUser(userID);
        }

        //Calls identical method in IGetTransactionAccessor
        public IList<TransactionWithUserInfoDTO> GetTransactionsForDateRange(DateTime startTime, DateTime endTime)
        {
            return getTransactionAccessor.GetTransactionsForDateRange(startTime, endTime);
        }
    }
}
