# Tests Guidelines
- Use xUnit for all C# tests and organize them under the `tests/` directory mirroring the namespace layout.
- Prefer async-friendly APIs when interacting with web servers in integration tests and dispose test fixtures deterministically.
- Skip proxy-dependent tests when the corresponding environment variables are not defined instead of failing outright.
