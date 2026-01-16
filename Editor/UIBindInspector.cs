using UnityEditor;
using UnityEngine;
namespace com.koto.UIFramework
{
    [CustomEditor(typeof(UIBind))]
    public class UIBindInspector : Editor
    {

        SerializedProperty indexProp, targetProp, manualProp;
        void OnEnable()
        {
            indexProp = serializedObject.FindProperty("index");
            targetProp = serializedObject.FindProperty("target");
            manualProp = serializedObject.FindProperty("manualOverride");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(indexProp, new GUIContent("Index"));

            EditorGUILayout.PropertyField(manualProp, new GUIContent("手动指定组件"));

            var bind = (UIBind)target;
            var auto = UIBindAutoResolver.Resolve(bind);
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("自动推断结果", auto, typeof(Component), true);

            if (!manualProp.boolValue)
            {
                if (auto != null && targetProp.objectReferenceValue != auto)
                {
                    Undo.RecordObject(bind, "自动绑定");
                    targetProp.objectReferenceValue = auto;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(targetProp, new GUIContent("绑定组件"));
            }

            serializedObject.ApplyModifiedProperties();

            DrawStatus(bind);
        }
        void DrawStatus(UIBind bind)
        {
            Color bg = bind.Target == null ? new Color(1f, 0.4f, 0.4f) : new Color(0.6f, 1f, 0.6f); // 红色 / 绿色
            string msg = bind.Target == null ? "未绑定任何组件（运行时会报错）" : $"已绑定：{bind.Target.GetType().Name}";

            // 自定义颜色背景 HelpBox
            var rect = GUILayoutUtility.GetRect(0, 40, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(rect, bg);
            GUIStyle style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.black }
            };
            EditorGUI.LabelField(rect, msg, style);
        }
    }
}