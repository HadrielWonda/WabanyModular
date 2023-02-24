using BuildingBlocks.Abstractions.CQRS.Event;
using BuildingBlocks.Abstractions.CQRS.Event.Internal;
using BuildingBlocks.Abstractions.Messaging;
using ECommerce.Modules.Catalogs.Products.Features.CreatingProduct.Events.Domain;
using ECommerce.Modules.Catalogs.Products.Features.CreatingProduct.Events.Notification;
using ECommerce.Modules.Catalogs.Products.Features.DebitingProductStock.Events.Domain;
using ECommerce.Modules.Catalogs.Products.Features.ReplenishingProductStock.Events.Domain;

namespace ECommerce.Modules.Catalogs.Products;

public class ProductEventMapper : IEventMapper
{
    public IIntegrationEvent? MapToIntegrationEvent(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            ProductCreated e =>
                new Features.CreatingProduct.Events.Integration.ProductCreated(
                    e.Product.Id,
                    e.Product.Name,
                    e.Product.Category.Id,
                    e.Product.Category.Name,
                    e.Product.Stock.Available),
            ProductStockDebited e => new
                Features.DebitingProductStock.Events.Integration.ProductStockDebited(
                    e.ProductId, e.NewStock.Available, e.DebitedQuantity),
            ProductStockReplenished e => new
                Features.ReplenishingProductStock.Events.Integration.ProductStockReplenished(
                    e.ProductId, e.NewStock.Available, e.ReplenishedQuantity),
            _ => null
        };
    }

    public IDomainNotificationEvent? MapToDomainNotificationEvent(IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            ProductCreated e => new ProductCreatedNotification(e),
            _ => null
        };
    }

    public IReadOnlyList<IIntegrationEvent?> MapToIntegrationEvents(IReadOnlyList<IDomainEvent> domainEvents)
    {
        return domainEvents.Select(MapToIntegrationEvent).ToList().AsReadOnly();
    }

    public IReadOnlyList<IDomainNotificationEvent?> MapToDomainNotificationEvents(
        IReadOnlyList<IDomainEvent> domainEvents)
    {
        return domainEvents.Select(MapToDomainNotificationEvent).ToList().AsReadOnly();
    }
}
