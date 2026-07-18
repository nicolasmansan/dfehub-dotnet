using DFeHub.Providers.WebMania;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();
services.AddWebMania(options =>
{
    options.ApiV2.AccessToken = "package-smoke-test";
});

await using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IWebManiaClient>();

return client.NFSe.IsConfigured &&
    !client.NFe.IsConfigured &&
    !client.Validator.IsConfigured
    ? 0
    : 1;

