using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Extensions
{
    public static class BaseExtensions
    {
        #region Animator

        public static bool HasParameter(this Animator animator, string name)
        {
            return animator.parameters.Any(parameter => parameter.name == name);
        }

        #endregion

        #region Rigidbody

        public static void Reset(this Rigidbody rigidbody)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        public static void Reset(this Rigidbody2D rigidbody)
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.angularVelocity = 0.0f;
        }

        #endregion

        #region Collision

        public static void SetDynamic(this Collision collision)
        {
            var rigidbody = collision.rigidbody;
            rigidbody.isKinematic = false;
            rigidbody.AddForce(collision.relativeVelocity, ForceMode.Force);
        }

        #endregion

        #region Camera

        private static Camera _mainCamera;
        private static Camera MainCamera
        {
            get
            {
                if (_mainCamera == null) _mainCamera = Camera.main;
                return _mainCamera;
            }
        }

        #endregion

        #region Ray

        public static Vector3 GetGroundPoint(this Ray ray, Vector3 groundOffset)
        {
            var origin = ray.origin - groundOffset;
            return origin.Ground(ray.direction);
        }

        public static Ray ScreenPointToRay(Vector3 screenPoint)
        {
            return MainCamera.ScreenPointToRay(screenPoint);
        }

        #endregion

        #region Touch

#if UNITY_STANDALONE || UNITY_EDITOR
        /// <param name="pointerID"> <code> PointerInputModule.kMouseLeftId </code> by default. </param>
        public static bool IsOverUI(int pointerID = PointerInputModule.kMouseLeftId)
#else
        public static bool IsOverUI(int pointerID = 0)
#endif
        {
            return EventSystem.current.IsPointerOverGameObject(pointerID);
        }

        public static bool IsOverUI(this Touch touch)
        {
            return IsOverUI(touch.fingerId);
        }

        public static Ray GetRay(this Touch touch)
        {
            return ScreenPointToRay(touch.position);
        }

        public static bool GetHitInfo(this Touch touch, out RaycastHit hitInfo, int layerMask = -5, float maxDistance = 100)
        {
            var ray = touch.GetRay();
            return Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);
        }

        public static Vector3 GetWorldPoint(this Touch touch, Vector3 clipPoint, Vector2 screenOffset = default)
        {
            var clipPlane = Vector3.Distance(MainCamera.transform.position, clipPoint);
            var screenPoint = new Vector3(touch.position.x + screenOffset.x, touch.position.y + screenOffset.y, clipPlane);
            return MainCamera.ScreenToWorldPoint(screenPoint);
        }

        #endregion

        #region Texture

        public static Vector2 Size(this Texture texture)
        {
            return new Vector2(texture.width, texture.height);
        }

        public static Texture2D Copy(this Texture texture)
        {
            var sourceTexture = texture as Texture2D;
            if (sourceTexture == null) return new Texture2D(2048, 2048);

            var copiedTexture = new Texture2D(sourceTexture.width, sourceTexture.height);
            copiedTexture.SetPixels(sourceTexture.GetPixels());
            return copiedTexture;
        }

        #endregion

        #region Transform

        public static List<RectTransform> GetRectChilds(this Transform transform, bool includeInactive = true)
        {
            var childCount = transform.childCount;
            var childList = new List<RectTransform>(childCount);
            for (var i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child is not RectTransform rectChild)
                {
                    continue;
                }

                if (includeInactive || rectChild.gameObject.activeSelf)
                {
                    childList.Add(rectChild);
                }
            }

            return childList;
        }

        public static List<Transform> GetChilds(this Transform transform, bool includeInactive = true)
        {
            var childCount = transform.childCount;
            var childList = new List<Transform>(childCount);
            for (var i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);
                if (includeInactive || child.gameObject.activeSelf)
                {
                    childList.Add(child);
                }
            }

            return childList;
        }

        public static Vector3 GetPosition(this Transform transform, Space space = Space.Self)
        {
            return space == Space.Self ? transform.localPosition : transform.position;
        }

        public static void SetPosition(this Transform transform, Vector3 position, Space space = Space.Self)
        {
            if (space == Space.Self)
            {
                transform.localPosition = position;
            }
            else
            {
                transform.position = position;
            }
        }

        #endregion

        #region RectTransform

        public static Vector2 GetVector2(this TextAnchor anchor)
        {
            return anchor switch
            {
                TextAnchor.UpperLeft => new Vector2(0, 1),
                TextAnchor.UpperCenter => new Vector2(0.5f, 1),
                TextAnchor.UpperRight => new Vector2(1, 1),
                TextAnchor.MiddleLeft => new Vector2(0, 0.5f),
                TextAnchor.MiddleCenter => new Vector2(0.5f, 0.5f),
                TextAnchor.MiddleRight => new Vector2(1, 0.5f),
                TextAnchor.LowerLeft => new Vector2(0, 0),
                TextAnchor.LowerCenter => new Vector2(0.5f, 0),
                TextAnchor.LowerRight => new Vector2(1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null)
            };
        }

        public static void SetAnchors(this RectTransform rectTransform, TextAnchor anchor)
        {
            var vector = anchor.GetVector2();
            rectTransform.anchorMin = vector;
            rectTransform.anchorMax = vector;
        }

        public static void SetPivot(this RectTransform rectTransform, TextAnchor anchor)
        {
            rectTransform.pivot = anchor.GetVector2();
        }

        #endregion

        #region Component

        public static Component CopyTo(this Component original, GameObject target)
        {
            var type = original.GetType();
            var copy = target.AddComponent(type);
            var fields = type.GetFields();
            foreach (var field in fields) field.SetValue(copy, field.GetValue(original));
            return copy;
        }

        #endregion
    }
}