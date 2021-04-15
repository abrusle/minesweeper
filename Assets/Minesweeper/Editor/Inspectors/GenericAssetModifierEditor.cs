using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using static UnityEditor.EditorGUILayout;

namespace Minesweeper.Editor.Inspectors
{
    internal abstract class GenericAssetModifierEditor<TAssetIn, TAssetOut> : CustomEditorBase
        where TAssetIn : Object
        where TAssetOut : Object
    {
        private struct Properties
        {
            public SerializedProperty
                assetPairs;
        }

        private Properties _properties;
        private List<int> _toRemove;

        private void OnEnable()
        {
            _properties = new Properties
            {
                assetPairs = serializedObject.FindProperty("assets")
            };
            _toRemove = new List<int>();
        }
        
        protected abstract void ModifyAsset(TAssetIn source, TAssetOut result);
        
        protected abstract TAssetOut CreateAssetPreModification(TAssetIn source);

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            LabelField(target.name, EditorStyles.whiteLargeLabel);
            Separator();
            
            LabelField("Assets", EditorStyles.whiteLargeLabel);
            Separator();
            int assetCount = _properties.assetPairs.arraySize;
            for (int i = 0; i < assetCount; i++)
            {
                var assetPairProp = _properties.assetPairs.GetArrayElementAtIndex(i);
                DrawAssetPair(i, assetPairProp);
                Separator();
            }
            
            HandleToRemoveProperties();

            using (new HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button(" Cleanup Sub Assets ")) CleanupSubAssets();
                
                if (GUILayout.Button("+", GUILayout.Width(50)))
                {
                    _properties.assetPairs.AddEmptyArrayElement();
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void HandleToRemoveProperties()
        {
            foreach (int pairIndex in _toRemove)
            {
                var pairProp = _properties.assetPairs.GetArrayElementAtIndex(pairIndex);
                var srcProp = pairProp.FindPropertyRelative("source");
                srcProp.objectReferenceValue = null;
                GenerateResultAssetForPair(pairProp);
                _properties.assetPairs.DeleteArrayElementAtIndex(pairIndex);
            }

            _toRemove.Clear();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAssetPair(int pairIndex, SerializedProperty prop)
        {
            using (new HorizontalScope())
            {
                using (new VerticalScope(EditorStyles.helpBox))
                {
                    var sourceProp = prop.FindPropertyRelative("source");
                    var initialValue = sourceProp.objectReferenceValue;
                    var newValue = ObjectField(new GUIContent("Source"), initialValue, typeof(TAssetIn), false);
                    if (initialValue != newValue)
                    {
                        sourceProp.objectReferenceValue = newValue;
                        OnAssetPairSourceChanged(pairIndex, prop);
                    }

                    using (new EditorGUI.DisabledGroupScope(true))
                        PropertyField(prop.FindPropertyRelative("result"));

                    if (GUILayout.Button("Regenerate"))
                        GenerateResultAssetForPair(prop);
                }

                if (GUILayout.Button("x", GUILayout.ExpandHeight(true), GUILayout.Width(20)))
                    RemovePairAtIndex(pairIndex);
            }
        }

        private void RemovePairAtIndex(int pairIndex)
        {
            _toRemove.Add(pairIndex);
        }

        private void OnAssetPairSourceChanged(int pairIndex, SerializedProperty pairProp)
        {
            GenerateResultAssetForPair(pairProp);
            OnValueChanged();
        }

        private void GenerateResultAssetForPair(SerializedProperty pairProp)
        {
            var srcProp = pairProp.FindPropertyRelative("source");
            var resProp = pairProp.FindPropertyRelative("result");

            TAssetIn srcAsset = srcProp.objectReferenceValue as TAssetIn;
            TAssetOut resAsset = resProp.objectReferenceValue as TAssetOut;

            if (srcProp.objectReferenceValue == null)
            {
                if (resProp.objectReferenceValue != null)
                {
                    DestroyImmediate(resProp.objectReferenceValue, true);
                    resProp.objectReferenceValue = null;
                    serializedObject.ApplyModifiedProperties();
                    Reimport();
                }
                
                return;
            }
            
            
            if (resProp.objectReferenceValue == null)
            {
                resAsset = CreateAssetPreModification(srcAsset);
                AssetDatabase.AddObjectToAsset(resAsset, target);
                resProp.objectReferenceValue = resAsset;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                Reimport();
            }
            
            ModifyAsset(srcAsset, resAsset);
            serializedObject.ApplyModifiedProperties();
        }

        private void OnValueChanged()
        {
            serializedObject.ApplyModifiedProperties();
        }

        private void CleanupSubAssets()
        {
            string path = AssetDatabase.GetAssetPath(target);

            var resAssets = new Object[_properties.assetPairs.arraySize];
            int assetCount = _properties.assetPairs.arraySize;
            for (int i = 0; i < assetCount; i++)
            {
                resAssets[i] = _properties.assetPairs
                    .GetArrayElementAtIndex(i)
                    .FindPropertyRelative("result")
                    .objectReferenceValue as TAssetOut;
            }

            bool hasDestroyed = false;
            var assetAtPath = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var asset in assetAtPath)
            {
                if (asset == target) continue;
                if (Array.IndexOf(resAssets, asset) == -1)
                {
                    DestroyImmediate(asset, true);
                    hasDestroyed = true;
                }
            }

            if (hasDestroyed)
            {
                serializedObject.ApplyModifiedProperties();
                Reimport();
            }
        }
    }
}