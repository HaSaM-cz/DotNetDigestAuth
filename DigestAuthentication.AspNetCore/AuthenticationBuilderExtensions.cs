using FlakeyBit.DigestAuthentication.Implementation;
using Microsoft.AspNetCore.Authentication;

namespace FlakeyBit.DigestAuthentication.AspNetCore
{
    public static class AuthenticationBuilderExtensions
    {

        public static AuthenticationBuilder AddDigestAuthentication(this AuthenticationBuilder builder,
                                                                    DigestAuthenticationConfiguration config,
                                                                    string scheme = null)
        {
            scheme = DigestAuthentication.GetScheme(scheme);
            return builder.AddScheme<DigestAuthenticationOptions, DigestAuthenticationHandler>(
                scheme, scheme,
                options => options.Configuration = config);
        }
    }
}