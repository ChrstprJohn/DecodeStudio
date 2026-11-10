const mobileMenu = document.querySelector('header details');
if (mobileMenu) {
    mobileMenu.querySelectorAll('a').forEach(link => {
        link.addEventListener('click', () => { mobileMenu.open = false; });
    });
    document.addEventListener('keydown', event => {
        if (event.key === 'Escape' && mobileMenu.open) {
            mobileMenu.open = false;
            mobileMenu.querySelector('summary').focus();
        }
    });
}


// Keep the glass navigation readable over the dark landing sections.
const floatingHeader = document.querySelector('.home-header');
if (floatingHeader?.classList) {
    const sections = ['services', 'contact', 'footer'].map(id => document.getElementById(id)).filter(Boolean);
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
