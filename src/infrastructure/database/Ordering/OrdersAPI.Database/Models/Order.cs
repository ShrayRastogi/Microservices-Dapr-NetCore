using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Database.Models
{
    public class Order
    {
        public Order()
        {
            OrderDetails = new List<OrderDetail>();
        }
        public Guid OrderId { get; set; }
        public string PictureUrl { get; set; }
        public byte[] ImageData { get; set; }
        public string UserEmail { get; set; }
        public Status Status { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        [NotMapped]
        public string OrderStatus { get; set; }

    }
}
