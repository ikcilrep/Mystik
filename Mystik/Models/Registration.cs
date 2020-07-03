﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Mystik.Models
{
    public class Registration
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
