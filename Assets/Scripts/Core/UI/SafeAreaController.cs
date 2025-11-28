using System;
using UnityEngine;

namespace Core.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class SafeAreaController : MonoBehaviour
	{
		private ScreenOrientation _lastOrientation = ScreenOrientation.LandscapeLeft;
		private Vector2 _lastResolution = Vector2.zero;
		private Vector2 _lastSafeArea = Vector2.zero;

		private Canvas _canvas = null!;
		private RectTransform _rectTransform = null!;

		private Rect SafeArea
		{
			get
			{
#if UNITY_EDITOR
				return Rect.MinMaxRect(0, 32, Screen.width, Screen.height - 20);
#else
			return Screen.safeArea;
#endif
			}
		}

		private void Awake()
		{
			_canvas = GetComponentInParent<Canvas>();
			_rectTransform = GetComponent<RectTransform>();

			if (_canvas == null)
				throw new NullReferenceException(nameof(_canvas));

			if (_rectTransform == null)
				throw new NullReferenceException(nameof(_rectTransform));

			_lastOrientation = Screen.orientation;
			_lastResolution.x = Screen.width;
			_lastResolution.y = Screen.height;
			_lastSafeArea = SafeArea.size;
		}

		private void Start()
		{
			ApplySafeArea();
		}

		private void Update()
		{
			if (Application.isMobilePlatform)
			{
				if (Screen.orientation != _lastOrientation)
					OrientationChanged();

				if (SafeArea.size != _lastSafeArea)
					SafeAreaChanged();
			}
			else
			{
				if (Math.Abs(Screen.width - _lastResolution.x) > 0.1f ||
				    Math.Abs(Screen.height - _lastResolution.y) > 0.1f)
					ResolutionChanged();
			}
		}

		private void ApplySafeArea()
		{
			if (!_rectTransform)
				return;

			var safeArea = SafeArea;

			var anchorMin = safeArea.position;
			var anchorMax = safeArea.position + safeArea.size;

			var pixelRect = _canvas.pixelRect;
			anchorMin.x /= pixelRect.width;
			anchorMin.y /= pixelRect.height;
			anchorMax.x /= pixelRect.width;
			anchorMax.y /= pixelRect.height;

			_rectTransform.anchorMin = anchorMin;
			_rectTransform.anchorMax = anchorMax;
		}

		private void OrientationChanged()
		{
			_lastOrientation = Screen.orientation;
			_lastResolution.x = Screen.width;
			_lastResolution.y = Screen.height;

			ApplySafeArea();
		}

		private void ResolutionChanged()
		{
			if (Math.Abs(_lastResolution.x - Screen.width) < 0.1f && Math.Abs(_lastResolution.y - Screen.height) < 0.1f)
				return;

			_lastResolution.x = Screen.width;
			_lastResolution.y = Screen.height;

			ApplySafeArea();
		}

		private void SafeAreaChanged()
		{
			if (_lastSafeArea == SafeArea.size)
				return;

			_lastSafeArea = SafeArea.size;

			ApplySafeArea();
		}
	}
}