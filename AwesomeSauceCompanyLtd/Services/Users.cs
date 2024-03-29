﻿using AwesomeSauceCompanyLtd.Services.Entities;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeSauceCompanyLtd.Services
{
    public class Users : IUsers
    {
        private User[] _users;

        public Users(IWebHostEnvironment env)
        {
            _users = JsonConvert.DeserializeObject<User[]>(File.ReadAllText(env.ContentRootFileProvider.GetFileInfo("Data/employees.json").PhysicalPath));
        }

        public Task<User[]> All() => Task.FromResult(_users);

        public Task<User> WhereIdIs(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(x => x.Id == id));
        }

        public Task<User> WithName(string name)
        {
            return Task.FromResult(_users.FirstOrDefault(x => (x.FirstName + " " + x.LastName).Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
