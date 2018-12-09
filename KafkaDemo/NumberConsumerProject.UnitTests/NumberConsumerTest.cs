using Confluent.Kafka;
using Domain.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NumberConsumerProject.Consumers;
using System;
using System.Threading;

namespace NumberConsumerProject.UnitTests
{
    [TestClass]
    public class NumberConsumerTest
    {
        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public void ConsumeTest_CorrectData_ShouldCancel()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(s => s.LogException((It.IsAny<Exception>())));


            var returnValue = new ConsumeResult<Ignore, string>();

            var consumerMock = new Mock<IConsumer<Ignore, string>>();
            consumerMock
                .Setup(s => s.Consume(It.IsAny<CancellationToken>()))
                .Returns(returnValue);
            consumerMock.Setup(s => s.Dispose());
            consumerMock.Setup(s => s.Subscribe(It.IsAny<string>()));

            var source = new CancellationTokenSource();
            var token = source.Token;
            source.Cancel();

            var consumer = new NumberConsumer(consumerMock.Object, loggerMock.Object, "TestTopic");

            using (consumer)
            {
                consumer.Consume((value) => { }, token);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConsumeTest_NullForCallback_ShouldThrowArgumentNullException()
        {
            var consumerMock = new Mock<IConsumer<Ignore, string>>();
            consumerMock.Setup(s => s.Dispose());

            var consumer = new NumberConsumer(consumerMock.Object, null, "TestTopic");

            using (consumer)
            {
                consumer.Consume(null);
            }
        }
    }
}
