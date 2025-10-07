using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project_backend
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BookId { get; set; }

        public Order(Guid customerId, Guid bookId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            BookId = bookId;
        }
    }
}