using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLoginWithMaui.Messages
{
    public class ProtocolMessage : ValueChangedMessage<string>
    {
        public ProtocolMessage(string value) : base(value)
        {
        }
    }
}
