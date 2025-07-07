
export async function initializeSingleSelectAuthor() {
    const authorContainer = document.getElementById('authorContainer');
    const authorDropdown = document.getElementById('authorDropdown');
    const selectButton = authorContainer ? authorContainer.querySelector('.custom-select-btn') : null;
    const hiddenInput = document.getElementById('author');

    const initialSelectedValue = hiddenInput ? hiddenInput.value : '';

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

        hiddenInput.dispatchEvent(new Event('change'));
        hiddenInput.dispatchEvent(new Event('input')); 
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

            let foundInitialSelection = false; 
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

                if (initialSelectedValue && initialSelectedValue.trim() === authorId) {
                    selectButton.querySelector('span').textContent = authorName;
                    foundInitialSelection = true; 
                }
            });

            if (!foundInitialSelection) {
                selectButton.querySelector('span').textContent = 'Select Author';
            }

        } else {
            toastr.error("Server error fetching authors.");
            selectButton.querySelector('span').textContent = 'Select Author';
        }
    } catch (backendError) {
        toastr.error("Unexpected Error Occured while fetching authors.");
        
        selectButton.querySelector('span').textContent = 'Select Author';
    }
}

export async function initializeSingleSelectLanguage() {
    const languageContainer = document.getElementById('languageContainer');
    const languageDropdown = document.getElementById('languageDropdown');
    const languageSelectButton = languageContainer ? languageContainer.querySelector('.custom-select-btn') : null;
    const languageHiddenInput = document.getElementById('language');

    const initialSelectedValue = languageHiddenInput ? languageHiddenInput.value : '';

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
        // New: Trigger change event for validation systems
        languageHiddenInput.dispatchEvent(new Event('change'));
        languageHiddenInput.dispatchEvent(new Event('input'));
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
            selectLanguage('', 'Select Language'); // Allows "deselecting" by choosing the default
        });
        languageDropdown.appendChild(defaultOption);

        let foundInitialSelection = false; // New: Track if initial value was matched
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

            // Modified: Pre-select the display text if initialSelectedValue matches
            if (initialSelectedValue && initialSelectedValue.trim() === languageId) {
                languageSelectButton.querySelector('span').textContent = languageName;
                foundInitialSelection = true; // New: Mark as found
            }
        });

        // New: If no initial selection was found (e.g., new book or invalid old value), set to default placeholder
        if (!foundInitialSelection) {
            languageSelectButton.querySelector('span').textContent = 'Select Language';
        }

    } catch (backendError) {
        toastr.error("Unexpected Error Occurred while fetching languages.");
        // New: Ensure default text if error occurs
        languageSelectButton.querySelector('span').textContent = 'Select Language';

        // Fallback options logic (keep as is for robustness)
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
            // Modified: This part also needs to check initialSelectedValue, not hiddenLanguageValue
            if (initialSelectedValue && initialSelectedValue.trim() === languageId) {
                languageSelectButton.querySelector('span').textContent = languageName;
            }
        });
    }
}

