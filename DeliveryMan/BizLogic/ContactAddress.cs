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
        public static string getFullAddress(this Contact contact)
        {
            if (contact.AddressLine2 != null)
            {
                return contact.AddressLine1 + ", " + contact.AddressLine2 + ", " + 
                    contact.City + ", " + contact.State + ", " + contact.ZipCode; ;
            }
            else
            {
                return contact.AddressLine1 + ", " + contact.City + ", " + 
                    contact.State + ", " + contact.ZipCode; ;
            }
        }

        public static string getPartialAddress(this Contact contact)
        {
            if (contact.AddressLine2 != null)
            {
                return contact.AddressLine1 + ", " + contact.AddressLine2;
            }
            else
            {
                return contact.AddressLine1;
            }
        }
    }
}
