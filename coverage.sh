rm -rf test-results

dotnet test --results-directory "test-results" --collect:"Code Coverage;Format=cobertura"

# Run `dotnet tool install -g dotnet-reportgenerator-globaltool` to install the report generator.

reportgenerator -reports:"test-results/**/*.xml" -targetdir:"coverage-report" -reporttypes:Html