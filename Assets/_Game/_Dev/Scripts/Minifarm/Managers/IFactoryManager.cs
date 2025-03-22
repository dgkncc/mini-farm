using Minifarm.Factory;

namespace Minifarm.Managers
{
    public interface IFactoryManager
    {
        BaseFactory[] GetFactories();
        BaseFactory GetFactoryById(string id);
    }
}
