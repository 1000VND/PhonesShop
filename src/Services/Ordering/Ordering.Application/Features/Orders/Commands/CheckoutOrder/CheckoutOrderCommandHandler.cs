using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
	public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IMapper _mapper;
		private readonly IEmailService _emailService;
		private readonly ILogger<CheckoutOrderCommandHandler> _logger;

		public CheckoutOrderCommandHandler
			(
			IOrderRepository orderRepository,
			IMapper mapper,
			IEmailService emailService,
			ILogger<CheckoutOrderCommandHandler> logger
			)
		{
			_orderRepository = orderRepository;
			_mapper = mapper;
			_emailService = emailService;
			_logger = logger;
		}
		public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
		{
			var orderEntity = _mapper.Map<Order>(request);
			var newOrder = await _orderRepository.AddAsync(orderEntity);

			_logger.LogInformation($"Order {newOrder.Id} is successfully created.");

			await SendEmail(newOrder);

			return newOrder.Id;
		}

		private async Task SendEmail(Order newOrder)
		{
			var email = new Email()
			{
				To = "lqhung57@gmail.com",
				Body = GetBody(),
				Subject = "Order was created."
			};

			try
			{
				await _emailService.SendEmail(email);
			}
			catch (System.Exception ex)
			{
				_logger.LogError($"Order {newOrder.Id} failed due to an error with the mail service: {ex.Message}");
			}
		}

		private string GetBody()
		{
			var result = "<!DOCTYPE html>" +
				"<html lang=\"en\">" +
				"<head>" +
					"<meta charset=\"UTF-8\">" +
					"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
					"<style>" +
						"body {" +
							"font-family: Arial, sans-serif;" +
							"background-color: #f4f4f4;" +
							"margin: 0;padding: 0;" +
						"}" +
						".container {" +
							"width: 80%;" +
							"margin: 20px auto;" +
							"background-color: #ffffff;" +
							"padding: 20px;" +
							"box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);" +
						"}" +
						".header {" +
							"text-align: center;" +
						"}" +
						".success-message {" +
							"color: #28a745;font-size: 20px;" +
							"font-weight: bold;" +
						"}" +
						".order-details {" +
							"margin-top: 20px;" +
						"}" +
						".product {" +
							"border-bottom: 1px solid #ddd;" +
							"padding: 10px 0;" +
							"display: flex;" +
							"align-items: center;" +
						"}" +
						".product img {" +
							"max-width: 100px;" +
							"margin-right: 20px;" +
						"}" +
						".product-info {" +
							"flex-grow: 1;" +
						"}" +
						".total {" +
							"margin-top: 20px;" +
							"text-align: right;" +
							"font-size: 18px;" +
							"font-weight: bold;" +
						"}" +
					"</style>" +
				"</head>" +
				"<body>" +
					"<div class=\"container\">" +
						"<div class=\"header\">" +
							"<h1>Thông báo đặt hàng thành công!</h1>" +
							"<p class=\"success-message\">Cảm ơn bạn đã đặt hàng tại cửa hàng chúng tôi.</p>" +
							"</div>" +
						"<div class=\"order-details\">" +
							"<div class=\"product\">" +
								"<img src=\"url_to_your_product_image.jpg\" alt=\"Product Image\">" +
								"<div class=\"product-info\">" +
									"<h3>Tên Sản Phẩm</h3>" +
									"<p>Số lượng: 1</p>" +
									"<p>Giá: $99.99</p>" +
								"</div>" +
							"</div>" +
							"<div class=\"total\">" +
							"<p>Tổng cộng: $99.99</p>" +
							"</div>" +
						"</div>" +
					"</div>" +
				"</body>" +
				"</html>";
			return result;
		}
	}
}
