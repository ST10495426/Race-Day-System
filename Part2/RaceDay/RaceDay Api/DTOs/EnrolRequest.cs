using System.ComponentModel.DataAnnotations;

namespace RaceDay_Api.DTOs
{
    public class EnrolRequest
    {
        [Required]
        public int CategoryID { get; set; }
    }
}