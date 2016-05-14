using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeliveryMan.Models
{
    public class ChangeRestaurantInfoModel
    {
        [Required]
        public string Name { get; set; }


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

        public HttpPostedFileBase file { get; set; }
    }
}