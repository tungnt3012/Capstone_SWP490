using Capstone_SWP490.DTO;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace Capstone_SWP490.Helper
{
    public class MailHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MailHelper));
        public string readMailContent(string fname)
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/App_Data/"+ fname);//Path of the xml script  
                return File.ReadAllText(path); ;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public bool sendMailAsync(EmailModel emailModel)
        {
            try
            {
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(emailModel.toEmail));
                    message.Subject = emailModel.title;
                    message.Body = emailModel.body;
                    message.IsBodyHtml = true;
                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(message);
                        return true;
                    }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return false;
        }
        public void sendMailToInsertedUser(app_user user)
        {
            EmailModel emailModel = new EmailModel();
            string mailContent = readMailContent("CreateAccount.txt");
            emailModel.toEmail = user.email;
            string hostName = "";
            try
            {
                hostName = WebConfigurationManager.AppSettings["HostName"];
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            string changePswUrl = hostName + "/Login";
            emailModel.body = string.Format(mailContent, user.psw, changePswUrl, changePswUrl);
            emailModel.title = "ICPC Asia-VietNam " + DateTime.Now.Year;
            sendMailAsync(emailModel);
        }

        public void sendMailAfterConfirm(app_user user)
        {
            EmailModel emailModel = new EmailModel();
            string mailContent = readMailContent("ConfirmAccount.txt");
            emailModel.toEmail = user.email;
            string hostName = "";
            try
            {
                hostName = WebConfigurationManager.AppSettings["HostName"];
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            string changePswUrl = hostName + "/Login";
            emailModel.body = string.Format(mailContent, user.full_name,user.psw, changePswUrl, changePswUrl);
            emailModel.title = "ICPC Asia-VietNam " + DateTime.Now.Year;
            sendMailAsync(emailModel);
        }
    }
}