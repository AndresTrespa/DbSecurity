﻿namespace Entity.Model
{
    public class Persona
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int UserId { get; set; }
        public User user { get; set; }
    }
}
