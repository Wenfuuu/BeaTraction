using System.Diagnostics;

namespace BeaTraction.Application.Telemetry;

public static class AppTelemetry
{
    public const string ServiceName = "beatraction-api";
    public static readonly ActivitySource Source = new(ServiceName);
}
