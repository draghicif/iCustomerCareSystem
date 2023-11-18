using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iCustomerCareSystem.Models
{
    public class ClientProducts
    {
        [Key]
        public long ClientProductId { get; set; }
        public string Name { get; set; }
        public string Configuration { get; set; }
        public ProductType ProductType { get; set; }
        public long ProductTypeId { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }
}
