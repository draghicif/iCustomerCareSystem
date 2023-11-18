using System;
using System.Collections.Generic;

namespace iCustomerCareSystem.Models
{
    public class Client
    {
        public long ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Telephone { get; set; }
        public virtual ICollection<ClientProducts> ClientProducts { get; set; }
    }
}
