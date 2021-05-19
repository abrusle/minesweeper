using System;

namespace Minesweeper.Runtime.Input
{
    public class LevelActions : InputHandler
    {
        protected virtual void Update()
        {
            HandleAction(UserAction.Primary, OnClickPrimary);
            HandleAction(UserAction.Secondary, OnClickSecondary);
        }
        
        protected virtual void OnClickPrimary(bool pressed) {}
        
        protected virtual void OnClickSecondary(bool pressed) {}
    }
}