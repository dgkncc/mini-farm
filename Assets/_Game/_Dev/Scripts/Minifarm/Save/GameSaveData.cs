using System;
using System.Collections.Generic;

namespace Minifarm.Save
{
    [Serializable]
    public class GameSaveData
    {
        public List<FactorySaveData> Factories = new List<FactorySaveData>();
        public List<ResourceSaveData> Resources = new List<ResourceSaveData>();
        public long LastSaveTime;
    }
}