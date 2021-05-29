using Microsoft.EntityFrameworkCore;
using OrdersAPI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Database.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersContext _ordersContext;
        public OrderRepository(OrdersContext ordersContext)
        {
            _ordersContext = ordersContext;
        }
        public Order GetOrder(Guid id)
        {
            return _ordersContext.Orders.Include("OrderDetails").FirstOrDefault(c => c.OrderId == id);
        }

        public async Task<Order> GetOrderAsync(Guid id)
        {
            return await _ordersContext.Orders.Include("OrderDetails").FirstOrDefaultAsync(c => c.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _ordersContext.Orders.ToListAsync();
        }

        public Task RegisterOrder(Order order)
        {
            _ordersContext.Add(order);
            _ordersContext.SaveChanges();
            return Task.FromResult(true);
        }

        public void UpdateOrder(Order order)
        {
            _ordersContext.Entry(order).State = EntityState.Modified;
            _ordersContext.SaveChanges();
        }
    }
}
