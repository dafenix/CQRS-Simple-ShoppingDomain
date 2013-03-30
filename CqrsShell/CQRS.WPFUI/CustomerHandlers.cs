using System.Windows;
using CQRS.Domain.Events;
using CQRS.Infrastructure;

namespace CQRS.WPFUI
{
    internal class CustomerHandlers : Handles<CustomerCanNotBeCreatedEvent>
    {
        public void Handle(CustomerCanNotBeCreatedEvent message)
        {
            MessageBox.Show("Customer can not be created, reason:" + message.Reason);
        }
    }
}