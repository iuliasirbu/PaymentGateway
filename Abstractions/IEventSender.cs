namespace PaymentGateway.Abstractions
{
    public interface IEventSender
    {
        public void EventSender(object e);
    }
}
