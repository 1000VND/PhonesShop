﻿using AspnetRunBasics.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspnetRunBasics.Services.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName);
    }

}