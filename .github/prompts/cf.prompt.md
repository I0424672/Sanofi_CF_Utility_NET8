# Copilot Prompt

**Important:**
- All code suggestions must use only types, methods, and classes from the `Medidata.Core.Objects` namespace and the `System` namespace.
- Do not use any other namespaces, libraries, or APIs unless absolutely required and not available in `Medidata.Core.Objects` or `System`.
- Avoid suggesting code from unrelated, external, or third-party libraries.
- For reference, see `.cf/Basic Function.cs` and `.cf/Public Function.cs` in the `cf` directory for example usage.

## Variable Naming Convention
- For local variables, use the following prefixes with underscores and the relevant identifier (folder, form, or field OID) for clarity:
  - Instance: ins_<identifier> (e.g., ins_FOLDER_OID)
  - DataPage: dp_<identifier> (e.g., dp_FORM_OID)
  - DataPoint: dpt_<identifier> (e.g., dpt_FIELD_OID)
- Use PascalCase for class names, method names, and public properties (e.g. ValidateDatapoint, FecthMaxDate).
- Use ALL_CAPS with underscores for constants (e.g., MAX_COUNT).
- Variable names should be descriptive and avoid abbreviations unless they are standard or widely understood.
- Do not use single-letter variable names except for loop counters.

### Examples
- For an Instance object representing a folder OID, use: `ins_FOLDER_OID`
- For a DataPage object for a form OID, use: `dp_FORM_OID`
- For a DataPoints collection, use: `dpts_FIELD_OID`
- For a DataPoint representing a field OID, use: `dpt_FIELD_OID`
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

## Custom Function Input Convention
- In all custom function programming, always define the input DataPoint using:
  `ActionFunctionParams afp = (ActionFunctionParams) ThisObject;`
- This pattern should be used at the start of each function to access the input DataPoint and related context.

## Frequently Used Methods
- `DataPoints.FindByFieldOID(fieldOID)`: Finds a DataPoint by its field OID within a record.
- `CustomFunction.FetchAllDataPointsForOIDPath(string FieldOID, string FormOID, string FolderOID, Subject subject, bool activeOnly)` : Fetches all DataPoints for a given field and form within a subject.
- `CustomFunction.PerformQueryAction(string QueryText, int MarkingGroupID, bool AnswerOnChange, bool CloseOnChange, DataPoint DataPoint, bool Condition, int CheckID, string CheckHash)`: Opens a query on a specific DataPoint.

## Query and Query Closure Conventions
- Always use `CustomFunction.PerformQueryAction(string QueryText, int MarkingGroupID, bool AnswerOnChange, bool CloseOnChange, DataPoint DataPoint, bool Condition, int CheckID, string CheckHash)` to open queries. Do not use any other method for opening queries.
- Parameter clarification for `PerformQueryAction`:
  - `QueryText`: The message to display to the user in the query.
  - `MarkingGroupID`: Always use `1`.
  - `AnswerOnChange`: Should default to `false` (do not require answer on change).
  - `CloseOnChange`: Should default to `false` (do not auto-close on change).
  - `DataPoint`: The target DataPoint to open the query on in the CRF page.
  - `Condition`: Boolean, whether to open (`true`) or close (`false`) the query.
  - `CheckID` and `CheckHash`: Always use the values provided in the function parameters (typically `afp.CheckID` and `afp.CheckHash`).
- Check if a query already exists and not locked (!DataPoint.IsDataPointLocked) before opening a new one, if required by business rules.

## Coding and Dictionary Handling
- Use the `Data` or `StandardValue()` property of a DataPoint to access coded values.
- Always check for nulls before accessing coding information.

## Performance Tips
- Minimize loops over large collections; use targeted search methods.
- Use `DataPoints.FindByFieldOID` to quickly access specific DataPoints instead of iterating through all DataPoints.

## Error and Exception Handling
- Use try/catch blocks in all custom functions.

### Sample Templates
#### Edit Check Template
```csharp
try {
    ActionFunctionParams afp = (ActionFunctionParams) ThisObject;
    DataPoint dpt_action = afp.ActionDataPoint;
    // ...edit check logic...
} catch {}
return null;
```

## Comments and Documentation
- Use meaningful comments to explain logic and business rules.
- Use XML documentation for public methods and classes.
