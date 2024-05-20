using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public class PoolData
    {
        public GameObject prefab;
        public int maxCount;
        public Queue<Poolable> pool;
    }
}