
using System.ComponentModel.DataAnnotations;
using RaceDay_Api.Models; 

namespace RaceDay_Api.Models
{
    public class EventType
    {
        // A simple lookup table that stores Run, Walk, Cycle as the three types

        [Key] //this is the primary key of the table
        public int EventTypeID { get; set; }

        [Required] //this field cannot be empty
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // "Run", "Walk", "Cycle"

        [MaxLength(255)]
        public string? Description { get; set; } // ? after string means the field can be null

    }
}