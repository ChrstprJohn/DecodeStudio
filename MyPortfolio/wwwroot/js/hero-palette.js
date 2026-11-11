// Curated pairs for the selected previews. Other project images are sampled below.
const demoPalettes = {
    '/images/projects/preview-01-tanaka-architecture.jpg': ['#24382b', '#d6c9a7'],
    '/images/projects/preview-02-petal-stem.jpg': ['#bb3326', '#ffd4b6'],
    '/images/projects/preview-03-toneworks.jpg': ['#cc8a23', '#222222'],
    '/images/projects/preview-04-common-form.jpg': ['#e5ddd2', '#584435'],
    '/images/projects/preview-10-line-grain.jpg': ['#302f2b', '#e4d8c9'],
    '/images/projects/preview-13-margin.jpg': ['#e95320', '#f8e7bf'],
    '/images/projects/preview-15-relay.jpg': ['#d4c5fa', '#282043'],
    '/images/projects/preview-16-quartz.jpg': ['#bbdfcc', '#1d4939'],
    '/images/projects/preview-14-ledger.jpg': ['#154d39', '#e9e3ce'],
    '/images/projects/preview-17-morrow.jpg': ['#344e79', '#e1e9fa']
};
// Read two prominent image colors without adding database fields just for the hero.
document.querySelectorAll('[data-gallery-project]').forEach(project => {
    const image = project.querySelector('img');
    const panel = project.querySelector('.gallery-palette');
    const theme = demoPalettes[image.getAttribute('src').split('?')[0]];
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
