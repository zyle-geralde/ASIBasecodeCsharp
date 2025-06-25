document.addEventListener('DOMContentLoaded', function ()
{
    const profileDropdownTrigger = document.querySelector('.profile-dropdown-trigger');
    const dropdown = profileDropdownTrigger ? profileDropdownTrigger.querySelector('.dropdown') : null;

    if (!profileDropdownTrigger || !dropdown)
    {
        console.error('Required dropdown elements not found. Dropdown functionality will not be active.');
        return;
    }

    profileDropdownTrigger.addEventListener('click', function (event)
    {
        dropdown.classList.toggle('show-dropdown');
        event.stopPropagation();
    });

    document.addEventListener('click', function (event)
    {
        if (!profileDropdownTrigger.contains(event.target))
        {
            dropdown.classList.remove('show-dropdown');
        }
    });
});