using PaymentGateway.Abstractions;
using System;

namespace PaymentGateway.ExternalServices
{
    public class EventSender : IEventSender
    {
        void IEventSender.EventSender(object e)
        {
            throw new NotImplementedException();
        }
    }
}
