Type: fix

`LogCommandText` and `LogDbCommand` now render parameter values with the invariant culture, so a `double`, `decimal` or `DateTime` value logs the same text on every host regardless of its current culture.
