﻿using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.DTOs;

namespace NelnetProject.Tests.Engines.MockedAccessors
{
    public class MockTransactionAccessor : IGetTransactionAccessor
    {
        private List<Transaction> MockDB = new List<Transaction>{
            new Transaction()
            {
                TransactionID = 1,
                UserID = 1,
                AmountCharged = 99.00,
                DateDue = new DateTime(2018, 2, 9),
                DateCharged = new DateTime(2018, 2, 11),
                ProcessState = ProcessState.SUCCESSFUL,
                ReasonFailed = "Card expired"
            },
            new Transaction()
            {
                TransactionID = 2,
                UserID = 2,
                AmountCharged = 64.00,
                DateDue = new DateTime(2018, 2, 9),
                DateCharged = new DateTime(2018, 2, 9),
                ProcessState = ProcessState.SUCCESSFUL
            },
            new Transaction()
            {
                TransactionID = 3,
                UserID = 3,
                AmountCharged = 55.00,
                DateDue = new DateTime(2018, 2, 9),
                DateCharged = null,
                ProcessState = ProcessState.FAILED,
                ReasonFailed = "Insufficient funds"
            },
            new Transaction()
            {
                TransactionID = 4,
                UserID = 1,
                AmountCharged = 108.00,
                DateDue = new DateTime(2018, 3, 11),
                DateCharged = null,
                ProcessState = ProcessState.NOT_YET_CHARGED
            }
        };

        public IList<Transaction> GetAllTransactionsForUser(int userID)
        {
            return MockDB.Where(x => x.UserID == userID).ToList();
        }

        public IList<Transaction> GetAllUnsettledTransactions()
        {
            return MockDB.Where(x => x.ProcessState != ProcessState.SUCCESSFUL && x.ProcessState != ProcessState.FAILED).ToList();
        }

        public Transaction GetMostRecentTransactionForUser(int userID)
        {
            return MockDB.OrderByDescending(x => x.TransactionID).FirstOrDefault(x => x.UserID == userID);
        }

        public IList<TransactionWithUserInfoDTO> GetTransactionsForDateRange(DateTime startTime, DateTime endTime)
        {
            var result = new List<TransactionWithUserInfoDTO>();
            foreach(Transaction t in MockDB.Where(x => x.DateDue >= startTime && x.DateDue <= endTime).ToList())
            {
                result.Add(new TransactionWithUserInfoDTO()
                {
                    TransactionID = t.TransactionID,
                    FirstName = "Bob",
                    LastName = "Smith",
                    AmountCharged = t.AmountCharged,
                    DateDue = t.DateDue,
                    DateCharged = t.DateCharged,
                    ProcessState = t.ProcessState.ToString(),
                    ReasonFailed = t.ReasonFailed
                });
            }
            return result;
        }
    }
}
