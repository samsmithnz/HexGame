# Copilot Instructions: Coding Standards

## Braces for Control Statements
- Always use curly braces `{}` for the body of all `if`, `else`, `for`, `while`, and similar control statements, even if the body contains only a single statement.

## Example
```csharp
// Correct:
if (condition)
{
    DoSomething();
}

// Incorrect:
if (condition)
    DoSomething();
```

## Additional Guidelines
- Do not remove comments from any code.
- Always update the game rules in the help popup when adding new features or changing game mechanics.
- Don't use the `var` keyword for variable declarations; always specify the type explicitly.
- We are using Unity 6, so ensure compatibility with that version.
