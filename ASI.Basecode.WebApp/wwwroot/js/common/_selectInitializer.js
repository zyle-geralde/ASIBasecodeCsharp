
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

    // Initial validation for essential DOM elements
    if (!selectButton || !dropdown || !selectedContainer || !hiddenInput) {
        console.error(`Missing elements for multi-select initialization: ${containerId}`);
        return;
    }

    selectButton.querySelector('span').textContent = placeholderText;

    // --- Dropdown Toggle and Outside Click Handling ---
    selectButton.addEventListener('click', (event) => {
        event.stopPropagation(); // Prevent immediate closing by document click
        dropdown.classList.toggle('show');
    });

    document.addEventListener('click', (event) => { // close if clicked outside
        if (!selectButton.contains(event.target) && !dropdown.contains(event.target)) {
            dropdown.classList.remove('show');
        }
    });

    // in order selection of genre, first is the main
    // This array stores { value, text } pairs in the order they should be displayed.
    let currentSelectedItemsOrder = [];

    // Functions for Managing Displayed Tags (UI) 

    // creates and appends a selected tag to the display area.
    function addSelectedTagToDisplay(itemText, itemValue) {
        const tag = document.createElement('div');
        tag.className = 'selected-tag';
        tag.setAttribute('data-value', itemValue); // Store value for easy lookup/removal
        tag.innerHTML = `
            ${itemText}
            <span class="remove-tag" data-value="${itemValue}">×</span>
        `;
        selectedContainer.appendChild(tag); // Append to the container

        // removing a tag via its 'x' button
        tag.querySelector('.remove-tag').addEventListener('click', () => {
            tag.remove(); // Remove tag from UI

            // Find and uncheck the corresponding checkbox in the dropdown
            const checkbox = dropdown.querySelector(`input[value="${itemValue}"]`);
            if (checkbox) checkbox.checked = false;

            // Remove item 
            currentSelectedItemsOrder = currentSelectedItemsOrder.filter(item => item.value !== itemValue);

            updateHiddenInputFromOrder();
        });
    }

    // updates the hidden input field based on the current order in `currentSelectedItemsOrder`.
    function updateHiddenInputFromOrder() {
        hiddenInput.value = currentSelectedItemsOrder.map(item => item.value).join(',');
    }

    // cehckboxes for dropdown , add/remove

    dropdown.addEventListener('change', (event) => {
        if (event.target.type === 'checkbox') {
            const changedCheckbox = event.target;
            const itemValue = changedCheckbox.value;
            const itemText = dropdown.querySelector(`label[for="${changedCheckbox.id}"]`).textContent;

            if (changedCheckbox.checked) {
                // Add item to internal order array if not already present
                if (!currentSelectedItemsOrder.some(item => item.value === itemValue)) {
                    currentSelectedItemsOrder.push({ value: itemValue, text: itemText });
                    addSelectedTagToDisplay(itemText, itemValue); // Add to UI display
                }
            } else {
                // Remove item from internal order array
                currentSelectedItemsOrder = currentSelectedItemsOrder.filter(item => item.value !== itemValue);
                // Remove item from UI display
                const existingTag = selectedContainer.querySelector(`.selected-tag[data-value="${itemValue}"]`);
                if (existingTag) {
                    existingTag.remove();
                }
            }
            updateHiddenInputFromOrder(); //for synchronized hidden input with display order
        }
    });

    // --- Initial Data Fetching and UI Population on Load ---

    try {
        const response = await fetch(fetchUrl, { method: 'GET' });
        let itemsToPopulate = [];

        if (response.ok) {
            const result = await response.json();
            itemsToPopulate = result.message;
        } else if (fallbackItems) {
            console.warn(`Failed to fetch from ${fetchUrl}, using fallback items.`);
            itemsToPopulate = fallbackItems;
            toastr.warn("Failed to fetch data, using fallback options."); // Direct toastr call
        } else {
            toastr.error(`Server error fetching data from ${fetchUrl} and no fallback provided.`, 'Error'); // Direct toastr call
            return;
        }

        dropdown.innerHTML = ''; // Clear previous options in dropdown before populating

        // Store all available items in a map for efficient lookup (value -> text)
        const allItemsLookup = new Map();
        itemsToPopulate.forEach(item => {
            const parts = item.split(',');
            const itemName = parts[0].trim();
            const itemValue = parts[1].trim();
            allItemsLookup.set(itemValue, itemName);

            // Create and append dropdown option
            const optionDiv = document.createElement('div');
            optionDiv.className = 'dropdown-item';
            optionDiv.innerHTML = `
                <input type="checkbox" value="${itemValue}" id="${dropdownId}-${itemValue}">
                <label for="${dropdownId}-${itemValue}">${itemName}</label>
            `;
            dropdown.appendChild(optionDiv);
        });

        // --- Handle Initial Pre-selection on Page Load (Edit Scenario) ---
        if (hiddenInput.value) {
            // Get preselected values from the hidden input, filter out empty strings
            const preselectedValues = hiddenInput.value.split(',').map(v => v.trim()).filter(v => v !== '');

            // Populate the internal `currentSelectedItemsOrder` array and check corresponding checkboxes
            preselectedValues.forEach(value => {
                const text = allItemsLookup.get(value);
                if (text) {
                    currentSelectedItemsOrder.push({ value: value, text: text }); // Add to order
                    const checkbox = dropdown.querySelector(`input[value="${value}"]`);
                    if (checkbox) {
                        checkbox.checked = true; // Check the checkbox
                    }
                }
            });

            // Add initial tags to the display in the order they appeared in the hidden input
            currentSelectedItemsOrder.forEach(item => {
                addSelectedTagToDisplay(item.text, item.value);
            });

            // Ensure the hidden input is correctly synchronized
            updateHiddenInputFromOrder();
        }

    } catch (error) {
        console.error(`Error initializing dropdown for ${containerId}:`, error);
        toastr.error(`An unexpected error occurred while initializing dropdown for ${containerId}: ${error.message}`, 'Error'); // Direct toastr call

        // Fallback if fetching data fails
        if (fallbackItems) {
            dropdown.innerHTML = ''; // Clear existing dropdown content
            const allItemsLookupFallback = new Map(); // Create lookup for fallback items
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

            // Re-check checkboxes and re-render tags based on fallback and preselected values
            if (hiddenInput.value) {
                const preselectedValues = hiddenInput.value.split(',').map(v => v.trim()).filter(v => v !== '');
                currentSelectedItemsOrder = []; // Reset internal order for fallback re-initialization
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
                // Clear and re-add tags based on the restored order
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