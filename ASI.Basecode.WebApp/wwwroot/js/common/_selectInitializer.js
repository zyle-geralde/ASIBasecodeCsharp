
export async function initializeSingleSelectAuthor() {
    const authorContainer = document.getElementById('authorContainer');
    const authorDropdown = document.getElementById('authorDropdown');
    const selectButton = authorContainer ? authorContainer.querySelector('.custom-select-btn') : null;
    const hiddenInput = document.getElementById('author');
    const hiddenAuthorValue = document.getElementById('hiddenAuthor') ? document.getElementById('hiddenAuthor').value : '';

    if (!selectButton || !authorDropdown || !hiddenInput || !authorContainer) {
        console.error("Missing elements for author single-select initialization.");
        return;
    }

    selectButton.addEventListener('click', (event) => {
        event.stopPropagation();
        authorDropdown.classList.toggle('show');
    });

    document.addEventListener('click', (event) => {
        if (!authorContainer.contains(event.target)) {
            authorDropdown.classList.remove('show');
        }
    });

    function selectAuthor(value, text) {
        hiddenInput.value = value;
        selectButton.querySelector('span').textContent = text;
        authorDropdown.classList.remove('show');
    }

    try {
        const response = await fetch('/Book/GetAuthor', { method: 'GET' });

        if (response.ok) {
            const result = await response.json();
            const author_list = result.message;

           
            authorDropdown.innerHTML = '';

            const defaultOption = document.createElement('div');
            defaultOption.className = 'dropdown-item';
            defaultOption.innerHTML = `<label>Select Author</label>`;
            defaultOption.addEventListener('click', () => {
                selectAuthor('', 'Select Author');
            });
            authorDropdown.appendChild(defaultOption);

            author_list.forEach(author_name_id => {
                const parts = author_name_id.split(',');
                const authorName = parts[0].trim();
                const authorId = parts[1].trim();

                const option = document.createElement('div');
                option.className = 'dropdown-item';
                option.innerHTML = `<label>${authorName}</label>`;
                option.addEventListener('click', () => {
                    selectAuthor(authorId, authorName);
                });
                authorDropdown.appendChild(option);

                if (hiddenAuthorValue && hiddenAuthorValue.trim() === authorId) {
                    selectButton.querySelector('span').textContent = authorName;
                }
            });
        } else {
            toastr.error("Server error fetching authors.");
        }
    } catch (backendError) {
        toastr.error("Unexpected Error Occured while fetching authors.");
    }
}

export async function initializeSingleSelectLanguage() {
    const languageContainer = document.getElementById('languageContainer');
    const languageDropdown = document.getElementById('languageDropdown');
    const languageSelectButton = languageContainer ? languageContainer.querySelector('.custom-select-btn') : null;
    const languageHiddenInput = document.getElementById('language');
    const hiddenLanguageValue = document.getElementById('hiddenLanguage') ? document.getElementById('hiddenLanguage').value : '';

    if (!languageContainer || !languageDropdown || !languageSelectButton || !languageHiddenInput) {
        console.error("Missing elements for language single-select initialization.");
        return;
    }

    languageSelectButton.addEventListener('click', (event) => {
        event.stopPropagation();
        languageDropdown.classList.toggle('show');
    });

    document.addEventListener('click', (event) => {
        if (!languageContainer.contains(event.target)) {
            languageDropdown.classList.remove('show');
        }
    });

    function selectLanguage(value, text) {
        languageHiddenInput.value = value;
        languageSelectButton.querySelector('span').textContent = text;
        languageDropdown.classList.remove('show');
    }

    try {
        const response = await fetch('/Book/GetLanguage', { method: 'GET' });
        let language_list = [];

        if (response.ok) {
            const result = await response.json();
            language_list = result.message;
        } else {
            language_list = [
                'English,English', 'Tagalog,Tagalog', 'Bisaya,Bisaya',
                'Spanish,Spanish', 'French,French', 'German,German', 'Japanese,Japanese'
            ];
            toastr.warn("Failed to fetch languages, using fallback options.");
        }

       
        languageDropdown.innerHTML = '';

        const defaultOption = document.createElement('div');
        defaultOption.className = 'dropdown-item';
        defaultOption.innerHTML = `<label>Select Language</label>`;
        defaultOption.addEventListener('click', () => {
            selectLanguage('', 'Select Language');
        });
        languageDropdown.appendChild(defaultOption);

        language_list.forEach(language_name_id => {
            const parts = language_name_id.split(',');
            const languageName = parts[0].trim();
            const languageId = parts[1].trim();

            const option = document.createElement('div');
            option.className = 'dropdown-item';
            option.innerHTML = `<label>${languageName}</label>`;
            option.addEventListener('click', () => {
                selectLanguage(languageId, languageName);
            });
            languageDropdown.appendChild(option);

            if (hiddenLanguageValue && hiddenLanguageValue.trim() === languageId) {
                languageSelectButton.querySelector('span').textContent = languageName;
            }
        });
    } catch (backendError) {
        toastr.error("Unexpected Error Occurred while fetching languages.");
        const fallbackLanguageList = [
            'English,English', 'Tagalog,Tagalog', 'Bisaya,Bisaya',
            'Spanish,Spanish', 'French,French', 'German,German', 'Japanese,Japanese'
        ];
       
        languageDropdown.innerHTML = '';
        fallbackLanguageList.forEach(language_name_id => {
            const parts = language_name_id.split(',');
            const languageName = parts[0].trim();
            const languageId = parts[1].trim();
            const option = document.createElement('div');
            option.className = 'dropdown-item';
            option.innerHTML = `<label>${languageName}</label>`;
            option.addEventListener('click', () => {
                selectLanguage(languageId, languageName);
            });
            languageDropdown.appendChild(option);
            if (hiddenLanguageValue && hiddenLanguageValue.trim() === languageId) {
                languageSelectButton.querySelector('span').textContent = languageName;
            }
        });
    }
}

