﻿using BookTrekker.DataAccess.Data;
using BookTrekker.DataAccess.Repository.IRepository;
using BookTrekker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookTrekker.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) :base(db) 
        {
                _db = db;
        }

       

        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
