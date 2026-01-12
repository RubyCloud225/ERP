# Testing Notes for ERP Project

**Date:** 

---

## Overview

This document summarizes the current state of testing in the ERP project, highlighting known issues, workarounds, and next steps for improving test reliability and coverage.

---

## Current Status

### 1. Integration Tests

- Integration tests now run against the real application pipeline using a shared `TestWebApplicationFactory`.
- Test authentication is configured using a custom test auth handler to bypass real authentication.
- The test HTTP client includes required headers (e.g., `User-Agent`) to satisfy middleware validation.
- Some integration tests still fail due to business logic errors or incomplete test setup.
- Assertions on response status codes have been adjusted to reflect actual application behaviour (e.g., expecting 400 Bad Request or 401 Unauthorized where appropriate).

### 2. Service and Controller Tests

- Several service tests mock `ApplicationDbContext` directly, which is problematic due to EF Core's complex internals.
- Mocking DbContext has led to runtime errors; migration to EF Core InMemory provider or real test database instances is planned.
- Some controller tests exhibit assertion mismatches caused by changes in controller responses or domain model updates.
- Tests interacting with external services (e.g., CloudStorageService) lack proper configuration mocks, causing failures.

---

## Known Issues

- EF Core model relationship configurations require explicit foreign key mappings to resolve validation errors.
- Some tests contain long delays (`Task.Delay`) causing flaky behavior in CI and local runs.
- Static files are not found during testing due to missing `wwwroot` folder.
- Configuration dependencies are missing in some service tests, causing exceptions.

---

## Workarounds and Recommendations

- Tests using `ApplicationDbContext` mocks are temporarily marked as skipped or require refactoring.
- Future work includes migrating service tests to use EF Core InMemory provider for reliable behavior.
- Explicit model configuration in `OnModelCreating` is necessary to fix EF Core validation errors.
- Provide minimal `IConfiguration` mocks in tests relying on external service configurations.
- Adjust test assertions to expect realistic status codes and error messages from the current app version.
- Avoid overriding the host pipeline in integration tests; use a shared test factory to maintain realistic request processing.

---

## Next Steps

- Complete migration of service tests away from DbContext mocks.
- Fix EF Core relationship configurations to remove validation errors.
- Enhance test coverage for edge cases and error conditions.
- Improve test performance by reducing unnecessary delays and mocks.
- Add documentation for testing strategy and guidelines for contributors.

---

*This document will be updated as test coverage and stability improve.*