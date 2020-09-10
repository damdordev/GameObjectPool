using System.Collections.Generic;
using UnityEngine.Events;

namespace Tests.Damdor
{
    public class UnityEventTester<T>
    {
        public int InvocationCount { get; private set; }
        public List<T> InvocationParameters { get; private set; } = new List<T>();

        public UnityEventTester(UnityEvent<T> @event)
        {
            @event?.AddListener(OnInvocation);
        }

        private void OnInvocation(T parameter)
        {
            ++InvocationCount;
            InvocationParameters.Add(parameter);
        }
        
    }
}
