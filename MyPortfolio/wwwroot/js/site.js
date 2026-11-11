const successAlert = document.querySelector('[data-success-alert]');
if (successAlert) {
    window.setTimeout(() => successAlert.remove(), 5000);
}

const deleteConfirmation = document.querySelector('#delete-confirmation');
if (deleteConfirmation) {
    const form = deleteConfirmation.querySelector('form');
    for (const link of document.querySelectorAll('[data-confirm-delete]')) {
        link.addEventListener('click', event => {
            if (event.metaKey || event.ctrlKey || event.shiftKey || event.altKey) return;
            event.preventDefault();
            form.action = link.href;
            deleteConfirmation.querySelector('#delete-confirmation-title').textContent = link.dataset.confirmDelete;
            deleteConfirmation.querySelector('#delete-confirmation-message').textContent = link.dataset.deleteMessage;
            deleteConfirmation.querySelector('[data-delete-submit]').textContent = link.dataset.deleteLabel || 'Delete';
            deleteConfirmation.showModal();
        });
    }
}

for (const input of document.querySelectorAll('[data-upload-preview]')) {
    const preview = input.nextElementSibling;
    const image = preview.querySelector('img');
    const status = preview.nextElementSibling;
    const savedSource = image.getAttribute('src');
    let previewUrl;

    input.addEventListener('change', () => {
        if (previewUrl) URL.revokeObjectURL(previewUrl);
        const file = input.files[0];
        previewUrl = file ? URL.createObjectURL(file) : null;
        const source = previewUrl || savedSource;
        if (source) image.src = source;
        else image.removeAttribute('src');
        image.hidden = !source;
        status.textContent = file ? `Selected ${file.name}. Save to apply.` : '';
    });
}

const socialPlatforms = document.querySelector('#social-platforms');
if (socialPlatforms) {
    const label = socialPlatforms.parentElement.querySelector('#Label');
    const icons = new Map(Array.from(socialPlatforms.children, platform => [
        platform.dataset.platform.toLowerCase(), platform.dataset.icon
    ]));
    const upload = document.querySelector('[data-social-icon-upload]');
    const preview = upload.nextElementSibling;
    const image = preview.querySelector('img');
    const status = preview.nextElementSibling;
    const remove = document.querySelector('#RemoveCustomIcon');
    let selectedIcon;
    const updateIcon = () => {
        const name = label.value.trim().replace(/ \(demo\)$/i, '').toLowerCase();
        const source = selectedIcon || (!remove?.checked && upload.dataset.currentIcon) || icons.get(name);
        image.hidden = !source;
        if (source) image.src = source;
        else image.removeAttribute('src');
    };
    label.addEventListener('input', updateIcon);
    upload.addEventListener('change', () => {
        if (selectedIcon) URL.revokeObjectURL(selectedIcon);
        const file = upload.files[0];
        selectedIcon = file ? URL.createObjectURL(new Blob([file], { type: 'image/svg+xml' })) : null;
        if (file && remove) remove.checked = false;
        status.textContent = file ? `Selected ${file.name}. Save to apply.` : '';
        updateIcon();
    });
    remove?.addEventListener('change', () => {
        if (remove.checked) {
            upload.value = '';
            upload.dispatchEvent(new Event('change'));
            return;
        }
        updateIcon();
    });
    updateIcon();
}

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

const mobileMenu = document.querySelector('header .mobile-menu');
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
        if (window.gsap) gsap.set([backdrop, nav, ...labels, ...wipes], { clearProps: 'all' });
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
        const wipeStart = .27;
        const wipeIn = .21;
        const revealStart = wipeStart + wipeIn;
        animation = gsap.timeline()
            .fromTo([backdrop, nav], { xPercent: -100 }, { xPercent: 0, duration: .68, ease: 'power3.inOut' }, 0)
            .set(linkLabels, { opacity: 0 }, 0)
            .fromTo(wipes, { clipPath: 'inset(0 100% 0 0)', opacity: 1 }, { clipPath: 'inset(0 0% 0 0)', duration: wipeIn, stagger, ease: 'power2.out' }, wipeStart)
            .set(linkLabels, { opacity: 1, stagger }, revealStart)
            .to(wipes, { clipPath: 'inset(0 0 0 100%)', duration: .32, stagger, ease: 'power2.inOut' }, revealStart);
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
            const focusable = [toggle, ...Array.from(nav.querySelectorAll('a,button,summary'))
                .filter(element => element.tagName === 'SUMMARY' || !element.closest('.profile-menu:not([open])'))];
            const first = focusable[0], last = focusable[focusable.length - 1];
            if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last.focus(); }
            else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first.focus(); }
        }
    });
    window.matchMedia(home ? '(min-width: 1200px)' : '(min-width: 1024px)').addEventListener('change', event => {
        if (event.matches) { animation?.kill(); finishClose(); toggle.setAttribute('aria-expanded', 'false'); }
    });
}
const adminMenu = document.querySelector('[data-admin-menu]');
if (adminMenu) {
    adminMenu.addEventListener('click', () => {
        adminMenu.setAttribute('aria-expanded', String(adminMenu.getAttribute('aria-expanded') !== 'true'));
    });
    document.addEventListener('keydown', event => {
        if (event.key !== 'Escape' || adminMenu.getAttribute('aria-expanded') !== 'true') return;
        if (!adminMenu.closest('.admin-sidebar').contains(document.activeElement)) return;
        adminMenu.setAttribute('aria-expanded', 'false');
        adminMenu.focus();
    });
    window.matchMedia('(min-width: 64rem)').addEventListener('change', event => {
        if (event.matches) adminMenu.setAttribute('aria-expanded', 'false');
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
