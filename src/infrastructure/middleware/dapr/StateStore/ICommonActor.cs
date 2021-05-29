using Dapr.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateStore
{
    public interface ICommonActor: IActor
    {
        Task UpdateOrderStatus(OrderStatus orderStatus);
        Task<OrderStatus> GetOrderStatus();
    }
}
