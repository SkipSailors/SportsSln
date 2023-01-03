﻿namespace SportsStore.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

public class LoginModel
{
    [Required] public string? Name { get; set; }
    [Required] public string? Password { get; set; }
    public string ReturnUrl { get; set; } = "/";
}