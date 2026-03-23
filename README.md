# GHAS .NET / Blazor Vulnerability Playground (Intentionally Insecure)

⚠️ **WARNING:** This repository contains **intentionally vulnerable code** for **training/demo purposes only**.
- Do **NOT** deploy this code to production
- Do **NOT** expose it to the internet
- Use only in a private training environment

Prepared: February 27, 2026

## Goal
This repo is designed to generate findings in:
- **GitHub Code Scanning (CodeQL)** for **C#**
- **Secret Scanning** (contains fake token strings shaped like common secrets)
- **Dependabot** (intentionally older NuGet packages)

## Quick start
1. Create a new GitHub repo (e.g., `ghas-dotnet-playground`) and push this code.
2. Go to **Security → Code scanning** and ensure the CodeQL workflow runs.
3. Enable **Dependabot alerts** and **Dependabot security updates**.
4. Enable **Secret scanning** if available for your plan/org.

## Projects
- `src/InsecureBlazorServer/` – Blazor Server app with intentionally unsafe endpoints/pages

## Running locally
```bash
cd src/InsecureBlazorServer
dotnet restore
dotnet run
```
Browse:
- https://localhost:5001

## Intentionally unsafe areas (examples)
- Reflected XSS via rendering raw HTML (MarkupString)
- Open redirect
- Path traversal-like file read based on user input
- SSRF-like behavior by fetching arbitrary URLs
- Hardcoded secrets
- Weak crypto / insecure randomness
- Risky JSON deserialization configuration

