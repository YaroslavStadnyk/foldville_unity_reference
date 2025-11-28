using System;
using Core.Ordinaries;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Core.Pooling
{
    /// <summary>
    /// Default pool implementation for PoolBehaviour.
    /// </summary>
    [Serializable]
    public class Pool<T> where T : PoolBehaviour
    {
        private readonly IObjectPool<PoolBehaviour> _pool;

        #region Inspector

        [SerializeField] public T prefab;
        [SerializeField] public Transform parent;

        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 100;

        protected Pool()
        {
            _pool = new ObjectPool<PoolBehaviour>(CreatePoolBehaviour, OnGetPoolBehaviour, OnReleasePoolBehaviour, OnDestroyPoolBehaviour, true, defaultCapacity, maxSize);
        }

        #endregion

        public Pool(int defaultCapacity = 10, int maxSize = 100)
        {
            _pool = new ObjectPool<PoolBehaviour>(CreatePoolBehaviour, OnGetPoolBehaviour, OnReleasePoolBehaviour, OnDestroyPoolBehaviour, true, defaultCapacity, maxSize);
        }

        public Pool(T prefab, Transform parent = null, int defaultCapacity = 10, int maxSize = 100)
        {
            _pool = new ObjectPool<PoolBehaviour>(CreatePoolBehaviour, OnGetPoolBehaviour, OnReleasePoolBehaviour, OnDestroyPoolBehaviour, true, defaultCapacity, maxSize);

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

        public virtual T Spawn()
        {
            var poolBehaviour = _pool.Get();
            return poolBehaviour as T;
        }

        public virtual void Clear()
        {
            _pool.Clear();
        }
    }
}