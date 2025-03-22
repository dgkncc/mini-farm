using Minifarm.Common.Enums;

namespace Minifarm.Factory.Factories
{
    public class FlourFactory : BaseFactory
    {
        protected override ResourceType GetResourceType() => ResourceType.Flour;
    }
}