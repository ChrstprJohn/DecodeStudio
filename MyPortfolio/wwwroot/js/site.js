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
    rows.forEach(row => {
        if (row.tagName === 'A') {
            const label = document.createElement('span');
            label.className = 'mobile-link-label';
            label.append(...row.childNodes);
            row.append(label);
        }
    });
    const labels = nav.querySelectorAll('.mobile-link-label, form button');
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
        if (window.gsap) gsap.set([backdrop, nav, ...labels], { clearProps: 'all' });
    }
    function closeMenu() {
        if (!mobileMenu.open || closing) return;
        closing = true;
        toggle.setAttribute('aria-expanded', 'false');
        animation?.kill();
        if (!home || !window.gsap || reducedMotion.matches) { finishClose(); return; }
        animation = gsap.timeline({ onComplete: finishClose })
            .to(labels, { x: -24, opacity: 0, duration: .18, stagger: .018, ease: 'power2.in' }, 0)
            .to([backdrop, nav], { xPercent: -100, duration: .4, ease: 'power3.inOut' }, .08);
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
        const ease = window.CustomEase ? CustomEase.create('menu-reveal', '.4,.4,0,1') : 'power3.out';
        animation = gsap.timeline()
            .fromTo([backdrop, nav], { xPercent: -100 }, { xPercent: 0, duration: .5, ease }, 0)
            .fromTo(labels, { x: -44, opacity: 0 }, { x: 0, opacity: 1, duration: .55, stagger: .075, ease }, .2);
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
