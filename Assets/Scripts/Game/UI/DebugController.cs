using Game.Logic.Configs;
using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI
{
    public class DebugController : MonoBehaviour
    {
        [SerializeField] private GUIConsole guiConsole;
        [SerializeField] private NetworkPingDisplay networkPingDisplay;

        [Space] [SerializeField] private KeyCode hotKey = KeyCode.F12;
        [SerializeField] private Color defaultColor = Color.black;

        private bool _isShown = true;

        private void Start()
        {
            Hide();

            networkPingDisplay.color = defaultColor;
        }

        private void Hide()
        {
            _isShown = false;

            guiConsole.enabled = false;
            networkPingDisplay.enabled = false;
        }

        private void Show()
        {
            _isShown = true;

            guiConsole.enabled = true;
            networkPingDisplay.enabled = true;
        }

        private void Update()
        {
            UpdateInput();

            if (!_isShown)
            {
                return;
            }

            UpdateFPS();
        }

        private void OnGUI()
        {
            if (!_isShown)
            {
                return;
            }

            DrawFPS();
            DrawButtons();
        }

        private void UpdateInput()
        {
            if (!Input.GetKeyDown(hotKey))
            {
                return;
            }

            if (_isShown)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private float _avgDeltaTime;
        private int _fps;

        private void UpdateFPS()
        {
            _avgDeltaTime = (_avgDeltaTime + Time.unscaledDeltaTime) * 0.5f;
            _fps = _avgDeltaTime > 0f ? Mathf.CeilToInt(1f / _avgDeltaTime) : 0;
        }

        private void DrawFPS()
        {
            var padding = new Vector2(2, 22);
            var width = 150;
            var height = 25;

            var rect = new Rect(Screen.width - width - padding.x, Screen.height - height - padding.y, width, height);
            var style = GUI.skin.GetStyle("Label");
            style.alignment = TextAnchor.LowerRight;

            GUI.color = defaultColor;
            GUI.Label(rect, $"FPS: {_fps}", style);
        }

        private void DrawButtons()
        {
            var padding = new Vector2(2, 2);
            var width = 250;
            var height = 25;

            var rect = new Rect(padding.x, Screen.height - height - padding.y, width, height);
            var style = GUI.skin.GetStyle("Label");
            style.alignment = TextAnchor.LowerLeft;

            GUI.color = defaultColor * 0.5f;
            GUILayout.BeginArea(rect, style);

            DrawTimerEnabledButton();

            GUILayout.EndArea();
        }

        private static bool TimerEnabled
        {
            get => GameManager.Instance.Turn.SecondsPerTurn != -1;
            set => GameManager.Instance.Turn.CustomSecondsPerTurn = value ? null : -1;
        }

        private void DrawTimerEnabledButton()
        {
            GUI.color = defaultColor;
            TimerEnabled = GUILayout.Toggle(TimerEnabled, "Timer Enabled");
        }
    }
}
