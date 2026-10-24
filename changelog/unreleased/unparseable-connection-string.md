Type: fix

`LogDbConnection` no longer throws `ArgumentException` when the connection string does not conform to the generic `key=value;` grammar; it logs the placeholder `<unparseable connection string redacted>` instead of the raw string.
