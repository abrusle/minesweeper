using Minesweeper.Runtime.Animation;
using Minesweeper.Runtime.Data;
using Minesweeper.Runtime.UI.MainMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minesweeper.Runtime.UI.Views
{
    public class ClassicModeLaunchView : MonoBehaviour, IMainMenuStateChangeListener
    {
        [Header("References")]
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private LevelSettings targetLevelSettings;
        [SerializeField] private LevelDifficultyMap difficultySettings;
        [SerializeField] private Button playButton;
        [SerializeField] private TMP_Dropdown presetDropdown;
        [SerializeField] private TMP_InputField mineCountField, rowCountField, columnCountField;
        [Header("Settings")]
        [SerializeField] private Difficulty startingDifficulty;

        private void OnEnable()
        {
            playButton.onClick.AddListener(OnPlayButtonClick);
            presetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            mineCountField.onValueChanged.AddListener(OnAnyFieldValueChange);
            rowCountField.onValueChanged.AddListener(OnAnyFieldValueChange);
            columnCountField.onValueChanged.AddListener(OnAnyFieldValueChange);
        }

        private void OnDisable()
        {
            playButton.onClick.RemoveListener(OnPlayButtonClick);
            presetDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            mineCountField.onValueChanged.RemoveListener(OnAnyFieldValueChange);
            rowCountField.onValueChanged.RemoveListener(OnAnyFieldValueChange);
            columnCountField.onValueChanged.RemoveListener(OnAnyFieldValueChange);
        }

        private void Start()
        {
            presetDropdown.SetValueWithoutNotify((int) startingDifficulty);
            SetFieldValuesFromSettings(difficultySettings[startingDifficulty]);
        }

        private void OnDropdownValueChanged(int selectedIndex)
        {
            print(selectedIndex);
            var difficulty = (Difficulty) selectedIndex;
            if (difficultySettings.ContainsKey(difficulty))
            {
                var levelSettings = difficultySettings[difficulty];
                SetFieldValuesFromSettings(levelSettings);
            }
        }

        private void OnAnyFieldValueChange(string value)
        {
            presetDropdown.SetValueWithoutNotify((int) Difficulty.Custom);
        }

        private void OnPlayButtonClick()
        {
            if (!int.TryParse(mineCountField.text, out targetLevelSettings.mineCount))
            {
                targetLevelSettings.mineCount = 1;
            }

            if (!int.TryParse(rowCountField.text, out int parsedInt))
            {
                parsedInt = 1;
            }
            targetLevelSettings.size.y = parsedInt;
            
            if (!int.TryParse(columnCountField.text, out parsedInt))
            {
                parsedInt = 1;
            }
            targetLevelSettings.size.x = parsedInt;
            
            SceneManager.Instance.SwitchScene(SceneManager.SceneId.GameClassic);
        }

        private void SetFieldValuesFromSettings(LevelSettings settings)
        {
            mineCountField.SetTextWithoutNotify(settings.mineCount.ToString());
            rowCountField.SetTextWithoutNotify(settings.size.x.ToString());
            columnCountField.SetTextWithoutNotify(settings.size.y.ToString());
        }

        /// <inheritdoc />
        void IMainMenuStateChangeListener.OnMainMenuStateChange(MainMenuState from, MainMenuState to)
        {
            graphicRaycaster.enabled = to == MainMenuState.Classic;
        }
    }
}