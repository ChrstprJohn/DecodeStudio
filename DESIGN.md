# Decode Studio design language

Direction: Studio.Design-inspired editorial portfolio, approved for the public
landing page, account screens, user portal, and admin screens. The live reference
was inspected on desktop and mobile, including its moving hero and project gallery.

## Foundation

- Canvas: #eeeeee; paper surfaces: #ffffff; primary ink/controls: #222222.
- Supporting text: #555555. Project imagery supplies the color.
- Self-hosted Inter variable font, 400/500/600/700 weights. License is stored with
  the font under wwwroot/fonts. No external font request at runtime.
- Display tracking: -0.04em; large headings use tight leading. Body uses readable
  line spacing. Public hero scales from 40px on mobile to 80px on large screens.
- Standard controls use 8px corners, compact images use 4–8px. Form surfaces use
  12px corners. No decorative gradients or card shadows.
- Shared Tailwind theme, base rules, and small component classes live in
  Styles/input.css. Views use these classes and ordinary Tailwind utilities.

## Public site

The sticky monochrome header contains the wordmark, section navigation, and
account actions. On smaller screens it becomes a native details menu, which
closes after navigation or Escape.

The hero presents the database headline at left and a diagonal moving stream
of published project images. Copy/CTA sit at lower right on desktop; mobile
places the gallery above the headline and description. Use up to eight published
projects, repeating rows when fewer are available. The gallery is decorative;
the full project section supplies accessible names and links.

About is an editorial split layout. Services and Contact use charcoal fields.
Projects use staggered image-led columns and captions. Team uses photo/name/role
entries. Keep the required Hero, About, Services, Showcase, Team, Contact order.

## Application screens

Login/register/profile forms use white inset surfaces on the gray canvas.
Admin screens use a compact sidebar with a dark active item; small screens use
scrollable navigation. Tables use white surfaces and subtle row dividers. Forms
share consistent labels, inputs, file pickers, validation, and action buttons.
Analytics remains intentionally blank except for its heading and navigation.

## Motion and accessibility

- Gallery movement is transform-only and linear over 64 seconds per loop.
- A visible pause/play control is supplied. Hovering the artwork also pauses it.
- Respect prefers-reduced-motion: disable gallery movement and smooth scrolling.
- Pause when offscreen or when the document is hidden.
- Preserve visible keyboard focus, form labels, headings, anti-forgery tokens,
  mobile navigation, and every existing authorization boundary.

The source reference is https://studio.design/. Use Decode Studio database copy
and images; source artwork is not copied. Current seeded images remain clearly
marked demo assets for replacement with real work.


The latest landing reference details are in docs/hero-design.md. Services now use reference-led white image cards over a dark-to-light gradient; the fixed full-width glass header uses a 50px blur. Gallery motion is isolated in hero-animation.js, styling in hero.css and landing.css.