export async function initializeMultiSelectWithPreselected({
    containerId,
    dropdownId,
    hiddenInputId,
    selectedContainerId,
    fetchUrl,
    placeholderText = 'Select Items',
    fallbackItems = null,
    itemType
}) {
    const selectButton = document.querySelector(`#${containerId} .custom-select-btn`);
    const dropdown = document.getElementById(dropdownId);
    const selectedContainer = document.getElementById(selectedContainerId);
    const hiddenInput = document.getElementById(hiddenInputId);

    if (!selectButton || !dropdown || !selectedContainer || !hiddenInput) {
        console.error(`Missing elements for multi-select initialization: ${containerId}`);
        return;
    }

    selectButton.querySelector('span').textContent = placeholderText;

    selectButton.addEventListener('click', (event) => {
        event.stopPropagation();
        dropdown.classList.toggle('show');
    });

    document.addEventListener('click', (event) => {
        if (!selectButton.contains(event.target) && !dropdown.contains(event.target)) {
            dropdown.classList.remove('show');
        }
    });

    function addSelectedTag(itemText, itemValue) {
        const tag = document.createElement('div');
        tag.className = `selected-tag ${itemType}-tag`;
        tag.innerHTML = `
                                    ${itemText}
                                    <span class="remove-tag" data-value="${itemValue}" data-type="${itemType}">×</span>
                                `;
        selectedContainer.appendChild(tag);

        tag.querySelector('.remove-tag').addEventListener('click', () => {
            tag.remove();
            const checkbox = dropdown.querySelector(`input[value="${itemValue}"]`);
            if (checkbox) checkbox.checked = false;
            updateHiddenInput();
        });
    }

    function updateHiddenInput() {
        const selectedCheckboxes = Array.from(dropdown.querySelectorAll('input[type="checkbox"]:checked'));
        const selectedValues = selectedCheckboxes.map(checkbox => checkbox.value);
        hiddenInput.value = selectedValues.join(',');
    }

    dropdown.addEventListener('change', (event) => {
        if (event.target.type === 'checkbox') {
            selectedContainer.innerHTML = '';
            const selectedCheckboxes = Array.from(dropdown.querySelectorAll('input[type="checkbox"]:checked'));
            selectedCheckboxes.forEach(checkbox => {
                const label = dropdown.querySelector(`label[for="${checkbox.id}"]`).textContent;
                addSelectedTag(label, checkbox.value);
            });
            updateHiddenInput();
        }
    });

    try {
        const response = await fetch(fetchUrl, { method: 'GET' });
        let itemsToPopulate = [];

        if (response.ok) {
            const result = await response.json();
            itemsToPopulate = result.message;
        } else if (fallbackItems) {
            console.warn(`Failed to fetch from ${fetchUrl}, using fallback items.`);
            itemsToPopulate = fallbackItems;
        } else {
            toastr.error(`Server error fetching data from ${fetchUrl} and no fallback provided.`, 'Error');
            return;
        }

        dropdown.innerHTML = '';

        itemsToPopulate.forEach(item => {
            const parts = item.split(',');
            const itemName = parts[0].trim();
            const itemValue = parts[1].trim();
            const optionDiv = document.createElement('div');
            optionDiv.className = 'dropdown-item';
            optionDiv.innerHTML = `
                                            <input type="checkbox" value="${itemValue}" id="${dropdownId}-${itemValue}">
                                            <label for="${dropdownId}-${itemValue}">${itemName}</label>
                                        `;
            dropdown.appendChild(optionDiv);
        });

        if (hiddenInput.value) {
            const preselectedValues = hiddenInput.value.split(',').map(v => v.trim());
            Array.from(dropdown.querySelectorAll('input[type="checkbox"]')).forEach(checkbox => {
                if (preselectedValues.includes(checkbox.value)) {
                    checkbox.checked = true;
                    const label = dropdown.querySelector(`label[for="${checkbox.id}"]`).textContent;
                    addSelectedTag(label, checkbox.value);
                }
            });
        }
    } catch (error) {
        console.error(`Error initializing dropdown for ${containerId}:`, error);
        if (fallbackItems) {
           
            dropdown.innerHTML = '';
            fallbackItems.forEach(item => {
                const parts = item.split(',');
                const itemName = parts[0].trim();
                const itemValue = parts[1].trim();
                const optionDiv = document.createElement('div');
                optionDiv.className = 'dropdown-item';
                optionDiv.innerHTML = `
                                                    <input type="checkbox" value="${itemValue}" id="${dropdownId}-${itemValue}">
                                                    <label for="${dropdownId}-${itemValue}">${itemName}</label>
                                                `;
                dropdown.appendChild(optionDiv);
            });
            if (hiddenInput.value) {
                const preselectedValues = hiddenInput.value.split(',').map(v => v.trim());
                Array.from(dropdown.querySelectorAll('input[type="checkbox"]')).forEach(checkbox => {
                    if (preselectedValues.includes(checkbox.value)) {
                        checkbox.checked = true;
                        const label = dropdown.querySelector(`label[for="${checkbox.id}"]`).textContent;
                        addSelectedTag(label, checkbox.value);
                    }
                });
            }
        } else {
            toastr.error(`Failed to load items for ${placeholderText}. Please try again.`, 'Error');
        }
    }
}