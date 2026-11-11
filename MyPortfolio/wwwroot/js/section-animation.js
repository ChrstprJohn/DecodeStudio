// Reveal content as it enters the viewport; layout stays in the stylesheets.
(() => {
    if (!window.gsap) return;
    gsap.matchMedia().add('(prefers-reduced-motion: no-preference)', () => {
        const aboutItems = Array.from(document.querySelectorAll('#about > h2, #about > div > p'));
        const textItems = [...aboutItems, ...document.querySelectorAll('.showcase-signature')];
        const cards = document.querySelectorAll('.studio-services .studio-section-heading, .service-card, .showcase-project, .studio-team .studio-section-heading, .team-person');
        const ease = window.CustomEase
            ? CustomEase.create('sectionReveal', '0,0.5,0.22,1')
            : 'power3.out';
        const serviceEase = window.CustomEase
            ? CustomEase.create('serviceReveal', '0.2,1,1,1')
            : 'power3.out';
        const serviceLinks = document.querySelectorAll('.service-card .service-link');
        const clearProps = 'opacity,visibility,transform';
        gsap.set(serviceLinks, { autoAlpha: 0, x: -8, y: 8 });
        const observer = new IntersectionObserver(entries => {
            for (const entry of entries) {
                if (!entry.isIntersecting) continue;
                const card = entry.target;
                observer.unobserve(card);
                const aboutIndex = aboutItems.indexOf(card);
                if (aboutIndex !== -1 || card.matches('.showcase-signature')) {
                    gsap.to(card, { autoAlpha: 1, y: 0, duration: .8, delay: Math.max(0, aboutIndex) * .08, ease: serviceEase, clearProps });
                    continue;
                }
                if (card.matches('.studio-services .studio-section-heading')) {
                    gsap.to(card, { autoAlpha: 1, duration: 1.6, ease: serviceEase, clearProps });
                    continue;
                }
                if (card.matches('.service-card')) {
                    gsap.timeline({ defaults: { autoAlpha: 1, y: 0, duration: .8, ease: serviceEase, clearProps } })
                        .to(card, {}, .2)
                        .to(card.querySelector('.service-link'), { x: 0 }, .4);
                    continue;
                }
                const wall = card.classList.contains('showcase-project');
                const index = wall ? Array.from(card.parentElement.children).indexOf(card) : 0;
                gsap.to(card, {
                    autoAlpha: 1, y: 0, scale: 1, duration: 1.6,
                    delay: wall ? [0, .05, .1, .2, .15, .2][index % 6] : 0,
                    ease, clearProps
                });
            }
        }, { threshold: 0, rootMargin: '0px 0px -32px 0px' });
        gsap.set(textItems, { autoAlpha: 0, y: 16 });
        textItems.forEach(item => observer.observe(item));
        for (const card of cards) {
            if (card.matches('.studio-services .studio-section-heading')) gsap.set(card, { autoAlpha: 0 });
            else if (card.matches('.service-card')) gsap.set(card, { autoAlpha: 0, y: 16 });
            else gsap.set(card, { autoAlpha: 0, y: 32, scale: card.matches('.showcase-project') ? 1.1 : 1 });
            observer.observe(card);
        }
        return () => {
            observer.disconnect();
            gsap.set([...textItems, ...cards, ...serviceLinks], { clearProps });
        };
    });
})();
