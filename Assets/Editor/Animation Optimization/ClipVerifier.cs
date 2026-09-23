using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnimOptimizer
{
    public sealed class ClipSample
    {
        public float[] Times;
        public Dictionary<EditorCurveBinding, float[]> Values = new Dictionary<EditorCurveBinding, float[]>();
    }

    public static class ClipVerifier
    {
        public static ClipSample Sample(AnimationClip clip, OptimizerSettings settings)
        {
            float rate = clip.frameRate * Mathf.Max(1, settings.VerifySamplesPerFrame);
            int count = Mathf.Max(2, Mathf.CeilToInt(clip.length * rate) + 1);

            var sample = new ClipSample { Times = new float[count] };
            for (int i = 0; i < count; i++)
            {
                sample.Times[i] = i / rate;
            }
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                if (curve == null) continue;

                var values = new float[count];
                for (int i = 0; i < count; i++)
                {
                    values[i] = curve.Evaluate(sample.Times[i]);
                }
                sample.Values[binding] = values;
            }

            return sample;
        }

        public static float MaxDelta(ClipSample before, AnimationClip after, OptimizerSettings settings)
        {
            float worst = 0f;

            foreach (var pair in before.Values)
            {
                var binding = pair.Key;
                var oldValues = pair.Value;
                var curve = AnimationUtility.GetEditorCurve(after, binding);

                for (int i = 0; i < before.Times.Length; i++)
                {
                    float newValue = curve != null ? curve.Evaluate(before.Times[i]) : RestValueFor(binding, settings);

                    float delta = Mathf.Abs(newValue - oldValues[i]);
                    if (delta > worst) worst = delta;
                }
            }

            return worst;
        }

        static float RestValueFor(EditorCurveBinding binding, OptimizerSettings settings)
        {
            return CurveGrouping.GetKind(CurveGrouping.GetPrefix(binding.propertyName)) == CurveKind.Scale ? OptimizerSettings.AssumedRestScale : 0f;
        }
    }
}
