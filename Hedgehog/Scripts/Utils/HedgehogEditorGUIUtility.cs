﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Hedgehog.Utils
{
    public class HedgehogEditorGUIUtility
    {
        // Source: http://answers.unity3d.com/questions/42996/how-to-create-layermask-field-in-a-custom-editorwi.html
        public static LayerMask LayerMaskField(string label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();
 
            for (int i = 0; i < 32; i++) {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "") {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++) {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++) {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }

        public static CollisionMode CollisionModeField(CollisionMode value)
        {
            var buttonStyle = new GUIStyle(EditorStyles.toolbarButton);

            var selectedButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
            selectedButtonStyle.normal = selectedButtonStyle.active;

            CollisionMode selected = value;
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Layers", value == CollisionMode.Layers ? selectedButtonStyle : buttonStyle))
            {
                selected = CollisionMode.Layers;
            } else if (GUILayout.Button("Tags", value == CollisionMode.Tags ? selectedButtonStyle : buttonStyle))
            {
                selected = CollisionMode.Tags;
            } else if (GUILayout.Button("Names", value == CollisionMode.Names ? selectedButtonStyle : buttonStyle))
            {
                selected = CollisionMode.Names;
            }

            EditorGUILayout.EndHorizontal();

            return selected;
        }

        /// <summary>
        /// There is no return value - any changes to this field in the editor are immediately applied to the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <param name="serializedObject"></param>
        /// <param name="elements"></param>
        public static void ReorderableListField<T>(string label, SerializedObject serializedObject, SerializedProperty elements)
        {
            var list = new ReorderableList(serializedObject, elements, true, true, false, false);

            list.drawHeaderCallback += rect => GUI.Label(rect, label);
            list.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;
                EditorGUI.PropertyField(rect,
                    list.serializedProperty.GetArrayElementAtIndex(index),
                    GUIContent.none);
            };

            // Issue: The remove button is always disabled for unknown reasons.
            // Workaround: Lets make our own!

            list.onAddCallback += reorderableList => ReorderableList.defaultBehaviours.DoAddButton(list);
            list.onRemoveCallback += reorderableList => elements.DeleteArrayElementAtIndex(list.count - 1);
            list.onCanRemoveCallback += reorderableList => list.count > 0;

            serializedObject.Update();

            list.DoLayoutList();
            
            var buttonStyle = EditorStyles.miniButton;
            var disabledButtonStyle = new GUIStyle(buttonStyle) {normal = buttonStyle.active};

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add", buttonStyle))
            {
                list.onAddCallback(list);
            }

            if (GUILayout.Button("Remove Last", list.onCanRemoveCallback(list) ? buttonStyle : disabledButtonStyle))
            {
                if(list.onCanRemoveCallback(list)) list.onRemoveCallback(list);
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
