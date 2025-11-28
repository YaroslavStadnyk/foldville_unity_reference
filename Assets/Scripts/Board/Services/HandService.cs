using System.Collections.Generic;
using System.Linq;
using Board.Interfaces;
using Board.Models;
using Board.Structs;
using UnityEngine;

namespace Board.Services
{
    public class HandService : IHandService
    {
        private readonly Dictionary<string, Hand> _hands = new();

        public void Initialize()
        {
        }

        public Hand CreateHand(string handID)
        {
            if (_hands.ContainsKey(handID))
            {
                Debug.LogWarning($"{nameof(Hand)}: {handID} is already created!");
                return null;
            }

            var playerHand = new Hand(handID);
            BoardEvents.Instance.OnHandCreated?.Invoke(playerHand.GetInfo());

            _hands.Add(handID, playerHand);

            return playerHand;
        }

        public bool RemoveHand(string handID)
        {
            if (handID == null)
            {
                Debug.LogWarning($"{nameof(Desk)} id can't be null.");
                return false;
            }

            if (_hands.Remove(handID, out var playerHand))
            {
                BoardEvents.Instance.OnHandRemoved?.Invoke(playerHand.GetInfo());
                return true;
            }

            return false;
        }

        public void RemoveAllHands()
        {
            foreach (var playerID in _hands.Keys.ToArray())
            {
                RemoveHand(playerID);
            }
        }

        public Hand GetCreatedHand(string handID)
        {
            if (_hands.TryGetValue(handID, out var playerHand))
            {
                return playerHand;
            }

            Debug.LogWarning($"{nameof(Hand)}: {handID} not found.");
            return null;
        }

        public List<Hand> GetCreatedHands()
        {
            return new List<Hand>(_hands.Values);
        }

        public bool TryReadCard(string handID, string cardID, out CardInfo cardInfo)
        {
            if (_hands.TryGetValue(handID, out var hand))
            {
                return TryReadCard(hand, cardID, out cardInfo);
            }

            Debug.LogWarning($"{nameof(Hand)}: {handID} not found.");
            cardInfo = default;
            return false;
        }

        public Card TakeCard(string handID, string cardID)
        {
            if (_hands.TryGetValue(handID, out var hand))
            {
                var card = TakeCard(hand, cardID);
                if (card != null)
                {
                    BoardEvents.Instance.OnHandChanged?.Invoke(hand.GetInfo());
                }

                return card;
            }

            Debug.LogWarning($"{nameof(Hand)}: {handID} not found.");
            return null;
        }

        public bool PutCard(string handID, Card card)
        {
            if (_hands.TryGetValue(handID, out var hand))
            {
                var isSuccess = PutCard(hand, card);
                if (isSuccess)
                {
                    BoardEvents.Instance.OnHandChanged?.Invoke(hand.GetInfo());
                }

                return isSuccess;
            }

            Debug.LogWarning($"{nameof(Hand)}: {handID} not found.");
            return false;
        }

        private static bool TryReadCard(Hand hand, string cardID, out CardInfo cardInfo)
        {
            if (hand.Cards.TryGetValue(cardID, out var card))
            {
                cardInfo = card.GetInfo();
                return true;
            }

            cardInfo = default;
            return false;
        }

        private static Card TakeCard(Hand hand, string cardID)
        {
            if (hand.Cards.Remove(cardID, out var card))
            {
                return card;
            }

            Debug.LogWarning($"{nameof(Hand)}: {hand.ID} doesn't contain a card with id {cardID}");
            return null;
        }

        private static bool PutCard(Hand hand, Card card)
        {
            if (hand.Cards.TryAdd(card.ID, card))
            {
                return true;
            }

            Debug.LogWarning($"{nameof(Hand)}: {hand.ID} already contains a card with id {card.ID}");
            return false;
        }
    }
}