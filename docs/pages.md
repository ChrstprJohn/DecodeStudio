# Requested pages and sections

Status: functional implementation with the shared Studio.Design-inspired visual direction. Working brand: DCD / Decode Studio.

Landing sections render saved published content. Login/signup, an admin sidebar,
content CRUD, account activation, and the own-profile portal are implemented.
See [features.md](features.md) for actual endpoints. Analytics remains blank.

## Landing page

Begin with one landing page and section navigation. Contact and About can stay
as sections rather than separate pages for the initial version.

| Section | Content and behavior | Database source |
| --- | --- | --- |
| Header/navigation | Studio name and links to sections; account navigation depends on access mode | SiteContent plus fixed navigation markup |
| Hero | Gray editorial canvas, moving project gallery, database headline/description and primary link | SiteContent and published Projects |
| About the studio | Business/company description | SiteContent |
| Services | Web design, web development, deployment, and other studio services | Published Services in SortOrder |
| Website showcase | Preview image, project title/description, link to visit each website | Published Projects in SortOrder |
| Contact | Email, optional phone, social/contact links | SiteContent and SocialLinks |
| Team | Separate first/last names displayed together, role, bio, optional photo | Published TeamMembers in SortOrder |
| Footer | Studio identity, footer text, reused contact/social links and navigation | SiteContent and SocialLinks |

This covers the assignment's About the Business/Company, Products and Services,
Contact Information, and Owners Information. Showcase entries are examples of
websites, not products with purchasing or checkout.

## Landing implementation and section order

The visitor-facing order is fixed for now:

1. Hero
2. About the studio
3. Services
4. Products / website showcase
5. Contact
6. Owner and team

The shared header/navigation precedes these sections; the shared footer follows.
Products means showcased websites, not ecommerce product listings.

Views/Home/Index.cshtml is the composition file. It calls one Razor partial per
section, in that order:

| Section | File under Views/Home/Sections | Data |
| --- | --- | --- |
| Hero | _Hero.cshtml | Database headline/description/link and published project-image gallery |
| About | _About.cshtml | About title and description |
| Services | _Services.cshtml | Published service entries |
| Showcase | _Projects.cshtml | Published project titles, images, descriptions, links |
| Contact | _Contact.cshtml | Email, optional phone, ordered social links |
| Team | _Team.cshtml | Published owner/team names, roles, photos, biographies |

Each partial receives HomeViewModel. The controller/service still loads data once
for the page; views perform no database queries. Published collections retain
SortOrder then Id ordering. Section containers/headings remain present if data
is empty; optional images and links render only when configured.

The shared layout owns the sticky public header and mobile menu. All section
anchors, account links, and role-aware actions remain available. The current gray,
white, and charcoal design applies across public sections, auth/profile forms,
and admin views. See [hero-design.md](hero-design.md) and [DESIGN.md](../DESIGN.md).
Edit one section partial when designing that section, or Index.cshtml to reorder.

## Account pages

- Login, registration, and logout behavior.
- My profile: view/edit own first, middle, and last name and profile image.
- Change password through the custom account service and Microsoft's password hasher.

Users cannot edit studio content or other users' accounts. Their profile does
not automatically appear in the studio's team section.

## Admin pages

| Page | Features |
| --- | --- |
| Analytics (default) | Blank page for now; statistics are deferred |
| Studio content | Edit studio name, hero, About, contact details, and footer |
| Projects | Add/edit/remove showcase entries, images and live links; order and publish/unpublish |
| Services | Add/edit/remove services; order and publish/unpublish |
| Team | Manage owner/team names, roles, bios, photos; order and publish/unpublish |
| Social links | Manage shared contact/social links and ordering |
| Accounts | List/view users and deactivate/reactivate accounts; personal details remain read-only |

Admin pages require the Admin role in every deployment mode. Deactivating users
must also prevent continued access through an existing signed-in session.

## Access modes

- School/local demo: require login to view the portfolio. Login, registration,
  and their required public assets remain accessible.
- Public deployment: allow anonymous portfolio browsing. Keep admin and profile
  operations protected. Owner login remains available.

The `Portfolio:RequireLogin` setting controls portfolio access on the
server. Hiding navigation alone does not enforce permissions.

Contact forms, ecommerce, visitor tracking, and a general drag-and-drop page
builder are outside the initial scope. Traffic analytics can be planned later.

## Landing visual refinement (2026-11-10)

The landing sequence is Hero, About, Services, Website Showcase, People, Contact, then the footer. Services have one wide featured card followed by two-column image cards over a dark-to-light gradient. The showcase is a two-row image wall; the people section uses tall grayscale portraits in two columns. Navigation is fixed with full-width frosted glass, and the footer has dark grouped links. See [hero-design.md](hero-design.md) for the files and responsive behavior.
