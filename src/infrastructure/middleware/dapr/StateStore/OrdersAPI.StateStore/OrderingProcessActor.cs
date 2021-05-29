using Dapr.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersAPI.StateStore
{
    public class OrderingProcessActor : Actor, IOrderingProcessActor
    {
        private const string OrderStatusStateName = "OrderStatus";

        public OrderingProcessActor(ActorHost host)
            : base(host)
        {
        }

        public async Task<OrderStatus> GetOrderStatus()
        {
            try
            {
                return await StateManager.GetStateAsync<OrderStatus>(OrderStatusStateName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task UpdateOrderStatus(OrderStatus orderStatus)
        {
            var prevOrderStatus = await GetOrderStatus();
            bool shouldUpdateOrderStatus = (prevOrderStatus == null) || orderStatus.Status switch
            {
                "Registered" => !prevOrderStatus.Status.Equals("Processed") &&
                              !prevOrderStatus.Status.Equals("Sent"),
                "Processed" => !prevOrderStatus.Status.Equals("Sent"),
                "Sent" => !prevOrderStatus.Status.Equals("Sent"),
                _ => false,
            };
            if (shouldUpdateOrderStatus)
            {
                await StateManager.SetStateAsync(OrderStatusStateName, orderStatus);
            }
            else
            {
                throw new Exception("Invalid Order Status");
            }
        }
    }
}
