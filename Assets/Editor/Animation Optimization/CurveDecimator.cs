using System.Collections.Generic;
using UnityEngine;

namespace AnimOptimizer
{
    public static class CurveDecimator
    {
        const float SafetyMargin = 0.9f;

        public static List<int> Decimate(CurveGroup group, float frameRate, OptimizerSettings settings)
        {
            if (settings == null || !settings.DecimateCurves) return new List<int>();

            if (group.Kind == CurveKind.Other) return new List<int>();

            if (group.Curves.Count == 0) return new List<int>();

            int count = group.Curves[0].length;
            if (count < 3) return new List<int>();

            if (!IsEligible(group, count)) return new List<int>();
            float epsilon = settings.DecimateEpsilon * SafetyMargin;
            float rate = Mathf.Max(1f, frameRate) * Mathf.Max(1, settings.VerifySamplesPerFrame);

            var dropped = new List<int>();
            var pending = new List<int>();
            int left = 0;

            for (int i = 1; i < count - 1; i++)
            {
                pending.Add(i);

                if (SegmentHolds(group, left, i + 1, pending, rate, epsilon))
                {
                    dropped.Add(i);
                }
                else
                {
                    pending.Clear();
                    left = i;
                }
            }

            return dropped;
        }

        static bool IsEligible(CurveGroup group, int count)
        {
            foreach (var curve in group.Curves)
            {
                if (curve.length != count) return false;

                for (int k = 0; k < curve.length; k++)
                {
                    var key = curve[k];
                    if (IsNotFinite(key.time) || IsNotFinite(key.value) || IsNotFinite(key.inTangent) || IsNotFinite(key.outTangent)) return false;
                }
            }

            return true;
        }

        static bool SegmentHolds(CurveGroup group, int left, int right, List<int> pending, float rate, float epsilon)
        {
            foreach (var curve in group.Curves)
            {
                float t0 = curve[left].time;
                float t1 = curve[right].time;
                float span = t1 - t0;
                if (span <= 0f) return false;

                var candidate = new AnimationCurve(new[] { curve[left], curve[right] });

                foreach (int k in pending)
                {
                    if (Mathf.Abs(candidate.Evaluate(curve[k].time) - curve[k].value) > epsilon) return false;
                }
                int first = Mathf.CeilToInt(t0 * rate);
                int last = Mathf.FloorToInt(t1 * rate);
                for (int i = first; i <= last; i++)
                {
                    float t = i / rate;
                    if (t <= t0 || t >= t1) continue;
                    if (Mathf.Abs(candidate.Evaluate(t) - curve.Evaluate(t)) > epsilon) return false;
                }
            }

            return true;
        }

        static bool IsNotFinite(float v)
        {
            return float.IsNaN(v) || float.IsInfinity(v);
        }
    }
}
