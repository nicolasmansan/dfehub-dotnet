using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DFeHub.Diagnostics;

internal static class WebManiaDiagnostics
{
    internal static readonly ActivitySource ActivitySource =
        new(WebManiaTelemetry.ActivitySourceName);

    internal static readonly Meter Meter =
        new(WebManiaTelemetry.MeterName);

    internal static readonly Counter<long> RequestCounter =
        Meter.CreateCounter<long>(
            "dfehub.webmania.http.requests",
            "{request}",
            "Number of HTTP requests sent to Webmania.");

    internal static readonly Histogram<double> RequestDuration =
        Meter.CreateHistogram<double>(
            "dfehub.webmania.http.request.duration",
            "ms",
            "Duration of HTTP requests sent to Webmania.");
}

