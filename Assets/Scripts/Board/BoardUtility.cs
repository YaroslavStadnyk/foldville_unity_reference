using System.Collections.Generic;
using System.Linq;
using Board.Models;
using Board.Structs;
using Grid.Common;

namespace Board
{
    public static class BoardUtility
    {
        public static CardInfo FirstOrDefault(this IEnumerable<CardInfo> cardInfos, string id)
        {
            foreach (var cardInfo in cardInfos)
            {
                if (cardInfo.ID == id)
                {
                    return cardInfo;
                }
            }

            return default;
        }

        public static CardInfo FirstOrDefault(this IEnumerable<CardInfo> cardInfos, TileType type)
        {
            foreach (var cardInfo in cardInfos)
            {
                if (cardInfo.Type == type)
                {
                    return cardInfo;
                }
            }

            return default;
        }

        public static bool Contains(this IEnumerable<CardInfo> cardInfos, string id)
        {
            foreach (var cardInfo in cardInfos)
            {
                if (cardInfo.ID == id)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains(this IEnumerable<CardInfo> cardInfos, TileType type)
        {
            foreach (var cardInfo in cardInfos)
            {
                if (cardInfo.Type == type)
                {
                    return true;
                }
            }

            return false;
        }

        public static CardInfo GetInfo(this Card card)
        {
            return new CardInfo(card.ID, card.Type);
        }

        public static List<CardInfo> GetInfo(this IEnumerable<Card> cards)
        {
            return cards.Select(GetInfo).ToList();
        }

        public static DeskInfo GetInfo(this Desk desk)
        {
            var hasCards = desk.Cards.Count > 0;
            var hasCapacity = desk.Capacity > 0 || desk.IsInfinite;

            return new DeskInfo (desk.ID, !(hasCards || hasCapacity));
        }

        public static List<DeskInfo> GetInfo(this IEnumerable<Desk> desks)
        {
            return desks.Select(GetInfo).ToList();
        }

        public static HandInfo GetInfo(this Hand hand)
        {
            return new HandInfo(hand.ID, hand.Cards.Values.GetInfo());
        }

        public static List<HandInfo> GetInfo(this IEnumerable<Hand> hands)
        {
            return hands.Select(GetInfo).ToList();
        }
    }
}