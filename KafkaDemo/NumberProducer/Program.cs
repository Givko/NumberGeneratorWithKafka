using Confluent.Kafka;
using Domain.Loggers;
using Domain.TimeProviders;
using NumberProducerProject.Producers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NumberProducerProject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("If you want to stop the application at any given type press the [Escape] key on the keyboard.");
            Console.WriteLine();
            Console.WriteLine("If you have read the above information, please feel free to press any key to continue...");
            Console.ReadLine();

            var source = new CancellationTokenSource();
            var token = source.Token;

            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() =>
            {
                while (true)
                {
                    var key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Escape)
                    {
                        source.Cancel();
                        return;
                    }
                }
            });
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            var logger = new Logger();
            var timeProvider = new TimeProvider();

            Console.WriteLine("Producing numbers...");

            using (var producer = new NumberProducer(new Producer<Null, string>(config), logger, timeProvider, "NumberStorage"))
            {
                try
                {
                    await producer.ProduceNumbersAsync(token);
                    Console.WriteLine("Numbers have been successfully produced.");
                }
                catch(OperationCanceledException ex)
                {
                    logger.LogInfo(ex.Message);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Failed due to: {ex.Message}. Press any key to close the app...");
                    Console.ReadLine();
                    return;
                }
            }

            Console.WriteLine("Press any key to close the app...");
            Console.ReadLine();
        }
    }
}
