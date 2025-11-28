using Core.Models;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Game.Environment.Common
{
    public class ObjectRandomizer : SerializedMonoBehaviour
    {
        #region Inspector

        [InfoBox("It makes objects active by their chance.")]
        [HideLabel] [OdinSerialize] private readonly RandomPack<GameObject> _gameObjectsPack = new();

        [Button]
        private void CastChildGameObjects()
        {
            _gameObjectsPack.Elements.Clear();

            for (var i = 0; i < transform.childCount; i++)
            {
                var childGameObject = transform.GetChild(i).gameObject;
                var element = new RandomPack<GameObject>.Element(childGameObject);
                _gameObjectsPack.Elements.Add(element);
            }
        }

        #endregion

        private void OnEnable()
        {
            Randomize();
        }

        public void Randomize()
        {
            foreach (var obj in _gameObjectsPack)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }

            foreach (var obj in _gameObjectsPack.GetRandomValues())
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}