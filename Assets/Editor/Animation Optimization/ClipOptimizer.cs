using UnityEditor;
using UnityEngine;

namespace AnimOptimizer
{
    public static class ClipOptimizer
    {
        public static ClipReport Decide(AnimationClip source, OptimizerSettings settings)
        {
            var report = new ClipReport { ClipName = source.name };
            var clip = Object.Instantiate(source);

            try
            {
                clip.legacy = false;
                foreach (var group in CurveGrouping.Group(clip))
                {
                    report.CurvesBefore += group.Curves.Count;
                    report.KeysBefore += group.KeyCount;

                    var rawClass = CurveClassifier.Classify(group, settings);

                    CurveClass effectiveClass = rawClass;
                    if (rawClass == CurveClass.IdentityScale && !settings.DeleteIdentityScale)
                    {
                        effectiveClass = CurveClass.Animated;
                    }
                    else if (rawClass == CurveClass.Constant && !settings.CollapseConstant)
                    {
                        effectiveClass = CurveClass.Animated;
                    }
                    if (group.Kind == CurveKind.Other)
                    {
                        effectiveClass = CurveClass.Animated;
                    }
                    var decision = new GroupDecision
                    {
                        Path = group.Path,
                        Kind = group.Kind,
                        PropertyPrefix = group.PropertyPrefix,
                        Class = effectiveClass,
                        KeyframeCount = group.Curves.Count > 0 ? group.Curves[0].length : 0
                    };
                    if (group.Curves.Count > 0)
                    {
                        var first = group.Curves[0];
                        for (int k = 0; k < first.length; k++)
                        {
                            decision.KeyframeTimes.Add(first[k].time);
                        }
                    }
                    report.Decisions.Add(decision);

                    if (effectiveClass == CurveClass.IdentityScale)
                    {
                        report.DeletedGroups++;
                    }
                    else if (effectiveClass == CurveClass.Constant)
                    {
                        report.CollapsedGroups++;
                        report.CurvesAfter += group.Curves.Count;
                        foreach (var curve in group.Curves)
                        {
                            report.KeysAfter += curve.length >= 2 ? 2 : curve.length;
                        }
                    }
                    else
                    {
                        var drops = CurveDecimator.Decimate(group, clip.frameRate, settings);

                        if (drops.Count > 0)
                        {
                            decision.DroppedKeyframes.AddRange(drops);
                            report.DecimatedGroups++;
                        }
                        else
                        {
                            report.UntouchedGroups++;
                        }

                        report.CurvesAfter += group.Curves.Count;
                        foreach (var curve in group.Curves)
                        {
                            report.KeysAfter += curve.length - drops.Count;
                        }
                    }
                }
            }
            finally
            {
                Object.DestroyImmediate(clip);
            }

            return report;
        }
    }
}
