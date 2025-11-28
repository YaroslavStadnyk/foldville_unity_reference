using UnityEngine;
using UnityEngine.Pool;

namespace Core.Ordinaries
{
    public class PoolBehaviour : MonoBehaviour
    {
        public IObjectPool<PoolBehaviour> Pool { get; internal set; }

        public void Release()
        {
            Pool?.Release(this);
        }

        public virtual void OnSpawn()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnRelease()
        {
            gameObject.SetActive(false);
        }
    }
}