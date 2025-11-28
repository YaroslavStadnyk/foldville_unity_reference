using System.Collections.Generic;
using System.Linq;
using Board.Configs;
using Board.Interfaces;
using Board.Models;
using Board.Structs;
using Core.Managers;
using UnityEngine;

namespace Board.Services
{
    public class DeskService : IDeskService
    {
        private ICardService _cardService;

        private readonly Dictionary<string, Desk> _desks = new();

        public void Initialize()
        {
            _cardService = ServiceManager.Instance.GetService<ICardService>();
        }

        public Desk CreateDesk(string deskName)
        {
            if (!DeskConfig.Instance.DeskTemplates.TryGetValue(deskName, out var deskTemplate))
            {
                Debug.LogWarning($"{nameof(Desk)}: {deskName} doesn't exist!");
                return null;
            }

            if (_desks.ContainsKey(deskName))
            {
                Debug.LogWarning($"{nameof(Desk)}: {deskName} is already created!");
                return null;
            }

            var desk = new Desk(deskTemplate, deskName);
            BoardEvents.Instance.OnDeskCreated?.Invoke(desk.GetInfo());

            _desks.Add(desk.ID, desk);

            return desk;
        }

        public bool RemoveDesk(string deskID)
        {
            if (deskID == null)
            {
                Debug.LogWarning($"{nameof(Desk)} id can't be null.");
                return false;
            }

            if (_desks.Remove(deskID, out var desk))
            {
                BoardEvents.Instance.OnDeskRemoved?.Invoke(desk.GetInfo());
                return true;
            }

            return false;
        }

        public void RemoveAllDesks()
        {
            foreach (var deskID in _desks.Keys.ToArray())
            {
                RemoveDesk(deskID);
            }
        }

        public Desk GetCreatedDesk(string deskID)
        {
            if (_desks.TryGetValue(deskID, out var desk))
            {
                return desk;
            }

            Debug.LogWarning($"{nameof(Desk)}: {deskID} not found.");
            return null;
        }

        public List<Desk> GetCreatedDesks()
        {
            return new List<Desk>(_desks.Values);
        }

        public bool TryReadCard(string deskID, out CardInfo cardInfo)
        {
            if (_desks.TryGetValue(deskID, out var desk))
            {
                return TryReadCard(desk, out cardInfo);
            }

            Debug.LogWarning($"{nameof(Desk)}: {deskID} not found.");
            cardInfo = default;
            return false;
        }

        public Card TakeCard(string deskID)
        {
            if (_desks.TryGetValue(deskID, out var desk))
            {
                var card = TakeCard(desk);
                if (card != null)
                {
                    BoardEvents.Instance.OnDeskChanged?.Invoke(desk.GetInfo());
                }

                return card;
            }

            Debug.LogWarning($"{nameof(Desk)}: {deskID} not found.");
            return null;
        }

        public bool PutCard(string deskID, Card card)
        {
            if (_desks.TryGetValue(deskID, out var desk))
            {
                var isSuccess = PutCard(desk, card);
                if (isSuccess)
                {
                    BoardEvents.Instance.OnDeskChanged?.Invoke(desk.GetInfo());
                }

                return isSuccess;
            }

            Debug.LogWarning($"{nameof(Desk)}: {deskID} not found.");
            return false;
        }

        private bool TryReadCard(Desk desk, out CardInfo cardInfo)
        {
            var hasCards = desk.Cards.Count > 0;
            var hasCapacity = desk.Capacity > 0 || desk.IsInfinite;
            if (!hasCards && hasCapacity)
            {
                var createdCard = _cardService.CreateRandomCard(desk.CardsTag);
                hasCards = PutCard(desk, createdCard);
            }

            if (!hasCards)
            {
                cardInfo = default;
                return false;
            }

            var card = desk.Cards[0];
            cardInfo = card.GetInfo();
            return true;
        }

        private Card TakeCard(Desk desk)
        {
            var hasCards = desk.Cards.Count > 0;
            var hasCapacity = desk.Capacity > 0 || desk.IsInfinite;
            if (!hasCards && hasCapacity)
            {
                var createdCard = _cardService.CreateRandomCard(desk.CardsTag);
                hasCards = PutCard(desk, createdCard);
            }

            if (!hasCards)
            {
                Debug.LogWarning($"{nameof(Desk)}: {desk.ID} is empty.");
                return null;
            }

            var card = desk.Cards[0];
            desk.Cards.RemoveAt(0);

            return card;
        }

        private bool PutCard(Desk desk, Card card)
        {
            var hasCapacity = desk.Capacity > 0 || desk.IsInfinite;
            if (!hasCapacity)
            {
                Debug.LogWarning($"{nameof(Desk)}: {desk.ID} was filled.");
                return false;
            }

            desk.Capacity -= 1;
            desk.Cards.Add(card);

            return true;
        }
    }
}