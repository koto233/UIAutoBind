using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
namespace Koto.UIAutoBind
{
    public static class UIBindResolver
    {
        // 常见 UI 组件优先级
        static readonly Type[] PreferredTypes =
        {
        typeof(Button),
        typeof(Image),
        typeof(Text),
        typeof(Toggle),
        typeof(Slider),
        typeof(ScrollRect),
        typeof(Dropdown),
    };

        public static Component Resolve(UIBindMarker bind)
        {
            // 1️⃣ 手动指定永远最高优先级
            if (bind.Target != null)
                return bind.Target;

            // 2️⃣ 常见 UI 组件
            foreach (var t in PreferredTypes)
            {
                var c = bind.GetComponent(t);
                if (c != null)
                    return c;
            }

            // 3️⃣ 排除型兜底
            return bind.GetComponents<Component>()
                .FirstOrDefault(c =>
                    !(c is Transform) &&
                    !(c is CanvasRenderer) &&
                    !(c is UIBindMarker));
        }

        public static Type[] GetCandidateTypes(GameObject go)
        {
            return go.GetComponents<Component>()
                .Where(c =>
                    !(c is Transform) &&
                    !(c is CanvasRenderer) &&
                    !(c is UIBindMarker))
                .Select(c => c.GetType())
                .Distinct()
                .ToArray();
        }
        public static HashSet<UIBindBehaviour> CollectReferencedUIs(UIBindBehaviour ui)
        {
            var result = new HashSet<UIBindBehaviour>();
            if (ui == null)
                return result;

            var type = ui.GetType();

            // 扫描实例字段（public + private）
            var fields = type.GetFields(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

            foreach (var field in fields)
            {
                // 只关心 UIBase 或其子类
                if (!typeof(UIBindBehaviour).IsAssignableFrom(field.FieldType))
                    continue;

                var value = field.GetValue(ui) as UIBindBehaviour;
                if (value == null)
                    continue;

                result.Add(value);
            }

            return result;
        }
    }
}