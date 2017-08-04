using DiR.EfChangeLog.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DiR.EfChangeLog.Models.Interfaces;

namespace DiR.EfChangeLog.Contexts
{
    public class OnlineShopContext
        : DbContext
    {
        #region DbSets

        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<OrderItem> OrderItems { get; set; }

        #endregion

        #region Constructors

        public OnlineShopContext()
            : base("OnlineShop")
        {

        }

        #endregion

        #region Overrides

        public override int SaveChanges()
        {
            WriteChangeLog();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            WriteChangeLog();
            return base.SaveChangesAsync();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            WriteChangeLog();
            return base.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Change Log Data processing

        private void WriteChangeLog()
        {
            string currentUser = "John Wayne"; // Actually you would use here the user id from the current HttpContext / Thread.
            DateTime currentDateTime = DateTime.Now;            
            var dbEntityEntries = ChangeTracker.Entries<IHaveChangeLog>();            

            foreach (var item in dbEntityEntries)
            {
                if (item.State == EntityState.Added)
                {
                    item.Entity.CreatedAtAuthor = currentUser;
                    item.Entity.CreatedAt = currentDateTime;                    
                }

                item.Entity.LastModifiedAuthor = currentUser;
                item.Entity.LastModifiedAt = currentDateTime;                
            }
        }

        #endregion
    }
}