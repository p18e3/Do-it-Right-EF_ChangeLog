using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiR.EfChangeLog.Models.Interfaces
{
    public interface IHaveChangeLog
    {
        DateTime CreatedAt { get; set; }
        string CreatedAtAuthor { get; set; }
        DateTime LastModifiedAt { get; set; }
        string LastModifiedAuthor { get; set; }        
    }
}