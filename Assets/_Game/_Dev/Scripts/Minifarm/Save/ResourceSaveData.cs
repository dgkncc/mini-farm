using System;
using Minifarm.Common.Enums;

namespace Minifarm.Save
{
    [Serializable]
    public class ResourceSaveData
    {
        public ResourceType Type;
        public int Amount;
    }
}