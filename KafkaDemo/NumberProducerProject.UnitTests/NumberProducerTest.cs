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
        public async Task ProduceNumersAsyncTest_PassNoData_ShouldCompleteSuccessfully()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(s => s.LogException((It.IsAny<Exception>())));

            var producerMock = new Mock<IProducer<Null, string>>();
            producerMock.Setup(s => s.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(new DeliveryReport<Null, string>()));
            producerMock.Setup(s => s.Dispose());

            var timeProviderMock = new Mock<ITimeProvider>();
            timeProviderMock.Setup(s => s.GetMilisecondsForMinutes(It.IsAny<int>())).Returns(6);
            timeProviderMock.Setup(s => s.GetMilisecondsForSeconds(It.IsAny<int>())).Returns(6000);

            var producer = new NumberProducer(producerMock.Object, loggerMock.Object, timeProviderMock.Object, "TestTopic");

            using (producer)
            {
                await producer.ProduceNumbersAsync();
            }

            producerMock.Verify(v => v.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            producerMock.Verify(v => v.Dispose(), Times.Once);

            timeProviderMock.Verify(v => v.GetMilisecondsForMinutes(It.IsAny<int>()), Times.Once);
            timeProviderMock.Verify(v => v.GetMilisecondsForSeconds(It.IsAny<int>()), Times.Once);
            
            loggerMock.Verify(v => v.LogException(It.IsAny<Exception>()), Times.Never);
        }

        [TestMethod]
        public async Task ProduceNumersAsyncTest_PassNoData_ThrowException_ShouldLogException()
        {
            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(s => s.LogException((It.IsAny<Exception>())));

            var producerMock = new Mock<IProducer<Null, string>>();
            producerMock
                .Setup(s => s.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), It.IsAny<CancellationToken>()))
                .Throws(new KafkaException(new Error(ErrorCode.IllegalGeneration)));

            producerMock.Setup(s => s.Dispose());
            
            var timeProviderMock = new Mock<ITimeProvider>();
            timeProviderMock.Setup(s => s.GetMilisecondsForMinutes(It.IsAny<int>())).Returns(6);
            timeProviderMock.Setup(s => s.GetMilisecondsForSeconds(It.IsAny<int>())).Returns(6000);
            
            var producer = new NumberProducer(producerMock.Object, loggerMock.Object, timeProviderMock.Object, "TestTopic");

            using (producer)
            {
                await producer.ProduceNumbersAsync();
            }

            producerMock.Verify(v => v.ProduceAsync(It.IsAny<string>(), It.IsAny<Message<Null, string>>(), It.IsAny<CancellationToken>()), Times.Once);
            producerMock.Verify(v => v.Dispose(), Times.Once);

            timeProviderMock.Verify(v => v.GetMilisecondsForMinutes(It.IsAny<int>()), Times.Once);
            timeProviderMock.Verify(v => v.GetMilisecondsForSeconds(It.IsAny<int>()), Times.Once);

            loggerMock.Verify(v => v.LogException(It.IsAny<Exception>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public async Task ProduceNumersAsyncTest_PassNoData_ShouldCancel()
        {
            var producerMock = new Mock<IProducer<Null, string>>();
            producerMock.Setup(s => s.Dispose());

            var timeProviderMock = new Mock<ITimeProvider>();
            timeProviderMock.Setup(s => s.GetMilisecondsForMinutes(It.IsAny<int>())).Returns(6);
            timeProviderMock.Setup(s => s.GetMilisecondsForSeconds(It.IsAny<int>())).Returns(6000);

            var source = new CancellationTokenSource();
            var token = source.Token;
            source.Cancel();
            
            var producer = new NumberProducer(producerMock.Object, null, timeProviderMock.Object, "TestTopic");

            using (producer)
            {
                await producer.ProduceNumbersAsync(token);
            }
        }
    }
}
