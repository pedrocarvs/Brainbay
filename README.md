# Brainbay Rick & Morty .NET 8 Solution (Full VS-ready)

This scaffold contains:
- src/Brainbay.Core
- src/Brainbay.Infrastructure
- src/Brainbay.Console
- src/Brainbay.Web
- tests/Brainbay.Tests

How to run:
1. Open `Brainbay.sln` in Visual Studio 2022/2023 or use `dotnet` CLI.
2. From CLI, run `dotnet restore` and `dotnet build`.
3. Seed DB: set `Brainbay.Console` as startup and run, or `dotnet run --project src/Brainbay.Console`.
4. Run web app: `dotnet run --project src/Brainbay.Web` and open the browser.