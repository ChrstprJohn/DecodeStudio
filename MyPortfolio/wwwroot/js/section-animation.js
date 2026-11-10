// Reveal cards as they enter the viewport; layout stays in the stylesheets.
(() => {
    if (!window.gsap) return;
    gsap.matchMedia().add('(prefers-reduced-motion: no-preference)', () => {
        const cards = document.querySelectorAll('.studio-services .studio-section-heading, .service-card, .showcase-project, .studio-team .studio-section-heading, .team-person');
        const ease = window.CustomEase
            ? CustomEase.create('sectionReveal', '0,0.5,0.22,1')
            : 'power3.out';
        gsap.set(cards, { autoAlpha: 0, y: 32 });
        const observer = new IntersectionObserver(entries => {
            for (const entry of entries) {
                if (!entry.isIntersecting) continue;
                const card = entry.target;
                const wall = card.classList.contains('showcase-project');
                const index = wall ? Array.from(card.parentElement.children).indexOf(card) : 0;
                gsap.to(card, {
                    autoAlpha: 1, y: 0, scale: 1, duration: 1.6,
                    delay: wall ? [0, .05, .1, .2, .15, .2][index % 6] : 0,
                    ease, clearProps: 'opacity,visibility,transform'
                });
                observer.unobserve(card);
            }
        }, { threshold: 0, rootMargin: '0px 0px -32px 0px' });
        for (const card of cards) {
            if (card.classList.contains('showcase-project')) gsap.set(card, { scale: 1.1 });
            observer.observe(card);
        }
        return () => {
            observer.disconnect();
            gsap.set(cards, { clearProps: 'opacity,visibility,transform' });
        };
    });
})();
