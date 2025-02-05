using System;
using System.Collections.Generic;

namespace BlazingPizza
{
    public class OrderWithStatus
    {
        // 准备时间
        public readonly static TimeSpan PreparationDuration = TimeSpan.FromSeconds(10);
        // 配送时间（不现实，但更有趣）
        public readonly static TimeSpan DeliveryDuration = TimeSpan.FromMinutes(1);

        // 订单
        public Order Order { get; set; }

        // 状态文本
        public string StatusText { get; set; }

        // 是否已送达
        public bool IsDelivered => StatusText == "Delivered";

        // 从订单创建OrderWithStatus对象
        public static OrderWithStatus FromOrder(Order order)
        {
            // 为了模拟真实的后端处理，我们根据订单下单后的时间来伪造状态更新
            string statusText;
            var dispatchTime = order.CreatedTime.Add(PreparationDuration);

            if (DateTime.Now < dispatchTime)
            {
                statusText = "Preparing"; // 准备中
            }
            else if (DateTime.Now < dispatchTime + DeliveryDuration)
            {
                statusText = "Out for delivery"; // 配送中
            }
            else
            {
                statusText = "Delivered"; // 已送达
            }

            return new OrderWithStatus
            {
                Order = order,
                StatusText = statusText
            };
        }
    }
}
