
export function initializeFeaturedToggle() {
    const featuredCheckbox = document.getElementById('isFeatured');
    const featuredStatus = document.getElementById('featuredStatus');

    if (featuredCheckbox && featuredStatus) {
        featuredStatus.textContent = featuredCheckbox.checked ? 'Yes' : 'No';
        featuredCheckbox.addEventListener('change', function () {
            featuredStatus.textContent = this.checked ? 'Yes' : 'No';
        });
    } else {
        console.warn("Featured toggle elements (isFeatured or featuredStatus) not found.");
    }
}