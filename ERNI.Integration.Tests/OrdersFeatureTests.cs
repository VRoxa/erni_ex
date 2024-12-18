using Bogus;
using ERNI.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Net.Http.Json;

namespace ERNI.Integration.Tests;

public class OrdersFeatureTests : IClassFixture<ApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrdersFeatureTests(ApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task OrderCanBeCreated()
    {
        async Task<string> CreateInitialUser()
        {
            var user = new Faker<UserDto>()
                .RuleFor(x => x.Name, f => f.Person.FullName)
                .RuleFor(x => x.Email, (f, x) => f.Internet.Email(x.Name))
                .Generate();

            var response = await _client.PostAsJsonAsync("/api/users", user);
            var content = await response.Content.ReadFromJsonAsync<UserDto>();
            return content!.Id!;
        }

        var userId = await CreateInitialUser();
        var order = new Faker<OrderDto>()
            .RuleFor(x => x.Amount, f => f.Random.Double(1, 50))
            .RuleFor(x => x.UserId, _ => userId)
            .Generate();

        var createdResponse = await _client.PostAsJsonAsync("/api/orders", order);
        createdResponse.EnsureSuccessStatusCode();

        var response = await _client.GetAsync($"/api/orders/{userId}");
        var orders = await response.Content.ReadFromJsonAsync<IList<OrderDto>>();

        orders.Should().NotBeEmpty();
    }
}
