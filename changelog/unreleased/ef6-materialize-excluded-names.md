type: internal

The EF6 command interceptor materializes the excluded parameter names once, so a lazy sequence passed to `AddLoggingInterceptors` is no longer re-evaluated for every command.
