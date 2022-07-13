#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using ScenarioController;
using UnityEditor;
using UnityEditorInternal;

namespace ScenarioControllerEditor
{
    public class ScenarioTextEditorWindow : EditorWindow
    {
        SerializedProperty prop;
        SerializedObject so;

        Vector2 v;

        public static void Open(SerializedProperty prop)
        {
            ScenarioTextEditorWindow window = CreateInstance<ScenarioTextEditorWindow>();
            //Undoで表示バグる
            window.ShowAuxWindow();
            window.prop = prop;
            window.so = prop.serializedObject;
            window.titleContent = new GUIContent("ScenarioTextEditor");
        }

        void OnGUI()
        {
            Rect textRect = new Rect()
            {
                xMin = 2,
                width = position.width - 2,
                yMin = 2,
                height = position.height - 2
            };
            //なんか変だけどこの方法が一番バグが少ないのでこうする
            try
            {
                so.Update();
            }
            catch { }
            try
            {
                prop.FindPropertyRelative("text").stringValue = ScenarioControllerEditorUtility.ScrollableTextAreaInternal(textRect, prop.FindPropertyRelative("text").stringValue, ref v, EditorStyles.textArea);
            }
            catch { }
            try
            {
                so.ApplyModifiedProperties();
            }
            catch { }
        }
    }
}
#endif