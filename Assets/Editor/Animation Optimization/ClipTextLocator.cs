using System.Collections.Generic;

namespace AnimOptimizer
{
    public struct LocatedKeyframe
    {
        public int TimeLine;
        public int ValueLine;
        public int InSlopeLine;
        public int InSlopeEndLine;
        public int OutSlopeLine;
        public int OutSlopeEndLine;
        public int TangentModeLine;
        public int StartLine;
        public int EndLine;
    }

    public sealed class LocatedBlock
    {
        public string Path;
        public CurveKind Kind;
        public int StartLine;
        public int EndLine;
        public int PreInfinityLine = -1;
        public readonly List<LocatedKeyframe> Keyframes = new List<LocatedKeyframe>();
    }

    public sealed class LocatedClip
    {
        public string[] Lines;
        public string LineEnding = "\n";
        public readonly List<LocatedBlock> Blocks = new List<LocatedBlock>();
        public string FailureReason;
    }

    public static class ClipTextLocator
    {
        const string SectionRotation = "  m_RotationCurves:";
        const string SectionPosition = "  m_PositionCurves:";
        const string SectionScale = "  m_ScaleCurves:";

        const string CurveItem = "  - curve:";
        const string EmptyCurveList = "      m_Curve: []";
        const string KeyStart = "      - time: ";
        const string NewSchemaKeyStart = "      - serializedVersion:";
        const string FieldIndent = "        ";
        const string TimePrefix = "        time: ";
        const string ValuePrefix = "        value: ";
        const string InSlopePrefix = "        inSlope:";
        const string OutSlopePrefix = "        outSlope:";
        const string TangentModePrefix = "        tangentMode:";
        const string PathPrefix = "    path:";
        const string PreInfinity = "      m_PreInfinity:";

        public static bool TryLocate(string text, out LocatedClip located)
        {
            located = new LocatedClip();

            if (text == null)
            {
                located.FailureReason = "null input";
                return false;
            }

            located.LineEnding = text.Contains("\r\n") ? "\r\n" : "\n";
            located.Lines = text.Replace("\r\n", "\n").Split('\n');

            var lines = located.Lines;
            CurveKind section = CurveKind.Other;
            bool inSection = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line == SectionRotation) { section = CurveKind.Rotation; inSection = true; continue; }
                if (line == SectionPosition) { section = CurveKind.Position; inSection = true; continue; }
                if (line == SectionScale) { section = CurveKind.Scale; inSection = true; continue; }

                if (inSection && line.Length > 2 && line[0] == ' ' && line[1] == ' ' && line[2] != '-' && line[2] != ' ')
                {
                    inSection = false;
                    continue;
                }

                if (!inSection || line != CurveItem) continue;

                var block = new LocatedBlock { Kind = section, StartLine = i };
                if (!TryReadBlock(lines, ref i, block, located)) return false;
                located.Blocks.Add(block);
            }

            return true;
        }

        static bool TryReadBlock(string[] lines, ref int i, LocatedBlock block, LocatedClip located)
        {
            int n = lines.Length;
            bool declaredEmpty = false;
            i++;

            for (; i < n; i++)
            {
                string line = lines[i];

                if (line == EmptyCurveList) { declaredEmpty = true; continue; }

                if (line.StartsWith(NewSchemaKeyStart) || line.StartsWith(KeyStart))
                {
                    if (!TryReadKeyframe(lines, ref i, block, located)) return false;
                    continue;
                }

                if (line.StartsWith(PreInfinity)) { block.PreInfinityLine = i; continue; }

                if (line.StartsWith(PathPrefix))
                {
                    int pathLine = i;

                    block.Path = line.Length > PathPrefix.Length ? line.Substring(PathPrefix.Length).TrimStart(' ') : "";
                    block.EndLine = pathLine;

                    for (int j = pathLine + 1; j < n && IsPathContinuation(lines[j]); j++)
                    {
                        block.Path += " " + lines[j].TrimStart(' ');
                        block.EndLine = j;
                    }
                    i = block.EndLine;

                    if (block.Keyframes.Count == 0 && !declaredEmpty)
                    {
                        located.FailureReason = "curve block with no keyframes at line " + (pathLine + 1);
                        return false;
                    }

                    if (block.PreInfinityLine < 0)
                    {
                        located.FailureReason = "curve block missing m_PreInfinity line before line " + (pathLine + 1);
                        return false;
                    }
                    return true;
                }

                if (line == CurveItem || (line.Length > 2 && line[0] == ' ' && line[1] == ' ' && line[2] != ' '))
                {
                    located.FailureReason = "curve block missing path line before line " + (i + 1);
                    return false;
                }
            }

            located.FailureReason = "unterminated curve block";
            return false;
        }

        static bool TryReadKeyframe(string[] lines, ref int i, LocatedBlock block, LocatedClip located)
        {
            int n = lines.Length;
            int start = i;
            bool newSchema = lines[i].StartsWith(NewSchemaKeyStart);

            var key = new LocatedKeyframe
            {
                StartLine = start,
                TimeLine = newSchema ? -1 : start,
                ValueLine = -1,
                InSlopeLine = -1,
                OutSlopeLine = -1,
                TangentModeLine = -1
            };

            int j = start + 1;
            for (; j < n && lines[j].StartsWith(FieldIndent); j++)
            {
                if (key.TimeLine < 0 && lines[j].StartsWith(TimePrefix)) key.TimeLine = j;
                else if (key.ValueLine < 0 && lines[j].StartsWith(ValuePrefix)) key.ValueLine = j;
                else if (key.InSlopeLine < 0 && lines[j].StartsWith(InSlopePrefix))
                {
                    key.InSlopeLine = j;
                    key.InSlopeEndLine = FlowEnd(lines, j, n);
                    if (key.InSlopeEndLine < 0)
                    {
                        located.FailureReason = "unterminated inSlope at line " + (j + 1);
                        return false;
                    }
                    j = key.InSlopeEndLine;
                }
                else if (key.OutSlopeLine < 0 && lines[j].StartsWith(OutSlopePrefix))
                {
                    key.OutSlopeLine = j;
                    key.OutSlopeEndLine = FlowEnd(lines, j, n);
                    if (key.OutSlopeEndLine < 0)
                    {
                        located.FailureReason = "unterminated outSlope at line " + (j + 1);
                        return false;
                    }
                    j = key.OutSlopeEndLine;
                }
                else if (key.TangentModeLine < 0 && lines[j].StartsWith(TangentModePrefix)) key.TangentModeLine = j;
            }

            if (key.TimeLine < 0)
            {
                located.FailureReason = "keyframe without time at line " + (start + 1);
                return false;
            }

            if (key.ValueLine < 0)
            {
                located.FailureReason = "keyframe without value at line " + (start + 1);
                return false;
            }

            if (key.InSlopeLine < 0 || key.OutSlopeLine < 0 || key.TangentModeLine < 0)
            {
                located.FailureReason = "keyframe missing a tangent field at line " + (start + 1);
                return false;
            }

            key.EndLine = j - 1;
            block.Keyframes.Add(key);
            i = j - 1;
            return true;
        }

        static int FlowEnd(string[] lines, int start, int n)
        {
            if (lines[start].IndexOf('{') < 0) return start;

            for (int j = start; j < n; j++)
                if (lines[j].IndexOf('}') >= 0) return j;

            return -1;
        }

        static bool IsPathContinuation(string line)
        {
            if (line.Length <= 6 || !line.StartsWith("      ")) return false;
            if (line[6] == ' ' || line[6] == '-') return false;
            return line.IndexOf(": ") < 0 && !line.EndsWith(":");
        }
    }
}
