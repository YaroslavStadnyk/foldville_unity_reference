using System;
using System.Collections.Generic;
using Core.Ordinaries;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Core.Pooling
{
    [Serializable]
    public class PoolDictionary<T, TKey> where T : PoolBehaviour
    {
        private readonly IObjectPool<PoolBehaviour> _pool;
        private readonly Dictionary<TKey, T> _spawnedBehaviours = new();

        public IReadOnlyDictionary<TKey, T> SpawnedBehaviours => _spawnedBehaviours;

        #region Inspector

        [SerializeField] public T prefab;
        [SerializeField] public Transform parent;

        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 100;

        protected PoolDictionary()
        {
            _pool = new ObjectPool<PoolBehaviour>(CreatePoolBehaviour, OnGetPoolBehaviour, OnReleasePoolBehaviour, OnDestroyPoolBehaviour, true, defaultCapacity, maxSize);
        }

        #endregion

        public PoolDictionary(int defaultCapacity = 10, int maxSize = 100)
        {
            _pool = new ObjectPool<PoolBehaviour>(CreatePoolBehaviour, OnGetPoolBehaviour, OnReleasePoolBehaviour, OnDestroyPoolBehaviour, true, defaultCapacity, maxSize);
            _spawnedBehaviours.EnsureCapacity(defaultCapacity);
        }

        public PoolDictionary(T prefab, Transform parent = null, int defaultCapacity = 10, int maxSize = 100)
        {
            _pool = new ObjectPool<PoolBehaviour>(CreatePoolBehaviour, OnGetPoolBehaviour, OnReleasePoolBehaviour, OnDestroyPoolBehaviour, true, defaultCapacity, maxSize);
            _spawnedBehaviours.EnsureCapacity(defaultCapacity);

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

        /// <param name="key"> the key with which the object will be created. </param>
        /// <returns> The new object or an object that hasn't been released and has the same key. </returns>
        public T Spawn(TKey key)
        {
            if (_spawnedBehaviours.TryGetValue(key, out var poolBehaviour))
            {
                return poolBehaviour;
            }

            var newPoolBehaviour = _pool.Get() as T;
            _spawnedBehaviours.Add(key, newPoolBehaviour);
            return newPoolBehaviour;
        }

        public bool Release(TKey key)
        {
            if (!_spawnedBehaviours.Remove(key, out var poolBehaviour))
            {
                return false;
            }

            poolBehaviour.Release();
            return true;
        }

        public void ReleaseAll()
        {
            foreach (var poolBehaviour in _spawnedBehaviours.Values)
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