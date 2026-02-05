namespace GUPS.AntiCheat.Core.Binary
{
    /// <summary>
    /// Represents the interface for reading data.
    /// </summary>
    internal interface IReadAble
    {
        /// <summary>
        /// Reads the data from the binary reader.
        /// </summary>
        /// <param name="_Reader">The binary reader to read the data from.</param>
        void Read(BinaryReader _Reader);
    }
}
