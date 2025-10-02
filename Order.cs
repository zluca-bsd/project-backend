using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_backend
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        
        public Order( Guid customerId, Guid productId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            ProductId = productId;
        }
    }
}