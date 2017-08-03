using DiR.EfChangeLog.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiR.EfChangeLog.Models
{
    public class Order
        : IHaveChangeLog
    {
        public int Id { get; set; }

        #region Foreign keys

        public int CustomerId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public virtual Customer Customer { get; set; }

        #endregion

        #region IHaveChangeLog implementation.

        public DateTime CreatedAt { get; set; }
        public string CreatedAtAuthor { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedAuthor { get; set; }

        #endregion
    }
}