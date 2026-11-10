# Eight hero-only preview assets

These are **navigation + hero only**, not complete demo websites. They are fictional design samples used in the main landing hero. There are no generated people in the final assets.

| Theme | Editable HTML | Captured image |
|---|---|---|
| Architecture / editorial split | `MyPortfolio/wwwroot/demos/northline/index.html` | `MyPortfolio/wwwroot/images/projects/architecture.webp` |
| Flowers / centered | `MyPortfolio/wwwroot/demos/bloom-stem/index.html` | `MyPortfolio/wwwroot/images/projects/ecommerce.webp` |
| Hotel / centered full-image | `MyPortfolio/wwwroot/demos/coastline/index.html` | `MyPortfolio/wwwroot/images/projects/hospitality.webp` |
| Fitness / dark split | `MyPortfolio/wwwroot/demos/forma/index.html` | `MyPortfolio/wwwroot/images/projects/fitness.webp` |
| Coffee / editorial with centered navigation | `MyPortfolio/wwwroot/demos/cedar-coffee/index.html` | `MyPortfolio/wwwroot/images/projects/food.webp` |
| Learning / centered interface | `MyPortfolio/wwwroot/demos/atlas/index.html` | `MyPortfolio/wwwroot/images/projects/education.webp` |
| Skincare / product editorial | `MyPortfolio/wwwroot/demos/luma/index.html` | `MyPortfolio/wwwroot/images/projects/beauty.webp` |
| Creative studio / sidebar navigation | `MyPortfolio/wwwroot/demos/signal/index.html` | `MyPortfolio/wwwroot/images/projects/creative.webp` |

Shared styles are in `MyPortfolio/wwwroot/demos/shared/demo.css`. The generated photographs are stored alongside them in `shared/photos/`. The HTML and CSS produce the actual navigation, typography, buttons, and interface; those are not AI-generated lettering inside an image. Demo navigation points back to the corresponding portfolio sections.

## How the main hero uses them

`Views/Home/Sections/_Hero.cshtml` substitutes these preview assets only for the original eight `demo-project-*.svg` images when rendering the hero. Custom uploaded images still render normally. Project database records and their URLs were not changed, and the other landing sections were not modified for this asset update.

The gallery remains animated by `wwwroot/js/hero-animation.js`. That file also contains the reduced-motion-aware load sequence for the header, gallery, heading, and supporting copy. Layout and glass styles remain in `Styles/hero.css`.

## Rebuilding the assets

- `python scripts/Generate-DemoWebsites.py` rebuilds the eight hero-only HTML samples and shared CSS.
- Run the app, open each sample, and capture its first viewport at 1600 × 800. Captures were exported through the in-app browser because Chrome's capture returned excess canvas; the completed portfolio was also checked in the user's existing Chrome tab.
- `scripts/Optimize-DemoPreviews.py` converts the captured viewport images into WebP assets and makes a review sheet. The raw capture files and audit are local QA files under `.taste-skill/demo-captures`.
- Photography was generated with the built-in image-generation tool. The exact final photo prompts and original source paths are recorded in `scripts/seed/demo-photo-assets.json`. They request images without text, UI, logos, or people. Only the photography uses image generation; all page composition is HTML/CSS.

The discarded poster-style mockups and person photographs are not used in the project. The current images are frontend design concepts, not claims of completed client work.
