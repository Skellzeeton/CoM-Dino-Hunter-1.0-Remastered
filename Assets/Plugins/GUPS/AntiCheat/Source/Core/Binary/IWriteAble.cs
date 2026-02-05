namespace GUPS.AntiCheat.Core.Binary
{
    /// <summary>
    /// Represents the interface for reading data.
    /// </summary>
    internal interface IWriteAble
    {
        /// <summary>
        /// Writes the data to the binary writer.
        /// </summary>
        /// <param name="_Writer">The binary writer to write the data to.</param>
        void Write(BinaryWriter _Writer);
    }
}
