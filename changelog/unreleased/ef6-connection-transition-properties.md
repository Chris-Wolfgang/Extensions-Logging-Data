type: feature

The EF6 connection interceptor's "opened" and "closed" entries now carry `{DataSource}` and `{Database}` as structured properties, and `AddLoggingInterceptors` documents that registered loggers live for the whole process.
