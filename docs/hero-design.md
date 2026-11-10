# Studio.Design reference and landing implementation

The live reference was inspected in a browser, including its computed header style, at desktop and mobile sizes. Decode Studio keeps its own branding and database content.

## Page order

Hero → About → Services → Website showcase → People → Contact → Footer.

## Hero and navigation

- The hero fills the viewport. Its heading sits at the upper left and supporting copy and CTA at the lower right. On mobile, the gallery appears above the text.
- Each gallery step contains a 2:1 project preview and a square panel using colors sampled from that project image. The panel has a diagonal stripe and project title. No palette columns were added to the database.
- Desktop tile height is `100svh / 4.7`. Both step directions use that same measurement. GSAP recalculates the travel distance on resize and preserves animation progress.
- The header stays fixed 20px from the top and spans the viewport with 20px side margins. Its glass effect covers both the logo and links. The reference uses a transparent header with `blur(50px)`; ours also increases saturation to retain more color from the muted demo previews.
- A light tint is added over dark sections for readable navigation. The mobile menu closes after navigation and on Escape.
- The visible pause button was removed. The gallery keeps moving on hover and focus, while respecting reduced motion and pausing offscreen or in a background tab.

## Services, work, people, footer

Services use a black-to-light-gray gradient, a full-width featured card, then two-column white cards. Cards show database titles/descriptions and optional uploaded images. Sample services store generated WebP image paths in the database and seed JSON. Uploaded images take precedence; the known demo titles also have matching local fallbacks. Upload an image through Admin → Services to replace these.

The website showcase uses three aligned columns with 20px gutters and taller hero-only captures at the reference aspect ratio. On phones, the wall scrolls horizontally. People use a centered 1050px grid with two 430px-wide portrait columns, a shared heading edge, and role, name, and biography beneath each image. The footer uses a dark background with grouped studio, navigation, account, and contact information.

## Files

| File | Responsibility |
|---|---|
| `MyPortfolio/Views/Home/Sections/` | Database-driven section markup |
| `MyPortfolio/Styles/hero.css` | Hero, fixed navigation, glass, responsive layout |
| `MyPortfolio/Styles/landing.css` | Services, showcase, people, footer |
| `MyPortfolio/Styles/input.css` | Tailwind entry, font, theme, shared controls |
| `MyPortfolio/wwwroot/js/hero-animation.js` | GSAP gallery motion and playback |
| `MyPortfolio/wwwroot/js/hero-palette.js` | Color sampling, neutral fallback for inaccessible images |
| `MyPortfolio/wwwroot/js/site.js` | Mobile menu and header contrast |
| `MyPortfolio/Views/Shared/_Footer.cshtml` | Shared footer |
| `scripts/Add-ServiceImages.ps1` | Idempotent optional service image column upgrade |
| `scripts/Set-ServiceSampleImages.ps1` | Backfill service artwork paths without replacing uploaded images |

GSAP is installed through npm and copied to `wwwroot/lib/gsap` during the .NET build. It is loaded locally on the home page only. Inter is self-hosted with its license in `wwwroot/fonts`.

## Verification and limits

The .NET build, landing markup check, motion/resize test, and portal CRUD regression pass. Service image create/edit/delete uses the existing validated upload service and was included in the regression. Desktop/mobile browser checks cover the gallery, fixed glass header, services, showcase, people, footer, and horizontal overflow. A wider CSS viewport checks the geometry involved when zooming out; browser-native zoom was not separately automated.

Project previews and team portraits are still sample placeholders. Upload real images for the richer artwork seen in the reference. No reference artwork or builder embeds were copied. Analytics, password recovery, and deployment changes are outside this update.

## Hero-only sample previews

Eight distinct hero-only samples are saved under `wwwroot/demos/`, with captures under `wwwroot/images/projects/`. They contain no generated people. The main hero selects these images for the original demo placeholders without updating database records. See [hero-assets.md](hero-assets.md) for the editable files, prompts, and capture workflow. The later hero-only scope takes precedence over expanding the sample pages or changing additional landing sections.


Reference scale corrections: desktop hero 80px/600, hero description 20px/500, services heading 72px, service card headings 48px/700, service body 20px, service container 90% wide. Palette stripes use a full-square diagonal clip path. Shared slash marks are SVGs in _StudioWordmark.cshtml. Generated service prompts and source paths are in scripts/seed/service-image-assets.json; final images are in MyPortfolio/wwwroot/images/services/.



The hero copy is now 'Your next site. Starts here.' with a shorter description, saved in both SiteContent and the sample-data JSON. The load sequence is headline at 0.2s, navigation and description at 0.4s, then a 1.6s gallery fade starting at 0.6s. GSAP CustomEase uses the reference curve (0.2, 1, 1, 1). Continuous gallery travel is one row per 3.5 seconds and does not pause on hover. Service CTAs read 'Learn more', are 46px high, and use a white variant over the featured image. The featured artwork is cropped beyond the card edge.

