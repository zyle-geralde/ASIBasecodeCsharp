/**
 * Modal helper functions for confirmation and success modals
 */

/**
 * Shows a success modal and optionally reloads the page after dismissal
 * @param {string} modalId - ID of the success modal
 * @param {string} title - Title to display in the modal
 * @param {string} message - Message to display in the modal
 * @param {boolean} reloadOnClose - Whether to reload the page after closing the modal
 * @param {string} redirectUrl - URL to redirect to after closing (overrides reload)
 */
function showSuccessModal(modalId, title, message, reloadOnClose = false, redirectUrl = null) {
    // Set modal content if provided
    if (title) {
        $(`#${modalId}Title`).text(title);
    }
    
    if (message) {
        $(`#${modalId}Message`).text(message);
    }

    // Close any open modals first
    closeAllModals();
    
    // Show the success modal
    setTimeout(function() {
        const modal = new bootstrap.Modal(document.getElementById(modalId));
        modal.show();

        // Setup action on modal close
        $(`#${modalId}OkBtn`).off('click').on('click', function() {
            handleModalClose(reloadOnClose, redirectUrl);
        });

        $(`#${modalId}`).off('hidden.bs.modal').on('hidden.bs.modal', function() {
            handleModalClose(reloadOnClose, redirectUrl);
        });
    }, 100);
}

/**
 * Shows a confirmation modal
 * @param {string} modalId - ID of the confirmation modal
 * @param {string} title - Title to display in the modal (optional)
 * @param {string} message - Message to display in the modal (optional)
 * @param {function} onConfirm - Callback function to execute when confirmed
 */
function showConfirmationModal(modalId, title, message, onConfirm) {
    // Set modal content if provided
    if (title) {
        $(`#${modalId}Label`).text(title);
    }
    
    if (message) {
        $(`#${modalId}Text`).text(message);
    }

    // Show the confirmation modal
    const modal = new bootstrap.Modal(document.getElementById(modalId));
    modal.show();

    // Set up the confirm button click handler
    const confirmBtnId = $(`#${modalId}`).find('button').filter(function() {
        return !$(this).hasClass('btn-secondary') && !$(this).hasClass('btn-close');
    }).attr('id');

    $(`#${confirmBtnId}`).off('click').on('click', function() {
        if (typeof onConfirm === 'function') {
            onConfirm();
        }
    });
}

/**
 * Closes all open modals
 */
function closeAllModals() {
    $('.modal.show').each(function() {
        const modalInstance = bootstrap.Modal.getInstance(this);
        if (modalInstance) {
            modalInstance.hide();
        }
    });
}

/**
 * Handles what happens when a modal is closed
 */
function handleModalClose(reload, redirectUrl) {
    if (redirectUrl) {
        window.location.href = redirectUrl;
    } else if (reload) {
        location.reload();
    }
}

/**
 * Check and show success message from TempData if available
 * This should only be called explicitly when needed
 */
function checkAndShowTempDataSuccess() {
    const successMessage = document.getElementById('tempDataSuccessMessage');
    if (successMessage && successMessage.value) {
        showSuccessModal('successModal', 'Success!', successMessage.value, false);
        // Clear the value to prevent showing again
        successMessage.value = '';
    }
}

// Don't automatically check for success messages on document ready
// Only call checkAndShowTempDataSuccess() when explicitly needed