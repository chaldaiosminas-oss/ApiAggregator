# API Aggregator Service

## Overview
ASP.NET Core API that aggregates data from multiple external APIs and exposes a unified endpoint.

## Features
- Parallel API calls using async/await
- In-memory caching
- Graceful error handling with fallback
- Request performance statistics
- In-memory metrics store
- Background monitoring service

## External APIs
- GitHub Repositories API
- NewsAPI
- OpenWeather API

## Endpoints

### GET /api/aggregate
Query params:
- sort=date
- category=news|weather|technology

### GET /api/stats
Returns request count and response time buckets per API.

## Technologies
- ASP.NET Core
- HttpClientFactory
- Polly
- MemoryCache
- xUnit

## Running
1. Add API keys in appsettings.json
2. Run `dotnet run`
3. Open Swagger UI
