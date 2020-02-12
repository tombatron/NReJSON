# NReJSON

[![Build status](https://ci.appveyor.com/api/projects/status/fwah8euqttecil89?svg=true)](https://ci.appveyor.com/project/tombatron/nrejson)

## Overview

NReJSON is a series of extension methods for the [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) library that will enable you to interact with the [Redis](https://redis.io/) module [RedisJSON](https://github.com/RedisJSON/RedisJSON). This is made possible by the `Execute` and `ExecuteAsync` methods already present in the SE.Redis library. 

The following blog post by Marc Gravell was the inspiration behind this: [StackExchange.Redis and Redis 4.0 Modules](https://blog.marcgravell.com/2017/04/stackexchangeredis-and-redis-40-modules.html). He even has an example of how to call a command from the RedisJSON module! 

## Installation

`PM> Install-Package NReJSON -Version 1.0.0`

## Usage

I'm assuming that you already have the [RedisJSON](https://github.com/RedisJSON/RedisJSON) module installed on your Redis server. 

You can verify that the module is installed by executing the following command:

`MODULE LIST`

If RedisJSON is installed you should see output similar to the following:

```
1) 1) "name"
   2) "ReJSON"
   3) "ver"
   4) (integer) 10001
```

(The version of the module installed on your server obviously may vary.)

### Examples

In this repository there are a suite of integration tests that should be sufficent to serve as examples on how to use all supported RedisJSON commands.

[Integration Tests](https://github.com/tombatron/NReJSON/blob/master/NReJSON.IntegrationTests/DatabaseExtensionAsyncTests.cs)

## Major Issues

The extension method for `JSON.STRAPPEND` doesn't work, because honestly I'm not really sure how to use the command. This is actually the first issue for the project which can be found [here](https://github.com/tombatron/NReJSON/issues/1). 
