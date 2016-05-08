﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DataLayer
{
    public class Contact
    {
        [Key]
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public Role Role { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public String addressToString()
        {
            string res = this.AddressLine1 + " " + this.AddressLine2 + " " + this.City + " " + this.State + " " + this.ZipCode;
            return res;
        }
    }
  
    public enum Role
    {
        DELIVERYMAN,
        RESTAUTANT,
        CUSTOMER,
    }
}