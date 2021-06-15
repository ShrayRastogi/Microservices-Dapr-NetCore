using Binding.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binding.Abstractions
{
    public interface IBinding
    {
        Task SendNotificationAsync<Notification>(Notification notification);
        Task SendMailAsync<Mail>(Mail mail);

        Task CreateBindingAsync<BindingEvent>(BindingEvent bindingEvent);
    }
}
