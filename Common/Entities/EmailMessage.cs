using System.Collections.Generic;
using Common.Other;

namespace Common.Entities
{
    public class EmailMessage
    {
        public EmailMessage(
            ISet<string> targetEmails,
            string link, 
            string targetNickname, 
            EmailTypes operationType)
        {
            TargetEmails = targetEmails;
            Link = link;
            TargetNickname = targetNickname;
            OperationType = operationType;
        }

        protected EmailMessage()
        {
        }

        public virtual ISet<string> TargetEmails { get; protected set; }
        public virtual string Link { get; protected set; }
        public virtual string TargetNickname { get; protected set; }
        public virtual EmailTypes OperationType { get; protected set; }
        public virtual int MailId { get; protected set; }
    }
}
