﻿using Core;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Web;

namespace Web.Managers
{
    public class PaymentManager
    {
        static double timerInterval = 1000; //todo move
        static int chargingHour = 10; //todo move, change

        public Func<DateTime> dateProvider = () => DateTime.Now;

        IGetTransactionEngine getTransactionEngine;
        IPaymentEngine paymentEngine;
        INotificationEngine notificationEngine;

        public PaymentManager(IGetTransactionEngine getTransactionEngine, IPaymentEngine paymentEngine, INotificationEngine notificationEngine)
        {
            this.getTransactionEngine = getTransactionEngine;
            this.paymentEngine = paymentEngine;
            this.notificationEngine = notificationEngine;

            Timer timer = new Timer(timerInterval);
            timer.Elapsed += new ElapsedEventHandler(TimerIntervalElapsed);
            timer.Enabled = true;
        }

        public void TimerIntervalElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime today = dateProvider();
            Debug.WriteLine(String.Format("Time Elapsed at {0:yyyy MM dd HH mm ss}", today));
            
            if (today.Hour == chargingHour && today.Day == 1)
            {
                IList<Transaction> generatedTransactions = paymentEngine.GeneratePayments(today);
                notificationEngine.SendTransactionNotifications(generatedTransactions.ToList());
            }
            else if (today.Hour == chargingHour && today.Day > 1 && today.Day <= 12)
            {
                IList<Transaction> unsettledTransactions = getTransactionEngine.GetAllUnsettledTransactions();
                IList<Transaction> transactionResults = paymentEngine.ChargePayments(unsettledTransactions.ToList(), today);
                notificationEngine.SendTransactionNotifications(transactionResults.ToList());
            }
        }
 
    }
}