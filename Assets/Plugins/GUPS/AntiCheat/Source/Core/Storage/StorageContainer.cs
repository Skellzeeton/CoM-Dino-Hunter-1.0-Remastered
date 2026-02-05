// System
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Binary;
using GUPS.AntiCheat.Core.Hash;

// Allow the internal classes to be accessed by the test assembly.
[assembly: InternalsVisibleTo("GUPS.AntiCheat.Tests")]

namespace GUPS.AntiCheat.Core.Storage
{
    /// <summary>
    /// Represents a store able container that can hold data in a key value format. It allows to assign 
    /// an owner to the container and also sign it to verify its authenticity. Can be useful for save
    /// files and other data storage purposes.
    /// </summary>
    /// <remarks>
    /// This class provides a basic implementation of the IReadAble and IWriteAble interfaces.
    /// It is intended to be used as a container for multiple storage items. It can have an owner
    /// and also be signed to verify its authenticity.
    /// </remarks>
    public class StorageContainer : IReadAble, IWriteAble
    {
        internal const String ERROR_SIGNATURE = "Invalid signature.";
        internal const String ERROR_DUPLICATE = "An item with the same key has already been added.";
        internal const String ERROR_TYPE = "The type of the value read does not match the requested type.";

        /// <summary>
        /// A dictinary of storage items in the container.
        /// </summary>
        private Dictionary<String, StorageItem> items;

        /// <summary>
        /// Gets or sets the dictinary of storage items in the container.
        /// </summary>
        /// <remarks>
        /// This property is used to store multiple storage items.
        /// </remarks>
        internal Dictionary<String, StorageItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        /// <summary>
        /// The owner of the storage container.
        /// </summary>
        private String owner;

        /// <summary>
        /// Gets or sets the owner of the storage container.
        /// </summary>
        /// <remarks>
        /// This property is used to identify the owner of the storage container.
        /// </remarks>
        public String Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        /// <summary>
        /// Used for encryption and decryption of the storage container.
        /// </summary>
        private byte[] encryptionKey;

        /// <summary>
        /// The signature of the storage container.
        /// </summary>
        private String signature;

        /// <summary>
        /// Gets or sets the signature of the storage container.
        /// </summary>
        /// <remarks>
        /// This property is used to verify the authenticity of the storage container.
        /// </remarks>
        public String Signature
        {
            get { return signature; }
        }

        /// <summary>
        /// Instantiates a new <see cref="StorageContainer"/> object.
        /// </summary>
        /// <remarks>
        /// This is the default constructor that initializes an empty storage container.
        /// The owner and signature properties remain null until explicitly set.
        /// Use this constructor when the owner information is not immediately available
        /// or when working with temporary storage containers.
        /// </remarks>
        public StorageContainer()
        {
            this.items = new Dictionary<String, StorageItem>();
        }

        /// <summary>
        /// Instantiates a new <see cref="StorageContainer"/> object with the specified owner. You can for 
        /// example assign the owner to the player's name or <see cref="UnityEngine.SystemInfo.deviceUniqueIdentifier"/> 
        /// as unique device id.
        /// </summary>
        /// <param name="_Owner">The owner of the storage container.</param>
        /// <remarks>
        /// This constructor initializes a storage container with a specific owner.
        /// The owner parameter is useful for:
        /// - Tracking who created or has access to the container
        /// - Implementing ownership-based access control
        /// - Associating saved data with specific users or devices
        /// - Maintaining data persistence across gaming sessions
        /// 
        /// The signature property remains null until explicitly set or until
        /// the container is written to a binary format.
        /// </remarks>
        public StorageContainer(String _Owner)
        {
            this.items = new Dictionary<String, StorageItem>();
            this.owner = _Owner;
        }

        /// <summary>
        /// Instantiates a new <see cref="StorageContainer"/> object with the specified owner. You can for 
        /// example assign the owner to the player's name or <see cref="UnityEngine.SystemInfo.deviceUniqueIdentifier"/> 
        /// as unique device id. Also you can set a binary encryption key to encrypt and decrypt the storage container
        /// symmetrically.
        /// </summary>
        /// <param name="_Owner">The owner of the storage container.</param>
        /// <param name="_EncryptionKey">The encryption key to use for the storage container.</param>
        /// <remarks>
        /// This constructor initializes a storage container with a specific owner.
        /// The owner parameter is useful for:
        /// - Tracking who created or has access to the container
        /// - Implementing ownership-based access control
        /// - Associating saved data with specific users or devices
        /// - Maintaining data persistence across gaming sessions
        /// 
        /// The signature property remains null until explicitly set or until
        /// the container is written to a binary format.
        /// </remarks>
        public StorageContainer(String _Owner, byte[] _EncryptionKey)
        {
            this.items = new Dictionary<String, StorageItem>();
            this.owner = _Owner;
            this.encryptionKey = _EncryptionKey;
        }

