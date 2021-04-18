using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Models
{
    public class OrderDetail
    {
        public Guid OrderId { get; set; }
        public int OrderDetailId { get; set; }
        public byte[] FaceData { get; set; }

    }
}
