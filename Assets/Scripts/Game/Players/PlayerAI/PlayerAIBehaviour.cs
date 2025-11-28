namespace Game.Players.PlayerAI
{
    /*
    public class PlayerAI : MonoBehaviour
    {
        [SerializeField] private HexTileGrid hexTileGrid;
        [SerializeField] private List<HexTile> currentTiles = new();
        [SerializeField] private SerializedDictionary<TileType, List<ITile>> sortedTiles = new();

        private HexMapScoreCounter scoreCounter;

        private void Awake()
        {
            scoreCounter = hexTileGrid.GetComponent<HexMapScoreCounter>();
        }

        private void Start()
        {
            sortedTiles = GetTilesByType();

            Dictionary<TileType, IEnumerable<Int2>> availableMoves = GetAvailableMoves();
            Turn bestCurrentTurn = CalculateBestCurrentMove(availableMoves);
            hexTileGrid.RemoveTile(bestCurrentTurn.tilePosition);
        }

        private SerializedDictionary<TileType, List<ITile>> GetTilesByType()
        {
            IEnumerable<ITile> allTiles = hexTileGrid.GetTiles();

            SerializedDictionary<TileType, List<ITile>> tiles = new();
            foreach (TileType type in (TileType[])Enum.GetValues(typeof(TileType)))
            {
                tiles.Add(type, new List<ITile>());
            }

            foreach (ITile tile in allTiles)
            {
                tiles[tile.Type].Add(tile);
            }

            return tiles;
        }

        private Turn CalculateBestCurrentMove(Dictionary<TileType, IEnumerable<Int2>> availableMoves)
        {
            Turn turn = new Turn();
            foreach (TileType tileType in availableMoves.Keys)
            {
                foreach (Int2 tilePosition in availableMoves[tileType])
                {
                    int predictScore = scoreCounter.PredictScore(tilePosition, tileType);
                    if (predictScore > turn.score)
                    {
                        turn.tileType = tileType;
                        turn.tilePosition = tilePosition;
                        turn.score = predictScore;
                    }
                }
            }

            return turn;
        }

        private Dictionary<TileType, IEnumerable<Int2>> GetAvailableMoves()
        {
            Dictionary<TileType, IEnumerable<Int2>> availableMoves = new();

            foreach (HexTile tile in currentTiles)
            {
                List<Int2> placeRequirementTiles = GetAvailablePlaceTiles(tile);

                List<Int2> neighborRequirementTiles = GetAvailableNeighborTiles(tile);

                availableMoves.Add(tile.Type, placeRequirementTiles.Intersect(neighborRequirementTiles));
            }

            return availableMoves;
        }

        private List<Int2> GetAvailablePlaceTiles(HexTile currentTile)
        {
            List<Int2> availablePlaceTiles = new();
            foreach (TileType tileType in currentTile.Behaviour.RequiredUnderlay)
            {
                availablePlaceTiles.AddRange(sortedTiles[tileType]
                    .Where(neighborTile => neighborTile != null)
                    .Select(tile => tile.GetIndexPosition()));
            }

            return availablePlaceTiles;
        }

        private List<Int2> GetAvailableNeighborTiles(HexTile currentTile)
        {
            List<Int2> availableNeighborTiles = new();
            foreach (TileType tileType in currentTile.Behaviour.RequiredNeighbors.Keys)
            {
                IEnumerable<Int2> form = HexForm.Create(currentTile.Behaviour.RequiredNeighbors[tileType] + 1, 0);
                foreach (ITile existingTile in sortedTiles[tileType])
                {
                    availableNeighborTiles.AddRange(hexTileGrid.GetTiles(existingTile.GetIndexPosition(), form)
                        .Where(neighborTile => neighborTile != null)
                        .Select(neighborTile => neighborTile.GetIndexPosition()));
                }
            }

            return availableNeighborTiles;
        }
    }
    */
}