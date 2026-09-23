using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AnimOptimizer
{
    public sealed class BatchResult
    {
        public readonly List<ClipReport> Reports = new List<ClipReport>();
        public int Skipped;
        public bool Cancelled;

        public int KeysBefore, KeysAfter, CurvesBefore, CurvesAfter;
        public long BytesBefore;
        public long BytesAfter;

        public int KeysSaved { get { return KeysBefore - KeysAfter; } }
        public long BytesSaved { get { return BytesBefore - BytesAfter; } }
    }

    public static class ClipBatchRunner
    {
        public static string[] FindClips(string[] searchFolders)
        {
            string[] guids = (searchFolders != null && searchFolders.Length > 0) ? AssetDatabase.FindAssets("t:AnimationClip", searchFolders) : AssetDatabase.FindAssets("t:AnimationClip");

            var paths = new List<string>(guids.Length);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".anim", StringComparison.OrdinalIgnoreCase))
                {
                    paths.Add(path);
                }
            }

            paths.Sort(StringComparer.Ordinal);
            return paths.ToArray();
        }

        public static BatchResult Run(string[] paths, OptimizerSettings settings, bool apply)
        {
            var result = new BatchResult();

            try
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    string path = paths[i];

                    if (EditorUtility.DisplayCancelableProgressBar(apply ? "Optimizing clips" : "Scanning clips", string.Format("({0}/{1}) {2}", i + 1, paths.Length, path), (i + 1) / (float)paths.Length))
                    {
                        result.Cancelled = true;
                        break;
                    }

                    var report = ProcessOne(path, settings, apply);
                    if (report == null) { result.Skipped++; continue; }

                    result.Reports.Add(report);
                    result.CurvesBefore += report.CurvesBefore;
                    result.CurvesAfter += report.EffectiveCurvesAfter;
                    result.KeysBefore += report.KeysBefore;
                    result.KeysAfter += report.EffectiveKeysAfter;
                    result.BytesBefore += report.BytesBefore;
                    result.BytesAfter += report.BytesAfter;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return result;
        }

        static ClipReport ProcessOne(string path, OptimizerSettings settings, bool apply)
        {
            AnimationClip clip;
            try
            {
                clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("[AnimOptimizer] Could not load {0}: {1}", path, e.Message));
                return null;
            }

            if (clip == null)
            {
                Debug.LogWarning("[AnimOptimizer] Not an AnimationClip: " + path);
                return null;
            }

            ClipReport report;
            try
            {
                report = ClipOptimizer.Decide(clip, settings);
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("[AnimOptimizer] Failed on {0}: {1}", path, e.Message));
                return null;
            }

            report.AssetPath = path;
            report.ClipName = clip.name;
            report.BytesBefore = SafeFileLength(path);
            report.BytesAfter = report.BytesBefore;

            if (!apply)
            {
                try
                {
                    ClipTextWriter.Preview(path, report, settings);
                }
                catch (Exception e)
                {
                    report.SkipReason = "preview threw: " + e.Message;
                }
                return report;
            }

            try
            {
                ClipTextWriter.TryWrite(path, clip, report, settings);
            }
            catch (Exception e)
            {
                report.SkipReason = "write threw: " + e.Message;
            }

            if (report.SkipReason != null)
            {
                Debug.LogWarning(string.Format("[AnimOptimizer] Skipped {0}: {1}", path, report.SkipReason));
            }
            return report;
        }

        static long SafeFileLength(string assetPath)
        {
            try
            {
                var info = new FileInfo(assetPath);
                return info.Exists ? info.Length : 0L;
            }
            catch { return 0L; }
        }
    }
}
