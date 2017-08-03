using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiR.EfChangeLog.Models
{
    public class OrderItem
    {
        public int Id { get; set; }


        #region Foreign keys.

        public int OrderId { get; set; }

        #endregion

        #region Navigation Properties.

        public virtual Order Order { get; set; }

        #endregion
    }
}