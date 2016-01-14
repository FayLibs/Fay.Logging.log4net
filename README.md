Fay Logging log4net
===========

What is it?
-----------

It is a [Fay Logging][FayLog] facade for the [log4net logging framework][log4net]. This facade povides a simple delegate logging API making logging easier, while helping to make sure any of the code required to generate a log message is not executed unless the logging level is within scope.

Quick Start
---
Below is a simplified example for logging. A more robust implementation may use a service locator or dependency inject framework to initialize the logger.

```cs
    // Initialize logger someplace
    IDelegateLogger<string> logger = new L4NSimpleLogger(((ILoggerWrapper)LogManager.GetLogger("MyLogger")).Logger);

    // Use logger as needed
    logger.Critical(() => string.Format("Some text blah {0} blah", "blah"));

    logger.Verbose(() =>
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Foo");
        builder.Append(" ");
        builder.Append("Bar");
        return builder.ToString();
    });

    // When application is done needing the logger its recommend to dispose it
    logger.Dispose();
```

[FayLog]:  https://github.com/FayLibs/Fay.Logging
[log4net]: http://logging.apache.org/log4net/

## Downloads

Fey Logging is available via NuGet:

- [Fay Logging log4net] (https://www.nuget.org/packages/Fay.Logging.log4net/)
