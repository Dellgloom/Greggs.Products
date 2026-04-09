# Greggs Products API

## Overview
Brief description of what the API does.

## What I Did

### Task 1 - Real Product Data
Replaced the static random product list with a proper data access layer using the existing `IDataAccess<Product>` implementation.

### Task 2 - Currency Conversion
Added an optional `currency` query parameter (defaults to GBP) that applies an exchange rate multiplier to product prices. Supported currencies are GBP and EUR.

## Architecture
The solution follows an N-tier layered architecture:
- **Controller** - handles HTTP concerns only
- **Service** - business logic, validation, currency conversion
- **Repository** - abstracts the data access layer
- **DataAccess** - existing in-memory data store (unchanged)

### Async Implementation
Although the data access layer uses an in-memory list rather than a real database, async has been
implemented throughout the full stack — controller, service, and repository. The synchronous data
access call is wrapped in `Task.FromResult` at the repository level, simulating how the code would
behave with a real database connection (e.g. Entity Framework Core). Replacing the in-memory
implementation with a real database would require no changes to the layers above the repository.

## Assumptions
- Exchange rate of 1 GBP to 1.11 EUR is hardcoded as per the acceptance criteria
- PageStart defaults to 0 and PageSize defaults to 5

## Testing
The solution includes unit tests for each layer and integration tests for the exception handling middleware.