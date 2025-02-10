namespace BlazingPizza.Server.Services;

public sealed class FakeOrderStatusService : BackgroundService
{
    private readonly IBackgroundOrderQueue _orderQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FakeOrderStatusService> _logger;

    // 构造函数，初始化依赖注入的服务
    public FakeOrderStatusService(
        IBackgroundOrderQueue orderQueue,
        IServiceProvider serviceProvider,
        ILogger<FakeOrderStatusService> logger) =>
        (_orderQueue, _serviceProvider, _logger) = (orderQueue, serviceProvider, logger);

    [SuppressMessage(
        "Style",
        "IDE0063:Use simple 'using' statement",
        Justification = "We need explicit disposal of the IServiceScope to avoid errant conditions.")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 当取消令牌未被触发时，持续执行循环
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 从队列中获取下一个工作项
                var workItem = await _orderQueue.DequeueAsync(stoppingToken);
                var order = await workItem(stoppingToken);
                Console.WriteLine($"Processing order {order.OrderId}.");

                // 创建一个新的服务范围
                using (var scope = _serviceProvider.CreateScope())
                {
                    var hubContext =
                        scope.ServiceProvider
                            .GetRequiredService<IHubContext<OrderStatusHub, IOrderStatusHub>>();

                    // 这是一个模拟的后端订单处理系统，不要在生产环境中使用
                    var trackingOrderId = order.ToOrderTrackingGroupId();
                    var orderWithStatus = await GetOrderAsync(scope.ServiceProvider, order.OrderId);
                    while (!orderWithStatus.IsDelivered)
                    {
                        // 向客户端发送订单状态更新
                        await hubContext.Clients.Group(trackingOrderId).OrderStatusChanged(orderWithStatus);
                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);

                        // 更新订单状态
                        orderWithStatus = OrderWithStatus.FromOrder(orderWithStatus.Order);
                    }

                    // 发送最终的交付状态更新
                    await hubContext.Clients.Group(trackingOrderId).OrderStatusChanged(orderWithStatus);
                }
            }
            catch (OperationCanceledException)
            {
                // 如果取消令牌被触发，防止抛出异常
            }
            catch (Exception ex)
            {
                // 记录执行任务工作项时发生的错误
                _logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }

    // 获取订单及其状态的异步方法
    static async Task<OrderWithStatus> GetOrderAsync(IServiceProvider serviceProvider, int orderId)
    {
        var pizzeStoreContext =
            serviceProvider.GetRequiredService<PizzaStoreContext>();

        var order = await pizzeStoreContext.Orders
            .Where(o => o.OrderId == orderId)
            .Include(o => o.DeliveryLocation)
            .Include(o => o.Pizzas).ThenInclude(p => p.Special)
            .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
            .SingleOrDefaultAsync();

        return OrderWithStatus.FromOrder(order);
    }
}
