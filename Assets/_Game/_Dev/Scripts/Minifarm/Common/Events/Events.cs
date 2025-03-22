using Minifarm.Common.Enums;
using Minifarm.Factory;
namespace Minifarm.Common.Events
{
    public class ResourceChangeEvent
    {
        public ResourceType ResourceType;
        public int Amount;
        public int TotalAmount;
    }

    public class FactoryProductionEvent
    {
        public string FactoryId;
        public ResourceType ResourceType;
        public int Amount;
    }

    public class FactoryClickedEvent
    {
        public string FactoryId;
        public BaseFactory Factory;
    }

    public class EmptyClickEvent { }
}