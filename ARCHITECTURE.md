# Project Architecture

This repository is a layered application built with .NET Core  The diagrams below show the existing Web API and data layers, along with how the separately planned **VueJS App** would send requests to the API.
 

## Backend Component diagram

```mermaid
flowchart LR
    Vue["VueJS App<br/>External client"]
    Console["ConsoleUI<br/>.NET Core 3.1"]

    subgraph API["WebAPI — ASP.NET Core 3.1"]
        Middleware["HTTPS redirection · CORS<br/>JWT validation · Swagger"]
        Controllers["Controllers<br/>ProductsController<br/>AuthController"]
    end

    subgraph Business["Business — netstandard2.0"]
        Services["Service interfaces<br/>IProductService · IAuthService"]
        Managers["ProductManager<br/>AuthManager"]
        Autofac["AutofacBusinessModule<br/>dependency injection"]
    end

    subgraph Data["DataAccess — netstandard2.0"]
        DalInterfaces["DAL interfaces<br/>IProductDal · IUserDal"]
        EfDal["Entity Framework DAL<br/>EfProductDal · EfUserDal"]
        DbContext["NorthwindContext<br/>EF Core"]
    end

    Entities["Entities<br/>Entities and DTOs"]
    Core["Core<br/>Result types · JWT<br/>caching · AOP utilities"]
    Sql[("SQL Server<br/>Connection: DefaultConnection")]

    Vue <-->|"HTTPS · JSON<br/>REST requests/responses"| Middleware
    Middleware --> Controllers
    Controllers --> Services
    Services --> Managers
    Managers --> DalInterfaces
    DalInterfaces --> EfDal
    EfDal --> DbContext
    DbContext --> Sql

    Console --> Managers
    Console --> EfDal

    Autofac -. "service and DAL registrations" .-> Services
    Autofac -. "implementation registration" .-> Managers
    Autofac -. "implementation registration" .-> EfDal
    API -. "shared models" .-> Entities
    Business -. "shared models" .-> Entities
    Data -. "entity/DTO usage" .-> Entities
    API -. "security and infrastructure utilities" .-> Core
    Business -. "shared infrastructure" .-> Core
```

## VueJS App Scalable Frontend Architecture

```mermaid
flowchart TB
    User["User"] --> Browser["Web browser"]
    Browser --> AppEntry

    subgraph VueApp["VueJS App"]
        direction TB

        subgraph AppShell["Application shell"]
            AppEntry["App bootstrap"]
            Router["Vue Router<br/>routes · lazy loading"]
            Guards["Navigation guards<br/>authentication · authorization"]
            Layouts["Layouts<br/>public · authenticated"]
            AppEntry --> Router
            Router --> Guards
            Guards --> Layouts
        end

        subgraph Features["Feature modules — organized by business domain"]
            direction LR
            subgraph Auth["auth"]
                AuthPages["Login / Register pages"]
                AuthUI["Auth feature components"]
                AuthUseCases["Auth composables / use cases"]
                AuthStore["Auth store<br/>session · user"]
                AuthPages --> AuthUI
                AuthPages --> AuthUseCases
                AuthUseCases <--> AuthStore
            end
            subgraph Products["products"]
                ProductPages["Product list / detail / edit pages"]
                ProductUI["Product feature components"]
                ProductUseCases["Product composables / use cases"]
                ProductStore["Product state"]
                ProductPages --> ProductUI
                ProductPages --> ProductUseCases
                ProductUseCases <--> ProductStore
            end
        end

        subgraph Shared["Shared client capabilities"]
            SharedUI["Design system<br/>buttons · forms · dialogs"]
            SharedComposables["Shared composables<br/>reusable UI behavior"]
            ErrorHandling["Error handling<br/>notifications · API errors"]
            Localization["Localization"]
        end

        subgraph DataAccess["Data access"]
            Contracts["API contracts<br/>request / response types"]
            Repositories["Feature API modules / repositories<br/>auth · products"]
            HttpClient["HTTP client<br/>base URL · JSON · timeouts"]
            Interceptors["Request / response interceptors<br/>token · errors"]
            HttpClient <--> Interceptors
            Repositories --> HttpClient
            Repositories --> Contracts
        end

        subgraph Platform["Platform and quality"]
            EnvConfig["Environment configuration<br/>API base URL"]
            Observability["Logging / telemetry"]
            Tests["Tests<br/>unit · component · end-to-end"]
        end

        Layouts --> AuthPages
        Layouts --> ProductPages
        AuthUI -. "uses" .-> SharedUI
        ProductUI -. "uses" .-> SharedUI
        AuthPages -. "uses" .-> SharedComposables
        ProductPages -. "uses" .-> SharedComposables
        AuthUseCases --> Repositories
        ProductUseCases --> Repositories
        AuthUseCases -. "reports failures to" .-> ErrorHandling
        ProductUseCases -. "reports failures to" .-> ErrorHandling
        ErrorHandling -. "uses" .-> SharedUI
        Layouts -. "uses" .-> Localization
        AuthStore -. "session token" .-> Interceptors
        EnvConfig --> HttpClient
        Interceptors -. "reports failures to" .-> Observability
        Tests -. "verify app, features, and API integration" .-> Features
    end

    API["Existing WebAPI"]
    HttpClient <-->|"HTTPS · JSON<br/>REST requests/responses"| API
```

## Example product request

```mermaid
sequenceDiagram
    autonumber
    participant Vue as VueJS App
    participant API as WebAPI
    participant Controller as ProductsController
    participant Service as ProductManager
    participant DAL as EfProductDal
    participant DB as SQL Server

    Vue->>API: GET /api/products/getall (HTTPS, JSON)
    API->>Controller: Route the request
    Controller->>Service: IProductService.GetAll()
    Service->>DAL: IProductDal.GetAll()
    DAL->>DB: EF Core query
    DB-->>DAL: Product records
    DAL-->>Service: Product list
    Service-->>Controller: IDataResult<List<Product>>
    Controller-->>Vue: 200 OK + JSON (on success)
```

## Available endpoints for the Vue.js client

| Method | Endpoint | Purpose |
| --- | --- | --- |
| `GET` | `/api/products/getall` | List products |
| `GET` | `/api/products/getbyid?id={id}` | Get a product by ID |
| `POST` | `/api/products/add` | Add a product |
| `POST` | `/api/products/update` | Update a product |
| `POST` | `/api/auth/login` | Sign in and obtain an access token |
| `POST` | `/api/auth/register` | Register and obtain an access token |

The Vue application should configure the API base URL using an environment variable, with the appropriate URL for each environment. After a successful login, the returned token can be sent in the request headers to endpoints protected by JWT.

## Current configuration notes

- `WebAPI/Startup.cs` configures Swagger, HTTPS redirection, CORS, and JWT validation. CORS currently allows all origins, methods, and headers; in production, it should be restricted to the Vue application's actual origin.
- Although JWT validation middleware is enabled, the current `ProductsController` actions do not have the `[Authorize]` attribute. Therefore, these actions are not currently restricted by JWT at the controller level.
- `NorthwindContext` reads the SQL Server connection string from `ConnectionStrings:DefaultConnection` in `appsettings.json`. Connection details are intentionally omitted from this diagram.
- `ConsoleUI` uses the Business and DataAccess layers directly instead of sending HTTP requests to the Web API, so it is shown separately from the Vue.js request flow.
