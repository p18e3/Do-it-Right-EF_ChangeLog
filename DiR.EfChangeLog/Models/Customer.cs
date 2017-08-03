using DiR.EfChangeLog.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiR.EfChangeLog.Models
{
    public class Customer
        : IHaveChangeLog
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        #region Navigation Properties.

        public virtual ICollection<Order> Orders { get; set; }

        #endregion

        #region IHaveChangeLog implementation.

        public DateTime CreatedAt { get; set; }
        public string CreatedAtAuthor { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedAuthor { get; set; }

        #endregion
    }
}