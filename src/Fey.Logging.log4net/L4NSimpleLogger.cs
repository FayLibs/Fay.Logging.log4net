using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Fay.Logging;
using log4net;
using log4net.Core;

namespace Fey.Logging.Log4Net
{
    public sealed class L4NSimpleLogger : DelegateLogger<object>
    {
        private readonly Type _callerStackBoundaryDeclaringType;
        
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
        
        private ILogger MyLogger { get; set; }

        public L4NSimpleLogger(ILogger logger)
        {
            Contract.Requires<ArgumentNullException>(logger != null);            
            Contract.Ensures(MyLogger != null);
            
            MyLogger = logger;
            _callerStackBoundaryDeclaringType = typeof (LogImpl);
        }
        
        public override bool IsSeverityInScope(LogSeverity severity, Func<object> messageDelegate = null)
        {
            return MyLogger.IsEnabledFor(_logSeverityToLevelMap[severity]);
        }

        protected override void Write(LogSeverity severity, Func<object> messageDelegate)
        {
            if (!IsSeverityInScope(severity, null))
                return;

            object message = messageDelegate?.Invoke();

            if (message == null)
                return;

            MessageAndException msgAndEx = message as MessageAndException;
            Exception ex = null;

            if (msgAndEx != null)
            {
                ex = msgAndEx.Ex;
                message = msgAndEx.MessageDelegate?.Invoke();
            }

            MyLogger.Log(_callerStackBoundaryDeclaringType, _logSeverityToLevelMap[severity], message, ex);
        }

        protected override Func<object> InjectExceptionIntoMessageDelegate(Func<object> messageDelegate, Exception ex)
        {
            if (ex == null)
                return messageDelegate;
            
            Func<MessageAndException> msg = () => new MessageAndException(messageDelegate, ex);

            return msg;
        }

        private class MessageAndException
        {
            public Func<object> MessageDelegate { get; }
            public Exception Ex { get; }

            public MessageAndException(Func<object> messageDelegate, Exception ex)
            {                
                MessageDelegate = messageDelegate;
                Ex = ex;
            }
        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            lock (this)
            {
                if (disposing)
                {
                    MyLogger.Repository.Shutdown();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
