namespace BlazingPizza.Server.Hubs;

public interface IOrderStatusHub
{
    /// <summary>
    /// 此事件名称应与 <see cref="OrderStatusHubConsts.EventNames.OrderStatusChanged"/> 匹配，
    /// 该名称与客户端共享以便于发现。
    /// </summary>
    Task OrderStatusChanged(OrderWithStatus order);
}
