type: fix

The three-argument `EntityFramework6Logging.AddLoggingInterceptors(logger, excludedParameterNames, level)` overload no longer declares default values. They never took effect: every call that leaves an argument out binds to one of the `(logger)`, `(logger, level)` or `(logger, excludedParameterNames)` overloads. Compiled callers are unaffected, and calls that compile today bind to the same overload (InspectCode S3427, code-scanning alerts 342-344).
