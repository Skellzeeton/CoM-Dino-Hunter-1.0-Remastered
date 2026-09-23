using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AnimOptimizer
{
    public static class ClipTextWriter
    {
        const string ScratchPath = "Assets/__AnimOptimizerScratch.anim";
        const string ScratchClipName = "__AnimOptimizerScratch";
        const string NamePrefix = "  m_Name: ";

        static string RenameTo(string text, string name)
        {
            int start = text.StartsWith(NamePrefix) ? 0 : text.IndexOf("\n" + NamePrefix);
            if (start < 0) return text;
            if (start > 0) start++;

            int end = text.IndexOf('\n', start);
            if (end < 0) end = text.Length;

            return text.Substring(0, start) + NamePrefix + name + text.Substring(end);
        }

        public static bool TryWrite(string assetPath, AnimationClip source, ClipReport report, OptimizerSettings settings)
        {
            ClipSample before;
            var probe = UnityEngine.Object.Instantiate(source);
            try
            {
                probe.legacy = false;
                before = ClipVerifier.Sample(probe, settings);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(probe);
            }

            string original, candidate;
            if (!TryBuildCandidate(assetPath, report, settings, out original, out candidate)) return false;

            if (candidate == original) return false;

            float delta;
            if (!TryVerify(candidate, before, settings, out delta))
            {
                report.SkipReason = "candidate failed to import";
                return false;
            }

            report.MaxDelta = delta;
            float tolerance = settings.EffectiveVerifyTolerance;
            if (delta > tolerance)
            {
                report.SkipReason = string.Format("delta {0:G4} exceeds tolerance {1:G4}", delta, tolerance);
                return false;
            }

            try
            {
                File.WriteAllText(assetPath, candidate);
            }
            catch (Exception e)
            {
                report.SkipReason = "write failed: " + e.Message;
                return false;
            }

            report.BytesAfter = SafeLength(assetPath);
            report.Written = true;

            try
            {
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
            catch (Exception e)
            {
                report.SkipReason = "written but reimport failed: " + e.Message;
            }

            return true;
        }

        public static void Preview(string assetPath, ClipReport report, OptimizerSettings settings)
        {
            string original, candidate;
            if (!TryBuildCandidate(assetPath, report, settings, out original, out candidate)) return;

            report.BytesAfter = System.Text.Encoding.UTF8.GetByteCount(candidate);
        }

        static bool TryBuildCandidate(string assetPath, ClipReport report, OptimizerSettings settings, out string original, out string candidate)
        {
            candidate = null;
            original = null;

            try
            {
                original = File.ReadAllText(assetPath);
            }
            catch (Exception e)
            {
                report.SkipReason = "read failed: " + e.Message;
                return false;
            }

            LocatedClip located;
            if (!ClipTextLocator.TryLocate(original, out located))
            {
                report.SkipReason = "unlocatable: " + located.FailureReason;
                return false;
            }

            System.Collections.Generic.Dictionary<LocatedBlock, GroupDecision> matched;
            string matchFailure;
            int ignored;
            if (!ClipDecisionMatcher.TryMatch(located, report.Decisions, out matched, out matchFailure, out ignored))
            {
                report.SkipReason = "unmatched: " + matchFailure;
                return false;
            }

            int tangentsZeroed;
            if (!ClipTextEditor.TryApply(located, matched, settings, out candidate, out tangentsZeroed))
            {
                report.SkipReason = "edit failed";
                return false;
            }

            report.TangentsZeroed = tangentsZeroed;
            report.RecountFromText(located, matched);
            return true;
        }

        static bool TryVerify(string candidate, ClipSample before, OptimizerSettings settings, out float delta)
        {
            delta = float.MaxValue;
            try
            {
                File.WriteAllText(ScratchPath, RenameTo(candidate, ScratchClipName));
                AssetDatabase.ImportAsset(ScratchPath, ImportAssetOptions.ForceSynchronousImport);

                var scratch = AssetDatabase.LoadAssetAtPath<AnimationClip>(ScratchPath);
                if (scratch == null) return false;

                bool wasLegacy = scratch.legacy;
                scratch.legacy = false;
                try { delta = ClipVerifier.MaxDelta(before, scratch, settings); }
                finally { scratch.legacy = wasLegacy; }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[AnimOptimizer] verification import failed: " + e.Message);
                return false;
            }
            finally
            {
                AssetDatabase.DeleteAsset(ScratchPath);
            }
        }

        static long SafeLength(string path)
        {
            try { var i = new FileInfo(path); return i.Exists ? i.Length : 0L; }
            catch { return 0L; }
        }
    }
}