        /// <summary>
        /// Returns if the storage container has an item with the specified key.
        /// </summary>
        /// <param name="_Key">The key of the item to check.</param>
        /// <returns>True if the storage container has an item with the specified key; otherwise false.</returns>
        /// <remarks>
        /// This method checks if the storage container internal items dictionary contains an item with the specified key.
        /// </remarks>
        public bool Has(String _Key)
        {
           return this.Items.ContainsKey(_Key);
        }

        /// <summary>
        /// Adds a new item to the storage container.
        /// </summary>
        /// <param name="_Key">The key of the item to add.</param>
        /// <param name="_Value">The value of the item to add.</param>
        /// <remarks>
        /// This method creates a new StorageItem instance with the specified value and adds it to the 
        /// Items dictionary.
        /// </remarks>
        public void Add(String _Key, Object _Value)
        {
            if(this.Items.ContainsKey(_Key))
            {
                throw new ArgumentException(ERROR_DUPLICATE);
            }

            this.Items.Add(_Key, new StorageItem(_Value));
        }

        /// <summary>
        /// Sets the value of an existing item in the storage container.
        /// </summary>
        /// <param name="_Key">The key of the item to set.</param>
        /// <param name="_Value">The new value of the item.</param>
        /// <remarks>
        /// This method creates a new StorageItem instance with the specified value and updates the existing 
        /// item in the Items dictionary.
        /// </remarks>
        public void Set(String _Key, Object _Value)
        {
            this.Items[_Key] = new StorageItem(_Value);
        }

        /// <summary>
        /// Removes an item from the storage container.
        /// </summary>
        /// <param name="_Key">The key of the item to remove.</param>
        /// <remarks>
        /// This method removes the item with the specified key from the Items dictionary.
        /// </remarks>
        public void Remove(String _Key)
        {
            this.Items.Remove(_Key);
        }

        /// <summary>
        /// Gets the value of an item in the storage container.
        /// </summary>
        /// <param name="_Key">The key of the item to get.</param>
        /// <returns>The value of the item.</returns>
        /// <remarks>
        /// This method returns the value of the item with the specified key from the Items dictionary.
        /// </remarks>
        public Object Get(String _Key)
        {
            return this.Items[_Key].Value;
        }

        /// <summary>
        /// Gets the value of an item in the storage container, cast to a specific type.
        /// </summary>
        /// <typeparam name="T">The type to cast the value to.</typeparam>
        /// <param name="_Key">The key of the item to get.</param>
        /// <returns>The value of the item, cast to the specified type.</returns>
        /// <remarks>
        /// This method returns the value of the item with the specified key from the Items dictionary, cast to the specified type.
        /// </remarks>
        public T Get<T>(String _Key)
        {
            // Get the item by key.
            StorageItem var_Item = this.Items[_Key];

            // Get the type of T.
            Type var_Type = typeof(T);

            // Check if the type of the value is correct.
            if (var_Item.Type != StorageHelper.GetStorageType(var_Type))
            {
                throw new Exception(ERROR_TYPE);
            }

            // Return the value casted to T.
            return (T)this.Items[_Key].Value;
        }

        /// <summary>
        /// Clears all items from the storage container.
        /// </summary>
        /// <remarks>
        /// This method clears all items from the Items dictionary.
        /// </remarks>
        public void Clear()
        {
            this.Items.Clear();
        }

        /// <summary>
        /// Reads the storage container from a stream.
        /// </summary>
        /// <param name="_Stream">The stream to read from.</param>
        /// <remarks>
        /// This method reads the binary data from the specified stream and then reads the storage container using the Read(byte[]) method.
        /// </remarks>
        public void Read(System.IO.Stream _Stream)
        {
            // Read the binary of the stream.
            byte[] var_Binary = new byte[_Stream.Length];

            // Read the stream.
            _Stream.Read(var_Binary, 0, var_Binary.Length);

            // Read the binary.
            this.Read(var_Binary);
        }

        /// <summary>
        /// Reads the storage container from a binary array.
        /// </summary>
        /// <param name="_Binary">The binary array to read from.</param>
        /// <remarks>
        /// This method creates a binary reader from the specified binary array and reads the storage container using the IReadAble interface.
        /// </remarks>
        public void Read(byte[] _Binary)
        {
            // Create a binary reader.
            BinaryReader var_Reader = new BinaryReader(_Binary);

            // Read the container.
            ((IReadAble)this).Read(var_Reader);
        }

