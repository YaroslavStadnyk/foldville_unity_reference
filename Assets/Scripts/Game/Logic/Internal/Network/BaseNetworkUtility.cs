using System;
using Game.Logic.Common.Enums;
using Mirror;

namespace Game.Logic.Internal.Network
{
    public static class BaseNetworkUtility
    {
        public static OperationType ToType<TKey, TValue>(this SyncIDictionary<TKey, TValue>.Operation operation)
        {
            return operation switch
            {
                SyncIDictionary<TKey, TValue>.Operation.OP_ADD => OperationType.Add,
                SyncIDictionary<TKey, TValue>.Operation.OP_CLEAR => OperationType.Clear,
                SyncIDictionary<TKey, TValue>.Operation.OP_REMOVE => OperationType.Remove,
                SyncIDictionary<TKey, TValue>.Operation.OP_SET => OperationType.Set,
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
        }
    }
}