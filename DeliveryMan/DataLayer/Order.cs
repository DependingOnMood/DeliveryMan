using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        public virtual Restaurant Restaurant { get; set; }

        public int? DeliverymanId { get; set; }

        [ForeignKey("DeliverymanId")]
        public virtual Deliveryman Deliveryman { get; set; }

        public string ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        [Required]
        public Status Status { get; set; }

        [Required]
        public string Note { get; set; }

        [Required]
        [Column(TypeName = "DateTime2")]//xue dao le
        [DataType(DataType.DateTime)]
        public DateTime PlacedTime { get; set; }

        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        public DateTime? PickUpTime { get; set; }

        [Column(TypeName = "DateTime2")]
        [DataType(DataType.DateTime)]
        public DateTime? DeliveredTime { get; set; }

        [Required]
        public string ETA { get; set; }

        [Required]
        public decimal DeliveryFee { get; set; }
    }

    public enum Status
    {
        WAITING,
        PENDING,
        INPROGRESS,
        DELIVERED,
    }
}
