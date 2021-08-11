# DotNetDigestAuth
Implementation of Digest Authentication for ASP.NET Core (AuthenticationHandler).

Supports: ASP.NET Core 5.0+

## General:
- Depending on the usage scenario, you have the option of storing the user secrets (passwords) in plaintext or only storing the hashes of the secrets (recommended). 

     - If you decide to store just the hashes of user passwords then you will need to pre-compute those using the utility method `DigestAuthentication.ComputeA1Md5Hash` & implement `IUsernameHashedSecretProvider`:

        ```C#
            var hash = DigestAuthentication.ComputeA1Md5Hash("eddie","test-realm", "starwars123"); // 56bde614dc75372a1ef4323904f3beb7

            // ...
          
            internal class ExampleUsernameHashedSecretProvider : IUsernameHashedSecretProvider
            {
                public Task<string> GetA1Md5HashForUsernameAsync(string username, string realm) {
                    if (username == "eddie" && realm == "test-realm") {
                        // The hash value below would have been pre-computed & stored in the database
                        const string hash = "56bde614dc75372a1ef4323904f3beb7";

                        return Task.FromResult(hash);
                    }

                    // User not found
                    return Task.FromResult<string>(null);
                }
            }
        ```
    
    - If, on the other hand you decide to store secrets in plaintext, you will need to provide an implementation of `IUsernameSecretProvider`:

        ```C#
            internal class ExampleUsernameSecretProvider : IUsernameSecretProvider
            {
                public Task<string> GetSecretForUsernameAsync(string username) {
                    if (username == "eddie") {
                        return Task.FromResult("starwars123");
                    }

                    /// User not found
                    return Task.FromResult<string>(null);
                }
            }
        ```
        
        In this mode of operation, the library will automatically compute the (user secret) hashes on-the-fly.

### DigestAuthenticationConfiguration.Create:
`DigestAuthenticationConfiguration.Create` is used to configure the digest authentication.

* `ServerNonceSecret` is used when generating the challenges for the client. Keep this safe!
* `Realm` describes (to the user) the computer or system being accessed. This value must match the realm used when pre-computing hashes
* `MaxNonceAgeSeconds` is the number of seconds a given nonce is valid for. After that point, the client will be prompted to reauthenticate

E.g.

```C#
DigestAuthenticationConfiguration.Create("SomeVerySecureServerNonceSecret", "SomeDescriptiveRealmName", 30)
```

## Usage in ASP.NET Core:

- You'll want to reference the NuGet package & namespace `FlakeyBit.DigestAuthentication.AspNetCore`.

In your web host startup:

```C#
public class Startup
{
        public void ConfigureServices(IServiceCollection services) {
            // Register our implmentation of IUsernameHashedSecretProvider (or IUsernameSecretProvider if using plaintext)
            services.AddScoped<IUsernameHashedSecretProvider, ExampleUsernameHashedSecretProvider>();
            services.AddAuthentication("Digest")
                    .AddDigestAuthentication(DigestAuthenticationConfiguration.Create("SomeVerySecureServerNonceSecret", "SomeDescriptiveRealmName", 30));
            services.AddMvc();
        }
}
```

This will add a claim of type `DIGEST_AUTHENTICATION_NAME` (value will be the authenticated user name) to the principal on the request context. If you want to simply check for a claim of this type on a controller action, you can use the `[Authorize]` attribute as follows:

```C#
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Digest")]
        public string Get() {
            return "Hello Core!";
        }
    }
```

## More info:
For working examples, check out the AspNetCoreApp projects in the solution.
