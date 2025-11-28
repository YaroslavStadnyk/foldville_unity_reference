using System;
using System.Collections.Generic;
using Core.Ordinaries;
using UnityEngine;

namespace Core.Managers
{
    public class AsyncCallbackManager : SingletonBehaviour<AsyncCallbackManager>
    {
        private readonly object _queueLock = new();
        private readonly List<Action> _queuedActions = new();
        private readonly List<Action> _executingActions = new();

        public static void Queue(Action action)
        {
            if (action == null)
            {
                Debug.LogWarning("Trying to queue null action");
                return;
            }

            var instance = Instance;
            if (instance == null)
            {
                Debug.LogWarning("Instance is null. Will not queue action.");
                return;
            }

            lock (instance._queueLock)
            {
                instance._queuedActions.Add(action);
            }
        }

        private void Update()
        {
            MoveQueuedActionsToExecuting();

            while (_executingActions.Count > 0)
            {
                var action = _executingActions[0];
                _executingActions.RemoveAt(0);
                action();
            }
        }

        private void MoveQueuedActionsToExecuting()
        {
            lock (_queueLock)
            {
                while (_queuedActions.Count > 0)
                {
                    var action = _queuedActions[0];
                    _executingActions.Add(action);
                    _queuedActions.RemoveAt(0);
                }
            }
        }
    }
}