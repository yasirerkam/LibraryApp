﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace LibraryApp.Data.Library
{
    public partial class User
    {
        public User()
        {
            UserBooks = new HashSet<UserBook>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<UserBook> UserBooks { get; set; }
    }
}