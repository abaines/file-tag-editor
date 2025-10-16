# Copilot Instructions for This Repository

## Coding Style
- All variables, method parameters, and return values must have explicit static types.
- Use C#'s explicit typing system for all code (e.g., `int foo = 0;`, `string bar = "hello";`).
- Do not use `var` keyword or rely on type inference except for complex generic types where the type is obvious from the right side.
- When writing Markdown, break long sentences across multiple lines for easier diffs and code review.
	Do not add two spaces at the end of a line unless you want a line break in the rendered output.
	This will render as a single paragraph in Markdown preview, but makes version control diffs much easier to read.

## General Guidelines
- Prefer clarity and maintainability over brevity.
- Follow DRY (Don't Repeat Yourself): extract common patterns into reusable methods or constants.
- Prefer explicit behavior over implicit/automatic behavior: make control flow, resource management, and side effects visible and intentional.
- Avoid comments and use self-documenting code, like good method names and variable names.
- Use offensive (fail-fast) programming: fail immediately and loudly if an assumption is violated, rather than trying to recover or fallback.
Use Debug.Assert() and ArgumentNullException.ThrowIfNull() to catch logic errors early.
- Always include a `default` case in switch statements that throws an exception (e.g., `throw new ArgumentOutOfRangeException()`) to catch unexpected values and prevent silent failures.
- For all null/validity checks, use separate assertion calls for each condition (e.g., `ArgumentNullException.ThrowIfNull(x)` and `Debug.Assert(x.IsValid)`), rather than compound `if` or `||` checks.
This ensures error messages are specific and debugging is easier.
- Treat all variables as readonly where possible: use `readonly` fields and prefer immutable data structures.
Prefer single-assignment local variables for clarity and immutability, similar to Rust's `let` or `const` in other languages.
Use `const` for compile-time constants and `static readonly` for runtime constants.

## C#/.NET Specific
- Use nullable reference types (`string?`, `object?`) and enable nullable warnings in project file.
- Use `record` types for data containers and value objects.
- Follow Microsoft's C# naming conventions: PascalCase for public members, camelCase for private fields with underscore prefix (`_fieldName`).
- Use expression-bodied members for simple properties and methods.
- Prefer `using` statements with try-catch blocks for resource management to ensure clear responsibility separation and explicit error handling.

## AI/Assistant Behavior
- Always follow these instructions for all code suggestions and edits.
- If a compiler warning or error is possible, proactively fix it.
- If you are unsure about a type, prefer the most specific type possible.
- Always enable and respect nullable reference type annotations.

---

_This file is used by GitHub Copilot and compatible AI assistants to ensure consistent code quality and style._
