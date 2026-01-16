using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UIOverviewWindow : EditorWindow
{
    const float NAME_W = 160;
    const float TYPE_W = 180;
    const float LOAD_W = 80;
    const float LAYER_W = 80;
    const float BTN_W = 44;

    [MenuItem("工具/UI预览")]
    static void Open()
    {
        GetWindow<UIOverviewWindow>("UI预览");
    }

    Vector2 _scroll;
    List<UIInfo> _uis;

    void OnEnable()
    {
        Refresh();
    }
    void Refresh()
    {
        _uis = new List<UIInfo>();

        var guids = AssetDatabase.FindAssets("t:Prefab");

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                continue;

            var ui = prefab.GetComponent<UIBase>();
            if (ui == null)
                continue;

            _uis.Add(new UIInfo
            {
                Prefab = prefab,
                UIType = ui.GetType(),
                Path = path
            });
        }
    }
    void OnGUI()
    {
        DrawToolbar();

        EditorGUILayout.Space(4);
        DrawHeader();

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        foreach (var ui in _uis)
        {
            DrawRow(ui);
        }

        EditorGUILayout.EndScrollView();
    }

    void DrawHeader()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("名称", EditorStyles.boldLabel, GUILayout.Width(NAME_W));
            EditorGUILayout.LabelField("类", EditorStyles.boldLabel, GUILayout.Width(TYPE_W));
            GUILayout.FlexibleSpace();
        }
    }
    void DrawRow(UIInfo ui)
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            EditorGUILayout.LabelField(ui.Prefab.name, GUILayout.Width(NAME_W));
            EditorGUILayout.LabelField(ui.UIType.Name, EditorStyles.miniLabel, GUILayout.Width(TYPE_W));


            GUILayout.FlexibleSpace();

            if (GUILayout.Button("查看", EditorStyles.miniButtonLeft, GUILayout.Width(BTN_W)))
            {
                EditorGUIUtility.PingObject(ui.Prefab);
            }
            if (GUILayout.Button("打开", EditorStyles.miniButtonRight, GUILayout.Width(BTN_W)))
            {
                AssetDatabase.OpenAsset(ui.Prefab);
            }
        }
    }

    void DrawToolbar()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            GUI.backgroundColor = Color.green; // 或 Color.green / 高亮色
            if (GUILayout.Button(new GUIContent("↻ 刷新", "刷新 UI 列表"),
                EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                Refresh();
            }
            GUI.backgroundColor = Color.white; // 重置颜色

            GUILayout.FlexibleSpace();
            GUILayout.Label($"统计: {_uis?.Count ?? 0}", EditorStyles.miniLabel);
        }
    }


    class UIInfo
    {
        public GameObject Prefab;
        public System.Type UIType;
        public string Path;
    }

}
