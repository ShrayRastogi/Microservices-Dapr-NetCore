using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmailService
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<byte[]> Attachments { get; set; }

        public Message(IEnumerable<string> to, string subject, string content, List<byte[]> attachments)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress(x)));
            Subject = subject;
            Content = content;
            Attachments = attachments;
        }
    }
}
