using System;

namespace StateStore
{
    public class OrderStatus: CommonState
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
    }
}