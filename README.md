# NReJSON

[![Build Status](https://github.com/tombatron/NReJSON/actions/workflows/dotnet.yml/badge.svg)](https://github.com/tombatron/NReJSON/actions/workflows/dotnet.yml)

## Overview

NReJSON is a series of extension methods for the [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) library that will enable you to interact with the [Redis](https://redis.io/) module [RedisJSON](https://github.com/RedisJSON/RedisJSON). This is made possible by the `Execute` and `ExecuteAsync` methods already present in the SE.Redis library. 

The following blog post by Marc Gravell was the inspiration behind this: [StackExchange.Redis and Redis 4.0 Modules](https://blog.marcgravell.com/2017/04/stackexchangeredis-and-redis-40-modules.html). He even has an example of how to call a command from the RedisJSON module! 

## Installation

`PM> Install-Package NReJSON -Version 3.5.0`

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

## Major Changes in Version 4.0

All deprecated RedisJson commands have been removed. 

Changed param array in `JsonArrayAppend` and `JsonArrayAppendAsync` to be an array of type `object`.

## Major Changes in Version 3.0

In version 3.0 support for serialization and deserialization was added in the form of new generic overloads for the following extension methods:

- JsonGet/JsonGetAsync
- JsonMultiGet/JsonMultiGetAsync
- JsonSet/JsonSetAsync
- JsonArrayPop/JsonArrayPopAsync
- JsonIndexGet/JsonIndexGetAsync

In order to leverage the serialization/deserialization support you must create an implementation of the `ISerializerProxy` interface. The following is a sample implementation taken from the integration tests:

```csharp
public sealed class TestJsonSerializer : ISerializerProxy
{
    public TResult Deserialize<TResult>(RedisResult serializedValue) =>
        JsonSerializer.Deserialize<TResult>(serializedValue.ToString());

    public string Serialize<TObjectType>(TObjectType obj) =>
        JsonSerializer.Serialize(obj);
}
```

Once that is implemented it can be assigned to the static property `SerializerProxy` found in the static class `NReJSONSerializer`. The following is an example of how to do this: 

```csharp
NReJSONSerializer.SerializerProxy = new TestJsonSerializer();

```

If this isn't setup before leveraging the extension methods that make use of it, an `NReJSONException` will be thrown in order to remind you that it needs to be done. 

The result type for the following methods has change to `OperationResult`:

- JsonSet/JsonSetAsync
- JsonIndexAdd/JsonIndexAddAsync
- JsonIndexDelete/JsonIndexDeleteAsync

The `OperationResult` is a struct that will return and contain whether or not the operation was successful, and will also contain the raw response from Redis. This type is implicitly convertable to `bool` so it can be used in operations like:

```csharp
var result = await db.JsonSetAsync(key, obj);

if (result)
{
   // Do something if there was success...
} 
else
{
   // Do something if there wasn't success...
}
```

Last but not least, we have a new result type called `IndexedCollection` which is now returned by the overload which deserializes results on the following method:

- JsonIndexGet/JsonIndexGetAsync

This type is generic and allows for dealing with the result of the `JsonIndexGet` and `JsonIndexGetAsync` as a dictionary and as a collection. 

### Examples

Sam Dzirasa has authored a blog post full of practical examples of how to use NReJSON in an application:

[Using RedisJson](https://blog.alumdb.org/using-redisjson/)

Also, in this repository there are a suite of integration tests that should be sufficent to serve as examples on how to use all supported RedisJSON commands.

[Integration Tests](https://github.com/tombatron/NReJSON/blob/master/NReJSON.IntegrationTests/DatabaseExtensionAsyncTests.cs)

