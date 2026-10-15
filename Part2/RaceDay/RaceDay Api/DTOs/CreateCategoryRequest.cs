using System.ComponentModel.DataAnnotations;

namespace RaceDay_Api.DTOs
{
    public class CreateCategoryRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string CategoryType { get; set; } = string.Empty;

        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
    }
}