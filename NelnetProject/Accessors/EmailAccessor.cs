﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Core.Interfaces;
using Core.Models;
using Core.Exceptions;
using System.Diagnostics;

namespace Accessors
{
    /// <summary>
    /// Accessor to email sending service.
    /// </summary>
    public class EmailAccessor : IEmailAccessor
    {
        private readonly string _senderEmail;
        private readonly string _senderUsername;
        private readonly string _senderPassword;
        private readonly string _smtpHost;
        private readonly int _port;

        public EmailAccessor(string senderEmail, string senderUsername, string senderPassword, string smtpHost, int port)
        {
            _senderEmail = senderEmail;
            _senderUsername = senderUsername;
            _senderPassword = senderPassword;
            _smtpHost = smtpHost;
            _port = port;
        }
        
        public void SendEmail(EmailNotification emailNotification)
        {
            if (string.IsNullOrEmpty(emailNotification.To))
            {
                throw new EmailException("Could not send email: No addressee.");
            }
            if (string.IsNullOrEmpty(emailNotification.Subject))
            {
                throw new EmailException("Could not send email: No subject line.");
            }
            if (string.IsNullOrEmpty(emailNotification.Body))
            {
                throw new EmailException("Could not send email: No body.");
            }

            MailMessage email = new MailMessage();
            SmtpClient client = new SmtpClient(_smtpHost);

            email.From = new MailAddress(_senderEmail);
            email.To.Add(emailNotification.To);
            email.Subject = emailNotification.Subject;
            email.Body = emailNotification.Body;
            email.IsBodyHtml = true;

            client.Port = _port;
            client.Credentials = new System.Net.NetworkCredential(_senderUsername, _senderPassword);
            client.EnableSsl = true;

            try
            {
                client.Send(email);
                Debug.WriteLine("Sent email to " + email.To);

            }
            catch (SmtpException e)
            {
                throw new EmailException("Could not send email: SMTP error", e);
            }
        }
    }
}
