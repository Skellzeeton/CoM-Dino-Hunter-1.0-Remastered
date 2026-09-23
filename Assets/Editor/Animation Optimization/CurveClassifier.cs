using UnityEngine;

namespace AnimOptimizer
{
    public static class CurveClassifier
    {
        const float TangentPeak = 4f / 27f;

        public static bool IsConstant(AnimationCurve curve, float epsilon)
        {
            if (curve.length <= 1) return true;

            float first = curve[0].value;
            for (int i = 1; i < curve.length; i++)
            {
                if (Mathf.Abs(curve[i].value - first) > epsilon) return false;
            }
            for (int i = 0; i < curve.length - 1; i++)
            {
                float dt = curve[i + 1].time - curve[i].time;
                if (dt <= 0f) continue;

                float swing = dt * TangentPeak * (Mathf.Abs(curve[i].outTangent) + Mathf.Abs(curve[i + 1].inTangent));
                if (swing > epsilon) return false;
            }

            return true;
        }

        public static bool IsConstantAt(AnimationCurve curve, float value, float epsilon)
        {
            for (int i = 0; i < curve.length; i++)
            {
                if (Mathf.Abs(curve[i].value - value) > epsilon) return false;
            }
            return true;
        }

        public static CurveClass Classify(CurveGroup group, OptimizerSettings settings)
        {
            foreach (var curve in group.Curves)
            {
                if (curve.length == 0) return CurveClass.Animated;
            }
            foreach (var curve in group.Curves)
            {
                if (!IsConstant(curve, settings.Epsilon)) return CurveClass.Animated;
            }

            if (group.Kind == CurveKind.Scale)
            {
                bool identity = true;
                foreach (var curve in group.Curves)
                {
                    if (!IsConstantAt(curve, OptimizerSettings.AssumedRestScale, settings.Epsilon))
                    {
                        identity = false;
                        break;
                    }
                }
                if (identity) return CurveClass.IdentityScale;
            }

            return CurveClass.Constant;
        }
    }
}
