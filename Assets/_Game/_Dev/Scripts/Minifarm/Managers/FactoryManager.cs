using System.Collections.Generic;
using Minifarm.Factory;

namespace Minifarm.Managers
{
    public class FactoryManager : IFactoryManager
    {
        private List<BaseFactory> factories = new List<BaseFactory>();

        public void RegisterFactory(BaseFactory factory)
        {
            factories.Add(factory);
        }

        public BaseFactory[] GetFactories()
        {
            return factories.ToArray();
        }

        public BaseFactory GetFactoryById(string id)
        {
            return factories.Find(f => f.GetFactoryId() == id);
        }
    }
}