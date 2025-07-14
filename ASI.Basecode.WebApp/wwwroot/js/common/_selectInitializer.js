export async function initializeSingleSelectAuthor() {
    const authorContainer = document.getElementById('authorContainer');
    const authorDropdown = document.getElementById('authorDropdown');
    const selectButton = authorContainer ? authorContainer.querySelector('.custom-select-btn') : null;
    const hiddenInput = document.getElementById('author');

    const initialSelectedValue = hiddenInput ? hiddenInput.value : '';

    await createSearchableDropdown({
        container: authorContainer,
        dropdown: authorDropdown,
        selectButton: selectButton,
        hiddenInput: hiddenInput,
        fetchUrl: '/Book/GetAuthor',
        placeholderText: 'Select Author',
        initialSelectedValue: initialSelectedValue,
        itemType: 'author'
    });
}

export async function initializeSingleSelectLanguage() {
    const languageContainer = document.getElementById('languageContainer');
    const languageDropdown = document.getElementById('languageDropdown');
    const languageSelectButton = languageContainer ? languageContainer.querySelector('.custom-select-btn') : null;
    const languageHiddenInput = document.getElementById('language');

    const initialSelectedValue = languageHiddenInput ? languageHiddenInput.value : '';

    await createSearchableDropdown({
        container: languageContainer,
        dropdown: languageDropdown,
        selectButton: languageSelectButton,
        hiddenInput: languageHiddenInput,
        fetchUrl: '/Book/GetLanguage',
        placeholderText: 'Select Language',
        fallbackItems: [
            'English,English', 'Tagalog,Tagalog', 'Bisaya,Bisaya',
            'Spanish,Spanish', 'French,French', 'German,German', 'Japanese,Japanese'
        ],
        initialSelectedValue: initialSelectedValue,
        itemType: 'language'
    });
}

export async function initializeMultiSelect({
    containerId,
    dropdownId,
    hiddenInputId,
    selectedContainerId,
    fetchUrl,
    placeholderText = 'Select Items',
    fallbackItems = null,
    itemType = 'item'
}) {
    const container = document.getElementById(containerId);
    const dropdown = document.getElementById(dropdownId);
    const selectButton = container ? container.querySelector('.custom-select-btn') : null;
    const hiddenInput = document.getElementById(hiddenInputId);
    const selectedContainer = document.getElementById(selectedContainerId);

    if (!selectedContainer) {
        console.error(`Missing selectedContainer for multi-select: ${selectedContainerId}`);
        return;
    }

    await createSearchableDropdown({
        container: container,
        dropdown: dropdown,
        selectButton: selectButton,
        hiddenInput: hiddenInput,
        fetchUrl: fetchUrl,
        placeholderText: placeholderText,
        fallbackItems: fallbackItems,
        isMultiSelect: true,
        itemType: itemType,
        selectedContainer: selectedContainer
    });
}


