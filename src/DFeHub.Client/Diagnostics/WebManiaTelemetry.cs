namespace DFeHub.Diagnostics;

/// <summary>
/// Provides stable names for subscribing to DFeHub telemetry.
/// </summary>
public static class WebManiaTelemetry
{
    /// <summary>
    /// The name used by the SDK's <see cref="System.Diagnostics.ActivitySource"/>.
    /// </summary>
    public const string ActivitySourceName = "DFeHub.WebMania";

    /// <summary>
    /// The name used by the SDK's <see cref="System.Diagnostics.Metrics.Meter"/>.
    /// </summary>
    public const string MeterName = "DFeHub.WebMania";
}

