using System;
using System.Collections.Generic;
using Core.Ordinaries;
using UnityEngine;

namespace Core.Managers
{
    public interface IEvent
    {
    }

    public class EventManager : Singleton<EventManager>
    {
        private readonly Dictionary<Type, List<Action<IEvent>>> _eventDictionary = new();
        private readonly HashSet<Type> _disabledEvents = new();

        public void AddListener<T>(Action<IEvent> eventHandler) where T : IEvent
        {
            var eventKey = typeof(T);

            if (!_eventDictionary.ContainsKey(eventKey))
            {
                _eventDictionary.Add(eventKey, new List<Action<IEvent>>());
            }

            var eventHandlerList = _eventDictionary[eventKey];
            eventHandlerList.Add(eventHandler);
        }

        public void RemoveListener<T>(Action<IEvent> eventHandler) where T : IEvent
        {
            var eventKey = typeof(T);

            if (!_eventDictionary.ContainsKey(eventKey))
            {
                return;
            }

            var eventHandlerList = _eventDictionary[eventKey];
            eventHandlerList.Remove(eventHandler);
        }

        public void Invoke(IEvent eventObject)
        {
            var eventKey = eventObject.GetType();

            if (_disabledEvents.Contains(eventKey))
            {
                return;
            }

            if (!_eventDictionary.ContainsKey(eventKey))
            {
                return;
            }

            var eventHandlerList = _eventDictionary[eventKey];
            foreach (var eventHandler in eventHandlerList)
            {
                if (eventHandler == null)
                {
                    Debug.LogWarning($"{nameof(eventHandler)} is null.");
                    continue;
                }

                eventHandler.Invoke(eventObject);
            }
        }

        #region Disable/Enable

        public void Disable<T>() where T : IEvent
        {
            var eventKey = typeof(T);
            _disabledEvents.Add(eventKey);
        }

        public void DisableAll()
        {
            foreach (var eventKey in _eventDictionary.Keys)
            {
                _disabledEvents.Add(eventKey);
            }
        }

        public void Enable<T>() where T : IEvent
        {
            var eventKey = typeof(T);
            _disabledEvents.Remove(eventKey);
        }

        public void EnableAll()
        { 
            _disabledEvents.Clear();
        }

        public bool IsEnabled<T>() where T : IEvent
        {
            var eventKey = typeof(T);
            return !_disabledEvents.Contains(eventKey);
        }

        #endregion
    }
}