using System;

namespace ASI.Basecode.WebApp.Models
{
    public class ConfirmationModalModel
    {
        public string Id { get; set; } = "confirmationModal";
        public string Title { get; set; } = "Confirmation";
        public string Message { get; set; } = "Are you sure you want to proceed?";
        public string CancelButtonText { get; set; } = "Cancel";
        public string ConfirmButtonText { get; set; } = "Confirm";
        public string ConfirmButtonId { get; set; } = "confirmBtn";
        public string ConfirmButtonClass { get; set; } = "btn-danger";
    }

    public class SuccessModalModel
    {
        public string Id { get; set; } = "successModal";
        public string Title { get; set; } = "Success!";
        public string Message { get; set; } = "Operation completed successfully!";
        public string OkButtonText { get; set; } = "OK";
    }
}