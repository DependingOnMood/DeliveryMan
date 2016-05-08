using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizLogic
{
    public static class RestaurantAddress
    {
        public static string getAddress(this Restaurant restaurant)
        {
            StringBuilder builder = new StringBuilder();
            Contact contact = restaurant.Contact;
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
