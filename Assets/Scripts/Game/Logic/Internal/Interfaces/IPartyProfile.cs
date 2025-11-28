using UnityEngine;

namespace Game.Logic.Internal.Interfaces
{
    public interface IPartyProfile
    {
        public string ID { get; }

        /// <summary>
        /// This is called on the server when it is told that a client has finished switching from the room scene to a game player scene.
        /// </summary>
        /// <param name="gamePlayer"> The game player object. </param>
        public void OnRoomServerLoadedPlayer(GameObject gamePlayer);
    }
}