﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using Nybus.Logging;
using Ploeh.AutoFixture;
using ILogger = NLog.ILogger;
using LogLevel = Nybus.Logging.LogLevel;

namespace Tests.Logging
{
    [TestFixture]
    public class NLogLoggerTests
    {
        private IFixture fixture;
        private Mock<NLog.ILogger> mockLogger;

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture();

            mockLogger = new Mock<NLog.ILogger>();
        }

        [Test]
        [ExpectedException]
        public void Logger_is_required()
        {
            new NLogLogger(null);
        }

        private NLogLogger CreateSystemUnderTest()
        {
            return new NLogLogger(mockLogger.Object);
        }

        public IEnumerable<object> GetLogLevels()
        {
            yield return new object[] {LogLevel.Verbose, NLog.LogLevel.Trace};

            yield return new object[] {LogLevel.Debug, NLog.LogLevel.Debug};

            yield return new object[] {LogLevel.Information, NLog.LogLevel.Info};

            yield return new object[] {LogLevel.Error, NLog.LogLevel.Error};

            yield return new object[] {LogLevel.Critical, NLog.LogLevel.Fatal};

            yield return new object[] {LogLevel.Warning, NLog.LogLevel.Warn};

            yield return new object[] {(LogLevel) 0, NLog.LogLevel.Debug};
        }

        [Test]
        [TestCaseSource(nameof(GetLogLevels))]
        public void Log_levels_are_correctly_converted(LogLevel level, NLog.LogLevel expected)
        {
            var sut = CreateSystemUnderTest();

            var message = fixture.Create<string>();

            IDictionary<string, object> dictionary = new Dictionary<string, object>
            {
                ["message"] = message
            };

            sut.Log(level, dictionary, exception: null);

            mockLogger.Verify(p => p.Log(It.Is<LogEventInfo>(lei => lei.Level == expected)), Times.Once);
        }

        [Test]
        public void Data_is_added_as_properties()
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>
            {
                ["message"] = fixture.Create<string>(),
                ["text"] = fixture.Create<string>()
            };

            var sut = CreateSystemUnderTest();

            sut.Log(LogLevel.Information, dictionary, exception: null);

            mockLogger.Verify(p => p.Log(It.Is<LogEventInfo>(lei => string.Equals((string)lei.Properties["text"], (string)dictionary["text"]))), Times.Once);
        }

        [Test]
        public void Message_is_added_as_message()
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>
            {
                ["message"] = fixture.Create<string>(),
                ["text"] = fixture.Create<string>()
            };

            var sut = CreateSystemUnderTest();

            sut.Log(LogLevel.Information, dictionary, exception: null);

            mockLogger.Verify(p => p.Log(It.Is<LogEventInfo>(lei => string.Equals((string)dictionary["message"], lei.Message))), Times.Once);
        }

        [Test]
        public void Exception_information_are_added_to_log()
        {
            IDictionary<string, object> dictionary = new Dictionary<string, object>
            {
                ["message"] = fixture.Create<string>(),
                ["text"] = fixture.Create<string>()
            };

            var sut = CreateSystemUnderTest();

            try
            {
                Exception innerException = new Exception("This is an inner exception");
                throw new Exception("This is a test exception", innerException);
            }
            catch (Exception ex)
            {
                sut.Log(LogLevel.Error, dictionary, ex);
            }

            mockLogger.Verify(p => p.Log(It.Is<LogEventInfo>(lei => string.Equals((string)dictionary["message"], lei.Message))), Times.Once);
            mockLogger.Verify(p => p.Log(It.Is<LogEventInfo>(lei => string.Equals((string)lei.Properties["error-method"], nameof(Exception_information_are_added_to_log)))), Times.Once);
        }

        [Test]
        [TestCaseSource(nameof(GetLogLevels))]
        public void IsEnabled_forwards_to_NLog_logger(LogLevel level, NLog.LogLevel expected)
        {
            var sut = CreateSystemUnderTest();

            sut.IsEnabled(level);

            mockLogger.Verify(p => p.IsEnabled(expected), Times.Once);
        }
    }
}
