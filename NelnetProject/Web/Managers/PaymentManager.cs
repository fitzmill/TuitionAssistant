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
        public Func<DateTime> dateProvider = () => DateTime.Now;

        private double timerInterval;
        private int chargingHour;

        private IGetTransactionEngine getTransactionEngine;
        private IPaymentEngine paymentEngine;
        private INotificationEngine notificationEngine;

        public PaymentManager(double timerInterval, int chargingHour, IGetTransactionEngine getTransactionEngine, IPaymentEngine paymentEngine, INotificationEngine notificationEngine)
        {
            this.timerInterval = timerInterval;
            this.chargingHour = chargingHour;
            this.getTransactionEngine = getTransactionEngine;
            this.paymentEngine = paymentEngine;
            this.notificationEngine = notificationEngine;

            Timer timer = new Timer(timerInterval);
            timer.Elapsed += new ElapsedEventHandler(TimerIntervalElapsed);
            timer.Enabled = true;
        }

        public void TimerIntervalElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = dateProvider();
            Debug.WriteLine(String.Format("Time Elapsed at {0:yyyy MM dd HH mm ss}", now));
            
            if (now.Hour == chargingHour && now.Day == 1)
            {
                IList<Transaction> generatedTransactions = paymentEngine.GeneratePayments(now);
                notificationEngine.SendTransactionNotifications(generatedTransactions.ToList());
            }
            else if (now.Hour == chargingHour && now.Day > 1 && now.Day <= 12)
            {
                IList<Transaction> unsettledTransactions = getTransactionEngine.GetAllUnsettledTransactions();
                IList<Transaction> transactionResults = paymentEngine.ChargePayments(unsettledTransactions.ToList(), now);
                notificationEngine.SendTransactionNotifications(transactionResults.ToList());
            }
        }
 
    }
}