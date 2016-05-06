using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class RestaurantContext : DbContext
    {
        public RestaurantContext()
        {
        }

        public virtual DbSet<Restaurant> Restaurants { get; set; }
    }
}
