# Bookshelf

A full-stack application for managing personal book collections, featuring book discovery via ISBN scanning and public library sharing.

## Features

- Personal Library Management: Track your collection of books with details like title, author, publisher, and page count.
- ISBN Discovery: Automatically fetch book details and covers using ISBN. Support for multiple external services.
- Public Libraries: Share your collection with others or browse public libraries from other users.
- Book Scanning: Integrated support for scanning book barcodes to quickly add them to your collection.
- User management: Integrated support for OIDC providers.

### External Provider

Supported External Providers:
- Google Books API
- Thalia (German book seller)
- FindISBN

## Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js and npm
- Docker Desktop (for database and services via Aspire)

### Configuration

To enable book data fetching via Google Books API, set your API key using user-secrets:

```shell
dotnet user-secrets set "GoogleApi:ApiKey" "<Google-Api-Key>" --project Bookshelf.Api
```

### Running the Application

1. Clone the repository.
2. Ensure Docker is running.
3. Start the application using the Aspire AppHost:
   ```shell
   dotnet run --project Bookshelf/Bookshelf.csproj
   ```
4. Open the Aspire Dashboard URL provided in the terminal to access the web frontend and API.
