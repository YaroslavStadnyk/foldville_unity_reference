using System.Collections.Generic;
using Board;
using Board.Interfaces;
using Board.Structs;
using Core.Extensions;
using Core.Managers;
using Game.Logic.Common.Enums;
using Game.Logic.Internal.Interfaces;
using Mirror;

namespace Game.Logic.Internal.Network
{
    // TODO maybe should be implemented like context services and should have a performance review 
    public class BoardManagerNetwork : BaseNetwork, IBoardManager
    {
        // TODO cards data should be not visible for client that has not the context
        private readonly SyncDictionary<CardInfo, string> _cardsData = new(); // CardInfo | Hand ID
        private readonly SyncDictionary<string, bool> _desksData = new(); // Desk NameID | IsEmpty

        private readonly Dictionary<CardInfo, string> _oldCardsData = new();

        public IDictionary<string, CardInfo> Cards
        {
            get
            {
                var cards = new Dictionary<string, CardInfo>();
                foreach (var cardInfo in _cardsData.Keys)
                {
                    cards[cardInfo.ID] = cardInfo;
                }

                return cards;
            }
        }

        public IDictionary<string, DeskInfo> Desks
        {
            get
            {
                var desks = new Dictionary<string, DeskInfo>();
                foreach (var (nameID, isEmpty) in _desksData)
                {
                    desks[nameID] = new DeskInfo(nameID, isEmpty);
                }

                return desks;
            }
        }

        public IDictionary<string, HandInfo> Hands
        {
            get
            {
                var hands = new Dictionary<string, HandInfo>();
                foreach (var (cardInfo, handID) in _cardsData)
                {
                    if (hands.ContainsKey(handID))
                    {
                        hands[handID].Cards.Add(cardInfo);
                    }
                    else
                    {
                        hands[handID] = new HandInfo(handID, new List<CardInfo> { cardInfo });
                    }
                }

                return hands;
            }
        }

        private ICardService _cardService;
        private IDeskService _deskService;
        private IHandService _handService;

        private void Start()
        {
            _cardService = ServiceManager.Instance.GetService<ICardService>();
            _deskService = ServiceManager.Instance.GetService<IDeskService>();
            _handService = ServiceManager.Instance.GetService<IHandService>();
        }

        private void OnDestroy()
        {
            _deskService?.RemoveAllDesks();
            _handService?.RemoveAllHands();
        }

        public override void OnStartServer()
        {
            _oldCardsData.Clear();
        }

        public override void OnStartClient()
        {
            _cardsData.Callback += OnCardsDataChanged;
        }

        public override void OnStopClient()
        {
            _cardsData.Callback -= OnCardsDataChanged;
        }

        private void OnCardsDataChanged(SyncIDictionary<CardInfo, string>.Operation operation, CardInfo cardInfo, string ownerID)
        {
            var oldOwnerID = _oldCardsData.FirstOrDefault(cardInfo);
            var newOwnerID = _cardsData.FirstOrDefault(cardInfo);

            var operationType = operation.ToType();
            switch (operationType)
            {
                case OperationType.Add or OperationType.Set:
                    _oldCardsData[cardInfo] = newOwnerID;
                    break;
                case OperationType.Remove:
                    _oldCardsData.Remove(cardInfo);
                    break;
                case OperationType.Clear:
                    _oldCardsData.Clear();
                    break;
                default:
                    break;
            }

            GameEvents.Instance.OnCardOwnersChanged?.Invoke(operationType, cardInfo, oldOwnerID, newOwnerID);
        }

        private void OnEnable()
        {
            BoardEvents.Instance.OnHandChanged += OnHandChanged;
            BoardEvents.Instance.OnDeskCreated += OnDeskCreated;
            BoardEvents.Instance.OnDeskChanged += OnDeskChanged;
            BoardEvents.Instance.OnDeskRemoved += OnDeskRemoved;
        }

        private void OnDisable()
        {
            BoardEvents.Instance.OnHandChanged -= OnHandChanged;
            BoardEvents.Instance.OnDeskCreated -= OnDeskCreated;
            BoardEvents.Instance.OnDeskChanged -= OnDeskChanged;
            BoardEvents.Instance.OnDeskRemoved -= OnDeskRemoved;
        }

        [ServerCallback]
        private void OnHandChanged(HandInfo hand)
        {
            foreach (var (cardInfo, handID) in new Dictionary<CardInfo, string>(_cardsData))
            {
                if (handID != hand.ID)
                {
                    continue;
                }

                if (!hand.Cards.Contains(cardInfo))
                {
                    _cardsData.Remove(cardInfo);
                }
            }

            foreach (var cardInfo in hand.Cards)
            {
                if (!_cardsData.ContainsKey(cardInfo))
                {
                    _cardsData[cardInfo] = hand.ID;
                }
            }
        }

        [ServerCallback]
        private void OnDeskCreated(DeskInfo desk)
        {
            _desksData[desk.ID] = desk.IsEmpty;
        }

        [ServerCallback]
        private void OnDeskChanged(DeskInfo desk)
        {
            _desksData[desk.ID] = desk.IsEmpty;
        }

        [ServerCallback]
        private void OnDeskRemoved(DeskInfo desk)
        {
            _desksData.Remove(desk.ID);
        }
    }
}