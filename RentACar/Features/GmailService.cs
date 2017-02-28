using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RentACar.Features
{
    public class GmailService : SmtpClient
    {
        public GmailService(string username, string password)
        {
            this.Host = "smtp.gmail.com";
            this.Port = 587;
            this.EnableSsl = true;
            DeliveryMethod = SmtpDeliveryMethod.Network;
            this.Credentials = new NetworkCredential(username, password);
            Timeout = 20000;
        }
    }
}
