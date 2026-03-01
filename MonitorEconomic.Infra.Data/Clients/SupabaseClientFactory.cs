
using Microsoft.Extensions.Options;
using MonitorEconomic.Infra.Data.Configuration;
using Supabase;

namespace MonitorEconomic.Infra.Data.Clients
{
    public class SupabaseClientFactory
    {
        private readonly SupabaseConfig _config;
        private Supabase.Client? _client;

        public SupabaseClientFactory(IOptions<SupabaseConfig> options)
        {
            _config = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Supabase.Client> GetClientAsync()
        {
            if (_client != null)
                return _client;

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };
            _client = new Supabase.Client(_config.Url, _config.ApiKey, options);
            await _client.InitializeAsync();

            return _client;
        }
    }
}