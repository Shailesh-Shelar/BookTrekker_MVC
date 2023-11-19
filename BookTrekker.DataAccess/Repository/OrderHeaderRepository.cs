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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) :base(db) 
        {
                _db = db;
        }

       

        public void Update(OrderHeader obj)
        {
            _db.OrderHeaders.Update(obj);
        }
		public void UpdateStatus(int id, string orderstatus, string? paymentStatus)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u=>u.Id == id);
            if(orderFromDb != null) { 
                orderFromDb.OrderStatus = orderstatus;
                if(!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
		}

		public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if(!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;

            }
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
                
			}
		}
	}
}
