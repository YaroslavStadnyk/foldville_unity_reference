using System.Collections.Generic;
using System.Linq;
using Board.Configs;
using Board.Interfaces;
using Board.Models;
using Grid.Common;
using UnityEngine;

namespace Board.Services
{
    public class CardService : ICardService
    {
        public void Initialize()
        {
        }

        public Card CreateCard(TileType type)
        {
            var card = new Card(type);
            BoardEvents.Instance.OnCardCreated?.Invoke(card.GetInfo());

            return card;
        }

        public List<Card> CreateCards(string tag)
        {
            if (!CardConfig.Instance.CardTemplates.TryGetValue(tag, out var cardTemplates) || !cardTemplates.Any())
            {
                Debug.LogWarning($"Card templates by tag '{tag}' not found.");
                return new List<Card>();
            }

            var cards = new List<Card>(cardTemplates.Count());
            foreach (var cardTemplate in cardTemplates)
            {
                var card = new Card(cardTemplate);
                BoardEvents.Instance.OnCardCreated?.Invoke(card.GetInfo());

                cards.Add(card);
            }

            return cards;
        }

        public Card CreateRandomCard(string tag)
        {
            if (!CardConfig.Instance.CardTemplates.TryGetValue(tag, out var cardTemplates) || !cardTemplates.Any())
            {
                Debug.LogWarning($"Card templates by tag '{tag}' not found.");
                return null;
            }

            var cardTemplate = cardTemplates.GetRandomValue();
            var card = new Card(cardTemplate);
            BoardEvents.Instance.OnCardCreated?.Invoke(card.GetInfo());

            return card;
        }

        public List<Card> CreateRandomCards(string tag, int count)
        {
            if (!CardConfig.Instance.CardTemplates.TryGetValue(tag, out var cardTemplates) || !cardTemplates.Any())
            {
                Debug.LogWarning($"Card templates by tag '{tag}' not found.");
                return new List<Card>();
            }

            var cards = new List<Card>(count);
            for (var i = 0; i < count; i++)
            {
                var cardTemplate = cardTemplates.GetRandomValue();
                var card = new Card(cardTemplate);
                BoardEvents.Instance.OnCardCreated?.Invoke(card.GetInfo());

                cards.Add(card);
            }

            return cards;
        }
    }
}