using Game.Logic.Common.Enums;

namespace Game
{
	public interface IGameManager
	{
		void Initialize(GameMode gameMode);
		void ResetImplementation();
	}
}