async function createSearchableDropdown({
    container,
    dropdown,
    selectButton,
    hiddenInput,
    fetchUrl,
    placeholderText,
    fallbackItems,
    isMultiSelect = false,
    initialSelectedValue = '',
    itemType = 'item',
    selectedContainer = null
}) {
    if (!container || !dropdown || !selectButton || !hiddenInput) {
        console.error(`Missing elements for searchable dropdown initialization (${itemType}).`);
        return;
    }

    const searchInputDiv = document.createElement('div');
    searchInputDiv.className = 'dropdown-search-input';
    searchInputDiv.innerHTML = `
        <input type="text" placeholder="Search ${itemType}s..." class="form-control search-input">
    `;
    dropdown.prepend(searchInputDiv);

    const searchInput = searchInputDiv.querySelector('.search-input');

    selectButton.querySelector('span').textContent = placeholderText;

    selectButton.addEventListener('click', (event) => {
        event.stopPropagation();
        dropdown.classList.toggle('show');
        if (dropdown.classList.contains('show')) {
            searchInput.focus();
        }
    });

    document.addEventListener('click', (event) => {
        if (!container.contains(event.target) && !dropdown.contains(event.target)) {
            dropdown.classList.remove('show');
        }
    });

    let allItems = [];
    let currentSelectedItemsOrder = [];

    function renderDropdownItems(filter = '') {
        const scrollTop = dropdown.scrollTop;
        const wasFocused = document.activeElement === searchInput;

        const currentSearchInputDiv = dropdown.querySelector('.dropdown-search-input');
        dropdown.innerHTML = '';
        dropdown.appendChild(currentSearchInputDiv); 

        if (!isMultiSelect) {
            const defaultOption = document.createElement('div');
            defaultOption.className = 'dropdown-item';
            defaultOption.innerHTML = `<label>${placeholderText}</label>`;
            defaultOption.addEventListener('click', () => {
                hiddenInput.value = '';
                selectButton.querySelector('span').textContent = placeholderText;
                dropdown.classList.remove('show');
                hiddenInput.dispatchEvent(new Event('change'));
                hiddenInput.dispatchEvent(new Event('input'));
            });
            dropdown.appendChild(defaultOption);
        }

        const filteredItems = allItems.filter(item =>
            item.itemName.toLowerCase().includes(filter.toLowerCase())
        );

        if (filteredItems.length === 0 && filter.length > 0) {
            const noResults = document.createElement('div');
            noResults.className = 'dropdown-item no-results';
            noResults.textContent = 'No matching results';
            dropdown.appendChild(noResults);
        }

        filteredItems.forEach(item => {
            const option = document.createElement('div');
            option.className = 'dropdown-item';

            if (isMultiSelect) {
                const checkboxId = `${dropdown.id}-${item.itemValue}`;
                const isChecked = currentSelectedItemsOrder.some(selected => selected.value === item.itemValue);
                option.innerHTML = `
                    <input type="checkbox" value="${item.itemValue}" id="${checkboxId}" ${isChecked ? 'checked' : ''}>
                    <label for="${checkboxId}">${item.itemName}</label>
                `;
            } else {
                option.innerHTML = `<label>${item.itemName}</label>`;
                option.addEventListener('click', () => {
                    hiddenInput.value = item.itemValue;
                    selectButton.querySelector('span').textContent = item.itemName;
                    dropdown.classList.remove('show');
                    hiddenInput.dispatchEvent(new Event('change'));
                    hiddenInput.dispatchEvent(new Event('input'));
                });
            }
            dropdown.appendChild(option);
        });

        dropdown.scrollTop = scrollTop;

        if (wasFocused) {
            searchInput.focus();
        }

        // reattach multi-select change 
        if (isMultiSelect) {
            dropdown.querySelectorAll('input[type="checkbox"]').forEach(checkbox => {
                checkbox.addEventListener('change', (event) => {
                    const changedCheckbox = event.target;
                    const itemValue = changedCheckbox.value;
                    const itemText = dropdown.querySelector(`label[for="${changedCheckbox.id}"]`).textContent;

                    if (changedCheckbox.checked) {
                        if (!currentSelectedItemsOrder.some(selected => selected.value === itemValue)) {
                            currentSelectedItemsOrder.push({ value: itemValue, text: itemText });
                            addSelectedTagToDisplay(itemText, itemValue);
                        }
                    } else {
                        currentSelectedItemsOrder = currentSelectedItemsOrder.filter(selected => selected.value !== itemValue);
                        const existingTag = selectedContainer.querySelector(`.selected-tag[data-value="${itemValue}"]`);
                        if (existingTag) {
                            existingTag.remove();
                        }
                    }
                    updateHiddenInputFromOrder();
                });
            });
        }
    }

    searchInput.addEventListener('input', (e) => {
        e.stopPropagation(); 
        renderDropdownItems(searchInput.value);
    });

    searchInput.addEventListener('keydown', (e) => {
        e.stopPropagation();
    });

    try {
        const response = await fetch(fetchUrl, { method: 'GET' });
        if (response.ok) {
            const result = await response.json();
            allItems = result.message.map(item => {
                const parts = item.split(',');
                return { itemName: parts[0].trim(), itemValue: parts[1].trim() };
            });
        } else if (fallbackItems) {
            toastr.warn(`Failed to fetch ${itemType}s, using fallback options.`);
            allItems = fallbackItems.map(item => {
                const parts = item.split(',');
                return { itemName: parts[0].trim(), itemValue: parts[1].trim() };
            });
        } else {
            toastr.error(`Server error fetching ${itemType}s and no fallback provided.`, 'Error');
            return;
        }

        renderDropdownItems(); 

        // initial selection display
        if (!isMultiSelect && initialSelectedValue) {
            const selectedItem = allItems.find(item => item.itemValue === initialSelectedValue);
            if (selectedItem) {
                selectButton.querySelector('span').textContent = selectedItem.itemName;
            } else {
                selectButton.querySelector('span').textContent = placeholderText;
            }
        } else if (isMultiSelect && hiddenInput.value) {
            const preselectedValues = hiddenInput.value.split(',').map(v => v.trim()).filter(v => v !== '');
            preselectedValues.forEach(value => {
                const item = allItems.find(i => i.itemValue === value);
                if (item) {
                    currentSelectedItemsOrder.push({ value: item.itemValue, text: item.itemName });
                    addSelectedTagToDisplay(item.itemName, item.itemValue);
                }
            });
            updateHiddenInputFromOrder();
        }

    } catch (error) {
        console.error(`Error initializing searchable dropdown for ${itemType}:`, error);
        toastr.error(`An unexpected error occurred while initializing ${itemType} dropdown: ${error.message}. Please try again.`, 'Error');
        selectButton.querySelector('span').textContent = placeholderText;

        if (fallbackItems) {
            allItems = fallbackItems.map(item => {
                const parts = item.split(',');
                return { itemName: parts[0].trim(), itemValue: parts[1].trim() };
            });
            renderDropdownItems();
        }
    }

    // Multi-select
    function addSelectedTagToDisplay(itemText, itemValue) {
        const tag = document.createElement('div');
        tag.className = 'selected-tag';
        tag.setAttribute('data-value', itemValue);
        tag.innerHTML = `
            ${itemText}
            <span class="remove-tag" data-value="${itemValue}">×</span>
        `;
        selectedContainer.appendChild(tag);

        tag.querySelector('.remove-tag').addEventListener('click', () => {
            tag.remove();
            const checkbox = dropdown.querySelector(`input[type="checkbox"][value="${itemValue}"]`);
            if (checkbox) checkbox.checked = false;
            currentSelectedItemsOrder = currentSelectedItemsOrder.filter(item => item.value !== itemValue);
            updateHiddenInputFromOrder();
        });
    }

    function updateHiddenInputFromOrder() {
        hiddenInput.value = currentSelectedItemsOrder.map(item => item.value).join(',');
        hiddenInput.dispatchEvent(new Event('change'));
        hiddenInput.dispatchEvent(new Event('input'));
    }
}