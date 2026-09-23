using System.Collections.Generic;

namespace AnimOptimizer
{
    public sealed class GroupDecision
    {
        public string Path;
        public CurveKind Kind;
        public string PropertyPrefix;
        public CurveClass Class;

        public int KeyframeCount;

        public readonly List<int> DroppedKeyframes = new List<int>();

        public readonly List<float> KeyframeTimes = new List<float>();

        public bool ChangesText
        {
            get
            {
                return Class == CurveClass.IdentityScale || Class == CurveClass.Constant || DroppedKeyframes.Count > 0;
            }
        }
    }

    public static class ClipDecisionMatcher
    {
        public static bool TryMatch(LocatedClip located, IEnumerable<GroupDecision> decisions, out Dictionary<LocatedBlock, GroupDecision> matched, out string failureReason, out int ignored)
        {
            matched = new Dictionary<LocatedBlock, GroupDecision>();
            failureReason = null;
            ignored = 0;

            var index = new Dictionary<string, LocatedBlock>();
            foreach (var block in located.Blocks)
            {
                string key = Key(block.Path, block.Kind);
                if (index.ContainsKey(key))
                {
                    failureReason = "duplicate curve block for " + key;
                    return false;
                }
                index[key] = block;
            }

            foreach (var decision in decisions)
            {
                if (!decision.ChangesText) continue;
                LocatedBlock block;
                if (!index.TryGetValue(Key(decision.Path, decision.Kind), out block))
                {
                    ignored++;
                    continue;
                }

                if (decision.DroppedKeyframes.Count > 0 && !DropListFits(decision, block, located))
                {
                    decision.DroppedKeyframes.Clear();
                    ignored++;
                    if (!decision.ChangesText) continue;
                }

                matched[block] = decision;
            }

            return true;
        }

        static bool DropListFits(GroupDecision decision, LocatedBlock block, LocatedClip located)
        {
            if (block.Keyframes.Count != decision.KeyframeCount) return false;

            foreach (int i in decision.DroppedKeyframes)
            {
                if (i <= 0 || i >= block.Keyframes.Count - 1) return false;
            }
            if (decision.KeyframeTimes.Count != block.Keyframes.Count) return false;

            for (int i = 0; i < block.Keyframes.Count; i++)
            {
                float t;
                if (!TryReadTime(located, block.Keyframes[i], out t)) return false;

                float expected = decision.KeyframeTimes[i];
                float slack = 1e-4f * (System.Math.Abs(expected) + 1f);
                if (t < expected - slack || t > expected + slack) return false;
            }

            return true;
        }

        static bool TryReadTime(LocatedClip located, LocatedKeyframe key, out float time)
        {
            time = 0f;
            if (key.TimeLine < 0 || key.TimeLine >= located.Lines.Length) return false;

            string line = located.Lines[key.TimeLine];
            int colon = line.IndexOf(':');
            if (colon < 0) return false;

            return float.TryParse(line.Substring(colon + 1).Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out time);
        }

        static string Key(string path, CurveKind kind)
        {
            return kind + "|" + path;
        }
    }
}
