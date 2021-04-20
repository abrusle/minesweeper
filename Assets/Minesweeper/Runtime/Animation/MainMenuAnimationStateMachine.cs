using Minesweeper.Runtime.Views.UI.Animation;
using UnityEngine;

namespace Minesweeper.Runtime.Animation
{
    public enum MainMenuState
    {
        Default = 0,
        Classic = 1,
        Infinite = 2,
        Settings = 3
    }

    [CreateAssetMenu(fileName = "new Main Menu Animation State Machine", menuName = "Functional SO/UI/Main Menu Animation State Machine")]
    public class MainMenuAnimationStateMachine : AnimationStateMachine<MainMenuState>
    {
        
    }
}