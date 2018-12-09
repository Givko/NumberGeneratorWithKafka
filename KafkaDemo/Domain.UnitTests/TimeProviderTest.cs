using Domain.TimeProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Domain.UnitTests
{
    [TestClass]
    public class TimeProviderTest
    {
        [TestMethod]
        public void GetMilisecondsForMinutes_PassCorrectData_ShouldReturnCorrectData()
        {
            var minutes = 2;
            var expectedMiliseconds = 2 * 60 * 1000;

            var provider = new TimeProvider();

            var result = provider.GetMilisecondsForMinutes(minutes);

            Assert.AreEqual(expectedMiliseconds, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMilisecondsForMinutes_PassIncorrectData_ShouldThrowArgumentException()
        {
            var minutes = -2;

            var provider = new TimeProvider();

            var result = provider.GetMilisecondsForMinutes(minutes);
        }

        [TestMethod]
        public void GetMilisecondsForSeconds_PassCorrectData_ShouldReturnCorrectData()
        {
            var seconds = 2;
            var expectedMiliseconds = 2000;

            var provider = new TimeProvider();

            var result = provider.GetMilisecondsForSeconds(seconds);

            Assert.AreEqual(expectedMiliseconds, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMilisecondsForSeconds_PassIncorrectData_ShouldThrowArgumentException()
        {
            var seconds = -2;

            var provider = new TimeProvider();

            var result = provider.GetMilisecondsForSeconds(seconds);
        }
    }
}
