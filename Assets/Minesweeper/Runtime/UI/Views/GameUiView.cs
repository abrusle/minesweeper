using TMPro;
using UnityEngine;

namespace Minesweeper.Runtime.Views
{
    public class GameUiView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mineCountText;

        public void OnMinesLeftCountChange(int minesLeftCount)
        {
            mineCountText.text = minesLeftCount.ToString("D");
        }
    }
}