        /// <summary>
        /// Reads the storage container from a binary stream.
        /// </summary>
        /// <param name="_Reader">The binary reader to read from.</param>
        /// <remarks>
        /// This method reads the Owner and Signature fields first, and then reads the list of storage items.
        /// </remarks>
        void IReadAble.Read(BinaryReader _Reader)
        {
            // Read the binary data.
            byte[] var_Binary = _Reader.ReadBytesAndSize();

            // Calculate the hash.
            byte[] var_Hash = HashHelper.ComputeHash(EHashAlgorithm.SHA256, var_Binary);

            // Read the signature.
            this.signature = _Reader.ReadString();

            // Compare the signature.
            if (this.signature != "H_" + HashHelper.ToHex(var_Hash, false, false))
            {
                throw new Exception("Invalid signature.");
            }

            // If the encryption key is set, decrypt the binary data.
            if (this.encryptionKey != null && this.encryptionKey.Length > 0)
            {
                // Iterate over the value bytes and decrypt them using the encryption key.
                for (int i = 0; i < this.encryptionKey.Length; i++)
                {
                    var_Binary[i] ^= this.encryptionKey[i % this.encryptionKey.Length];
                }
            }

            // Read the content of the container.
            BinaryReader var_Reader = new BinaryReader(var_Binary);

            // Read the owner.
            this.Owner = var_Reader.ReadString();

            // Read the number of items.
            int var_Count = var_Reader.ReadInt32();

            // Init the items dictionary.
            this.Items = new Dictionary<String, StorageItem>();

            // Read each item.
            for (int i = 0; i < var_Count; i++)
            {
                String var_Key = var_Reader.ReadString();
                StorageItem var_Item = new StorageItem();
                var_Item.Read(var_Reader);
                this.Items.Add(var_Key, var_Item);
            }
        }

        /// <summary>
        /// Writes the storage container to a stream.
        /// </summary>
        /// <param name="_Stream">The stream to write to.</param>
        /// <remarks>
        /// This method writes the binary data of the storage container to the specified stream.
        /// </remarks>
        public void Write(System.IO.Stream _Stream)
        {
            // Write the binary data.
            _Stream.Write(this.Write());
        }

        /// <summary>
        /// Writes the storage container to a binary array.
        /// </summary>
        /// <returns>The binary array representation of the storage container.</returns>
        /// <remarks>
        /// This method creates a binary writer and writes the storage container using the IWriteAble interface.
        /// </remarks>
        public byte[] Write()
        {
            // Create a binary writer.
            BinaryWriter var_Writer = new BinaryWriter();

            // Write the container.
            ((IWriteAble)this).Write(var_Writer);

            // Return the binary data.
            return var_Writer.AsArray();
        }

        /// <summary>
        /// Writes the storage container to a binary stream.
        /// </summary>
        /// <param name="_Writer">The binary writer to write to.</param>
        /// <remarks>
        /// This method writes the Owner and Signature fields first, and then writes the list of storage items.
        /// </remarks>
        void IWriteAble.Write(BinaryWriter _Writer)
        {
            // Write the content of the container to an own writer allowing to calculate a hash on the stored data.
            BinaryWriter var_Writer = new BinaryWriter();

            // Write the owner.
            var_Writer.Write(this.Owner);

            // Write the number of items.
            var_Writer.Write(this.Items.Count);

            // Write each item.
            foreach (var var_Pair in this.Items)
            {
                var_Writer.Write(var_Pair.Key);
                var_Pair.Value.Write(var_Writer);
            }

            // Get the binary data.
            byte[] var_Binary = var_Writer.ToArray();

            // If the encryption key is set, encrypt the binary data.
            if (this.encryptionKey != null && this.encryptionKey.Length > 0)
            {
                // Iterate over the value bytes and decrypt them using the encryption key.
                for (int i = 0; i < this.encryptionKey.Length; i++)
                {
                    var_Binary[i] ^= this.encryptionKey[i % this.encryptionKey.Length];
                }
            }

            // Compute the hash.
            byte[] var_Hash = HashHelper.ComputeHash(EHashAlgorithm.SHA256, var_Binary);

            // Write the binary data.
            _Writer.WriteBytesFull(var_Binary);

            // Calculate the signature.
            this.signature = "H_" + HashHelper.ToHex(var_Hash, false, false);

            // Write the calculated signature.
            _Writer.Write(this.signature);
        }
    }
}
