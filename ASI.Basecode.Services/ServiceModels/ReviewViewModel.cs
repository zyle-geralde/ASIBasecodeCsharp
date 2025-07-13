using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ReviewViewModel
    {
        public string ReviewId { get; set; } //will be set automatically

        public string BookId { get; set; } //depends on where user mapped it
        public string BookTitle { get; set; } //depends on where user mapped it

        [Required(ErrorMessage = "Rating is required.")]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public float Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; }

        public int Likes { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        public DateTime? UploadDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}