using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class DeliverymanContext : DbContext
    {
        public DeliverymanContext()
        {
        }

        public virtual DbSet<Deliveryman> Deliverymen { get; set; }
    }
}
