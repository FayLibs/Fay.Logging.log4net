using System;
using System.Collections.Generic;
using System.Reflection;
using Fay.Logging;
using Fey.Logging.Log4Net;
using log4net.Core;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Shouldly;
using Xunit;

namespace Logging.log4net.UnitTests
{
    public class L4NSimpleLoggerUnitTests
    {

        [Theory]
        [InlineAutoData("Critical", LogSeverity.Critical, L4NLevel.Critical, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, L4NLevel.Error, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, L4NLevel.Warn, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, L4NLevel.Info, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, L4NLevel.Verbose, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, L4NLevel.All, true, null)]
        [InlineAutoData("Critical", LogSeverity.Critical, L4NLevel.Off, false, null)]
        [InlineAutoData("Error", LogSeverity.Error, L4NLevel.Critical, false, null)]
        [InlineAutoData("Error", LogSeverity.Error, L4NLevel.Error, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, L4NLevel.Warn, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, L4NLevel.Info, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, L4NLevel.Verbose, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, L4NLevel.All, true, null)]
        [InlineAutoData("Error", LogSeverity.Error, L4NLevel.Off, false, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, L4NLevel.Critical, false, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, L4NLevel.Error, false, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, L4NLevel.Warn, true, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, L4NLevel.Info, true, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, L4NLevel.Verbose, true, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, L4NLevel.All, true, null)]
        [InlineAutoData("Warning", LogSeverity.Warning, L4NLevel.Off, false, null)]
        [InlineAutoData("Information", LogSeverity.Information, L4NLevel.Critical, false, null)]
        [InlineAutoData("Information", LogSeverity.Information, L4NLevel.Error, false, null)]
        [InlineAutoData("Information", LogSeverity.Information, L4NLevel.Warn, false, null)]
        [InlineAutoData("Information", LogSeverity.Information, L4NLevel.Info, true, null)]
        [InlineAutoData("Information", LogSeverity.Information, L4NLevel.Verbose, true, null)]
        [InlineAutoData("Information", LogSeverity.Information, L4NLevel.All, true, null)]
        [InlineAutoData("Information", LogSeverity.Information, L4NLevel.Off, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, L4NLevel.Critical, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, L4NLevel.Error, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, L4NLevel.Warn, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, L4NLevel.Info, false, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, L4NLevel.Verbose, true, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, L4NLevel.All, true, null)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, L4NLevel.Off, false, null)]
        [InlineAutoData("CriticalException", LogSeverity.Critical, L4NLevel.Critical, true)]
        [InlineAutoData("CriticalException", LogSeverity.Critical, L4NLevel.Critical, false, null, null)]
        [InlineAutoData("CriticalException", LogSeverity.Critical, L4NLevel.Off, false)]
        [InlineAutoData("ErrorException", LogSeverity.Error, L4NLevel.Error, true)]
        [InlineAutoData("ErrorException", LogSeverity.Error, L4NLevel.Critical, false)]
        [InlineAutoData("ErrorException", LogSeverity.Error, L4NLevel.Error, false, null, null)]
        [InlineAutoData("ErrorException", LogSeverity.Error, L4NLevel.Error, true)]
        [InlineAutoData("ErrorException", LogSeverity.Error, L4NLevel.Off, false)]
        [InlineAutoData("Verbose", LogSeverity.Verbose, L4NLevel.Verbose, false, null, null)]
        [InlineAutoData("ErrorException", LogSeverity.Error, L4NLevel.Verbose, true, null)]
        public void LoggerLogProvidedExpectedDataIfInScopeAndDataIsValid(string methodName, LogSeverity expectedLogSeverity, L4NLevel thresholdLevel, bool isExpectedToBeLogged, Exception expectedException, string expectedMessage)
        {
            // Arrange
            Mock<ILogger> iLoggerMock = CreateILoggerMock(thresholdLevel);
            IDelegateLogger<object> sut = new L4NSimpleLogger(iLoggerMock.Object);
            Times times = isExpectedToBeLogged ? Times.Once() : Times.Never();
            Func<string> writeLogEntry = () => expectedMessage;

            // Act
            if (methodName.EndsWith("Exception"))
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry, expectedException });
            else
                sut.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, sut, new object[] { writeLogEntry });
            
            // Assert
            iLoggerMock.Verify(l => l.Log(It.IsAny<Type>(), _logSeverityToLevelMap[expectedLogSeverity], expectedMessage, expectedException), times);
        }

        [Theory]
        [InlineData(L4NLevel.Critical, LogSeverity.Critical, true)]
        [InlineData(L4NLevel.Critical, LogSeverity.Error, false)]
        [InlineData(L4NLevel.Critical, LogSeverity.Warning, false)]
        [InlineData(L4NLevel.Critical, LogSeverity.Information, false)]
        [InlineData(L4NLevel.Critical, LogSeverity.Verbose, false)]
        [InlineData(L4NLevel.Error, LogSeverity.Critical, true)]
        [InlineData(L4NLevel.Error, LogSeverity.Error, true)]
        [InlineData(L4NLevel.Error, LogSeverity.Warning, false)]
        [InlineData(L4NLevel.Error, LogSeverity.Information, false)]
        [InlineData(L4NLevel.Error, LogSeverity.Verbose, false)]
        [InlineData(L4NLevel.Warn, LogSeverity.Critical, true)]
        [InlineData(L4NLevel.Warn, LogSeverity.Error, true)]
        [InlineData(L4NLevel.Warn, LogSeverity.Warning, true)]
        [InlineData(L4NLevel.Warn, LogSeverity.Information, false)]
        [InlineData(L4NLevel.Warn, LogSeverity.Verbose, false)]
        [InlineData(L4NLevel.Info, LogSeverity.Critical, true)]
        [InlineData(L4NLevel.Info, LogSeverity.Error, true)]
        [InlineData(L4NLevel.Info, LogSeverity.Warning, true)]
        [InlineData(L4NLevel.Info, LogSeverity.Information, true)]
        [InlineData(L4NLevel.Info, LogSeverity.Verbose, false)]
        [InlineData(L4NLevel.Verbose, LogSeverity.Critical, true)]
        [InlineData(L4NLevel.Verbose, LogSeverity.Error, true)]
        [InlineData(L4NLevel.Verbose, LogSeverity.Warning, true)]
        [InlineData(L4NLevel.Verbose, LogSeverity.Information, true)]
        [InlineData(L4NLevel.Verbose, LogSeverity.Verbose, true)]
        [InlineData(L4NLevel.All, LogSeverity.Critical, true)]
        [InlineData(L4NLevel.All, LogSeverity.Error, true)]
        [InlineData(L4NLevel.All, LogSeverity.Warning, true)]
        [InlineData(L4NLevel.All, LogSeverity.Information, true)]
        [InlineData(L4NLevel.All, LogSeverity.Verbose, true)]
        [InlineData(L4NLevel.Off, LogSeverity.Critical, false)]
        [InlineData(L4NLevel.Off, LogSeverity.Error, false)]
        [InlineData(L4NLevel.Off, LogSeverity.Warning, false)]
        [InlineData(L4NLevel.Off, LogSeverity.Information, false)]
        [InlineData(L4NLevel.Off, LogSeverity.Verbose, false)]
        public void IsSeverityInScopeReturnsValid(L4NLevel thresholdLevel, LogSeverity logSeverity, bool expected)
        {
            // Arrange
            Mock<ILogger> iLoggerMock = CreateILoggerMock(thresholdLevel);
            IDelegateLogger<object> sut = new L4NSimpleLogger(iLoggerMock.Object);

            // Act
            bool result = sut.IsSeverityInScope(logSeverity, null);

            // Assert
            result.ShouldBe(expected);
        }

        public enum L4NLevel
        {
            Off,
            Critical,
            Error,
            Warn,
            Info,
            Verbose,
            All,
        }

        private readonly IDictionary<L4NLevel, Level> _levelToLevelMap = new Dictionary<L4NLevel, Level>
        {
            {L4NLevel.Off, Level.Off},
            {L4NLevel.Critical, Level.Critical},
            {L4NLevel.Error, Level.Error},
            {L4NLevel.Warn, Level.Warn},
            {L4NLevel.Info, Level.Info},
            {L4NLevel.Verbose, Level.Verbose},
            {L4NLevel.All, Level.All},
        };

        private readonly IDictionary<LogSeverity, Level> _logSeverityToLevelMap = new Dictionary<LogSeverity, Level>
        {
            {LogSeverity.Off, Level.Off},
            {LogSeverity.Critical, Level.Critical},
            {LogSeverity.Error, Level.Error},
            {LogSeverity.Warning, Level.Warn},
            {LogSeverity.Information, Level.Info},
            {LogSeverity.Verbose, Level.Verbose},
            {LogSeverity.All, Level.All},
        };

        private Mock<ILogger> CreateILoggerMock(L4NLevel thresholdLevel)
        {
            Mock<ILogger> iLoggerMock = new Mock<ILogger>();
            iLoggerMock.Setup(l => l.IsEnabledFor(It.IsAny<Level>())).Returns((Level level) => IsEnabledForStub(level, _levelToLevelMap[thresholdLevel]));
            return iLoggerMock;
        }

        private bool IsEnabledForStub(Level logLevel, Level thresholdLevel)
        {
            return logLevel >= thresholdLevel;
        }
    }
}

