# Everland Code Challenge

[![N|Solid](https://playeverland.com/everlandlogo1x.png)](https://playeverland.com/)

## What you need to have

As our new .NET backend (eventually Unity) developer you will need to design, implement and maintain powerful and perfoming APIs on .NET Core 2+ and  Websocket based servers using caches and persistent data storages.

You need to know and have been working with :

* .NET framework (better if Core)
* Web APIs
* Redis
* MySql
* JWT and OAuth2
* WebSockets
* Dependency Injection
* xUnit & Mocking frameworks
* Unity
* Swaggeer
* Git
 
## The challenge

You need to create a WebApi in .NET Core that :
* Expose a CRUD endpoint over Account resource.
* Uses Redis to cache the GET verb on the Account resource (you can choose the expirity).
* Uses MySQL to persist the Accounts.
* You need to isolate your Account domain entity from the model that you are going to return as response.
* Take advantage of the async/await keywords.
* You only can create an account that has a unique email and the account name is greater than 6 characters.
* Unit testing (try to show us how to mock dependency calls)

## Deliver

To deliver the code you have to send us a .rar file with all the thing that we need to run the code and test it.

Good luck & Have fun !!
