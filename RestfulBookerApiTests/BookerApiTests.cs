using System.Net;
using System.Text.Json;
using RestSharp;

namespace RestfulBookerApiTests;

public class BookerConfigurationTests
{
    [Test]
    public void Configuration_UsesDefaultBaseUrl_WhenEnvironmentIsMissing()
    {
        var config = new BookerConfiguration(BookerConfiguration.DefaultBaseUrl, null, null);
        Assert.That(config.BaseUrl, Is.EqualTo(BookerConfiguration.DefaultBaseUrl));
        Assert.That(config.HasCredentials, Is.False);
    }

    [Test]
    public void Configuration_UsesEnvironmentOverride_WhenProvided()
    {
        const string value = "https://example.test/api";
        var config = new BookerConfiguration(value, "booker-user", "booker-pass");
        Assert.That(config.BaseUrl, Is.EqualTo(value));
        Assert.That(config.UserName, Is.EqualTo("booker-user"));
        Assert.That(config.Password, Is.EqualTo("booker-pass"));
    }

    [Test]
    public void Configuration_RejectsInvalidBaseUrl()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => BookerConfiguration.ValidateBaseUrl("not-a-valid-url"));
        Assert.That(ex!.Message, Does.Contain("BOOKER_BASE_URL"));
    }
}

public class RetryPolicyTests
{
    [TestCase(HttpStatusCode.OK, false)]
    [TestCase(HttpStatusCode.NotFound, false)]
    [TestCase(HttpStatusCode.Unauthorized, false)]
    [TestCase(HttpStatusCode.InternalServerError, true)]
    [TestCase(HttpStatusCode.BadGateway, true)]
    [TestCase(HttpStatusCode.ServiceUnavailable, true)]
    public void RetryPolicy_OnlyRetriesTransientResponses(HttpStatusCode statusCode, bool expected)
    {
        var actual = RetryPolicy.ShouldRetry(statusCode, null, 0);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void RetryPolicy_StopsAfterTwoAttempts()
    {
        Assert.That(RetryPolicy.ShouldRetry(HttpStatusCode.InternalServerError, null, 2), Is.False);
    }
}

public class BookingApiTests
{
    private BookerConfiguration _configuration = null!;
    private BookerClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _configuration = BookerConfiguration.FromEnvironment();
        _client = new BookerClient(_configuration);
    }

    [Test]
    public void GetUnknownBooking_ReturnsNotFound()
    {
        var response = _client.GetBooking(int.MaxValue / 17);
        Assert.That((int)response.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public void CreateBooking_WithoutCredentials_IsSkipped()
    {
        if (!_configuration.HasCredentials)
        {
            Assert.Ignore(_configuration.GetMissingCredentialsReason());
        }

        var booking = BookingDataFactory.CreateUniqueBooking();
        var response = _client.CreateBooking(booking);
        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        var created = JsonSerializer.Deserialize<BookingResponse>(response.Content ?? "{}");
        Assert.That(created, Is.Not.Null);
        Assert.That(created!.bookingid, Is.GreaterThan(0));
    }

    [Test]
    public void BookingCrudWorkflow_WithCredentials()
    {
        if (!_configuration.HasCredentials)
        {
            Assert.Ignore(_configuration.GetMissingCredentialsReason());
        }

        var request = BookingDataFactory.CreateUniqueBooking();
        var createResponse = _client.CreateBooking(request);
        Assert.That((int)createResponse.StatusCode, Is.EqualTo(200), createResponse.Content ?? "No response body");

        var created = JsonSerializer.Deserialize<BookingResponse>(createResponse.Content ?? "{}");
        Assert.That(created, Is.Not.Null);
        Assert.That(created!.bookingid, Is.GreaterThan(0));

        var authResponse = _client.Authenticate(_configuration.UserName!, _configuration.Password!);
        Assert.That((int)authResponse.StatusCode, Is.EqualTo(200), authResponse.Content ?? "No response body");

        var token = JsonSerializer.Deserialize<AuthResponse>(authResponse.Content ?? "{}")?.token;
        Assert.That(token, Is.Not.Null.And.Not.Empty);

        var updated = BookingDataFactory.CreateUniqueBooking();
        updated.firstname = "Updated";
        updated.lastname = "Name";
        updated.additionalneeds = "Lunch";

        var updateResponse = _client.UpdateBooking(created.bookingid, updated, token!);
        Assert.That((int)updateResponse.StatusCode, Is.EqualTo(200), updateResponse.Content ?? "No response body");

        var getAfterPut = _client.GetBooking(created.bookingid);
        Assert.That((int)getAfterPut.StatusCode, Is.EqualTo(200), getAfterPut.Content ?? "No response body");

        var partial = new BookingRequest { firstname = "Patched", additionalneeds = "Dinner" };
        var patchResponse = _client.PartialUpdateBooking(created.bookingid, partial, token!);
        Assert.That((int)patchResponse.StatusCode, Is.EqualTo(200), patchResponse.Content ?? "No response body");

        var deleteResponse = _client.DeleteBooking(created.bookingid, token!);
        Assert.That((int)deleteResponse.StatusCode, Is.EqualTo(201), deleteResponse.Content ?? "No response body");

        var finalGet = _client.GetBooking(created.bookingid);
        Assert.That((int)finalGet.StatusCode, Is.EqualTo(404), finalGet.Content ?? "No response body");
    }

    [Test]
    public void AuthenticationNegativeTests_WithCredentials()
    {
        if (!_configuration.HasCredentials)
        {
            Assert.Ignore(_configuration.GetMissingCredentialsReason());
        }

        var booking = BookingDataFactory.CreateUniqueBooking();
        var id = 1;

        var invalidPut = _client.PutBookingWithoutToken(id, booking);
        Assert.That((int)invalidPut.StatusCode, Is.EqualTo(403).Or.EqualTo(401));

        var invalidPatch = _client.PatchBookingWithoutToken(id, booking);
        Assert.That((int)invalidPatch.StatusCode, Is.EqualTo(403).Or.EqualTo(401));

        var invalidDelete = _client.DeleteBookingWithoutToken(id);
        Assert.That((int)invalidDelete.StatusCode, Is.EqualTo(403).Or.EqualTo(401));
    }
}
