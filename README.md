# Everland Code Challenge

[![N|Solid](https://playeverland.com/everlandlogo1x.png)](https://playeverland.com/)

## Overview
My implementation of the challenge is a REST style API built on top of ASP.NET Core with basic HTTP authentication, as well as an "API Key" method for requests that originate from internals (such as game servers). I chose to use a RESTful API because it uses standard protocols (HTTP, JSON, etc) and is very flexible and consistent; allowing the API to be used from multiple frameworks/languages (if necessary).

I implemented it in a way where it can be used by both game clients and game servers because I was initially unsure how the API was intended to be used, but then further considered that if clients were the ones requesting these resources, then there is not a single point of failure, _and_ it becomes easier to load-balance and scale, but also considered that game-servers may have a reason to request this information for themselves.

The general format for URIs is currently:
`/api/v1/{ENTITY_TYPE}/...` where `ENTITY_TYPE` can currently be `account`. The `v1` is there such that, in the future, a complete reworking can be done without breaking outdated clients (They can be written to handle deprecation errors, and then force the user to update, for example). The API uses standard HTTP verbs (GET, POST, PUT, DELETE) to indicate intended action upon resources, and HTTP status codes are used alongside a JSON body that further explains any errors. The JSON body is implemented through a `ApiResponse<T>` and is always of the format:
```json
{
 "success": true,
 "errors": [ ],
 "data": { }
}
```

## Libraries Used
- AspNetCore:
  - I chose to use Asp.Net as the foundation of my WebAPI because out of the few alternatives (Nancy, ServiceStack), Asp.Net provides everything you should need in a single dependency (Logging, Caching, Dependency Injection, Data Persistence), and is more common in production environments. Nancy being extremely lightweight would mean I would spend more development time writing things that Asp.Net already provides, and ServiceStack has recently become a paid solution. Plus, a major advantage is that Asp.Net is created by Microsoft (who maintain C#, the CLI/CLR, and .NET), and so new C# language-features are quick to be made use of, and it is frequently updated to the latest versions of .Net Standard and .Net Core.
- BCrypt-Core
  - The most downloaded BCrypt implementation on Nuget with explicit support/intent for .Net Core. Provided an easy-to-use interface to hash/verify passwords, and the BCrypt algorithm is still a secure option (although this could be changed for scrypt or argon2 for more memory-hardness, but those haven't been around as long and realistically, all three options are "secure enough").
- Pomelo.EntityFrameworkCore.MySql
  - I chose to use the Pomelo MySQL connector over the official MySQL one because the Pomelo library is open-source, is in active development, and has a good track record of responding to and fixing issues. Plus, there was a time where it was the only option, which influenced my decision as I've used it before.
- StackExchange.Redis
  - One of two libraries [recommended by Redis](https://redis.io/clients) for use within a C# application. Described as being written for "very high performance needs".
- xUnit
  - A unit-testing framework that I have prior experience using, although, any of the available ones would suffice.
- Moq
  - Allowed me to create mocked-up implementations of services to test controllers.
- MockQueryable.Moq
  - Allowed me to more easily test the AccountContext without "reinventing the wheel".

## How to Run the Demonstration
- A local/remote MySQL-compatible server is required for data persistence. The connection string can be found in `EverlandApi/appsettings.json`.
- After configuring/starting the MySQL server, you will have to go to the `EverlandApi/` directory in a CLI (such as CMD, or Powershell) and execute: `dotnet ef database update` to apply migrations to the database.
- A local/remote Redis server for request caching, which can also be configured through `EverlandApi/appsettings.json` by adding a `Redis` section with a `ConnectionString` property.
  - This is optional, but not setting it will cause the first request to have a large response time (as it attempts to connect to Redis).
  - I recommend using [this](https://github.com/MicrosoftArchive/redis/releases/tag/win-3.0.504) on Windows.

There are a set of Python3 scripts available in the `scripts/` directory that allow you to interact with the API. Make sure to set the `base_api_url` in `scripts/config.py` to the URI that Visual Studio begins hosting on.

### Example Config
```json
{
 "Logging": {
  "LogLevel": {
   "Default": "Warning"
  }
 },
 "AllowedHosts": "*",
 "ConnectionStrings": {
  "Default": "server=localhost;database=everland_dev;uid=root;"
 },
 "Redis": {
  "ConnectionString": "localhost:6380"
 }
}
```

## Things I would Change
I understand that this section was not asked of me, but I feel like there are parts of my code that I'm personally unhappy with even though they function appropriately, and so I would like to highlight these.

- Use local and distributed logging and error reporting (Potentially Sentry?).
- Look into using `AddResponseCaching()` and `AddDistributedMemoryCache()` instead of a custom `RedisService`.
- Rework authentication entirely:
    - Instead of making filters perform the actual authentication, use a middleware and a`IIdentity` implementation that provides access to an `Account` instance.
    - Use filters such as `RequiresAuthentication` to restrict access to certain actions by checking that an `IIdentity` instance is available on the request.
    - Implement an abstraction for "Account" vs "API Key" authentication. My current idea is to implement some kind of virtual `HasPermission<TPermission>()` or `IPermissionHolder`.
    - Add OAuth and JWT tokens and the appropriate controllers to authenticate and begin sessions.
- General refactoring and code clean-up.
- Rate limiting on certain actions that are easy to spam (such as Create).
