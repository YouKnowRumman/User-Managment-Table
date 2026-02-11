# Copilot Instructions

## General Guidelines
- Use Bootstrap only for styling.
- Manual verification steps must be provided.

## Project-Specific Rules
- Target the latest .NET runtime (currently .NET 10) for the project.
- Use PostgreSQL (Npgsql) as the database and as the EF provider for migrations and runtime.
- Use Gmail SMTP placeholders for email functionality.
- Implement global middleware to block users.
- Seed admin credentials via environment variables.
- Produce a full project structure and apply changes directly.
- Prioritize Razor Pages for web development; prefer guidance on Razor Pages over Blazor or MVC.