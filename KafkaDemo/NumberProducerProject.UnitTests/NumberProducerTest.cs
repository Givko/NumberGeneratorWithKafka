using Confluent.Kafka;
using Domain.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NumberProducerProject.Producers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NumberProducerProject.UnitTests
{
    [TestClass]
    public class NumberProducerTest
    {
        [TestMethod]
        public async Task ProduceNUmersAsyncTest_PassNoData_ShouldCompleteSuccessfully()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(s => s.LogException((It.IsAny<Exception>())));

            var producerMock = new Mock<IProducer<Null, string>>();
            producerMock.Setup(s => s.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new DeliveryReport<Null, string>()));
            producerMock.Setup(s => s.Dispose());

            var timeProviderMock = new Mock<ITimeProvider>();
            timeProviderMock.Setup(s => s.GetMilisecondsForMinutes(It.IsAny<int>())).Returns(6000);

            var producer = new NumberProducer(producerMock.Object, loggerMock.Object, timeProviderMock.Object, "TestTopic");

            using (producer)
            {
                await producer.ProduceNumbersAsync();
            }

            producerMock.Verify(v => v.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
            producerMock.Verify(v => v.Dispose(), Times.Once);
            timeProviderMock.Verify(v => v.GetMilisecondsForMinutes(It.IsAny<int>()), Times.Once);
        }
    }
}
