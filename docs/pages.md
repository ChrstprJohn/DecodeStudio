# Requested pages and sections

Status: planned, not implemented. Working brand: DCD / Decode Studio.

## Landing page

Begin with one landing page and section navigation. Contact and About can stay
as sections rather than separate pages for the initial version.

| Section | Content and behavior | Database source |
| --- | --- | --- |
| Header/navigation | Studio name and links to sections; account navigation depends on access mode | SiteContent plus fixed navigation markup |
| Hero | Main heading, studio introduction, optional image, primary link | SiteContent |
| About the studio | Business/company description | SiteContent |
| Website showcase | Preview image, project title/description, link to visit each website | Published Projects in SortOrder |
| Services | Web design, web development, deployment, and other studio services | Published Services in SortOrder |
| Contact | Email, optional phone, social/contact links | SiteContent and SocialLinks |
| Owner / our team | Separate first/last names displayed together, role, bio, optional photo | Published TeamMembers in SortOrder; IsOwner identifies owner |
| Footer | Studio identity, footer text, reused contact/social links and navigation | SiteContent and SocialLinks |

This covers the assignment's About the Business/Company, Products and Services,
Contact Information, and Owners Information. Showcase entries are examples of
websites, not products with purchasing or checkout.

## Account pages

- Login, registration, and logout behavior.
- My profile: view/edit own first, middle, and last name and profile image.
- Change password through the custom account service and Microsoft's password hasher.

Users cannot edit studio content or other users' accounts. Their profile does
not automatically appear in the studio's team section.

## Admin pages

| Page | Features |
| --- | --- |
| Dashboard | Account totals, active/inactive users, registrations by date, published content counts |
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

A proposed `Portfolio:RequireLogin` setting controls portfolio access on the
server. Hiding navigation alone does not enforce permissions.

Contact forms, ecommerce, visitor tracking, and a general drag-and-drop page
builder are outside the initial scope. Traffic analytics can be planned later.
