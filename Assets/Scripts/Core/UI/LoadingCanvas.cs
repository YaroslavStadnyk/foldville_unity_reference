using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Ordinaries;
using UnityEngine;

namespace Core.UI
{
    public abstract class LoadingCanvas : SingletonBehaviour<LoadingCanvas>
    {
        private readonly List<AsyncOperation> _operations = new();

        public void AddOperation(AsyncOperation operation)
        {
            _operations.Add(operation);
            _updateOperationsCoroutine ??= StartCoroutine(UpdateOperations());
        }

        public void RemoveOperation(AsyncOperation operation)
        {
            _operations.Remove(operation);
        }

        private bool IsOperationsDone => _operations.All(operation => operation?.isDone ?? true);
        private float OperationsProgress => _operations.Count == 0 ? 0f : _operations.Sum(operation => operation?.progress ?? 0f) / _operations.Count;

        private Coroutine _updateOperationsCoroutine;

        private IEnumerator UpdateOperations()
        {
            OnOperationsBegan();

            while (!IsOperationsDone)
            {
                OnOperationsUpdated(OperationsProgress);
                _operations.RemoveAll(operation => operation?.isDone ?? true);
                yield return null;
            }

            OnOperationsCompleted();
            _updateOperationsCoroutine = null;
        }

        protected abstract void OnOperationsBegan();
        protected abstract void OnOperationsUpdated(float operationsProgress);
        protected abstract void OnOperationsCompleted();
    }
}