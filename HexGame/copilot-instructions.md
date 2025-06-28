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
- Apply this standard consistently throughout the codebase.
- This improves readability and reduces the risk of bugs during future code modifications.
