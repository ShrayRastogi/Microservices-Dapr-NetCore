using Dapr.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersAPI.StateStore
{
    public interface IOrderingProcessActor: IActor
    {
        Task UpdateOrderStatus(OrderStatus orderStatus);
        Task<OrderStatus> GetOrderStatus();
    }
}
