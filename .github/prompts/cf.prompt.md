# Copilot Prompt

This project is primarily focused on using the Medidata.Core.Objects namespace. When generating code, prioritize using types, methods, and classes from Medidata.Core.Objects and System. Avoid using other namespaces unless absolutely necessary. Example usages can be found in .cf/Basic Function.cs and .cf/Public Function.cs in the cf directory.

## Variable Naming Convention
- For local variables, use the following prefixes with underscores and the relevant identifier (folder, form, or field OID) for clarity:
  - Instance: ins_<identifier> (e.g., ins_FOLDER_OID)
  - DataPage: dp_<identifier> (e.g., dp_FORM_OID)
  - Record: rec_<identifier> (e.g., rec_FORM_OID)
  - DataPoint: dpt_<identifier> (e.g., dpt_FIELD_OID)
- Use PascalCase for class names, method names, and public properties (e.g., MyClass, MyMethod).
- Use ALL_CAPS with underscores for constants (e.g., MAX_COUNT).
- Variable names should be descriptive and avoid abbreviations unless they are standard or widely understood.
- Do not use single-letter variable names except for loop counters.

### Examples
- For an Instance object representing a folder OID, use: `ins_FOLDER_OID`
- For a DataPage object for a form OID, use: `dp_FORM_OID`
- For a Record object for a form OID, use: `rec_FORM_OID`
- For a DataPoint representing a field OID, use: `dpt_FIELD_OID`
- For a DataPoints collection, use: `dataPoints`
- For a constant maximum count, use: `MAX_COUNT`

## Data Structure Clarification
- **Instance**: Represents a specific occurrence of a form within a folder for a subject (e.g., a visit or cycle).
- **Folder**: A logical grouping of forms, often representing a visit, cycle, or phase in the study schedule. Contains one or more form instances.
- **DataPage**: Represents the data entry page for a specific form instance within a folder. Contains records for that form instance.
- **Form**: The electronic case report form (eCRF) definition, specifying the fields and layout. Multiple instances of a form can exist within different folders or for different subjects.
- **Record**: Represents a single row of data entry for a form instance (e.g., a log line or repeated entry). Contains DataPoints for each field in the form.
- **DataPoint**: Represents the value entered for a specific field in a record. Each DataPoint is linked to a field OID and contains the data value.
- **Field**: The definition of a single data item on a form (e.g., a question or variable). Identified by a field OID and has properties such as type, label, and dictionary linkage.

## Common Patterns and Best Practices
- Use try/catch blocks for error handling and log exceptions where appropriate.
- Always check for nulls before accessing properties or methods on objects.
- Use FindByFieldOID and FetchAllDataPointsForOIDPath for efficient data access.
- Use comments and XML documentation to clarify function purpose and logic.

## Frequently Used Methods
- `CustomFunction.FetchAllDataPointsForOIDPath(fieldOID, formOID, instance, subject)`: Fetches all DataPoints for a given field and form within a subject.
- `CustomFunction.PerformQueryAction(record, fieldOID, queryText)`: Opens a query on a specific DataPoint.
- `DataPoints.FindByFieldOID(fieldOID)`: Finds a DataPoint by its field OID within a record.

## Query and Query Closure Conventions
- Always use `CustomFunction.PerformQueryAction` to open queries.
- Check if a query already exists before opening a new one, if required by business rules.
- Use appropriate query text to clearly communicate the issue to the user.
- To close a query, use the appropriate method provided by the API (e.g., `CustomFunction.CloseQueryAction`).

## Coding and Dictionary Handling
- Use the `Coding` or `StandardValue` property of a DataPoint to access coded values.
- Use the `Data` property to get the display value (what the user sees in the CRF).
- Always check for nulls before accessing coding information.

## Performance Tips
- Minimize loops over large collections; use targeted search methods.
- Cache frequently accessed objects if possible within the function scope.

## Security and Data Privacy
- Do not log or expose sensitive subject data in error messages or queries.
- Follow all applicable data privacy and security guidelines.

## Sample Templates
### Edit Check Template
```csharp
try {
    ActionFunctionParams afp = (ActionFunctionParams) ThisObject;
    DataPoint dpt_FIELD_OID = afp.ActionDataPoint;
    // ...edit check logic...
} catch (Exception ex) {
    // Handle exception
}
```
### Derivation Template
```csharp
try {
    ActionFunctionParams afp = (ActionFunctionParams) ThisObject;
    DataPoint dpt_FIELD_OID = afp.ActionDataPoint;
    // ...derivation logic...
    return derivedValue;
} catch (Exception ex) {
    // Handle exception
    return null;
}
```

## Error and Exception Handling
- Use try/catch blocks in all custom functions.
- Log or handle exceptions as appropriate for the environment.

## Comments and Documentation
- Use meaningful comments to explain logic and business rules.
- Use XML documentation for public methods and classes.

## Testing and Validation
- Structure code to allow for easy testing and validation.
- Use descriptive variable names and modularize logic for maintainability.

## Custom Function Input Convention
- In all custom function programming, always define the input DataPoint using:
  `ActionFunctionParams afp = (ActionFunctionParams) ThisObject;`
- This pattern should be used at the start of each function to access the input DataPoint and related context.

## Query Action Convention
- When you need to open or raise a query in code, always use the `CustomFunction.PerformQueryAction` method. Do not use any other method for opening queries.
