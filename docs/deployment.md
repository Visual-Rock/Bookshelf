## external book providers

currently three book providers are supported, these are Google Books Api, Thalia (German book seller) and FindISBN. FindISBN is the only provider active by default the others need to be configured.

### Google Books Api (GBA)

You need to configure a Api Key to activate the GBA-provider. This is achived by either adding an environment variable or adding the relevant section to the appsettings.json.

docker-compose:
```yaml
services:  
  ⋮
  bookshelf-api:
    image: ghcr.io/visual-rock/bookshelf-api:${BOOKSHELF_VERSION}
    environment:
    ⋮
      GoogleApi__ApiKey: <your-api-key>
  ⋮
```

Valid Environment variable names are `GoogleApi__ApiKey` and `GoogleApi:ApiKey`.

appsettings.json:
```json
{
    "GoogleApi": {
        "ApiKey": "<your-api-key>"
    },
    // or
    "GoogleApi:ApiKey": "<your-api-key>",
}
```

### Thalia

Thalia is secured by Cloudflare and will send challenges back when scraped. To avoid this [FlareSolverr](https://github.com/FlareSolverr/FlareSolverr) is needed as additional service. Bookshelf needs the url of FlareSolverr.

You can add FlareSolverr to your docker-compose like this:
```yaml
services:
  bookshelf-flaresolverr:
    image: ghcr.io/flaresolverr/flaresolverr:latest
    networks:
      - app-network
```

for other deployments referre to the FlareSolverr documentation.

for the Api to use FlareSolverr add at least the server URL using either FlareSolverr:ServerUrl as environment variable or the appsettings.

docker-compose:
```yaml
services:  
  ⋮
  bookshelf-api:
    image: ghcr.io/visual-rock/bookshelf-api:${BOOKSHELF_VERSION}
    environment:
    ⋮
      FlareSolverr__ServerUrl: <flare-solverr-url>
      # optionally
      FlareSolverr__MaxTimeout: <flare-solverr-max-timeout>
  ⋮
```

appsettings.json:
```json
{
    "FlareSolverr": {
        "ServerUrl": "<flare-solverr-url>",
        // optionally:
        "MaxTimeout": <flare-solverr-max-timeout>
    },
    // or
    "FlareSolverr:ServerUrl": "<your-api-key>",
    // optionally:
    "FlareSolverr:MaxTimeout": <flare-solverr-max-timeout>
}
```
