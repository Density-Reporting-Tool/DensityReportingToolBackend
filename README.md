# DensityReportingTooltBackend

## Getting started Notes
appsettings.json: Base config for the app, has settings that apply to ALL environments
appsettings.development.json: Only loaded when environment is set to development, overrides values in appsettings.json
appsettings.development.json is ignored, use for Db connection strings, API keys, secrets

Properties/launchSettings.json: controls how the app is run during development, defines environment variables
                                for each profile, not used in production
Properties/PublishProfiles/*: Contains .pubxml files which determine where you're deploying your app and the configuration

DensityReportingToolBackend.http: lets us run HTTP requests difrectly from VisualStudio without needing Postman, curl, etc. OPTIONAL