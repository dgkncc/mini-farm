using System;
using UnityEngine;

namespace Minifarm.Save
{
    [Serializable]
    public class FactorySaveData
    {
        public string ConfigId;
        public string InstanceId;
        public int StorageAmount;
        public int QueuedAmount;
        public float ProductionProgress;
        public long LastProductionTime;
        public Vector3 Position;
    }
}