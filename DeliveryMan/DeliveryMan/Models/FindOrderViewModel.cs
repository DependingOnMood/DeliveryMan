using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliveryMan.Models
{

    public class FindOrderViewModel
    {
        [Required]
        [Display(Name = "line1")]
        public string line1 { get; set; }

        [Display(Name = "line2")]
        public string line2 { get; set; }

        [Required]
        [Display(Name = "City")]
        public string city { get; set; }

        [Required]
        [Display(Name = "State")]
        public string state { get; set; }

        [Required]
        [Display(Name = "Zip Code")]
        [DataType(DataType.PostalCode)]
        public string zipCode { get; set; }




    }


}