﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Entitati
{
    public class WebApplicationContext : DbContext
    {



        public WebApplicationContext(DbContextOptions options) : base(options)
        {

        }

     public DbSet<Users> Users { get; set; }
 
     public DbSet<Post> Posts { get; set; }
    }
}
