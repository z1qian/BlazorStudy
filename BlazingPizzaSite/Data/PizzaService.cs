namespace BlazingPizza.Data;

public class PizzaService
{
    public Task<Pizza[]> GetPizzasAsync()
    {
        // 模拟返回一些数据

        // 一些常见的披萨名称
        var PizzaNames = new[]
        {
            "Hawaiian",
            "Pepperoni",
            "Meat Feast",
            "Vegetarian",
            "Margherita",
            "Ham and Mushroom",
            "Spicy Chicken",
            "BBQ Meat Feast",
            "Stuffed Crust Meat Feast",
            "Meatball",
            "Cheeseburger",
            "Hot Dog Stuffed Crust",
            "Vegan",
            "Vegan Hot 'N' Spicy",
            "Vegan BBQ",
            "Vegan Margherita",
            "Vegan Vegi Supreme",
            "Vegan Chick'n",
            "Vegan Chick'n Feast",
            "Vegan Chick'n BBQ",
            "Vegan Chick'n Hot 'N' Spicy",
            "Vegan Chick'n Delight",
            "Vegan Chick'n Supreme",
            "Vegan Chick'n Stuffed Crust",
            "Vegan Chick'n Sizzler",
            "Vegan Chick'n Feast Stuffed Crust",
            "Vegan Chick'n BBQ Stuffed Crust",
            "Vegan Chick'n Hot"
        };
        var rng = new Random();
        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new Pizza
        {
            PizzaId = index,
            Name = PizzaNames[rng.Next(PizzaNames.Length)],
            Description = "Some description",
            Price = rng.Next(5, 20),
            Vegetarian = rng.Next(0, 2) == 0,
            Vegan = rng.Next(0, 2) == 0
        }).ToArray());
    }
}