1. Replace mocks of ApplicationDbContext with real EF Core InMemory instances
	•	UserServiceTests and other tests currently mock ApplicationDbContext incorrectly.
	•	Refactor these tests to instantiate ApplicationDbContext with UseInMemoryDatabase.
	•	Seed test data as needed.
	•	Dispose DbContext properly after tests.

2. Fix CloudStorageService initialization errors in tests
	•	Provide a minimal mock or real IConfiguration instance in tests that instantiate CloudStorageService.
	•	Alternatively, fully mock CloudStorageService where appropriate.
	•	Ensure dependencies like configuration are correctly passed to avoid ArgumentNullException.

3. Adjust integration tests and middleware tests for HTTP headers
	•	Add missing required headers such as User-Agent in HTTP requests inside integration tests.
	•	Example: client.DefaultRequestHeaders.UserAgent.ParseAdd("IntegrationTestClient/1.0");
	•	Verify test clients send all required headers expected by middleware.

4. Review and fix assertion mismatches in controller tests
	•	Revisit failing tests where expected and actual types or status codes differ.
	•	Confirm test data seeding is accurate and consistent with test input.
	•	Handle exceptions and error returns in controller methods as tests expect.
	•	Modify tests or code if needed to align actual vs expected results.

5. Validate database provider registrations
	•	Make sure only one EF Core database provider is registered per service provider.
	•	Avoid using both PostgreSQL and InMemory providers simultaneously in tests.
	•	Remove or conditionally register providers as appropriate.

⸻
