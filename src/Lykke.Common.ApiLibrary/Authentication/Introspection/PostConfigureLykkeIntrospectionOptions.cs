using System;
using System.Collections.Concurrent;
using IdentityModel.Client;
using Lykke.Common.ApiLibrary.Authentication.Introspection.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Lykke.Common.ApiLibrary.Authentication.Introspection
{
    [Obsolete("Introspection client was removed due to IdentityModel changes")]
    public class PostConfigureLykkeIntrospectionOptions
        : IPostConfigureOptions<LykkeIntrospectionOptions>
    {
        private readonly IDistributedCache _cache;

        public PostConfigureLykkeIntrospectionOptions(IDistributedCache cache = null)
        {
            _cache = cache;
        }

        public void PostConfigure(string name, LykkeIntrospectionOptions options)
        {
            options.Validate();

            if (options.EnableCaching && _cache == null)
            {
                throw new ArgumentException(
                    "Caching is enabled, but no IDistributedCache is found in the services collection",
                    nameof(_cache)
                );
            }

            options.LazyIntrospections =
                new ConcurrentDictionary<string, AsyncLazy<TokenIntrospectionResponse>>();
        }
    }
}
