﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Common.Utility;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;

namespace WhiteLagoon.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly ApplicationDBContext _context;
        public BookingRepository(ApplicationDBContext context):base(context)
        {
            _context = context;
        }

        public void Update(Booking entity)
        {
            _context.Bookings.Update(entity);
        }

        public void UpdateStatus(int bookingId, string bookingStatus)
        {
            var bookingFromDb=_context.Bookings.FirstOrDefault(b=>b.ID==bookingId);
            if(bookingFromDb != null) {
                bookingFromDb.Status= bookingStatus;
                if(bookingStatus==SD.StatusCheckedIn)
                {
                    bookingFromDb.ActualCheckInDate= DateTime.Now;
                }
                if (bookingStatus == SD.StatusCompleted)
                {
                    bookingFromDb.ActualCheckOutDate = DateTime.Now;
                }
            }
        }

        public void UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId)
        {
            var bookingFromDb = _context.Bookings.FirstOrDefault(b => b.ID == bookingId);
            if (bookingFromDb != null)
            {
                if(!string.IsNullOrEmpty(sessionId))
                {
                    bookingFromDb.StripeSessionId= sessionId;
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    bookingFromDb.StripePaymentIntentId = paymentIntentId;
                    bookingFromDb.PaymentDate= DateTime.Now;
                    bookingFromDb.IsPaymentSuccessful = true;
                }
            }
        }
    }
}
