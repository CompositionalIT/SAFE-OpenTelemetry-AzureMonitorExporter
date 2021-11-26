module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open System.Diagnostics
open OpenTelemetry.Trace
open Azure.Monitor.OpenTelemetry.Exporter
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection;

open Shared

[<Literal>]
let SOURCE = "OTel.AzureMonitor.SAFE.Demo"
let activitySource = new ActivitySource(SOURCE)

type Storage() =
    let todos = ResizeArray<_>()

    let getTodos = async {
        use activity = activitySource.StartActivity "Loading Todos"
        do! Async.Sleep 300
        return List.ofSeq todos
    }

    let validateTodo todo = async {
        use activity = activitySource.StartActivity "Validating Todo"
        do! Async.Sleep 300
        return Todo.isValid todo
    }

    member __.GetTodos() = async {
        use activity = activitySource.StartActivity "Getting Todos"
        do! Async.Sleep 200
        return! getTodos
    }

    member __.AddTodo(todo: Todo) = async {

        use activity = activitySource.StartActivity "Adding Todo" 
        let activityOption = activity |> Option.ofObj

        do! Async.Sleep 200

        let! todoIsValid = validateTodo todo.Description

        if todoIsValid then

            todos.Add todo

            activityOption
            |> Option.map (fun act -> act.AddTag("Success", todo.Id))
            |> ignore

            return Ok()
        else
            activityOption
            |> Option.map (fun act -> act.AddTag("Failure", todo.Id))
            |> ignore

            return Error "Invalid todo"
        }

let storage = Storage()

let todosApi =
    { getTodos = fun () -> async { return! storage.GetTodos() }
      addTodo =
          fun todo ->
              async {
                  match! storage.AddTodo todo with
                  | Ok () -> return todo
                  | Error e -> return failwith e
              } }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler

[<Literal>]
let APP_INSIGHTS_KEY = "AppInsightsKey"

let configureServices (services : IServiceCollection) =
    let config = services.BuildServiceProvider().GetService<IConfiguration>()
    services.AddOpenTelemetryTracing(fun builder ->
        builder
            .AddAspNetCoreInstrumentation()
            .AddSource(SOURCE)
            .AddAzureMonitorTraceExporter(fun options ->
                options.ConnectionString <- $"InstrumentationKey={config.Item APP_INSIGHTS_KEY};IngestionEndpoint=https://westeurope-2.in.applicationinsights.azure.com/" 
            )
        |> ignore
    )


let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        service_config configureServices
        use_static "public"
        use_gzip
    }

run app
