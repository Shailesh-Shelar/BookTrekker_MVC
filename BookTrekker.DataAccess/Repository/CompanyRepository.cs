using BookTrekker.DataAccess.Data;
using BookTrekker.DataAccess.Repository.IRepository;
using BookTrekker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookTrekker.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) :base(db) 
        {
                _db = db;
        }

       

        public void Update(Company obj)
        {
            _db.Companies.Update(obj);
        }
    }
}
