# Open Telemetry and Azure Monitor Exporter (Preview) with the SAFE Stack

This is a small tweak of the [SAFE Template](https://safe-stack.github.io/docs/template-overview/) which demonstrates how to get going with [Open Telemetry](https://devblogs.microsoft.com/dotnet/opentelemetry-net-reaches-v1-0/), including trying out the preview [Azure Monitor Exporter](https://docs.microsoft.com/en-us/azure/azure-monitor/app/opentelemetry-enable?tabs=net&WT.mc_id=DT-MVP-5003978).

Open Telemetry is a whole set of tools and standards which provide vendor and platform neutral observability. It is a big topic and this demo only touches the surface.

In particular, it is recommended that you use the [Open Telemetry Collector](https://opentelemetry.io/docs/collector/) rather than directly export to vendor specific services as we are here.

The Azure Monitor exporter is very much a work in progress and still lacks most features other than tracing.

I also haven't included examples of the automatic [HttpClient](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Instrumentation.Http/README.md) and [SqlClient](https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Instrumentation.SqlClient/README.md) instrumentation

It is just an example of how easy it is to get up and running with the bare minimum of tracing in Application Insights.


# Deployment

The SAFE template comes with [Farmer](https://compositionalit.github.io/farmer/quickstarts/quickstart-3/). Providing you have the Azure CLI installed as explained in that link, then you can deploy the app by simply running

`dotnet run azure`

I have set up the Farmer template in `Build.fs` to add the App Insights instrumentation key to the app settings. This allows me to load it at startup in Server.fs, which is where the app setup and logging code lives.


## SAFE Stack Documentation

If you want to know more about the full Azure Stack and all of it's components (including Azure) visit the official [SAFE documentation](https://safe-stack.github.io/docs/).

You will find more documentation about the used F# components at the following places:

* [Saturn](https://saturnframework.org/)
* [Fable](https://fable.io/docs/)
* [Elmish](https://elmish.github.io/elmish/)
