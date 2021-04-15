using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Runtime.Views.UI
{
    public class MainMenuView : MonoBehaviour
    {
        public Button quitButton;

        private void Start()
        {
            quitButton.onClick.AddListener(OnQuitButtonClick);
        }

        private void OnQuitButtonClick()
        {
            #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
            #else
            Application.Quit();
            #endif
        }
    }
}