using System.Collections.Generic;

namespace iCustomerCareSystem.Models
{
    public class OperationType
    {
        public long OperationTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }
}
