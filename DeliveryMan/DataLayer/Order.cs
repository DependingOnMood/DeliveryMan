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

        public int DeliverymanId { get; set; }

        [ForeignKey("DeliverymanId")]
        public virtual Restaurant Deliveryman { get; set; }

        public int DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public virtual Restaurant Destination { get; set; }

        public Status Status { get; set; }

        public string Note { get; set; }

        public DateTime PlacedTime { get; set; }

        public DateTime PickUpTime { get; set; }

        public DateTime DeliveredTime { get; set; }

        public TimeSpan ETA { get; set; }

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
