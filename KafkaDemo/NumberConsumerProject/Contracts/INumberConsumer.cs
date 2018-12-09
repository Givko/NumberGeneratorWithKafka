using System;
using System.Threading;

namespace NumberConsumerProject.Contracts
{
    public interface INumberConsumer
    {
        /// <summary>
        /// Consumes all the number produced by a producer.
        /// </summary>
        /// <param name="callback">A method to be executed for each produced number.</param>
        void Consume(Action<string> callback);
        void Consume(Action<string> callback, CancellationToken token);
    }
}
