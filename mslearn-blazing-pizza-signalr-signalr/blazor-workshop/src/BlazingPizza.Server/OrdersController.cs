namespace BlazingPizza.Server;

[Route("orders")]
[ApiController]
[Authorize]
public class OrdersController : Controller
{
    private readonly PizzaStoreContext _db;
    private readonly IBackgroundOrderQueue _orderQueue;

    public OrdersController(
        PizzaStoreContext db, IBackgroundOrderQueue orderQueue) =>
        (_db, _orderQueue) = (db, orderQueue);

    [HttpGet]
    public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
    {
        var orders = await _db.Orders
            .Where(o => o.UserId == GetUserId())
            .Include(o => o.DeliveryLocation)
            .Include(o => o.Pizzas).ThenInclude(p => p.Special)
            .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
            .OrderByDescending(o => o.CreatedTime)
            .ToListAsync();

        return orders.Select(OrderWithStatus.FromOrder).ToList();
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(int orderId)
    {
        var order = await _db.Orders
            .Where(o => o.OrderId == orderId)
            .Where(o => o.UserId == GetUserId())
            .Include(o => o.DeliveryLocation)
            .Include(o => o.Pizzas).ThenInclude(p => p.Special)
            .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
            .SingleOrDefaultAsync();

        return order is null ? NotFound() : OrderWithStatus.FromOrder(order);
    }

    /// <summary>
    /// 下单并将订单状态通知加入队列。
    /// </summary>
    /// <param name="order">要下的订单。</param>
    /// <returns>下单的订单ID。</returns>
    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrder(Order order)
    {
        order.CreatedTime = DateTime.Now;
        order.DeliveryLocation = new LatLong(51.5001, -0.1239);
        order.UserId = GetUserId();

        // 确保 Pizza.SpecialId 和 Topping.ToppingId 在数据库中存在 - 防止提交者伪造新的 specials 和 toppings
        foreach (var pizza in order.Pizzas)
        {
            pizza.SpecialId = pizza.Special.Id;
            pizza.Special = null;

            foreach (var topping in pizza.Toppings)
            {
                topping.ToppingId = topping.Topping.Id;
                topping.Topping = null;
            }
        }

        _db.Orders.Attach(order);
        await _db.SaveChangesAsync();

        // 在后台，如果可能，发送推送通知
        var subscription = await _db.NotificationSubscriptions.Where(e => e.UserId == GetUserId()).SingleOrDefaultAsync();
        if (subscription is not null)
        {
            await QueueNotificationsAsync(order, subscription);
        }

        return order.OrderId;
    }

    /// <summary>
    /// 从当前 HTTP 上下文中获取用户ID。
    /// </summary>
    /// <returns>用户ID。</returns>
    private string GetUserId() =>
        HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

    /// <summary>
    /// 将订单状态通知加入队列。
    /// </summary>
    /// <param name="order">要通知的订单。</param>
    /// <param name="subscription">通知订阅。</param>
    private async Task QueueNotificationsAsync(
        Order order, NotificationSubscription subscription)
    {
        // 在实际情况下，其他后端进程会跟踪订单交付进度并在状态变化时发送通知。由于这里没有这样的进程，所以伪造它。
        await _orderQueue.QueueBackgroundOrderStatusAsync(async canellationToken =>
        {
            await Task.Delay(OrderWithStatus.PreparationDuration, canellationToken);
            return await SendNotificationAsync(order, subscription, "您的订单已发出！");
        });

        await _orderQueue.QueueBackgroundOrderStatusAsync(async canellationToken =>
        {
            await Task.Delay(OrderWithStatus.DeliveryDuration, canellationToken);
            return await SendNotificationAsync(order, subscription, "您的订单已送达。请享用！");
        });
    }

    /// <summary>
    /// 发送订单的推送通知。
    /// </summary>
    /// <param name="order">要发送通知的订单。</param>
    /// <param name="subscription">通知订阅。</param>
    /// <param name="message">通知消息。</param>
    /// <returns>订单。</returns>
    private static async Task<Order> SendNotificationAsync(Order order, NotificationSubscription subscription, string message)
    {
        // 对于实际应用程序，请生成您自己的密钥
        var publicKey = "BLC8GOevpcpjQiLkO7JmVClQjycvTCYWm6Cq_a7wJZlstGTVZvwGFFHMYfXt6Njyvgx_GlXJeo5cSiZ1y4JOx1o";
        var privateKey = "OrubzSz3yWACscZXjFQrrtDwCKg-TGFuWhluQ2wLXDo";

        var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
        var vapidDetails = new VapidDetails("mailto:<someone@example.com>", publicKey, privateKey);
        var webPushClient = new WebPushClient();
        try
        {
            var payload = JsonSerializer.Serialize(new
            {
                message,
                url = $"myorders/{order.OrderId}",
            });
            await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"发送推送通知时出错: {ex.Message}");
        }

        return order;
    }
}
