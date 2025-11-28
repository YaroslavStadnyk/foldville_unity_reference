using System;
using System.Collections.Generic;
using Core.Ordinaries;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Core.Pooling
{
    [Serializable]
    public class PoolList<T> where T : PoolBehaviour
    {
        private readonly IObjectPool<PoolBehaviour> _pool;
        private readonly List<T> _spawnedBehaviours = new();

        public IReadOnlyList<T> SpawnedBehaviours => _spawnedBehaviours;

        #region Inspector

        [SerializeField] public T prefab;
        [SerializeField] public Transform parent;

        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 100;

        protected PoolList()
        {
            _pool = new ObjectPool<PoolBehaviour>(CreatePoolBehaviour, OnGetPoolBehaviour, OnReleasePoolBehaviour, OnDestroyPoolBehaviour, true, defaultCapacity, maxSize);
        }

        #endregion

        public PoolList(int defaultCapacity = 10, int maxSize = 100)
        {
            _pool = new ObjectPool<PoolBehaviour>(CreatePoolBehaviour, OnGetPoolBehaviour, OnReleasePoolBehaviour, OnDestroyPoolBehaviour, true, defaultCapacity, maxSize);
            _spawnedBehaviours.Capacity = defaultCapacity;
        }

        public PoolList(T prefab, Transform parent = null, int defaultCapacity = 10, int maxSize = 100)
        {
            _pool = new ObjectPool<PoolBehaviour>(CreatePoolBehaviour, OnGetPoolBehaviour, OnReleasePoolBehaviour, OnDestroyPoolBehaviour, true, defaultCapacity, maxSize);
            _spawnedBehaviours.Capacity = defaultCapacity;

            this.prefab = prefab;
            this.parent = parent;
        }

        protected virtual PoolBehaviour CreatePoolBehaviour()
        {
            var poolBehaviour = Object.Instantiate(prefab, parent);
            poolBehaviour.Pool = _pool;

            return poolBehaviour;
        }

        protected virtual void OnGetPoolBehaviour(PoolBehaviour poolBehaviour)
        {
            poolBehaviour.OnSpawn();
        }

        protected virtual void OnReleasePoolBehaviour(PoolBehaviour poolBehaviour)
        {
            poolBehaviour.OnRelease();
        }

        protected virtual  void OnDestroyPoolBehaviour(PoolBehaviour poolBehaviour)
        {
            Object.Destroy(poolBehaviour.gameObject);
        }

        public T Spawn()
        {
            var poolBehaviour = _pool.Get() as T;

            _spawnedBehaviours.Add(poolBehaviour);
            return poolBehaviour;
        }

        public bool Release()
        {
            var lastIndex = _spawnedBehaviours.Count - 1;
            if (lastIndex < 0)
            {
                return false;
            }

            var poolBehaviour = _spawnedBehaviours[lastIndex];
            poolBehaviour.Release();

            _spawnedBehaviours.RemoveAt(lastIndex);
            return true;
        }

        public void ReleaseAll()
        {
            foreach (var poolBehaviour in _spawnedBehaviours)
            {
                poolBehaviour.Release();
            }

            _spawnedBehaviours.Clear();
        }

        public void DestroyAll()
        {
            ReleaseAll();
            Clear();
        }

        public virtual void Clear()
        {
            _pool.Clear();
        }
    }
}