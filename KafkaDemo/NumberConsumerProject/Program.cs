using Confluent.Kafka;
using Domain.Loggers;
using NumberConsumerProject.Consumers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NumberConsumerProject
{
    class Program
    {
        static List<int> numbersConsumed = new List<int>();

        static void Main(string[] args)
        {
            Console.WriteLine("If you want to stop the application at any given type press the [Escape] key on the keyboard.");
            Console.WriteLine();
            Console.WriteLine("If you have read the above information, please feel free to press any key to continue and start consuming the super secret information...");
            Console.ReadLine();

            var source = new CancellationTokenSource();
            var token = source.Token;

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

            var conf = new ConsumerConfig
            {
                GroupId = "test-groupid",
                BootstrapServers = "localhost:9092",

                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetResetType.Earliest
            };

            var logger = new Logger();
            var numberConsumer = new NumberConsumer(new Consumer<Ignore, string>(conf), logger, "NumberStorage");

            Action<string> callbackAction = (value) =>
            {
                //If the value is not correct we want to log the exception thus notifying for the error.
                //However, we want to continue listening in order not to miss any important information(given the information is important)
                if (!int.TryParse(value, out int number))
                {
                    logger.LogException(new FormatException("The number from the consumer is not in a correct format."));
                    return;
                }

                numbersConsumed.Add(number);
                if (numbersConsumed.Count == 10)
                {
                    Console.Write($"{string.Join(", ", numbersConsumed)} - ");
                    Console.WriteLine($"[{DateTime.Now}] : {numbersConsumed.Average()}");
                    numbersConsumed.Clear();
                }
            };

            using (numberConsumer)
            {
                try
                {
                    numberConsumer.Consume(callbackAction, token);
                }
                catch (OperationCanceledException ex)
                {
                    logger.LogInfo(ex.Message);
                }
                catch (Exception ex)
                {
                    logger.LogException(ex);
                    Console.ReadLine();
                    return;
                }
            }

            Console.WriteLine("Press any key to close the application...");
            Console.ReadLine();
        }
    }
}
