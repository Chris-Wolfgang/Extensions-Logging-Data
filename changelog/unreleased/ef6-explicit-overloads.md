type: feature

`EntityFramework6Logging.AddLoggingInterceptors` gains three overloads with no optional parameters: `(logger)`, `(logger, level)` and `(logger, excludedParameterNames)`, matching the explicit-overload style of the core package. The existing overload with optional parameters is unchanged.
