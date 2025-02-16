﻿using System.ComponentModel.DataAnnotations;

namespace EnergyManagementSystemUser.Models
{
    public class UserDto
    {
        public string Name { get; set; }

        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }
    }
}
