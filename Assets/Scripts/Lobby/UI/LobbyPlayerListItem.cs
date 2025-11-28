using System;
using System.Collections.Generic;
using Core.Models;
using Core.Ordinaries;
using Game;
using Game.Logic.Common.Enums;
using Game.Players.Player;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Lobby.UI
{
    public class LobbyPlayerListItem : PoolBehaviour
    {
        [Serializable]
        public class PlayerHolder
        {
            [SerializeField] public GameObject root;
            [SerializeField] public TMP_InputField playerIDInputField;
            [SerializeField] public Button closeButton;

            [Space] [SerializeField] public TMP_Text readyText;
            [SerializeField] public Button readyButton;
            [SerializeField] public Button cancelButton;

            [Space] [SerializeField] public float colorFlow = 0.25f;
            [SerializeField] public PaintableGroup paintableGroup = new();

            [Space] [SerializeField] public Image iconImage;
            [SerializeField] public List<Sprite> iconSprites = new();
            [SerializeField] public bool isRandomIconsEnabled = true;
        }

        [HideLabel] [SerializeField] private PlayerHolder playerHolder = new();

        private string _playerID;

        public void Initialize(string playerID)
        {
            _playerID = playerID;

            playerHolder.playerIDInputField.text = PlayerProfile.GetNickname(_playerID);
            playerHolder.closeButton.gameObject.SetActive(PlayerProfile.LocalLatest != null && PlayerProfile.LocalLatest.OwnedIDs.Count > 2);

            playerHolder.paintableGroup.SetColor(PlayerProfile.GetColor(playerID), playerHolder.colorFlow);
            if (playerHolder.isRandomIconsEnabled)
            {
                playerHolder.iconImage.sprite = playerHolder.iconSprites[Random.Range(0, playerHolder.iconSprites.Count)];
            }

            playerHolder.root.SetActive(true);
        }

        private void OnEnable()
        {
            playerHolder.playerIDInputField.onValueChanged.AddListener(OnNicknameChanged);
            playerHolder.readyButton.onClick.AddListener(OnReadyButtonClicked);
            playerHolder.cancelButton.onClick.AddListener(OnCancelButtonClicked);
            playerHolder.closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnDisable()
        {
            playerHolder.playerIDInputField.onValueChanged.RemoveListener(OnNicknameChanged);
            playerHolder.readyButton.onClick.RemoveListener(OnReadyButtonClicked);
            playerHolder.cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
            playerHolder.closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }

        private void OnNicknameChanged(string nickname)
        {
            PlayerProfile.SetNickname(_playerID, nickname);
        }

        public void SetReadyState(bool isReady)
        {
            if (GameManager.Instance.Mode is GameMode.OnlineMultiplayer)
            {
                playerHolder.readyButton.gameObject.SetActive(!isReady);
                playerHolder.cancelButton.gameObject.SetActive(isReady);
            }
            else
            {
                playerHolder.readyButton.gameObject.SetActive(!isReady);
                playerHolder.cancelButton.gameObject.SetActive(false);
            }
        }

        private void OnReadyButtonClicked()
        {
            GameManager.Instance.Party.SetPlayerReady(_playerID, true);
        }

        private void OnCancelButtonClicked()
        {
            GameManager.Instance.Party.SetPlayerReady(_playerID, false);
        }

        private void OnCloseButtonClicked()
        {
            PlayerProfile.LocalLatest.RemoveOwnedPlayer(_playerID);
        }
    }
}