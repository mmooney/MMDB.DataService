using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MMDB.DataService.Data.Dto.Email
{
    //Why?  Because you can't serialize a MailAddress, that's why
    public class DataServiceMailAddress
    {
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }


        public DataServiceMailAddress()
        {

        }

        public DataServiceMailAddress(MailAddress mailAddress)
        {
            this.DisplayName = mailAddress.DisplayName;
            this.EmailAddress = mailAddress.Address;
        }

        public static List<DataServiceMailAddress> GetList(IEnumerable<MailAddress> list)
        {
            return list.Select(i => new DataServiceMailAddress(i)).ToList();
        }

        public static List<MailAddress> ToMailAddressList(List<DataServiceMailAddress> list)
        {
            return list.Select(i=> new MailAddress(i.EmailAddress, i.DisplayName)).ToList();
        }

        public MailAddress ToMailAddress()
        {
            return new MailAddress(this.EmailAddress, this.DisplayName);
        }
    }
}
