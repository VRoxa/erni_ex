using Bogus;
using ERNI.Core;
using ERNI.Entities;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace ERNI.Integration.Tests;

public class UsersFeatureTests : IClassFixture<ApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UsersFeatureTests(ApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UserCanBeCreated()
    {
        var user = new Faker<UserDto>()
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.Name))
            .Generate();

        var response = await _client.PostAsJsonAsync("/api/users", user);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();

        createdUser.Should().NotBeNull();
        createdUser!.Id.Should().NotBeEmpty();
        createdUser!.Name.Should().Be(user.Name);
        createdUser!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task UsersCanBeQueried()
    {
        async Task<string> CreateUser(UserDto user)
        {
            var response = await _client.PostAsJsonAsync("/api/users", user);
            var content = await response.Content.ReadFromJsonAsync<UserDto>();
            return content!.Id!;
        }

        var users = new Faker<UserDto>()
            .RuleFor(x => x.Name, f => f.Person.FullName)
            .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.Name))
            .Generate(10);

        var ids = await Task.WhenAll(users.Select(CreateUser));

        var response = await _client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var queriedUsers = await response.Content.ReadFromJsonAsync<IList<UserDto>>();
        queriedUsers!.Select(x => x.Id).Should().Contain(ids);
    }

    [Fact]
    public async Task UsersCanBeDeleted()
    {
        var response = await _client.GetAsync("/api/users");
        var existingUsers = await response.Content.ReadFromJsonAsync<IList<UserDto>>();

        var deleteResponse = await _client.DeleteAsync($"/api/users/{existingUsers!.First().Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}