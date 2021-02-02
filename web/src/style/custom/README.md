# Import order for nhs-frontend imports:

To avoid build issues, all `nhsuk-frontend/packages/core` imports must be listed before any `nhsuk-frontend/packages/components`.

All other imports are ordered in alphabetical order, with `nhsuk-frontend` imports before any custom file imports.