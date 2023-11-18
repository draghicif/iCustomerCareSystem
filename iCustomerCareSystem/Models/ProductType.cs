using System.Collections.Generic;

namespace iCustomerCareSystem.Models
{
    public class ProductType
    {
        public long ProductTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ClientProducts> ClientProducts { get; set; }
    }
}
