using Minesweeper.Runtime.Animation;

namespace Minesweeper.Runtime.UI.MainMenu
{
    public interface IMainMenuStateChangeListener
    {
        void OnMainMenuStateChange(MainMenuState from, MainMenuState to);
    }
}