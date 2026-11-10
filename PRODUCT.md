# Decode Studio

School portfolio/business website built with ASP.NET Core MVC, Entity Framework
Core, SQL Server, Razor views, and Tailwind CSS. The studio is called Decode Studio
(DCD). Maintain a small, readable codebase and conventional controller/action URLs.

Visitors browse hero, About, services, website showcase, contact, and team content.
Local school mode requires login; public mode allows anonymous browsing.

Registered users manage only their own profile/photo/password. Admins manage
website content and can view/activate/deactivate accounts, without deleting them.
Admin login opens an analytics page that is intentionally blank for now.

All content and account features use the existing portfolio database. Image file
pickers save files under wwwroot/uploads/images and SQL stores relative paths.
Sample content and accounts are available for local design work.

The approved visual direction is documented in DESIGN.md. Detailed architecture,
features, endpoints, testing, and work history remain under docs/.
