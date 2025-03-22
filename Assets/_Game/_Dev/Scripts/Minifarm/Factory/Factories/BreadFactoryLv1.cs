using Minifarm.Common.Enums;

namespace Minifarm.Factory.Factories
{
    public class BreadFactoryLv1 : BaseFactory
    {
        protected override ResourceType GetResourceType() => ResourceType.Bread;
    }
}
