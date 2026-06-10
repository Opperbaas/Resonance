using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Resonance.BusinessLogicLayer.Integrations;

namespace Resonance.PresentationLayer.Controllers
{
    public class SpotifyController : Controller
    {
        private readonly SpotifyApiSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public SpotifyController(IOptions<SpotifyApiSettings> options, IHttpClientFactory httpClientFactory)
        {
            _settings = options.Value;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("/spotify/connect")]
        public IActionResult Connect()
        {
            var state = Guid.NewGuid().ToString("N");
            HttpContext.Session.SetString("SpotifyAuthState", state);

            var scopes = string.Join(" ", new[]
            {
                "streaming",
                "user-read-playback-state",
                "user-modify-playback-state",
                "user-read-email",
                "user-read-private"
            });

            var redirectUri = GetSpotifyRedirectUri();
            var authorizeUrl = new UriBuilder("https://accounts.spotify.com/authorize")
            {
                Query = $"response_type=code&client_id={Uri.EscapeDataString(_settings.ClientId)}&scope={Uri.EscapeDataString(scopes)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&state={Uri.EscapeDataString(state)}&show_dialog=true"
            };

            return Redirect(authorizeUrl.ToString());
        }

        [HttpGet]
        [Route("/spotify/callback")]
        [Route("/callback")]
        public async Task<IActionResult> Callback(string code, string state, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                TempData["ErrorMessage"] = $"Spotify authorization failed: {error}";
                return RedirectToAction("Index", "Library");
            }

            var expectedState = HttpContext.Session.GetString("SpotifyAuthState");
            if (string.IsNullOrEmpty(expectedState) || state != expectedState)
            {
                return BadRequest("Invalid Spotify state parameter.");
            }

            var tokenResult = await ExchangeAuthorizationCodeAsync(code);
            if (tokenResult == null)
            {
                TempData["ErrorMessage"] = "Spotify token exchange failed.";
                return RedirectToAction("Index", "Library");
            }

            HttpContext.Session.SetString("SpotifyAccessToken", tokenResult.AccessToken);
            HttpContext.Session.SetString("SpotifyRefreshToken", tokenResult.RefreshToken ?? string.Empty);
            HttpContext.Session.SetString("SpotifyTokenExpiresAt", DateTime.UtcNow.AddSeconds(tokenResult.ExpiresIn).ToString("o"));
            TempData["SuccessMessage"] = "Spotify connected successfully.";

            return RedirectToAction("Playback", "Spotify");
        }

        [HttpGet]
        [Route("/spotify/playback")]
        public IActionResult Playback(string trackUri = "")
        {
            var token = HttpContext.Session.GetString("SpotifyAccessToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Connect");
            }

            ViewBag.SpotifyAccessToken = token;
            ViewBag.TrackUri = trackUri;
            return View();
        }

        [HttpPost]
        [Route("/spotify/play")]
        public async Task<IActionResult> Play([FromBody] SpotifyPlayRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.DeviceId) || string.IsNullOrWhiteSpace(request.TrackUri))
            {
                return BadRequest("Missing deviceId or trackUri.");
            }

            var token = HttpContext.Session.GetString("SpotifyAccessToken");
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Spotify access token not found. Please reconnect Spotify.");
            }

            if (IsSpotifyTokenExpired())
            {
                var refreshToken = HttpContext.Session.GetString("SpotifyRefreshToken") ?? string.Empty;
                var refreshed = await RefreshAccessTokenAsync(refreshToken);
                if (refreshed == null)
                {
                    return Unauthorized();
                }

                token = refreshed.AccessToken;
                HttpContext.Session.SetString("SpotifyAccessToken", token);
                HttpContext.Session.SetString("SpotifyRefreshToken", refreshed.RefreshToken ?? string.Empty);
                HttpContext.Session.SetString("SpotifyTokenExpiresAt", DateTime.UtcNow.AddSeconds(refreshed.ExpiresIn).ToString("o"));
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new { uris = new[] { request.TrackUri } };
            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            var playUrl = $"https://api.spotify.com/v1/me/player/play?device_id={Uri.EscapeDataString(request.DeviceId)}";
            var response = await client.PutAsync(playUrl, content);
            return StatusCode((int)response.StatusCode);
        }

        private bool IsSpotifyTokenExpired()
        {
            var expiresAtValue = HttpContext.Session.GetString("SpotifyTokenExpiresAt");
            if (DateTime.TryParse(expiresAtValue, out var expiresAt))
            {
                return DateTime.UtcNow >= expiresAt.AddSeconds(-30);
            }

            return true;
        }

        private async Task<SpotifyTokenResult?> ExchangeAuthorizationCodeAsync(string code)
        {
            var values = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = GetSpotifyRedirectUri()
            };

            return await SendTokenRequestAsync(values);
        }

        private string GetSpotifyRedirectUri()
        {
            if (Uri.TryCreate(_settings.RedirectUri, UriKind.Absolute, out var absoluteUrl))
            {
                return absoluteUrl.ToString();
            }

            var request = HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{_settings.RedirectUri}";
        }

        private async Task<SpotifyTokenResult?> RefreshAccessTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            var values = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            return await SendTokenRequestAsync(values);
        }

        private async Task<SpotifyTokenResult?> SendTokenRequestAsync(Dictionary<string, string> values)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenEndpoint)
            {
                Content = new FormUrlEncodedContent(values)
            };

            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            using var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);

            return new SpotifyTokenResult
            {
                AccessToken = document.RootElement.GetProperty("access_token").GetString() ?? string.Empty,
                RefreshToken = document.RootElement.TryGetProperty("refresh_token", out var refresh) ? refresh.GetString() : null,
                ExpiresIn = document.RootElement.GetProperty("expires_in").GetInt32()
            };
        }

        private class SpotifyTokenResult
        {
            public string AccessToken { get; set; } = string.Empty;
            public string? RefreshToken { get; set; }
            public int ExpiresIn { get; set; }
        }

        public class SpotifyPlayRequest
        {
            public string DeviceId { get; set; } = string.Empty;
            public string TrackUri { get; set; } = string.Empty;
        }
    }
}
