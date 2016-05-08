using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class ContactAddress
    {
        public static string getAddress(this Contact contact)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(contact.AddressLine1);
            builder.Append(" ");
            builder.Append(contact.AddressLine2);
            builder.Append(" ");
            builder.Append(contact.City);
            builder.Append(" ");
            builder.Append(contact.State);
            builder.Append(" ");
            builder.Append(contact.ZipCode);
            return builder.ToString();
        }
    }
}
