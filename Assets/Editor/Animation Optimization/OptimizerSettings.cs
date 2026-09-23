namespace AnimOptimizer
{
    public sealed class OptimizerSettings
    {
        public float Epsilon = 1e-3f;
        public float TangentEpsilon = 1e-5f;

        public bool DeleteIdentityScale = true;
        public bool CollapseConstant = true;
        public bool ZeroTinyTangents = true;

        public bool RoundValues = true;

        public int ValueDecimals = 5;

        public float RoundingError
        {
            get
            {
                return RoundValues ? (float)(0.5 * System.Math.Pow(10.0, -ValueDecimals)) : 0f;
            }
        }

        public bool DecimateCurves = true;

        public float DecimateEpsilon = 1e-3f;

        public int VerifySamplesPerFrame = 4;

        public float VerifyTolerance = 1e-3f;

        public float EffectiveVerifyTolerance
        {
            get
            {
                float floor = Epsilon;
                if (DecimateCurves && DecimateEpsilon > floor) floor = DecimateEpsilon;

                floor += RoundingError;

                return VerifyTolerance > floor ? VerifyTolerance : floor;
            }
        }

        public const float AssumedRestScale = 1f;

        public OptimizerSettings Clone()
        {
            return new OptimizerSettings
            {
                Epsilon = Epsilon,
                TangentEpsilon = TangentEpsilon,
                DeleteIdentityScale = DeleteIdentityScale,
                CollapseConstant = CollapseConstant,
                ZeroTinyTangents = ZeroTinyTangents,
                DecimateCurves = DecimateCurves,
                DecimateEpsilon = DecimateEpsilon,
                RoundValues = RoundValues,
                ValueDecimals = ValueDecimals,
                VerifySamplesPerFrame = VerifySamplesPerFrame,
                VerifyTolerance = VerifyTolerance,
            };
        }
    }
}
