using DFeHub.Providers.WebMania;
using DFeHub.Providers.WebMania.NFe;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddLogging();
services.AddWebMania(options =>
{
    options.ApiV2.AccessToken = "package-smoke-test";
    options.ApiV1.ConsumerKey = "package-smoke-test";
    options.ApiV1.ConsumerSecret = "package-smoke-test";
    options.ApiV1.AccessToken = "package-smoke-test";
    options.ApiV1.AccessTokenSecret = "package-smoke-test";
});

await using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IWebManiaClient>();
var publicNFeContract = typeof(INFeClient)
    .GetMethod(
        nameof(INFeClient.IssueAsync),
        [typeof(NFeIssueRequest), typeof(CancellationToken)]);

return client.NFSe.IsConfigured &&
    client.NFe.IsConfigured &&
    publicNFeContract is not null &&
    !client.Validator.IsConfigured
    ? 0
    : 1;
