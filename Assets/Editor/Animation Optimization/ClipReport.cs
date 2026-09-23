namespace AnimOptimizer
{
    public sealed class ClipReport
    {
        public string AssetPath;
        public string ClipName;

        public int CurvesBefore;
        public int CurvesAfter;
        public int KeysBefore;
        public int KeysAfter;
        public long BytesBefore;
        public long BytesAfter;
        public readonly System.Collections.Generic.List<GroupDecision> Decisions = new System.Collections.Generic.List<GroupDecision>();

        public int DeletedGroups;
        public int CollapsedGroups;
        public int UntouchedGroups;
        public int DecimatedGroups;
        public int TangentsZeroed;

        public float MaxDelta;
        public bool Written;
        public string SkipReason;

        public int EffectiveCurvesAfter { get { return SkipReason != null ? CurvesBefore : CurvesAfter; } }

        public int EffectiveKeysAfter { get { return SkipReason != null ? KeysBefore : KeysAfter; } }

        public int KeysSaved { get { return KeysBefore - EffectiveKeysAfter; } }

        public void RecountFromText(LocatedClip located, System.Collections.Generic.Dictionary<LocatedBlock, GroupDecision> matched)
        {
            int curvesBefore = 0, keysBefore = 0, curvesAfter = 0, keysAfter = 0;

            foreach (var block in located.Blocks)
            {
                int components = block.Kind == CurveKind.Rotation ? 4 : 3;
                curvesBefore += components;
                keysBefore += block.Keyframes.Count * components;

                GroupDecision decision;
                if (!matched.TryGetValue(block, out decision))
                {
                    curvesAfter += components;
                    keysAfter += block.Keyframes.Count * components;
                    continue;
                }

                if (decision.Class == CurveClass.IdentityScale) continue;

                curvesAfter += components;
                keysAfter += components * (decision.Class == CurveClass.Constant && block.Keyframes.Count >= 2 ? 2 : block.Keyframes.Count - decision.DroppedKeyframes.Count);
            }

            CurvesBefore = curvesBefore;
            KeysBefore = keysBefore;
            CurvesAfter = curvesAfter;
            KeysAfter = keysAfter;
        }

        public long BytesSaved { get { return BytesBefore - BytesAfter; } }

        public bool Changed
        {
            get
            {
                return DeletedGroups > 0 || CollapsedGroups > 0 || DecimatedGroups > 0 || TangentsZeroed > 0;
            }
        }

        public override string ToString()
        {
            string status = Written ? "written" : (SkipReason ?? "dry run");
            string extra = "";
            if (DecimatedGroups > 0)
            {
                extra += string.Format(", {0:N0} curves decimated", DecimatedGroups);
            }
            if (TangentsZeroed > 0)
            {
                extra += string.Format(", {0:N0} tangents flattened", TangentsZeroed);
            }

            return string.Format("{0}: {1} -> {2} curves, {3} -> {4} keys ({5} saved){6}, delta {7:G4} [{8}]", ClipName, CurvesBefore, EffectiveCurvesAfter, KeysBefore, EffectiveKeysAfter, KeysSaved, extra, MaxDelta, status);
        }
    }
}
