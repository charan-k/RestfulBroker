using System.Net;
using System.Text.Json.Serialization;
using RestSharp;

namespace RestfulBookerApiTests;

public sealed class BookerConfiguration
{
    public const string DefaultBaseUrl = "https://restful-booker.herokuapp.com";

    public BookerConfiguration(string baseUrl, string? userName, string? password)
    {
        BaseUrl = ValidateBaseUrl(baseUrl);
        UserName = userName;
        Password = password;
    }

    public string BaseUrl { get; }
    public string? UserName { get; }
    public string? Password { get; }
    public bool HasCredentials => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);

    public static BookerConfiguration FromEnvironment()
    {
        var configuredBaseUrl = Environment.GetEnvironmentVariable("BOOKER_BASE_URL");
        var baseUrl = string.IsNullOrWhiteSpace(configuredBaseUrl) ? DefaultBaseUrl : configuredBaseUrl.Trim();

        return new BookerConfiguration(baseUrl, Environment.GetEnvironmentVariable("BOOKER_USERNAME"), Environment.GetEnvironmentVariable("BOOKER_PASSWORD"));
    }

    public string GetMissingCredentialsReason() => "Missing BOOKER_USERNAME or BOOKER_PASSWORD environment variables; authenticated tests are skipped.";

    public static string ValidateBaseUrl(string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("The API base URL is required.");
        }

        if (!Uri.TryCreate(baseUrl.Trim(), UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException($"BOOKER_BASE_URL must be a valid http(s) URL. Value received: '{baseUrl}'.");
        }

        return uri.ToString().TrimEnd('/');
    }
}

public static class BookingDataFactory
{
    public static BookingRequest CreateUniqueBooking()
    {
        var suffix = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        return new BookingRequest
        {
            firstname = $"Test{suffix}",
            lastname = $"User{suffix}",
            totalprice = 123,
            depositpaid = true,
            bookingdates = new BookingDates
            {
                checkin = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
                checkout = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd")
            },
            additionalneeds = "Breakfast"
        };
    }
}

public sealed class BookingRequest
{
    [JsonPropertyName("firstname")]
    public string firstname { get; set; } = string.Empty;

    [JsonPropertyName("lastname")]
    public string lastname { get; set; } = string.Empty;

    [JsonPropertyName("totalprice")]
    public int totalprice { get; set; }

    [JsonPropertyName("depositpaid")]
    public bool depositpaid { get; set; }

    [JsonPropertyName("bookingdates")]
    public BookingDates bookingdates { get; set; } = new();

    [JsonPropertyName("additionalneeds")]
    public string additionalneeds { get; set; } = string.Empty;
}

public sealed class BookingDates
{
    [JsonPropertyName("checkin")]
    public string checkin { get; set; } = string.Empty;

    [JsonPropertyName("checkout")]
    public string checkout { get; set; } = string.Empty;
}

public sealed class BookingResponse
{
    [JsonPropertyName("bookingid")]
    public int bookingid { get; set; }

    [JsonPropertyName("booking")]
    public BookingRequest? booking { get; set; }
}

public sealed class AuthRequest
{
    [JsonPropertyName("username")]
    public string username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string password { get; set; } = string.Empty;
}

public sealed class AuthResponse
{
    [JsonPropertyName("token")]
    public string token { get; set; } = string.Empty;
}

public sealed class BookerClient
{
    private readonly RestClient _client;

    public BookerClient(BookerConfiguration configuration)
    {
        _client = new RestClient(configuration.BaseUrl);
    }

    public RestResponse GetBooking(int bookingId)
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Get);
        return Execute(request);
    }

    public RestResponse CreateBooking(BookingRequest booking)
    {
        var request = new RestRequest("/booking", Method.Post)
            .AddJsonBody(booking);
        return Execute(request);
    }

    public RestResponse UpdateBooking(int bookingId, BookingRequest booking, string token)
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Put)
            .AddHeader("Cookie", $"token={token}")
            .AddJsonBody(booking);
        return Execute(request);
    }

    public RestResponse PartialUpdateBooking(int bookingId, BookingRequest booking, string token)
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Patch)
            .AddHeader("Cookie", $"token={token}")
            .AddJsonBody(booking);
        return Execute(request);
    }

    public RestResponse DeleteBooking(int bookingId, string token)
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Delete)
            .AddHeader("Cookie", $"token={token}");
        return Execute(request);
    }

    public RestResponse Authenticate(string userName, string password)
    {
        var request = new RestRequest("/auth", Method.Post)
            .AddJsonBody(new AuthRequest { username = userName, password = password });
        return Execute(request);
    }

    public RestResponse DeleteBookingWithoutToken(int bookingId)
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Delete);
        return Execute(request);
    }

    public RestResponse PatchBookingWithoutToken(int bookingId, BookingRequest booking)
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Patch)
            .AddJsonBody(booking);
        return Execute(request);
    }

    public RestResponse PutBookingWithoutToken(int bookingId, BookingRequest booking)
    {
        var request = new RestRequest($"/booking/{bookingId}", Method.Put)
            .AddJsonBody(booking);
        return Execute(request);
    }

    private RestResponse Execute(RestRequest request)
    {
        var attempt = 0;
        while (true)
        {
            var response = _client.Execute(request);
            if (RetryPolicy.ShouldRetry(response.StatusCode, response.ErrorException, attempt))
            {
                attempt++;
                if (attempt > 2)
                {
                    return response;
                }

                Thread.Sleep(250 * attempt);
                continue;
            }

            return response;
        }
    }
}

public static class RetryPolicy
{
    public static bool ShouldRetry(HttpStatusCode? statusCode, Exception? exception, int attempt)
    {
        if (attempt >= 2)
        {
            return false;
        }

        if (statusCode.HasValue)
        {
            var code = (int)statusCode.Value;
            if (code == 408 || code >= 500)
            {
                return true;
            }

            return false;
        }

        return exception is HttpRequestException or TimeoutException or IOException or System.Net.Sockets.SocketException;
    }
}
