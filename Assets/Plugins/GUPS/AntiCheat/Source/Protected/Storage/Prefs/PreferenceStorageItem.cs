// System
using System;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Binary;
using GUPS.AntiCheat.Core.Hash;
using GUPS.AntiCheat.Core.Storage;

namespace GUPS.AntiCheat.Protected.Storage.Prefs
{
    /// <summary>
    /// Represents a player preference storage item with owner and signature information.
    /// </summary>
    /// <remarks>
    /// This class extends the StorageItem class and adds functionality for ownership and signature verification.
    /// </remarks>
    internal class PreferenceStorageItem : StorageItem
    {
        /// <summary>
        /// The owner of the player preference storage item.
        /// </summary>
        private String owner;

        /// <summary>
        /// Gets or sets the owner of the player preference storage item.
        /// </summary>
        /// <remarks>
        /// This property is used to identify the owner of the player preference storage item.
        /// </remarks>
        public String Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        /// <summary>
        /// The signature of the player preference storage item.
        /// </summary>
        private byte[] signature;

        /// <summary>
        /// Gets or sets the signature of the player preference storage item.
        /// </summary>
        /// <remarks>
        /// This property is used to verify the authenticity of the item.
        /// </remarks>
        public byte[] Signature
        {
            get { return signature; }
        }

        /// <summary>
        /// Instantiates a new <see cref="PreferenceStorageItem"/> object. With an empty value and the owner set to the device unique identifier.
        /// </summary>
        public PreferenceStorageItem()
            :base()
        {
            // More Infos: https://docs.unity3d.com/ScriptReference/SystemInfo-deviceUniqueIdentifier.html
            this.owner = UnityEngine.SystemInfo.deviceUniqueIdentifier;
        }

        /// <summary>
        /// Instantiates a new <see cref="PreferenceStorageItem"/> object with the specified value and the owner set to the device unique identifier.
        /// </summary>
        /// <param name="_Value">The value of the storage item.</param>
        public PreferenceStorageItem(Object _Value)
            :base(_Value)
        {
            // More Infos: https://docs.unity3d.com/ScriptReference/SystemInfo-deviceUniqueIdentifier.html
            this.owner = UnityEngine.SystemInfo.deviceUniqueIdentifier;
        }

        /// <summary>
        /// Computes the signature for the current state of the storage item.
        /// </summary>
        /// <remarks>
        /// This method creates a binary representation of the item's data and computes a SHA256 hash.
        /// </remarks>
        /// <returns>A byte array representing the computed signature.</returns>
        public byte[] ComputeSignature()
        {
            // Create a binary writer.
            BinaryWriter var_Writer = new BinaryWriter();

            // Write the storage type.
            var_Writer.Write((Byte)this.Type);

            // Write the value.
            StorageHelper.Write(var_Writer, this.Value);

            // Write the owner.
            var_Writer.Write(this.Owner);

            // Get the binary data.
            byte[] var_Binary = var_Writer.ToArray();

            // Return the hash of the binary data.
            return HashHelper.ComputeHash(EHashAlgorithm.SHA256, var_Binary);
        }

        /// <summary>
        /// Verifies the integrity of the storage item by comparing the stored signature with a newly computed one.
        /// </summary>
        /// <remarks>
        /// This method is used to ensure that the item's data has not been tampered with.
        /// </remarks>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        public bool VerifySignature()
        {
            // Compute the signature.
            byte[] var_Signature = this.ComputeSignature();

            // Compare the signatures.
            return HashHelper.CompareHashes(this.Signature, var_Signature);
        }

        /// <summary>
        /// Reads the storage item data from a binary stream.
        /// </summary>
        /// <remarks>
        /// This method overrides the base class implementation to include owner and signature information.
        /// </remarks>
        /// <param name="_Reader">The BinaryReader to read from.</param>
        public override void Read(BinaryReader _Reader)
        {
            // Read the binary data.
            byte[] var_Binary = _Reader.ReadBytesAndSize();

            // Read the signature.
            this.signature = _Reader.ReadBytesAndSize();

            // Read the content of the item.
            BinaryReader var_Reader = new BinaryReader(var_Binary);

            // Read the storage type.
            this.Type = (EStorageType)var_Reader.ReadByte();

            // Read the value.
            this.Value = StorageHelper.Read(var_Reader, this.Type);

            // Read the owner.
            this.Owner = var_Reader.ReadString();
        }

        /// <summary>
        /// Writes the storage item data to a binary stream.
        /// </summary>
        /// <remarks>
        /// This method overrides the base class implementation to include owner and signature information.
        /// It also computes and writes the signature for the item.
        /// </remarks>
        /// <param name="_Writer">The BinaryWriter to write to.</param>
        public override void Write(BinaryWriter _Writer)
        {
            // Write the content of the item to an own writer allowing to calculate a hash on the stored data.
            BinaryWriter var_Writer = new BinaryWriter();

            // Write the storage type.
            var_Writer.Write((Byte)this.Type);

            // Write the value.
            StorageHelper.Write(var_Writer, this.Value);

            // Write the owner.
            var_Writer.Write(this.Owner);

            // Get the binary data.
            byte[] var_Binary = var_Writer.ToArray();

            // Compute the hash.
            this.signature = HashHelper.ComputeHash(EHashAlgorithm.SHA256, var_Binary);

            // Write the binary data.
            _Writer.WriteBytesFull(var_Binary);

            // Write the calculated signature.
            _Writer.WriteBytesFull(this.signature);
        }
    }
}
