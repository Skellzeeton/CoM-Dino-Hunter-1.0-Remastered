// System
using System;
using System.Runtime.CompilerServices;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Binary;

// Allow the internal classes to be accessed by the test assembly.
[assembly: InternalsVisibleTo("GUPS.AntiCheat.Tests")]

namespace GUPS.AntiCheat.Core.Storage
{
    /// <summary>
    /// Represents a storage item that can be read from and written to a binary stream.
    /// </summary>
    /// <remarks>
    /// This class provides a basic implementation of the IReadAble and IWriteAble interfaces.
    /// It is intended to be used as a base class for more specific storage item types.
    /// </remarks>
    internal class StorageItem : IReadAble, IWriteAble
    {
        /// <summary>
        /// Gets or sets the type of the storage item.
        /// </summary>
        /// <remarks>
        /// This field is used to determine how the Value field should be read and written.
        /// </remarks>
        public EStorageType Type;

        /// <summary>
        /// Gets or sets the value of the storage item.
        /// </summary>
        /// <remarks>
        /// This field can hold any type of object, but it is recommended to use only types that can be serialized.
        /// </remarks>
        public Object Value;

        /// <summary>
        /// Instantiates a new <see cref="StorageItem"/> object with an empty value.
        /// </summary>
        public StorageItem()
        {
        }

        /// <summary>
        /// Instantiates a new <see cref="StorageItem"/> object with the specified value.
        /// </summary>
        /// <param name="_Value">The value of the storage item.</param>
        public StorageItem(Object _Value)
        {
            // Get the type of the value.
            this.Type = StorageHelper.GetStorageType(_Value);

            // Set the value.
            this.Value = _Value;
        }

        /// <summary>
        /// Reads the storage item from a binary stream.
        /// </summary>
        /// <param name="_Reader">The binary reader to read from.</param>
        /// <remarks>
        /// This method reads the Type field first, and then uses the StorageHelper class to read the Value field.
        /// </remarks>
        public virtual void Read(BinaryReader _Reader)
        {
            // Read the storage type.
            this.Type = (EStorageType)_Reader.ReadByte();

            // Read the value.
            this.Value = StorageHelper.Read(_Reader, this.Type);
        }

        /// <summary>
        /// Writes the storage item to a binary stream.
        /// </summary>
        /// <param name="_Writer">The binary writer to write to.</param>
        /// <remarks>
        /// This method writes the Type field first, and then uses the StorageHelper class to write the Value field.
        /// </remarks>
        public virtual void Write(BinaryWriter _Writer)
        {
            // Write the storage type.
            _Writer.Write((Byte)this.Type);

            // Write the value.
            StorageHelper.Write(_Writer, this.Value);
        }
    }
}
