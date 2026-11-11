// GSAP controls motion only. Dimensions and placement belong in Styles/hero.css.
(() => {
    document.documentElement.classList.remove('hero-pending');
    if (window.gsap) {
        let revealEase = 'power3.out';
        if (window.CustomEase) {
            gsap.registerPlugin(CustomEase);
            revealEase = CustomEase.create('studioReveal', '0.2,1,1,1');
        }
        gsap.matchMedia().add('(prefers-reduced-motion: no-preference)', () => {
            gsap.timeline({ delay: 0.5, defaults: { duration: 0.8, ease: revealEase, clearProps: 'opacity,visibility,transform' } })
                .from('.hero-heading', { autoAlpha: 0, x: -16, y: 16 }, 0.2)
                .addLabel('intro', 0.4)
                .from('.hero-introduction', { autoAlpha: 0, x: -16, y: 16 }, 'intro')
                .from('.home-header', { autoAlpha: 0, y: -32 }, 'intro')
                .from('.gallery-window', { autoAlpha: 0, duration: 1.6, ease: 'none' }, 0.6);
        });
    }
    const gallery = document.querySelector('[data-gallery]');
    const track = gallery?.querySelector('[data-gallery-track]');
    if (!track || !track.firstElementChild || !window.gsap) return;

    const rows = track.children.length / 2;
    const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)');
    let visible = true;
    let animation;

    function updatePlayback() {
        animation?.paused(reducedMotion.matches || !visible || document.hidden);
    }

    function resizeGallery() {
        const progress = animation?.progress() || 0;
        animation?.kill();
        const tile = track.firstElementChild.getBoundingClientRect().height;
        animation = gsap.fromTo(track, { x: 0, y: 0 }, {
            x: rows * tile, y: -rows * tile,
            duration: rows * 3.5, ease: 'none', repeat: -1
        });
        animation.progress(progress);
        updatePlayback();
    }

    reducedMotion.addEventListener('change', updatePlayback);
    document.addEventListener('visibilitychange', updatePlayback);
    new IntersectionObserver(entries => {
        visible = entries[0].isIntersecting;
        updatePlayback();
    }).observe(gallery);
    new ResizeObserver(resizeGallery).observe(track.firstElementChild);
    resizeGallery();
})();