export async function initializeMultiSelect({
    containerId,
    dropdownId,
    hiddenInputId,
    selectedContainerId,
    fetchUrl,
    placeholderText = 'Select Items',
    fallbackItems = null
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

    let currentSelectedItemsOrder = [];

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
            const checkbox = dropdown.querySelector(`input[value="${itemValue}"]`);
            if (checkbox) checkbox.checked = false;
            // New: Trigger change event for validation systems
            hiddenInput.dispatchEvent(new Event('change'));
            hiddenInput.dispatchEvent(new Event('input'));
            updateHiddenInputFromOrder();
        });
    }

    function updateHiddenInputFromOrder() {
        hiddenInput.value = currentSelectedItemsOrder.map(item => item.value).join(',');
        // New: Trigger change event for validation systems
        hiddenInput.dispatchEvent(new Event('change'));
        hiddenInput.dispatchEvent(new Event('input'));
    }

    dropdown.addEventListener('change', (event) => {
        if (event.target.type === 'checkbox') {
            const changedCheckbox = event.target;
            const itemValue = changedCheckbox.value;
            const itemText = dropdown.querySelector(`label[for="${changedCheckbox.id}"]`).textContent;

            if (changedCheckbox.checked) {
                if (!currentSelectedItemsOrder.some(item => item.value === itemValue)) {
                    currentSelectedItemsOrder.push({ value: itemValue, text: itemText });
                    addSelectedTagToDisplay(itemText, itemValue);
                }
            } else {
                currentSelectedItemsOrder = currentSelectedItemsOrder.filter(item => item.value !== itemValue);
                const existingTag = selectedContainer.querySelector(`.selected-tag[data-value="${itemValue}"]`);
                if (existingTag) {
                    existingTag.remove();
                }
            }
            updateHiddenInputFromOrder();
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
            toastr.warn("Failed to fetch data, using fallback options.");
            itemsToPopulate = fallbackItems;
        } else {
            toastr.error(`Server error fetching data from ${fetchUrl} and no fallback provided.`, 'Error');
            return;
        }

        dropdown.innerHTML = '';

        const allItemsLookup = new Map();
        itemsToPopulate.forEach(item => {
            const parts = item.split(',');
            const itemName = parts[0].trim();
            const itemValue = parts[1].trim();
            allItemsLookup.set(itemValue, itemName);

            const optionDiv = document.createElement('div');
            optionDiv.className = 'dropdown-item';
            optionDiv.innerHTML = `
                <input type="checkbox" value="${itemValue}" id="${dropdownId}-${itemValue}">
                <label for="${dropdownId}-${itemValue}">${itemName}</label>
            `;
            dropdown.appendChild(optionDiv);
        });

        if (hiddenInput.value) {
            const preselectedValues = hiddenInput.value.split(',').map(v => v.trim()).filter(v => v !== '');
            preselectedValues.forEach(value => {
                const text = allItemsLookup.get(value);
                if (text) {
                    currentSelectedItemsOrder.push({ value: value, text: text });
                    const checkbox = dropdown.querySelector(`input[value="${value}"]`);
                    if (checkbox) {
                        checkbox.checked = true;
                    }
                }
            });

            selectedContainer.innerHTML = ''; // Clear existing tags before re-adding
            currentSelectedItemsOrder.forEach(item => {
                addSelectedTagToDisplay(item.text, item.value);
            });
            updateHiddenInputFromOrder(); // Ensure hidden input reflects order
        }

    } catch (error) {
        console.error(`Error initializing dropdown for ${containerId}:`, error);
        toastr.error(`An unexpected error occurred while initializing dropdown for ${containerId}: ${error.message}`, 'Error');

        if (fallbackItems) {
            dropdown.innerHTML = '';
            const allItemsLookupFallback = new Map();
            fallbackItems.forEach(item => {
                const parts = item.split(',');
                const itemName = parts[0].trim();
                const itemValue = parts[1].trim();
                allItemsLookupFallback.set(itemValue, itemName);
                const optionDiv = document.createElement('div');
                optionDiv.className = 'dropdown-item';
                optionDiv.innerHTML = `
                    <input type="checkbox" value="${itemValue}" id="${dropdownId}-${itemValue}">
                    <label for="${dropdownId}-${itemValue}">${itemName}</label>
                `;
                dropdown.appendChild(optionDiv);
            });

            if (hiddenInput.value) {
                const preselectedValues = hiddenInput.value.split(',').map(v => v.trim()).filter(v => v !== '');
                currentSelectedItemsOrder = [];
                preselectedValues.forEach(value => {
                    const text = allItemsLookupFallback.get(value);
                    if (text) {
                        currentSelectedItemsOrder.push({ value: value, text: text });
                        const checkbox = dropdown.querySelector(`input[value="${value}"]`);
                        if (checkbox) {
                            checkbox.checked = true;
                        }
                    }
                });
                selectedContainer.innerHTML = '';
                currentSelectedItemsOrder.forEach(item => {
                    addSelectedTagToDisplay(item.text, item.value);
                });
                updateHiddenInputFromOrder();
            }
        } else {
            toastr.error(`Failed to load items for ${placeholderText}. Please try again.`, 'Error');
        }
    }
}