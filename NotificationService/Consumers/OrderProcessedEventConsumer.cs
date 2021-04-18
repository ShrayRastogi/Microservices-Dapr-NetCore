using EmailService;
using MassTransit;
using Messaging.InterfacesConstant.MassTransit.Events;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NotificationService.Consumers
{
    public class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {
        private readonly IEmailSender _emailSender;
        public OrderProcessedEventConsumer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public async Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            var rootFolder = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
            var result = context.Message;
            var facesData = result.Faces;
            if (facesData.Count < 1)
            {
                await Console.Out.WriteLineAsync($"No Faces Detected");
            }
            else
            {
                int j = 0;
                foreach (var face in facesData)
                {
                    MemoryStream ms = new(face);
                    var image = Image.Load(ms.ToArray());
                    image.Save(rootFolder + "/Images/face" + j + ".jpg");
                    j++;
                }
            }

            string[] mailAddress = { result.UserEmail };
            //await _emailSender.SendEmailAsync(new Message
            //    (mailAddress, "Your Order " + result.OrderId, "From Faces and Faces", facesData));

            await context.Publish<IOrderDispatchedEvent>(new
            {
                context.Message.OrderId,
                DispachedDateTime = DateTime.UtcNow
            });
        }
    }
}
