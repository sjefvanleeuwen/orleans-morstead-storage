# Morstead Orleans Storage Provider

![morstead](./doc/img/morstead.svg)

[![nuget](https://img.shields.io/nuget/v/Orleans.Persistence.Morstead)](https://www.nuget.org/packages/Orleans.Persistence.Morstead/)

## What is it?

An embedded persistent storage provider for Microsoft Orleans for development purposes. It is based on LiteDB. Currently targeted to Developers that seek simple persistent storage as an 'in place' replacement for volatile memory storage that is shipped with Orleans.

## Status

The current MVP provides the following:

**Grain Persistence**
* ReadStateAsync
* WriteStateAsync
* ~~ClearStateAsync~~ (soon)

The next items that will be worked on in the roadmap (ordered by priority):

* Reminders storage provider
* Cluster storage provider

Items that might be worked on in the future but not on the roadmap:

* Transactions
* Stream providers

This project is open to pull requests.

## Setup

The setup comparable to other orleans storage providers and is build using .NET Standard 2.0.3

### Install nuget package

```
dotnet add package Orleans.Persistence.Morstead --version 0.0.3-alpha
```
or visit the nuget for other options on: https://www.nuget.org/packages/Orleans.Persistence.Morstead

### Adding the provider to your project

configure using:

```csharp
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder
                .AddMorsteadGrainStorage(name: "unit-test");
        }
```

## Notes

The storage model and configuration options might change in next releases.