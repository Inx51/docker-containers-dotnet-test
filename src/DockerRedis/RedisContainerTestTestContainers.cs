using Docker.DotNet;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using StackExchange.Redis;

namespace TestcontainersRedis;

public class RedisContainerTestFixtureTestContainters : IDisposable
{
    public RedisContainerTestFixtureTestContainters()
    {
        var container = new ContainerBuilder()
            .WithImage("redis:latest")
            .WithName("my-redis-testcontainer")
            .WithPortBinding("6379", "6379")
            .Build();

        container.StartAsync().Wait();
    }
    
    public void Dispose()
    {
        //Do nothing..
    }
}

public class RedisContainerTestTestContainers : IClassFixture<RedisContainerTestFixtureTestContainters>
{
    public RedisContainerTestTestContainers(RedisContainerTestFixtureTestContainters fixture)
    {

    }
    
    [Fact]
    public async Task Test1()
    {
        var multiplexor = await ConnectionMultiplexer.ConnectAsync("localhost:6379");

        multiplexor.IsConnected.Should().BeTrue();
    }
}