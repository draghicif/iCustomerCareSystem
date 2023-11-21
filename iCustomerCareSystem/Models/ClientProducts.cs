using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace iCustomerCareSystem.Models
{
    public class ClientProducts
    {
        public long ClientProductId { get; set; }
        public Client Client { get; set; }
        public long ClientId { get; set; }
        public string ProductName { get; set; }
        public string ProductConfiguration { get; set; }
        public ProductType ProductType { get; set; }
        public long ProductTypeId { get; set; }
        public string ServiceOperation { get; set; }
        public decimal Price { get; set; }
        public string Reason { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public bool IsUrgent { get; set; }
        public bool IsReturnInService { get; set; }
    }
}
