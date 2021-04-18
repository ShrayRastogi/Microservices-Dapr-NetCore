using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.InterfacesConstant.MassTransit.Commands
{
    public interface IRegisterOrderCommand
    {
        public Guid OrderId { get; set; }
        public string PictureUrl { get; set; }
        public string UserEmail { get; set; }
        public byte[] ImageData { get; set; }
    }
}
