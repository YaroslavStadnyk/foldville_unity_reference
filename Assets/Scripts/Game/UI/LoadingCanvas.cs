using Game.UI.Components.Pages;
using UnityEngine;

namespace Game.UI
{
    public class LoadingCanvas : Core.UI.LoadingCanvas
    {
        [SerializeField] private LoadingPage loadingPage;

        protected override void OnOperationsBegan()
        {
            loadingPage.Show();
        }

        protected override void OnOperationsUpdated(float operationsProgress)
        {
            loadingPage.ProgressionFill = operationsProgress;
        }

        protected override void OnOperationsCompleted()
        {
            loadingPage.Hide();
        }
    }
}
