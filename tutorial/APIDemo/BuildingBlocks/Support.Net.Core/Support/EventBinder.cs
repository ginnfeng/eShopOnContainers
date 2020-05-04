using System;
using System.Reflection;

namespace Support
{
    public class EventBinder
    {
        public EventBinder(object publisher)
        {
            this.publisher = publisher;
        }

        public void Unsubscribe(string eventName, object subscriber, string handlerMethodName)
        {
            Delegate dg = DelegateFactory(subscriber,eventName, handlerMethodName);

            Unsubscribe(eventName, dg);

        }

        private Delegate DelegateFactory(object subscriber,string eventName, string handlerMethodName)
        {
            Type type = publisher.GetType();
            EventInfo eventInfo = type.GetEvent(eventName);
            Type tDelegate = eventInfo.EventHandlerType;
            MethodInfo onHandler = subscriber.GetType().GetMethod(handlerMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Delegate dg = Delegate.CreateDelegate(tDelegate, subscriber, onHandler);
            return dg;
        }
        public void Subscribe(string eventName, object subscriber, string handlerMethodName)
        {

            Delegate dg = DelegateFactory(subscriber,eventName, handlerMethodName);
            // Create an instance of the delegate. Using the overloads
            // of CreateDelegate that take MethodInfo is recommended.
            Subscribe(eventName, dg);

        }
        public void Subscribe(string eventName, Delegate dg)
        {
            EventInfo eventInfo = publisher.GetType().GetEvent(eventName);

            eventInfo.AddEventHandler(publisher, dg);
            /*
            MethodInfo addHandler = eventInfo.GetAddMethod();
            Object[] addHandlerArgs = { dg };
            addHandler.Invoke(publisher, addHandlerArgs);
             */
        }
        public void Unsubscribe(string eventName, Delegate dg)
        {
            EventInfo eventInfo = publisher.GetType().GetEvent(eventName);
            eventInfo.RemoveEventHandler(publisher, dg);
            
        }

        object publisher;
    }
}
