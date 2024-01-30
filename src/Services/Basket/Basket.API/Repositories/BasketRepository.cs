using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
	public class BasketRepository : IBasketRepository
	{
		private readonly IDistributedCache _redisCache;

		public BasketRepository(IDistributedCache redisCache)
		{
			_redisCache = redisCache;
		}

		public async Task DeleteBasket(string userName)
		{
			await _redisCache.RemoveAsync(userName);
		}

		public async Task<ShoppingCart> GetBasket(string userName)
		{
			var basket = await _redisCache.GetStringAsync(userName);
			if (String.IsNullOrEmpty(basket))
				return null;

			// chuyển đổi từ chuỗi Json sang đối tượng ShoppingCart
			return JsonConvert.DeserializeObject<ShoppingCart>(basket);
		}

		public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
		{
			// chuyển đổi từ đối tượng ShoppingCart sang chuỗi Json
			await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
			return await GetBasket(basket.UserName);
		}
	}
}
