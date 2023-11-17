using System;

namespace iCustomerCareSystem.Models
{
    public class Client
    {
        public long ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public ProductType ProductType { get; set; }
        public OperationType OperationType { get; set; }
        public string Reason { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsReturnInService { get; set; }

    }
}
