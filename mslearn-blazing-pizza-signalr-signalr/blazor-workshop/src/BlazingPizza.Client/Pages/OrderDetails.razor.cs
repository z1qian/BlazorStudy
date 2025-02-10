namespace BlazingPizza.Client.Pages;

// OrderDetails 类实现 IAsyncDisposable 接口
public sealed partial class OrderDetails : IAsyncDisposable
{
    // SignalR Hub 连接对象
    private HubConnection _hubConnection;
    // 订单状态对象
    private OrderWithStatus _orderWithStatus;
    // 标识订单是否无效
    private bool _invalidOrder;

    // 订单 ID 参数
    [Parameter] public int OrderId { get; set; }

    // 注入的导航管理器
    [Inject] public NavigationManager Nav { get; set; } = default!;
    // 注入的订单客户端
    [Inject] public OrdersClient OrdersClient { get; set; } = default!;
    // 注入的访问令牌提供者
    [Inject] public IAccessTokenProvider AccessTokenProvider { get; set; } = default!;

    // 组件初始化时调用的方法
    protected override async Task OnInitializedAsync()
    {
        // 创建并配置 SignalR Hub 连接
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(
                Nav.ToAbsoluteUri("/orderstatus"),
                options => options.AccessTokenProvider = GetAccessTokenValueAsync)
            .WithAutomaticReconnect()
            .AddMessagePackProtocol()
            .Build();

        // 注册订单状态变更事件处理程序
        _hubConnection.On<OrderWithStatus>(
            OrderStatusHubConsts.EventNames.OrderStatusChanged, OnOrderStatusChangedAsync);

        // 启动 Hub 连接
        await _hubConnection.StartAsync();
    }

    // 获取访问令牌的异步方法
    private async Task<string> GetAccessTokenValueAsync()
    {
        var result = await AccessTokenProvider.RequestAccessToken();
        return result.TryGetToken(out var accessToken)
            ? accessToken.Value
            : null;
    }

    // 参数设置时调用的方法
    protected override async Task OnParametersSetAsync()
    {
        try
        {
            // 获取订单状态
            _orderWithStatus = await OrdersClient.GetOrder(OrderId);
            if (_orderWithStatus is null || _hubConnection is null)
            {
                return;
            }

            // 如果订单已送达，停止跟踪订单并停止 Hub 连接
            if (_orderWithStatus.IsDelivered)
            {
                await _hubConnection.InvokeAsync(
                    OrderStatusHubConsts.MethodNames.StopTrackingOrder, _orderWithStatus.Order);
                await _hubConnection.StopAsync();
            }
            // 否则，开始跟踪订单
            else
            {
                await _hubConnection.InvokeAsync(
                    OrderStatusHubConsts.MethodNames.StartTrackingOrder, _orderWithStatus.Order);
            }
        }
        catch (AccessTokenNotAvailableException ex)
        {
            // 处理访问令牌不可用异常
            ex.Redirect();
        }
        catch (Exception ex)
        {
            // 处理其他异常并标记订单无效
            _invalidOrder = true;
            Console.Error.WriteLine(ex);
        }
        finally
        {
            // 通知组件状态已更改
            StateHasChanged();
        }
    }

    // 订单状态变更事件处理程序
    private Task OnOrderStatusChangedAsync(OrderWithStatus orderWithStatus) =>
        InvokeAsync(() =>
        {
            _orderWithStatus = orderWithStatus;
            StateHasChanged();     
        });

    // 实现 IAsyncDisposable 接口的 DisposeAsync 方法
    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            if (_orderWithStatus is not null)
            {
                // 停止跟踪订单
                await _hubConnection.InvokeAsync(
                    OrderStatusHubConsts.MethodNames.StopTrackingOrder, _orderWithStatus.Order);
            }

            // 释放 Hub 连接资源
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }
}
