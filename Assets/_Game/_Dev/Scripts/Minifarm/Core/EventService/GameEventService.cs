using System;
using System.Collections.Generic;

namespace Minifarm._Core.EventService
{
    public static class GameEventService
    {
        public delegate void EventDelegate<T>(T e);
        private delegate void EventDelegate(object e);

        private static readonly Dictionary<Type, EventDelegate> delegates = new Dictionary<Type, EventDelegate>();
        private static readonly Dictionary<Delegate, EventDelegate> delegateLookup = new Dictionary<Delegate, EventDelegate>();

        public static void Fire<T>(T e)
        {
            if (delegates.TryGetValue(typeof(T), out EventDelegate eventDelegate))
                eventDelegate.Invoke(e);
        }

        public static void On<T>(EventDelegate<T> eventDelegate)
        {
            if (delegateLookup.ContainsKey(eventDelegate))
                return;

            void InternalDelegate(object e) => eventDelegate((T)e);
            delegateLookup[eventDelegate] = InternalDelegate;

            if (delegates.TryGetValue(typeof(T), out EventDelegate tempDelegate))
            {
                delegates[typeof(T)] += InternalDelegate;
            }
            else
            {
                delegates[typeof(T)] = InternalDelegate;
            }
        }

        public static void Off<T>(EventDelegate<T> eventDelegate)
        {
            if (delegateLookup.TryGetValue(eventDelegate, out EventDelegate internalDelegate))
            {
                if (delegates.TryGetValue(typeof(T), out EventDelegate tempDelegate))
                {
                    tempDelegate -= internalDelegate;

                    if (tempDelegate == null)
                    {
                        delegates.Remove(typeof(T));
                    }
                    else
                    {
                        delegates[typeof(T)] = tempDelegate;
                    }
                }

                delegateLookup.Remove(eventDelegate);
            }
        }

        public static void Clear()
        {
            delegates.Clear();
            delegateLookup.Clear();
        }
    }
}