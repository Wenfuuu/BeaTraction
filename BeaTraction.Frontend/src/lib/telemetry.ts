import { WebTracerProvider } from "@opentelemetry/sdk-trace-web";
import { BatchSpanProcessor } from "@opentelemetry/sdk-trace-base";
import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-http";
import { registerInstrumentations } from "@opentelemetry/instrumentation";
import { FetchInstrumentation } from "@opentelemetry/instrumentation-fetch";
import { DocumentLoadInstrumentation } from "@opentelemetry/instrumentation-document-load";
import { Resource } from "@opentelemetry/resources";

const otlpEndpoint =
  (import.meta.env.VITE_OTEL_EXPORTER_OTLP_ENDPOINT as string) ??
  "http://localhost:4318";

export function initTelemetry() {
  const provider = new WebTracerProvider({
    resource: new Resource({ "service.name": "beatraction-frontend" }),
    spanProcessors: [
      new BatchSpanProcessor(
        new OTLPTraceExporter({ url: `${otlpEndpoint}/v1/traces` })
      ),
    ],
  });

  provider.register();

  registerInstrumentations({
    instrumentations: [
      new FetchInstrumentation({
        clearTimingResources: true,
        // Propagates traceparent/tracestate headers to the backend.
        // Restrict this to your API domain in production.
        propagateTraceHeaderCorsUrls: [/.*/],
      }),
      new DocumentLoadInstrumentation(),
    ],
  });
}
