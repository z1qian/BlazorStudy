namespace BlazingPizza.Server.Hubs;

[Authorize]
public class OrderStatusHub : Hub<IOrderStatusHub>
{
    /// <summary>
    /// 将当前连接添加到订单的唯一组标识符中，实时通知订单状态更改。
    /// 此方法名称应与 <see cref="OrderStatusHubConsts.MethodNames.StartTrackingOrder"/> 匹配，
    /// 该名称与客户端共享以便于发现。
    /// </summary>
    public Task StartTrackingOrder(Order order) =>
        Groups.AddToGroupAsync(
            Context.ConnectionId, order.ToOrderTrackingGroupId());

    /// <summary>
    /// 将当前连接从订单的唯一组标识符中移除，结束此订单的实时更改更新。
    /// 此方法名称应与 <see cref="OrderStatusHubConsts.MethodNames.StopTrackingOrder"/> 匹配，
    /// 该名称与客户端共享以便于发现。
    /// </summary>
    public Task StopTrackingOrder(Order order) =>
        Groups.RemoveFromGroupAsync(
            Context.ConnectionId, order.ToOrderTrackingGroupId());
}
