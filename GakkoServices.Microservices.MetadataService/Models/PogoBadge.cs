using System;
using System.ComponentModel.DataAnnotations;

public class PogoBadge 
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string ImageUrl { get; set; }
}