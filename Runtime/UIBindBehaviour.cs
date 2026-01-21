using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace Koto.UIAutoBind
{
    public abstract class UIBindBehaviour : MonoBehaviour
    {
        private UIBindMarker[] _binds;
        [Header("自动绑定脚本路径")]
        private string _pathPrefix = "Assets/";
        [SerializeField]
        private string _generatedScriptPath = $"UI/Generated";

        public string GeneratedScriptPath
            => string.IsNullOrEmpty(_generatedScriptPath)
                ? "Assets/UI/Generated"
                : _pathPrefix + _generatedScriptPath;

        protected virtual void Awake()
        {
            CacheBinds();
            GetUI(); // 子类 partial 中生成
        }

        void CacheBinds()
        {
            _binds = UIBindResolver.GetBinds(this);
        }

        protected T GetBind<T>(int index) where T : Component
        {
            if (_binds == null || index < 0 || index >= _binds.Length)
            {
                Debug.LogError($"[UIBase] Bind index invalid: {index}", this);
                return null;
            }

            var bind = _binds[index];
            var target = bind.Target;

            if (target == null)
            {
                Debug.LogError(
                    $"[UIBase] Bind index {index} ({bind.name}) has no target component",
                    bind
                );
                return null;
            }

            if (target is T t)
                return t;

            Debug.LogError(
                $"[UIBase] Component {typeof(T).Name} not found on bind index {index}, " +
                $"actual type is {target.GetType().Name}",
                bind
            );
            return null;
        }

        /// <summary>
        /// Editor 生成的 partial 会 override
        /// </summary>
        protected virtual void GetUI() { }
#if UNITY_EDITOR
        public IEnumerable<(string name, Object value)> GetRuntimeBindPreview()
        {
            var fields = GetType().GetFields(
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            foreach (var f in fields)
            {
                if (!System.Attribute.IsDefined(f, typeof(UIAutoBindAttribute)))
                    continue;

                if (!typeof(Object).IsAssignableFrom(f.FieldType))
                    continue;

                yield return (f.Name, f.GetValue(this) as Object);
            }
        }

#endif
    }
}