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
