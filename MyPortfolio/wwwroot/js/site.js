const studioFooter = document.querySelector('.studio-footer');
if (studioFooter && window.gsap) {
    gsap.matchMedia().add('(prefers-reduced-motion: no-preference)', context => {
        const columns = studioFooter.querySelectorAll('.footer-columns > div');
        const word = studioFooter.querySelector('.footer-display-word');
        const letters = word.children;
        const copyright = studioFooter.querySelector('.footer-bottom p');

        gsap.set([...columns, ...letters, copyright], { opacity: 0, y: 12 });
        const observer = new IntersectionObserver(entries => {
            context.add(() => {
                for (const { target, isIntersecting } of entries) {
                    if (!isIntersecting) continue;
                    observer.unobserve(target);
                    gsap.to(target === word ? letters : target, {
                        opacity: 1,
                        y: 0,
                        duration: .55,
                        ease: 'power3.out',
                        stagger: target === word ? .1 : 0,
                        clearProps: 'opacity,transform'
                    });
                }
            });
        }, { threshold: .15 });

        [...columns, word, copyright].forEach(element => observer.observe(element));
        return () => observer.disconnect();
    });
}

const mobileMenu = document.querySelector('header details');
if (mobileMenu) {
    const header = mobileMenu.closest('header');
    const home = header.classList.contains('home-header');
    const toggle = mobileMenu.querySelector('summary');
    const nav = mobileMenu.querySelector('nav');
    const rows = [...nav.children];
    const backdrop = document.createElement('div');
    backdrop.className = 'mobile-menu-backdrop';
    backdrop.setAttribute('aria-hidden', 'true');
    header.prepend(backdrop);
    const brand = home ? header.querySelector('.public-width > a') : null;
    let closeIcon;
    if (brand) {
        const menuHeading = document.createElement('div');
        menuHeading.className = 'mobile-menu-heading';
        const menuBrand = brand.cloneNode(true);
        menuBrand.removeAttribute('href');
        closeIcon = document.createElement('span');
        closeIcon.className = 'mobile-menu-close-icon';
        menuHeading.append(menuBrand, closeIcon);
        backdrop.append(menuHeading);
        header.classList.add('menu-layered');
    }
    rows.forEach(row => {
        if (row.tagName === 'A') {
            const label = document.createElement('span');
            label.className = 'mobile-link-label';
            label.append(...row.childNodes);
            row.append(label);
        }
    });
    const labels = nav.querySelectorAll('.mobile-link-label, form button');
    const linkLabels = nav.querySelectorAll('.mobile-link-label');
    const wipes = home ? Array.from(linkLabels, label => {
        const wipe = document.createElement('span');
        wipe.className = 'mobile-link-wipe';
        wipe.setAttribute('aria-hidden', 'true');
        label.after(wipe);
        return wipe;
    }) : [];
    const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)');
    let animation;
    let closing = false;
    toggle.setAttribute('aria-expanded', 'false');
    nav.id = 'mobile-navigation';
    toggle.setAttribute('aria-controls', nav.id);
    function finishClose() {
        mobileMenu.open = false;
        closing = false;
        header.classList.remove('menu-active');
        document.querySelector('main').inert = false;
        document.querySelector('footer').inert = false;
        if (window.gsap) {
            gsap.set([backdrop, nav, ...labels, ...wipes], { clearProps: 'all' });
            if (closeIcon) gsap.set(closeIcon, { clearProps: 'opacity' });
        }
    }
    function closeMenu() {
        if (!mobileMenu.open || closing) return;
        closing = true;
        toggle.setAttribute('aria-expanded', 'false');
        animation?.kill();
        if (!home || !window.gsap || reducedMotion.matches) { finishClose(); return; }
        animation = gsap.timeline({ onComplete: finishClose })
            .set(wipes, { opacity: 0 }, 0)
            .to(labels, { x: -16, opacity: 0, duration: .24, stagger: .022, ease: 'power2.in' }, 0)
            .to([backdrop, nav], { xPercent: -100, duration: .55, ease: 'power3.inOut' }, .12);
        if (closeIcon) animation.to(closeIcon, { opacity: 0, duration: .18, ease: 'power2.in' }, 0);
    }
    toggle.addEventListener('click', event => {
        event.preventDefault();
        if (mobileMenu.open && !closing) { closeMenu(); return; }
        animation?.kill();
        closing = false;
        mobileMenu.open = true;
        header.classList.add('menu-active');
        toggle.setAttribute('aria-expanded', 'true');
        document.querySelector('main').inert = true;
        document.querySelector('footer').inert = true;
        if (!home || !window.gsap || reducedMotion.matches) return;
        const stagger = .09;
        const wipeStart = .49;
        const wipeIn = .21;
        const revealStart = wipeStart + wipeIn;
        if (closeIcon) gsap.set(closeIcon, { opacity: 0 });
        animation = gsap.timeline()
            .fromTo([backdrop, nav], { xPercent: -100 }, { xPercent: 0, duration: .68, ease: 'power3.inOut' }, 0)
            .set(linkLabels, { opacity: 0 }, 0)
            .fromTo(wipes, { clipPath: 'inset(0 100% 0 0)', opacity: 1 }, { clipPath: 'inset(0 0% 0 0)', duration: wipeIn, stagger, ease: 'power2.out' }, wipeStart)
            .set(linkLabels, { opacity: 1, stagger }, revealStart)
            .to(wipes, { clipPath: 'inset(0 0 0 100%)', duration: .32, stagger, ease: 'power2.inOut' }, revealStart);
        if (closeIcon) animation.to(closeIcon, { opacity: 1, duration: .3, ease: 'power2.out' }, .68);
        const logoutButton = nav.querySelector('form button');
        if (logoutButton) {
            animation.fromTo(logoutButton, { x: -12, opacity: 0 }, { x: 0, opacity: 1, duration: .4, ease: 'power3.out' }, revealStart + wipes.length * stagger);
        }
    });
    nav.querySelectorAll('a').forEach(link => link.addEventListener('click', () => {
        animation?.kill();
        toggle.setAttribute('aria-expanded', 'false');
        finishClose();
    }));
    document.addEventListener('keydown', event => {
        if (!mobileMenu.open) return;
        if (event.key === 'Escape') { closeMenu(); toggle.focus(); }
        if (event.key === 'Tab') {
            const focusable = [toggle, ...nav.querySelectorAll('a,button')];
            const first = focusable[0], last = focusable[focusable.length - 1];
            if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
            else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
        }
    });
    window.matchMedia(home ? '(min-width: 1200px)' : '(min-width: 1024px)').addEventListener('change', event => {
        if (event.matches) { animation?.kill(); finishClose(); toggle.setAttribute('aria-expanded', 'false'); }
    });
}
// Keep the glass navigation readable over the dark landing sections.
const floatingHeader = document.querySelector('.home-header');
if (floatingHeader?.classList) {
    const sections = ['services', 'footer'].map(id => document.getElementById(id)).filter(Boolean);
    let scheduled = false;
    function updateHeaderContrast() {
        scheduled = false;
        const overDark = sections.some(section => {
            const rect = section.getBoundingClientRect();
            const darkBottom = section.id === 'services' ? rect.top + rect.height * .55 : rect.bottom;
            return rect.top < 82 && darkBottom > 20;
        });
        floatingHeader.classList.toggle('is-over-dark', overDark);
    }
    window.addEventListener('scroll', () => {
        if (!scheduled) { scheduled = true; requestAnimationFrame(updateHeaderContrast); }
    }, { passive: true });
    window.addEventListener('resize', updateHeaderContrast);
    updateHeaderContrast();
}
