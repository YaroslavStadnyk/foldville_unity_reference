using System.Collections.Generic;
using Board.Models;
using Core.Managers;
using Grid.Common;

namespace Board.Interfaces
{
    public interface ICardService : IService
    {
        /// <summary>
        /// Create a card by type.
        /// </summary>
        public Card CreateCard(TileType type);

        /// <summary>
        /// Create cards by tag.
        /// </summary>
        public List<Card> CreateCards(string tag);

        /// <summary>
        /// Create a random card by tag.
        /// </summary>
        public Card CreateRandomCard(string tag);

        /// <summary>
        /// Create random cards by tag.
        /// </summary>
        public List<Card> CreateRandomCards(string tag, int count);
    }
}