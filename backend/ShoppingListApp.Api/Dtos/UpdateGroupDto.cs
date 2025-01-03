﻿using System.ComponentModel.DataAnnotations;

public class UpdateGroupDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }
}
