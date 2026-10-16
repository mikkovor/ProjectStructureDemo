using DemoApp.Layered.DTOs;

namespace DemoApp.Layered.Services;

public enum ChangePriceStatus
{
    Changed,
    NotFound,
    IncreaseTooLarge
}

public sealed record ChangePriceResult(ChangePriceStatus Status, ProductResponse? Product = null);
