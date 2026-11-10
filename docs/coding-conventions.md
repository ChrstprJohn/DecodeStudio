# MVC coding conventions

Use these conventions for new features in this project. Keep code readable for
someone learning ASP.NET Core; prefer explicit steps over compressed syntax.

## Controller and action URLs

Use the existing conventional route in Program.cs:

```csharp
pattern: "{controller=Home}/{action=Index}/{id?}"
```

URLs follow `/ControllerName/ActionName`, dropping the Controller suffix from
the class name. The optional id can be a third path segment.

| Controller class | Action method | URL |
| --- | --- | --- |
| HomeController | Index | / or /Home/Index |
| ServicesController | Index | /Services/Index |
| ServicesController | Create | /Services/Create |
| ServicesController | Edit | /Services/Edit |
| ServicesController | Delete | /Services/Delete |

These feature controllers/actions are examples unless implemented. Do not add
explicit attribute URL routes or action-name aliases for ordinary CRUD. Keep
the URL connected to the actual controller and method names.

```csharp
public IActionResult Delete(int id)
```

- Delete is the action name used in the URL.
- IActionResult is the response type, not part of the URL.
- id is input to the action, not its name.
- HttpGet/HttpPost attributes choose the HTTP method, not the URL path.

## Links and forms call actions

Use Razor tag helpers instead of manually constructing internal action URLs:

```cshtml
<a asp-controller="Services" asp-action="Index">View services</a>

<form asp-controller="Services" asp-action="Delete" method="post">
    <input type="hidden" name="id" value="5" />
    <button type="submit">Delete</button>
</form>
```

The link requests GET /Services/Index. The form sends POST /Services/Delete with
id = 5. Input names match action parameter names or form view-model properties.
Use real model IDs rather than the example value in implemented views.

## CRUD actions

Use Index for listing/display, Create for adding, Edit for updating, and Delete
for removal. Use GET for displaying pages and POST for changing saved data.
Never delete or otherwise change saved data through a GET request.

When a separate form page is needed, GET and POST actions may share the same
name, distinguished by their HTTP-method attributes and C# parameter signatures:

```text
GET  /Services/Create → display the creation form
POST /Services/Create → validate and save the submitted form
GET  /Services/Edit/5 → display the editing form for record 5
POST /Services/Edit   → validate and save the submitted changes
POST /Services/Delete → validate the request and remove the specified record
```

Apply ValidateAntiForgeryToken to POST form actions. Razor's form tag helper
generates the matching hidden token for these forms.

After a successful save/delete, redirect to Index or the relevant display action.
If input is invalid, redisplay the appropriate form with validation messages.
Check missing records and authorization before making changes. Content-management
actions require Admin authorization; account actions operate on the signed-in
user's permitted fields. Hidden input IDs are untrusted input.

## Responsibilities and readable constructors

- Controllers receive requests, validate inputs, call services, and return views
  or redirects. Keep database queries out of controller actions.
- Services contain feature operations and EF Core queries.
- Data contains context mappings and database registration.
- Models/Entities describes database records.
- Models/ViewModels describes form inputs and page display data.
- Views contains Razor markup and Tailwind classes; avoid database queries here.

Use explicit constructors and private readonly fields for injected services and
contexts, following the current HomeController and HomeService:

```csharp
private readonly HomeService _homeService;

public HomeController(HomeService homeService)
{
    _homeService = homeService;
}
```

ASP.NET supplies the registered service during construction. Methods use the
saved field afterward. Register concrete feature services with AddScoped in
Program.cs. Request only dependencies the class uses. Database registration
belongs in Data/DatabaseConfiguration.cs.

Use async/await for database operations. Keep simple operations straightforward;
do not add repositories, base classes, or interfaces for every service. Add
optional plumbing only when the feature requires it. This does not remove input
validation, error handling, or authorization requirements.

## Views and JavaScript

By default, return View(model) to render HTML on the server. View discovery uses
Views/{ControllerName}/{ActionName}.cshtml, with shared markup in Views/Shared.
If returning a different view, name it explicitly, such as View("Edit", model).

Start with normal links and form submissions. Use browser JavaScript for local
interactions and fetch only when a feature needs an update without reloading.
Do not introduce a separate React frontend for the current scope.

## Documentation

Update the work log after meaningful implementation changes. Keep architecture,
schema, and setup guides current when their behavior changes. Mark examples and
planned features clearly; do not document them as already implemented.
