using UnityEngine;

namespace Minesweeper.Runtime.Dev
{
    public sealed class MonoLogger : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Global
        public void Print(string message)
        {
            Debug.Log(message);
        }

        // ReSharper disable once UnusedMember.Global
        public void Print(Object obj)
        {
            Debug.Log(obj);
        }
    }
}