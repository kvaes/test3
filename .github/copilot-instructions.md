This is a repository containing both a VueJS based front-end and a C# powered Azure Functions based back-end.
It is primarily responsible for ingesting metered usage for GitHub and recording that usage.

Please follow these guidelines when contributing:

## Code Standards

### Required Before Each Commit

### Development Flow


## Repository Structure
- `.github/workflows`: Containing all github actions workflows, amongst others our CI flows
- `agent/`: Contains all the code based on Semantic Kernel in C#
  - `agent/Processes/`: A directory for all defined processes
  - `agent/Steps/`: A dedicated directory for reusable Steps
  - `agent/Functions/`: A folder containing your Kernel Function definitions.
- `docs/`: Documentation
- `readme.md` : General overview of this repo that helps guide newjoiners around. 

## Key Guidelines
1. Follow Go best practices and idiomatic patterns
2. Maintain existing code structure and organization
3. Use dependency injection patterns where appropriate
4. Write unit tests for new functionality. Use table-driven unit tests when possible. Ensure the unit tests are part of the CI flow.
5. Document public APIs and complex logic. Suggest changes to the `docs/` folder when appropriate
6. Ensure that both the code has a CI flow that is triggered when changes happen in their specific directory and that builds a container based artifact
