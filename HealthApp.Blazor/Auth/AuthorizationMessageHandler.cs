using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace HealthApp.Blazor.Auth
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthorizationMessageHandler(CustomAuthStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _authStateProvider.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }

        public AuthorizationMessageHandler ConfigureHandler(string[] authorizedUrls)
        {
            InnerHandler = new HttpClientHandler();
            return this;
        }
    }
}