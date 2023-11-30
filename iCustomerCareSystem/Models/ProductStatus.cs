using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCustomerCareSystem.Models
{
    public class ProductStatus
    {
        public long ProductStatusId { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public virtual ICollection<ClientProducts> ClientProducts { get; set; }
    }
}
