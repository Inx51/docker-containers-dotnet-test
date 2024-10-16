using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using StackExchange.Redis;

namespace DockerNetRedis;

public class RedisContainerTestFixtureDockerDotnet : IDisposable
{
    private readonly string _containerId;
    private readonly DockerClient _dockerClient;
    
    public RedisContainerTestFixtureDockerDotnet()
    {
        _dockerClient = new DockerClientConfiguration().CreateClient();
     
        var containerName = "my-redis-dockerdotnet";
        
        var container = _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
        {
            Image = "redis:latest",
            Name = containerName,
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                {
                    "6379", default
                }
            },
            HostConfig  = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    {"6379", new List<PortBinding> { new PortBinding { HostPort = "6379" } } }
                },
                PublishAllPorts = true
            }
        }).Result;
        
        _containerId = container.ID;

        _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()).Wait();
    }
    
    public void Dispose()
    {
        _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters()).Wait();
        _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters()).Wait();
    }
}

public class RedisContainerTestDockerDotnet : IClassFixture<RedisContainerTestFixtureDockerDotnet>
{
    public RedisContainerTestDockerDotnet(RedisContainerTestFixtureDockerDotnet fixture)
    {

    }
    
    [Fact]
    public async Task Test1()
    {
        var multiplexor = await ConnectionMultiplexer.ConnectAsync("localhost:6379");

        multiplexor.IsConnected.Should().BeTrue();
    }
}