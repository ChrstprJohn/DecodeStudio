// Colors from the eight saved HTML themes; uploaded images are sampled below.
const demoPalettes = {
    '/images/projects/architecture.webp': ['#243b2d', '#f1efe7'],
    '/images/projects/ecommerce.webp': ['#86152c', '#f8dfdc'],
    '/images/projects/hospitality.webp': ['#087c83', '#e9e4d8'],
    '/images/projects/fitness.webp': ['#d4f42c', '#161817'],
    '/images/projects/food.webp': ['#c95b32', '#f4e8d8'],
    '/images/projects/education.webp': ['#152e87', '#ffce42'],
    '/images/projects/beauty.webp': ['#3f4723', '#eeedc5'],
    '/images/projects/creative.webp': ['#41dded', '#171e20']
};
// Read two prominent image colors without adding database fields just for the hero.
document.querySelectorAll('[data-gallery-project]').forEach(project => {
    const image = project.querySelector('img');
    const panel = project.querySelector('.gallery-palette');
    const theme = demoPalettes[image.getAttribute('src')];
    if (theme) {
        panel.style.setProperty('--project-color', theme[0]);
        panel.style.setProperty('--project-accent', theme[1]);
        return;
    }
    function applyPalette() {
        try {
            const canvas = document.createElement('canvas');
            canvas.width = canvas.height = 40;
            const context = canvas.getContext('2d', { willReadFrequently: true });
            context.drawImage(image, 0, 0, 40, 40);
            const pixels = context.getImageData(0, 0, 40, 40).data;
            const colors = new Map();
            for (let i = 0; i < pixels.length; i += 4) {
                const rgb = [pixels[i], pixels[i + 1], pixels[i + 2]].map(value => Math.round(value / 24) * 24);
                if (pixels[i + 3] < 128 || Math.min(...rgb) > 240) continue;
                const key = rgb.join(',');
                colors.set(key, (colors.get(key) || 0) + 1);
            }
            const palette = [...colors].sort((a, b) => b[1] - a[1]).map(([rgb]) => rgb.split(',').map(Number));
            if (!palette.length) return;
            const brightness = rgb => rgb.reduce((sum, channel) => sum + channel) / 3;
            const colorful = rgb => Math.max(...rgb) < 250 && Math.max(...rgb) - Math.min(...rgb) > 24;
            const primary = palette.find(colorful) || palette.find(rgb => brightness(rgb) < 120) || palette[0];
            const accent = palette.find(rgb => Math.abs(brightness(rgb) - brightness(primary)) > 60) || [238, 238, 238];
            panel.style.setProperty('--project-color', `rgb(${primary.join(',')})`);
            panel.style.setProperty('--project-accent', `rgb(${accent.join(',')})`);
        } catch {
            // External images may block canvas access; retain the neutral CSS palette.
        }
    }
    if (image.complete && image.naturalWidth) applyPalette();
    else image.addEventListener('load', applyPalette, { once: true });
});




