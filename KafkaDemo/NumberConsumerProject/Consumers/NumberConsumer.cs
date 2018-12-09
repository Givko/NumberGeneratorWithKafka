using Confluent.Kafka;
using Domain.Contracts;
using NumberConsumerProject.Contracts;
using System;
using System.Threading;

namespace NumberConsumerProject.Consumers
{
    public class NumberConsumer : INumberConsumer, IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly string _kafkaTopic;
        private readonly ILogger _logger;

        public NumberConsumer(IConsumer<Ignore, string> consumer, ILogger logger, string topic)
        {
            _consumer = consumer;
            _kafkaTopic = topic;
            _logger = logger;
        }

        public void Consume(Action<string> callback)
        {
            Consume(callback, default(CancellationToken));
        }

        public void Consume(Action<string> callback, CancellationToken token)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("The callback must not be null.");
            }

            _consumer.Subscribe(_kafkaTopic);

            bool consuming = true;

            // The client will automatically recover from non-fatal errors. You typically
            // don't need to take any action unless an error is marked as fatal.
            _consumer.OnError += (_, e) => consuming = !e.IsFatal;

            while (consuming)
            {
                try
                {
                    var result = token == default(CancellationToken) ? _consumer.Consume(new TimeSpan(0, 0, 3)) : _consumer.Consume(token);

                    //Consume returns null if there are no records to be consumed from the Kafka topic
                    if (result == null)
                    {
                        // We want to continue listening for new produced records. We do not want to break the loop if currently there are no records for consumtion.
                        _logger.LogInfo("No message to consume.");
                        continue;
                    }

                    //On each iteration check if cancelation has been requested by the user.
                    if (token != default(CancellationToken))
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    callback(result.Value);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogException(ex);
                    return;
                }
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
                    _consumer.Dispose();
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
