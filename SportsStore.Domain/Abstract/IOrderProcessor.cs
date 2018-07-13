using SportsStore.Domain.Entities;

namespace SportsStore.Domain.Abstract
{
    public interface IOrderProcessor
    {
        void ProceesOrder(Cart cart, ShippingDetails shippingDetails);
    }
}
