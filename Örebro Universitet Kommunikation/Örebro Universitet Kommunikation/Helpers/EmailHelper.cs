using Örebro_Universitet_Kommunikation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Örebro_Universitet_Kommunikation.Helpers {
    public class EmailHelper {
        // Note: To send email you need to add actual email id and credential so that it will work as expected  
        public static readonly string EMAIL_SENDER = "orukommunikation@gmail.com"; // change it to actual sender email id or get it from UI input  
        public static readonly string EMAIL_CREDENTIALS = "Kakan1210"; // Provide credentials   
        public static readonly string SMTP_CLIENT = "smtp.gmail.com"; // as we are using outlook so we have provided smtp-mail.outlook.com   
        public static readonly string EMAIL_BODY = "Reset your Password <a href='http://{0}.safetychain.com/api/Account/forgotPassword?{1}'>Here.</a>";
        private string senderAddress;
        private string clientAddress;
        private string netPassword;
        public ApplicationDbContext Ctx { get; set; }

        public EmailHelper(string sender, string Password) {
            senderAddress = sender;
            netPassword = Password;
            Ctx = new ApplicationDbContext();
        }
        public void SendEmail(string recipient, string subject, string message) {
            try {
                var senderEmail = new MailAddress("orukommunikation@gmail.com", "Örebro Universitet-ish");
                var receiverEmail = new MailAddress(recipient, "Receiver");
                var password = "Kakan1210";
                var sub = subject;
                var body = message;
                var smtp = new SmtpClient {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail) {
                    Subject = subject,
                    Body = body
                }) {
                    smtp.Send(mess);
                }
            }
            catch (Exception) {
            }
        }

        public void SendEmailFormalBlog(string subject, string message, string userId) {
            var users = (from u in Ctx.Users
                         where u.Notifications == "BlogEvent"
                         || u.Notifications == "Blog"
                         where u.Id != userId
                         select u).ToList();
            if (users.Count > 0) {
                foreach (var u in users) {
                    SendEmail(u.Email, subject, message);
                }
            }
        }

        public void SendEmailMeeting(string subject, string message, string userId) {
            var users = (from u in Ctx.Users
                         where u.Notifications == "BlogEvent"
                         || u.Notifications == "Event"
                         where u.Id == userId
                         select u).ToList();
            if (users.Count > 0) {
                SendEmail(users[0].Email, subject, message);
            }
        }
    }
}