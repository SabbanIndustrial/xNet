# Repository Guidelines

## Coding Style
- This repository targets legacy .NET Framework 4.0. Keep code compatible with this target unless explicitly requested otherwise.
- Use four-space indentation and place opening braces on the same line as declarations to match the existing style.
- Preserve existing XML documentation comments and add new ones when introducing public APIs.

## Testing
- All changes must be covered by automated C# tests. New features require corresponding unit tests in a dedicated test project.
- Tests should execute quickly and avoid external network dependencies; mock or stub network interactions where possible.

## Logging & Diagnostics
- When adding logging, redact sensitive information such as credentials, tokens, and full URLs that may contain secrets.
- Favor structured logging patterns (`ILogger` with named scopes) when available.

## Pull Requests
- Keep commit history clean and ensure all builds and tests pass before submitting a PR.

## General
- Use four spaces for indentation in C# and XML files.
- Follow standard .NET naming conventions (PascalCase for types and methods, camelCase for locals and parameters).
- Prefer `var` when the type is evident from the right-hand side; otherwise use explicit types.
- Keep using directives sorted alphabetically and place `System.*` namespaces first.
- When adding new projects, place them in solution folders that match their purpose (e.g., `tests` for test projects).

## Testing
- All new code must be covered by automated tests. Place test projects under a `tests/` directory and use xUnit as the preferred test framework unless an existing project dictates otherwise.
- Enable code coverage support for every test project, exposing coverage data to CI via the default `.runsettings` or `coverlet.collector` integration.

## Documentation
- Update README or inline XML documentation comments when introducing new public APIs or behaviors.
