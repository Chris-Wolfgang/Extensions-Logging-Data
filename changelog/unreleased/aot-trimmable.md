Type: feature

`Wolfgang.Extensions.Logging.Data` now sets `IsTrimmable` and `IsAotCompatible` on net10.0, so the package is marked trimmable and the trim / Native AOT analyzers check it on every build.
