using Confluent.Kafka;
using System.Threading;
using System.Threading.Tasks;

namespace NumberProducerProject.Contracts
{
    public interface INumberProducer
    {
        /// <summary>
        /// Produces numbers to Kafka. For the first minute it produces 10 number 
        /// and each minute it produces 10 time more. 1 minute(10 number), 2 minute(100) etc. It will stop on after the 4th minute has passed.
        /// </summary>
        Task ProduceNumbersAsync();

        /// <summary>
        /// Produces numbers to Kafka. For the first minute it produces 10 number 
        /// and each minute it produces 10 time more. 1 minute(10 number), 2 minute(100) etc. It will stop on after the 4th minute has passed.
        /// </summary>
        /// <param name="token">Cancelation token to cancel the opration if requested.</param>
        Task ProduceNumbersAsync(CancellationToken token);
    }
}
