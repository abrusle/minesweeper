using UnityEngine;

namespace Minesweeper.Runtime.Animation
{
    public abstract class GenericAssetModifier<TAssetIn, TAssetOut> : ScriptableObject
        where TAssetIn : Object
        where TAssetOut : Object
    {
        [System.Serializable]
        public struct AssetPair
        {
            public TAssetIn source;
            public TAssetOut result;
        }

        [SerializeField] private AssetPair[] assets;

    }
}