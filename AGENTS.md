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

