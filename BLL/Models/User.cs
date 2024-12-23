﻿namespace BLL.Models
{
    public class User
    {
        public int id { get; set; }

        public string username { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public int wins { get; set; } = 0;

        public int rating { get; set; } = 0;

        public int gamesPlayed { get; set; } = 0;
    }
}
