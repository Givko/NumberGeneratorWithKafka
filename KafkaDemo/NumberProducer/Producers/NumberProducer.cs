using Confluent.Kafka;
using Domain.Contracts;
using NumberProducerProject.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NumberProducerProject.Producers
{
    public class NumberProducer : INumberProducer, IDisposable
    {
        private static readonly Random _rand = new Random();
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger _logger;
        private readonly ITimeProvider _timeProvider;
        private readonly string _kafkaTopic;
        private int _intervalInMiliseconds;

        public NumberProducer(IProducer<Null, string> producer, ILogger logger, ITimeProvider timeProvider, string topic)
        {
            _producer = producer;
            _logger = logger;
            _timeProvider = timeProvider;
            _kafkaTopic = topic;
            _intervalInMiliseconds = 6000;
        }
        
        public async Task ProduceNumbersAsync()
        {
            await ProduceNumbersAsync(default(CancellationToken));
        }

        public async Task ProduceNumbersAsync(CancellationToken token)
        {
            var milisecondsForOneMinute = _timeProvider.GetMilisecondsForMinutes(1);

            for (int i = 1; i <= 4; i++)
            {
                for(int miliseconds = 0; miliseconds < milisecondsForOneMinute; miliseconds += _intervalInMiliseconds)
                {
                    //On each iteration check if cancelation has been requested by the user.
                    if (token != default(CancellationToken))
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    var randNumber = _rand.Next(0, 101);

                    try
                    {
                        await _producer.ProduceAsync(_kafkaTopic, new Message<Null, string> { Value = randNumber.ToString() }, token);
                    }
                    catch (KafkaException ex)
                    {
                        _logger.LogException(ex);
                        return;
                    }
                    
                    //Delay for the required interval
                    await Task.Delay(_intervalInMiliseconds);
                }
                
                _intervalInMiliseconds /= 10;
            }
        }

        #region Base Dispose Pattern
        private bool _disposedValue = false; // To detect redundant calls
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _producer.Dispose();
                }
                
                _disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
