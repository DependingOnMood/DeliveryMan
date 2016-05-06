using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class DestinationContext : DbContext
    {
        public DestinationContext()
        {
        }

        public virtual DbSet<Destination> Destinations { get; set; }
    }
}
