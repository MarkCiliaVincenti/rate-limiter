# rate-limiter
Custom action filter for applying rate limit on ASP.NET Core API

## Applying rate limit
To apply rate limit filter globally, add following code in `Startup.cs`
```
services.AddRateLimit();

services.AddMvc(options =>
{
    options.Filters.Add<RateLimitFilter>();
});
```
For applying filter on specific action:
```
[HttpGet]
[ServiceFilter(typeof(RateLimitFilter))]
public string Get()
{
    return "Hello World!";
}
```

## Limitations
* Limits incoming requests per endpoint based on clinet IP address only.
* Can't apply default limit for endpoints at the moment,
* Uses in-memory cache for storing data - won't work for distributed systems. 
* Limits requests based on time window only; other limiting factors (e.g. location) hasn't been taken into account.
* Limiting requests per time interval with fixed precision of 1 second. This will create large memory foot prints for larger window.

## Considerations
* Configurable rate limiting policy per endpoint per client
* Allows having multiple rules per endpoint
* Applies individual locks to prevent access by multiple threads 
* Sliding window technique is used for limiting requests


# Codebase

## Pre-requisit
The project has been developed using .Net Core 3.1 on Visual Studio 2019. Following tool(s) are required to run the project on local machine:
* .Net Runtime
* Visual Studio (optional)

## Running the application
To run the web project from command line:
* Go to project directory `\rate-limiter\src\RateLimit.Web`
* run command `dotnet run`

To run the tests from command line:
* Go to project directory `\rate-limiter\src\RateLimit.Core.Tests`
* run command `dotnet test`

_Alternatively the project/tests can be directly run from visual studio._

Note: There is a `Seed()` method in `Program.cs` for storing initial test data

## Code Structure
There are following three projects:
* **RateLimit.Core** - this is the main library containing a custom action filter to apply rate limiting on API endpoints.
* **RateLimit.Core.Tests** - this is the test project for rate limit component.
* **RateLimit.Web** - this is a sample Web API project for testing rate limiting filter.



