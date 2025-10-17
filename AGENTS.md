# Repository Guidelines

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
