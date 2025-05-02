---
languages:
- csharp
products:
- dotnet
- dotnet-orleans
- dotnet-aspire
page_type: sample
name: "Orleans Batch Processing sample app on Aspire"
urlFragment: "orleans-voting-sample-app-on-aspire"
description: "An Orleans sample demonstrating a batch processing system on Aspire."
---

# .NET Aspire Orleans batch processing sample app

This is a simple .NET app that shows how to use Orleans to perform batch processing

## Demonstrates

- How to use Orleans to perform managed batch processing
- How to use a "Governor" to throttle requests
    -  This currently only throttles the number of concurrent larger processes but in the future could include more fine grain throttling
- *NOTE* - This example incorporates a set of "services" that simulates processing time using Delays.  Future iterations will include actual data integration

## Future
- Add Management API to control the Govenor
    - Change Max Capacity on the fly

- Add data intregration
    - Change the processing engine to use "real" data
    - Incorporate retry, restart, persisted run state management, etc.

- Add Cluster Scaling
    - Add a mechanisim to scale a depoyment.  While the Govenor could limit the number of processes and help manage this, it would be good to be able to increase the scale and allow the Govenor to play a part in that

## Sample prerequisites

This sample is written in C# and targets .NET 9.0. It requires the [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later.

For more information about using Orleans, see the [Orleans documentation](https://learn.microsoft.com/dotnet/orleans).