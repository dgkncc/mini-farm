using Minifarm.Common.Enums;

namespace Minifarm.Factory.Factories
{
    public class HayFactory : BaseFactory
    {
        protected override ResourceType GetResourceType() => ResourceType.Hay;
    }
}
