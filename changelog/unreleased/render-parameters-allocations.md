type: internal

`LogCommandText` and `LogDbCommand` allocate less when the logger is enabled: no exclusion set is built when nothing is excluded, excluded names are matched in place instead of copying each parameter name, and `LogDbCommand` no longer repeats its argument checks.
