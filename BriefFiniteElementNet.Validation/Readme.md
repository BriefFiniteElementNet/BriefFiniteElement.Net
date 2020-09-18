# Validation
`BriefFiniteElementNet.Validation` project aimed to be for validating the BFE output with other well known FEA software. Validations are sort of dynamic, instead of static in order to be used as integral tests too.
If validation is static and uses values from prior verions of BFE then it is not possible to use as test.

Each validation case goes in a separate folder like `Case_01\`. `Readme.md` file inside each folder with this pattern:

- Title (as brief title about validation case)
- Description (detailed description about case)
- Model Definition (details about model definition)
- Some visualization would be good!
- Result (Result of validation for this case)
- References (if example is taken from a reference)

Images are in same folder for each case
Codes are in same folder for each case, 

`ValidationResult` and `IValidationCase` is used for validation cases. ``ValidationResult` is a sort of report in HTML.