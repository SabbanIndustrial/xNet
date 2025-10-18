# Agent Instructions

- Follow the existing C# style in this repository: four-space indentation, braces on separate lines for type and member declarations, and prefer explicit type names over `var` unless the type is obvious from the right-hand side.
- Maintain existing XML documentation comments when touching files that contain them; add similar documentation to new public types or members introduced alongside them.
- Place new library code under the `xNet/` directory, keeping the `~`-prefixed subfolders structure when extending existing areas (e.g., `~Internal`).
- Add C# tests for new behavior using xUnit in the `tests/` directory.
- Keep cross-platform code free of Windows-only dependencies; gate Windows-specific logic behind `#if WINDOWS` checks and runtime `OperatingSystem.IsWindows()` guards.
