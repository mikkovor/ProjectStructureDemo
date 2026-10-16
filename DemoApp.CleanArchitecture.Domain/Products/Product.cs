namespace DemoApp.CleanArchitecture.Domain.Products;

public sealed class Product
{
    public const int MaxNameLength = 100;
    public const decimal MaxPriceIncreaseFactor = 1.5m;

    public Product(Guid id, string name, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(name.Length, MaxNameLength, nameof(name));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        Id = id;
        Name = name;
        Price = price;
    }

    public Guid Id { get; }
    public string Name { get; }
    public decimal Price { get; private set; }

    public static Product Create(string name, decimal price) => new(Guid.NewGuid(), name, price);

    /// <summary>
    /// Changes the price unless it would increase by more than <see cref="MaxPriceIncreaseFactor"/>.
    /// </summary>
    public bool TryChangePrice(decimal newPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newPrice);

        if (newPrice > Price * MaxPriceIncreaseFactor)
        {
            return false;
        }

        Price = newPrice;
        return true;
    }
}
