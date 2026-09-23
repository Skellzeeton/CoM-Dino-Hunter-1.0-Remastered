using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AnimOptimizer
{
    public static class ClipTextEditor
    {
        const string InSlopeField = "        inSlope:";
        const string OutSlopeField = "        outSlope:";
        const string ValueField = "        value:";

        public static bool TryApply(LocatedClip located, Dictionary<LocatedBlock, GroupDecision> decisions, OptimizerSettings settings, out string result, out int tangentsZeroed)
        {
            result = null;
            tangentsZeroed = 0;
            var lines = located.Lines;

            var byStart = new Dictionary<int, LocatedBlock>();
            foreach (var block in located.Blocks) byStart[block.StartLine] = block;

            var rewrites = BuildLineRewrites(located, decisions, settings, out tangentsZeroed);

            var sb = new StringBuilder(lines.Length * 40);

            for (int i = 0; i < lines.Length; i++)
            {
                LocatedBlock block;
                if (!byStart.TryGetValue(i, out block))
                {
                    AppendLine(sb, lines, i, located.LineEnding, rewrites);
                    continue;
                }

                GroupDecision decision;
                CurveClass cls = decisions.TryGetValue(block, out decision) ? decision.Class : CurveClass.Animated;

                if (block.Keyframes.Count == 0)
                {
                    cls = CurveClass.Animated;
                    decision = null;
                }

                if (cls == CurveClass.IdentityScale)
                {
                    i = block.EndLine;
                    continue;
                }

                if (cls == CurveClass.Constant && block.Keyframes.Count >= 2)
                {
                    AppendCollapsed(sb, located, block, rewrites);
                    i = block.EndLine;
                    continue;
                }

                if (decision != null && decision.DroppedKeyframes.Count > 0)
                {
                    AppendDecimated(sb, located, block, decision, rewrites);
                    i = block.EndLine;
                    continue;
                }

                for (; i <= block.EndLine; i++) AppendLine(sb, lines, i, located.LineEnding, rewrites);
                i = block.EndLine;
            }

            result = sb.ToString();
            return true;
        }

        static void AppendCollapsed(StringBuilder sb, LocatedClip located, LocatedBlock block, Dictionary<int, string> rewrites)
        {
            var lines = located.Lines;
            string nl = located.LineEnding;

            for (int i = block.StartLine; i < block.Keyframes[0].StartLine; i++) AppendLine(sb, lines, i, nl, rewrites);

            var first = block.Keyframes[0];
            var last = block.Keyframes[block.Keyframes.Count - 1];
            string slope = block.Kind == CurveKind.Rotation ? "{x: 0, y: 0, z: 0, w: 0}" : "{x: 0, y: 0, z: 0}";

            AppendKey(sb, lines, first, slope, nl, rewrites);
            AppendKey(sb, lines, last, slope, nl, rewrites);

            for (int i = block.PreInfinityLine; i <= block.EndLine; i++)
            {
                AppendLine(sb, lines, i, nl, rewrites);
            }
        }

        static void AppendDecimated(StringBuilder sb, LocatedClip located, LocatedBlock block, GroupDecision decision, Dictionary<int, string> rewrites)
        {
            var lines = located.Lines;
            string nl = located.LineEnding;
            var dropped = new HashSet<int>(decision.DroppedKeyframes);

            for (int i = block.StartLine; i < block.Keyframes[0].StartLine; i++)
            {
                AppendLine(sb, lines, i, nl, rewrites);
            }
            for (int k = 0; k < block.Keyframes.Count; k++)
            {
                if (dropped.Contains(k)) continue;

                var key = block.Keyframes[k];
                for (int i = key.StartLine; i <= key.EndLine; i++)
                {
                    AppendLine(sb, lines, i, nl, rewrites);
                }
            }

            for (int i = block.PreInfinityLine; i <= block.EndLine; i++)
            {
                AppendLine(sb, lines, i, nl, rewrites);
            }
        }

        static void AppendKey(StringBuilder sb, string[] lines, LocatedKeyframe key, string slope, string nl, Dictionary<int, string> rewrites)
        {
            for (int i = key.StartLine; i <= key.EndLine; i++)
            {
                if (i == key.InSlopeLine)
                {
                    sb.Append("        inSlope: ").Append(slope).Append(nl);
                    i = key.InSlopeEndLine;
                }
                else if (i == key.OutSlopeLine)
                {
                    sb.Append("        outSlope: ").Append(slope).Append(nl);
                    i = key.OutSlopeEndLine;
                }
                else if (i == key.TangentModeLine)
                {
                    sb.Append("        tangentMode: 0").Append(nl);
                }
                else
                {
                    string replacement;
                    if (rewrites != null && rewrites.TryGetValue(i, out replacement))
                    {
                        if (replacement == null) continue;
                        sb.Append(replacement).Append(nl);
                    }
                    else
                    {
                        sb.Append(lines[i]).Append(nl);
                    }
                }
            }
        }

        static Dictionary<int, string> BuildLineRewrites( LocatedClip located, Dictionary<LocatedBlock, GroupDecision> decisions, OptimizerSettings settings, out int tangentsZeroed)
        {
            tangentsZeroed = 0;
            var map = new Dictionary<int, string>();
            if (settings == null) return map;
            if (!settings.ZeroTinyTangents && !settings.RoundValues) return map;

            foreach (var block in located.Blocks)
            {
                if (block.Keyframes.Count == 0) continue;

                GroupDecision decision;
                CurveClass cls = decisions.TryGetValue(block, out decision) ? decision.Class : CurveClass.Animated;

                if (cls == CurveClass.IdentityScale) continue;
                if (cls == CurveClass.Constant && block.Keyframes.Count >= 2) continue;

                var dropped = decision != null && decision.DroppedKeyframes.Count > 0 ? new HashSet<int>(decision.DroppedKeyframes) : null;

                for (int k = 0; k < block.Keyframes.Count; k++)
                {
                    if (dropped != null && dropped.Contains(k)) continue;

                    var key = block.Keyframes[k];

                    bool flatIn = settings.ZeroTinyTangents && Flatten(map, located.Lines, key.InSlopeLine, key.InSlopeEndLine, InSlopeField, settings.TangentEpsilon);
                    bool flatOut = settings.ZeroTinyTangents && Flatten(map, located.Lines, key.OutSlopeLine, key.OutSlopeEndLine, OutSlopeField, settings.TangentEpsilon);

                    if (flatIn) tangentsZeroed++;
                    if (flatOut) tangentsZeroed++;

                    if (!settings.RoundValues) continue;

                    Round(map, located.Lines, key.ValueLine, ValueEnd(located.Lines, key), ValueField, settings.ValueDecimals);
                    if (!flatIn)
                    {
                        Round(map, located.Lines, key.InSlopeLine, key.InSlopeEndLine, InSlopeField, settings.ValueDecimals);
                    }
                    if (!flatOut)
                    {
                        Round(map, located.Lines, key.OutSlopeLine, key.OutSlopeEndLine, OutSlopeField, settings.ValueDecimals);
                    }
                }
            }

            return map;
        }

        static bool Flatten(Dictionary<int, string> map, string[] lines, int start, int end, string field, float epsilon)
        {
            string flattened;
            if (!TryFlattenSlope(lines, start, end, field, epsilon, out flattened)) return false;

            map[start] = flattened;
            for (int i = start + 1; i <= end; i++) map[i] = null;
            return true;
        }

        static int ValueEnd(string[] lines, LocatedKeyframe key)
        {
            if (key.ValueLine < 0) return -1;
            for (int i = key.ValueLine; i <= key.EndLine && i < lines.Length; i++)
            {
                if (lines[i].IndexOf('}') >= 0) return i;
            }
            return -1;
        }

        static void Round(Dictionary<int, string> map, string[] lines, int start, int end, string field, int decimals)
        {
            if (start < 0 || end < start || map.ContainsKey(start)) return;

            string rounded;
            if (!TryRoundVector(lines, start, end, field, decimals, out rounded)) return;

            map[start] = rounded;
            for (int i = start + 1; i <= end; i++) map[i] = null;
        }

        static bool TryRoundVector(string[] lines, int start, int end, string field, int decimals, out string result)
        {
            result = null;

            if (!lines[start].StartsWith(field)) return false;

            string text = lines[start];
            for (int i = start + 1; i <= end; i++) text += lines[i].Trim();

            int open = text.IndexOf('{');
            int close = text.LastIndexOf('}');
            if (open < 0 || close <= open) return false;

            string[] parts = text.Substring(open + 1, close - open - 1).Split(',');
            if (parts.Length == 0) return false;

            var sb = new StringBuilder(text.Length);
            sb.Append(field).Append(" {");

            for (int i = 0; i < parts.Length; i++)
            {
                int colon = parts[i].IndexOf(':');
                if (colon < 0) return false;

                float value;
                if (!float.TryParse(parts[i].Substring(colon + 1).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value)) return false;

                if (float.IsNaN(value) || float.IsInfinity(value)) return false;

                if (i > 0) sb.Append(", ");
                sb.Append(parts[i].Substring(0, colon).Trim()).Append(": ").Append(Shortest(value, decimals));
            }

            sb.Append('}');
            result = sb.ToString();

            if (end == start && result.Length >= lines[start].Length) { result = null; return false; }
            return true;
        }

        static string Shortest(float v, int decimals)
        {
            double r = System.Math.Round((double)v, decimals, System.MidpointRounding.AwayFromZero);
            if (r == 0.0) return "0";

            string s = r.ToString("F" + decimals, CultureInfo.InvariantCulture);
            if (s.IndexOf('.') >= 0) s = s.TrimEnd('0').TrimEnd('.');
            return s.Length == 0 || s == "-" || s == "-0" ? "0" : s;
        }

        static bool TryFlattenSlope(string[] lines, int start, int end, string field, float epsilon, out string result)
        {
            result = null;

            if (start < 0 || end < start || !lines[start].StartsWith(field)) return false;

            string text = lines[start];
            for (int i = start + 1; i <= end; i++) text += lines[i];

            int open = text.IndexOf('{');
            int close = text.LastIndexOf('}');
            if (open < 0 || close <= open) return false;

            string[] parts = text.Substring(open + 1, close - open - 1).Split(',');
            var names = new string[parts.Length];
            bool anyNonZero = false;

            for (int i = 0; i < parts.Length; i++)
            {
                int colon = parts[i].IndexOf(':');
                if (colon < 0) return false;

                names[i] = parts[i].Substring(0, colon).Trim();

                float value;
                if (!float.TryParse(parts[i].Substring(colon + 1).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value)) return false;

                if (float.IsNaN(value) || float.IsInfinity(value)) return false;
                if (value > epsilon || value < -epsilon) return false;
                if (value != 0f) anyNonZero = true;
            }

            if (names.Length == 0 || !anyNonZero) return false;

            var sb = new StringBuilder(text.Length);
            sb.Append(field).Append(" {");
            for (int i = 0; i < names.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(names[i]).Append(": 0");
            }
            sb.Append('}');

            result = sb.ToString();
            return true;
        }

        static void AppendLine(StringBuilder sb, string[] lines, int i, string nl, Dictionary<int, string> rewrites)
        {
            string replacement;
            if (rewrites != null && rewrites.TryGetValue(i, out replacement))
            {
                if (replacement == null) return;
                sb.Append(replacement);
            }
            else
            {
                sb.Append(lines[i]);
            }

            if (i < lines.Length - 1) sb.Append(nl);
        }
    }
}
