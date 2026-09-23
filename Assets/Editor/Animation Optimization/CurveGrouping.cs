using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimOptimizer
{
    public enum CurveKind { Position, Rotation, Scale, Other }

    public enum CurveClass { Animated, Constant, IdentityScale }

    public sealed class CurveGroup
    {
        public string Path;
        public string PropertyPrefix;
        public CurveKind Kind;
        public readonly List<EditorCurveBinding> Bindings = new List<EditorCurveBinding>();
        public readonly List<AnimationCurve> Curves = new List<AnimationCurve>();
        public CurveClass Class = CurveClass.Animated;

        public int KeyCount
        {
            get
            {
                int n = 0;
                foreach (var c in Curves) n += c.length;
                return n;
            }
        }
    }

    public static class CurveGrouping
    {
        public static string GetPrefix(string propertyName)
        {
            int dot = propertyName.LastIndexOf('.');
            return dot < 0 ? propertyName : propertyName.Substring(0, dot);
        }

        public static CurveKind GetKind(string prefix)
        {
            switch (prefix)
            {
                case "m_LocalPosition": return CurveKind.Position;
                case "m_LocalRotation": return CurveKind.Rotation;
                case "m_LocalScale": return CurveKind.Scale;
                default: return CurveKind.Other;
            }
        }

        public static List<CurveGroup> Group(AnimationClip clip)
        {
            var byKey = new Dictionary<string, CurveGroup>();
            var ordered = new List<CurveGroup>();

            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                if (curve == null) continue;

                string prefix = GetPrefix(binding.propertyName);
                string key = binding.path + "\0" + prefix;

                CurveGroup group;
                if (!byKey.TryGetValue(key, out group))
                {
                    group = new CurveGroup
                    {
                        Path = binding.path,
                        PropertyPrefix = prefix,
                        Kind = GetKind(prefix)
                    };
                    byKey[key] = group;
                    ordered.Add(group);
                }

                group.Bindings.Add(binding);
                group.Curves.Add(curve);
            }

            return ordered;
        }
    }
}
