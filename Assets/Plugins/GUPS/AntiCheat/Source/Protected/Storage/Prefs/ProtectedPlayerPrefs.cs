// System
using System;
using System.Runtime.CompilerServices;

// Unity
using UnityEngine;
using UnityEngine.Internal;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Binary;
using GUPS.AntiCheat.Core.Hash;
using GUPS.AntiCheat.Core.Protected;
using GUPS.AntiCheat.Core.Storage;

// GUPS - AntiCheat
using GUPS.AntiCheat.Settings;

// Allow the internal classes to be accessed by the test assembly.
[assembly: InternalsVisibleTo("GUPS.AntiCheat.Tests")]

namespace GUPS.AntiCheat.Protected.Storage.Prefs
{
    /// <summary>
    /// Protected version of the Unity PlayerPrefs. Provides additional functionality for saving 
    /// and loading custom types with added security features such as encryption, hashing, and 
    /// ownership verification.
    /// </summary>
    /// <remarks>
    /// This class wraps around Unity's PlayerPrefs and extends it to support more complex data 
    /// types with encryption and integrity checks. It also ensures that only the device owner 
    /// can access certain saved data.
    /// </remarks>
    public sealed class ProtectedPlayerPrefs
    {
        #region Error Messages

        internal const String ERROR_TYPE = "The type of the value read does not match the requested type.";
        internal const String ERROR_INTEGRITY = "The integrity of the storage item is not valid.";
        internal const String ERROR_OWNER = "You are not the owner of the storage item.";

        #endregion

        #region Key

        /// <summary>
        /// Retrieves the processed key name. Depending on the global settings, the key may be hashed.
        /// </summary>
        /// <param name="_Key">The original key name.</param>
        /// <returns>The processed key name, potentially hashed.</returns>
        /// <remarks>
        /// If the global setting `PlayerPreferences_Hash_Key` is enabled, the key is hashed using SHA1.
        /// </remarks>
        private static String GetKeyName(String _Key)
        {
            // This is the key stored in the PlayerPrefs.
            String var_Key = _Key;

            // Check the global settings if the key should be hashed.
            if (GlobalSettings.PlayerPreferences_Hash_Key)
            {
                // Get the UTF8 bytes of the key.
                byte[] var_KeyBytes = System.Text.Encoding.UTF8.GetBytes(_Key);

                // Compute the hash of the key.
                byte[] var_Hash = HashHelper.ComputeHash(EHashAlgorithm.SHA1, var_KeyBytes);

                // Convert the hash to a base64 string.
                var_Key = System.Convert.ToBase64String(var_Hash);
            }

            return var_Key;
        }

        /// <summary>
        /// Checks if a given key exists in the PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key to check.</param>
        /// <returns>True if the key exists, false otherwise.</returns>
        public static bool HasKey(String _Key)
        {
            // This is the key stored in the PlayerPrefs.
            String var_Key = GetKeyName(_Key);

            // Verify if the key exists.
            return PlayerPrefs.HasKey(var_Key);
        }

        #endregion

        #region Set

        /// <summary>
        /// Saves a raw storage item to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the value is stored.</param>
        /// <param name="_Item">The storage item to store.</param>
        /// <remarks>
        /// Saves the raw storage item to PlayerPrefs. This method is used for debugging and testing purposes.
        /// </remarks>
        internal static void SetRaw(String _Key, PreferenceStorageItem _Item)
        {
            // This is the key stored in the PlayerPrefs.
            String var_Key = GetKeyName(_Key);

            // Create a writer.
            BinaryWriter var_Writer = new BinaryWriter();

            // Write the storage item.
            _Item.Write(var_Writer);

            // Get the binary data.
            byte[] var_ValueBytes = var_Writer.ToArray();

            // Check the global settings if the value should be encrypted.
            if (!String.IsNullOrEmpty(GlobalSettings.PlayerPreferences_Value_Encryption_Key))
            {
                // Get the UTF8 bytes of the encryption key.
                byte[] var_EncryptionKeyBytes = System.Text.Encoding.UTF8.GetBytes(GlobalSettings.PlayerPreferences_Value_Encryption_Key);

                // Iterate over the value bytes and encrypt them using the encryption key.
                for (int i = 0; i < var_ValueBytes.Length; i++)
                {
                    var_ValueBytes[i] ^= var_EncryptionKeyBytes[i % var_EncryptionKeyBytes.Length];
                }
            }

            // Convert the value bytes to a base64 string.
            String var_Value = System.Convert.ToBase64String(var_ValueBytes);

            // Set the value in the PlayerPrefs.
            PlayerPrefs.SetString(var_Key, var_Value);
        }

        /// <summary>
        /// Saves a protected value to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the value is stored.</param>
        /// <param name="_Value">The value to store, implementing IProtected.</param>
        /// <remarks>
        /// This method stores the value by first retrieving the actual value of the IProtected object.
        /// </remarks>
        public static void Set(String _Key, IProtected _Value)
        {
            Set(_Key, _Value.Value);
        }

        /// <summary>
        /// Saves an object to PlayerPrefs with optional encryption. Supports all types of <see cref="EStorageType"/>.
        /// </summary>
        /// <param name="_Key">The key under which the value is stored.</param>
        /// <param name="_Value">The value to store.</param>
        /// <remarks>
        /// If encryption is enabled in the global settings, the value is encrypted before being 
        /// stored. The value is serialized into a binary format and then converted to a base64 string.
        /// </remarks>
        public static void Set(String _Key, System.Object _Value)
        {
            // This is the key stored in the PlayerPrefs.
            String var_Key = GetKeyName(_Key);

            // Create a new preference storage item.
            PreferenceStorageItem var_Item = new PreferenceStorageItem(_Value);

            // Create a writer.
            BinaryWriter var_Writer = new BinaryWriter();

            // Write the storage item.
            var_Item.Write(var_Writer);

            // Get the binary data.
            byte[] var_ValueBytes = var_Writer.ToArray();

            // Check the global settings if the value should be encrypted.
            if(!String.IsNullOrEmpty(GlobalSettings.PlayerPreferences_Value_Encryption_Key))
            {
                // Get the UTF8 bytes of the encryption key.
                byte[] var_EncryptionKeyBytes = System.Text.Encoding.UTF8.GetBytes(GlobalSettings.PlayerPreferences_Value_Encryption_Key);

                // Iterate over the value bytes and encrypt them using the encryption key.
                for (int i = 0; i < var_ValueBytes.Length; i++)
                {
                    var_ValueBytes[i] ^= var_EncryptionKeyBytes[i % var_EncryptionKeyBytes.Length];
                }
            }

            // Convert the value bytes to a base64 string.
            String var_Value = System.Convert.ToBase64String(var_ValueBytes);

            // Set the value in the PlayerPrefs.
            PlayerPrefs.SetString(var_Key, var_Value);

            // Auto save if activated.
            if(AutoSave)
            {
                Save();
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// Retrieves the underlying raw storage item from PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the value is stored.</param>
        /// <returns>The raw storage item, or null if the key does not exist.</returns>
        /// <remarks>
        /// Only used for automated testing and debugging purposes. This method retrieves the raw
        /// <see cref="PreferenceStorageItem"/> from PlayerPrefs without verification.
        /// </remarks>
        internal static PreferenceStorageItem GetRaw(String _Key)
        {
            // This is the key stored in the PlayerPrefs.
            String var_Key = GetKeyName(_Key);

            // If the key does not exist return null.
            if (!PlayerPrefs.HasKey(var_Key))
            {
                return null;
            }

            // Get the value from the PlayerPrefs.
            String var_Value = PlayerPrefs.GetString(var_Key);

            // Get the value bytes from the base64 string.
            byte[] var_ValueBytes = System.Convert.FromBase64String(var_Value);

            // Check the global settings if the value should be decrypted.
            if (!String.IsNullOrEmpty(GlobalSettings.PlayerPreferences_Value_Encryption_Key))
            {
                // Get the UTF8 bytes of the encryption key.
                byte[] var_EncryptionKeyBytes = System.Text.Encoding.UTF8.GetBytes(GlobalSettings.PlayerPreferences_Value_Encryption_Key);

                // Iterate over the value bytes and decrypt them using the encryption key.
                for (int i = 0; i < var_ValueBytes.Length; i++)
                {
                    var_ValueBytes[i] ^= var_EncryptionKeyBytes[i % var_EncryptionKeyBytes.Length];
                }
            }

            // Create a reader.
            BinaryReader var_Reader = new BinaryReader(var_ValueBytes);

            // Create a new preference storage item.
            PreferenceStorageItem var_Item = new PreferenceStorageItem();

            // Read the storage item.
            var_Item.Read(var_Reader);

            return var_Item;
        }

        /// <summary>
        /// Retrieves a stored object from PlayerPrefs by type. Supports all types of <see cref="EStorageType"/>.
        /// </summary>
        /// <param name="_Type">The type of the stored object.</param>
        /// <param name="_Key">The key under which the object is stored.</param>
        /// <returns>The retrieved object, or the default value of the type if the key does not exist.</returns>
        /// <remarks>
        /// The method checks for encryption, integrity, and ownership based on global settings.
        /// </remarks>
        public static System.Object Get(Type _Type, String _Key)
        {
            // This is the key stored in the PlayerPrefs.
            String var_Key = GetKeyName(_Key);

            // If the key does not exist return the default for the type.
            if (!PlayerPrefs.HasKey(var_Key))
            {
                // Is a struct, create the default instance.
                if (_Type.IsValueType)
                {
                    return Activator.CreateInstance(_Type);
                }

                // Is a class, return null.
                return null;
            }

            // Get the value from the PlayerPrefs.
            String var_Value = PlayerPrefs.GetString(var_Key);

            // Get the value bytes from the base64 string.
            byte[] var_ValueBytes = System.Convert.FromBase64String(var_Value);

            // Check the global settings if the value should be decrypted.
            if (!String.IsNullOrEmpty(GlobalSettings.PlayerPreferences_Value_Encryption_Key))
            {
                // Get the UTF8 bytes of the encryption key.
                byte[] var_EncryptionKeyBytes = System.Text.Encoding.UTF8.GetBytes(GlobalSettings.PlayerPreferences_Value_Encryption_Key);

                // Iterate over the value bytes and decrypt them using the encryption key.
                for (int i = 0; i < var_ValueBytes.Length; i++)
                {
                    var_ValueBytes[i] ^= var_EncryptionKeyBytes[i % var_EncryptionKeyBytes.Length];
                }
            }

            // Create a reader.
            BinaryReader var_Reader = new BinaryReader(var_ValueBytes);

            // Create a new preference storage item.
            PreferenceStorageItem var_Item = new PreferenceStorageItem();

            // Read the storage item.
            var_Item.Read(var_Reader);

            // Check if the type of the value is correct.
            if(var_Item.Type != StorageHelper.GetStorageType(_Type))
            {
                throw new Exception(ERROR_TYPE);
            }

            // Check if should verify integrity.
            if(GlobalSettings.PlayerPreferences_Verify_Integrity)
            {
                // Verify the integrity of the storage item.
                if(!var_Item.VerifySignature())
                {
                    throw new Exception(ERROR_INTEGRITY);
                }
            }

            // Check if should verify owner.
            if(!GlobalSettings.PlayerPreferences_Allow_Read_Any_Owner)
            {
                // Check if the owner is correct.
                if(var_Item.Owner != UnityEngine.SystemInfo.deviceUniqueIdentifier)
                {
                    throw new Exception(ERROR_OWNER);
                }
            }

            // Return the value.
            return var_Item.Value;
        }

        /// <summary>
        /// Retrieves a stored object from PlayerPrefs by generic type. Supports all types of <see cref="EStorageType"/>.
        /// </summary>
        /// <typeparam name="T">The type of the stored object.</typeparam>
        /// <param name="_Key">The key under which the object is stored.</param>
        /// <returns>The retrieved object of type T, or the default value of T if the key does not exist.</returns>
        /// <remarks>
        /// This method internally calls the <see cref="Get(Type, String)"/> method but casts the result to the specified type.
        /// </remarks>
        public static T Get<T>(String _Key)
        {
            // Get the type of T.
            Type var_Type = typeof(T);

            // Get the value.
            return (T)Get(var_Type, _Key);
        }

        /// <summary>
        /// Retrieves a stored object from PlayerPrefs without specifying the type. Supports all types of <see cref="EStorageType"/>.
        /// </summary>
        /// <param name="_Key">The key under which the object is stored.</param>
        /// <returns>The retrieved object, or null if the key does not exist.</returns>
        /// <remarks>
        /// This method retrieves the stored object as a generic System.Object and handles any decryption or verification.
        /// </remarks>
        public static System.Object Get(String _Key)
        {
            // This is the key stored in the PlayerPrefs.
            String var_Key = GetKeyName(_Key);

            // If the key does not exist return null.
            if (!PlayerPrefs.HasKey(var_Key))
            {
                // Return null.
                return null;
            }

            // Get the value from the PlayerPrefs.
            String var_Value = PlayerPrefs.GetString(var_Key);

            // Get the value bytes from the base64 string.
            byte[] var_ValueBytes = System.Convert.FromBase64String(var_Value);

            // Check the global settings if the value should be decrypted.
            if (!String.IsNullOrEmpty(GlobalSettings.PlayerPreferences_Value_Encryption_Key))
            {
                // Get the UTF8 bytes of the encryption key.
                byte[] var_EncryptionKeyBytes = System.Text.Encoding.UTF8.GetBytes(GlobalSettings.PlayerPreferences_Value_Encryption_Key);

                // Iterate over the value bytes and decrypt them using the encryption key.
                for (int i = 0; i < var_ValueBytes.Length; i++)
                {
                    var_ValueBytes[i] ^= var_EncryptionKeyBytes[i % var_EncryptionKeyBytes.Length];
                }
            }

            // Create a reader.
            BinaryReader var_Reader = new BinaryReader(var_ValueBytes);

            // Create a new preference storage item.
            PreferenceStorageItem var_Item = new PreferenceStorageItem();

            // Read the storage item.
            var_Item.Read(var_Reader);

            // Check if should verify integrity.
            if (GlobalSettings.PlayerPreferences_Verify_Integrity)
            {
                // Verify the integrity of the storage item.
                if (!var_Item.VerifySignature())
                {
                    throw new Exception(ERROR_INTEGRITY);
                }
            }

            // Check if should verify owner.
            if (!GlobalSettings.PlayerPreferences_Allow_Read_Any_Owner)
            {
                // Check if the owner is correct.
                if (var_Item.Owner != UnityEngine.SystemInfo.deviceUniqueIdentifier)
                {
                    throw new Exception(ERROR_OWNER);
                }
            }

            // Return the value.
            return var_Item.Value;
        }

        #endregion

        #region Int

        /// <summary>
        /// Saves an integer value to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the integer is stored.</param>
        /// <param name="_Value">The integer value to store.</param>
        /// <remarks>
        /// This method wraps the <see cref="Set(String, System.Object)"/> method to store an integer value.
        /// </remarks>
        public static void SetInt(String _Key, int _Value)
        {
            Set(_Key, _Value);
        }

        /// <summary>
        /// Retrieves an integer value from PlayerPrefs, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the integer is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored integer value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the PlayerPrefs. If it does not, the default value is returned. 
        /// It safely handles the retrieval of integer values, considering encryption and integrity checks if enabled.
        /// </remarks>
        public static int GetInt(String _Key, [DefaultValue("0")] int _DefaultValue)
        {
            if(HasKey(_Key))
            {
                return Get<Int32>(_Key);
            }

            return PlayerPrefs.GetInt(_Key, _DefaultValue);
        }

        /// <summary>
        /// Retrieves an integer value from PlayerPrefs, or 0 if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the integer is stored.</param>
        /// <returns>The stored integer value, or 0 if the key does not exist.</returns>
        /// <remarks>
        /// This method is a shortcut for retrieving integers with a default value of 0.
        /// </remarks>
        public static int GetInt(String _Key)
        {
            return GetInt(_Key, 0);
        }

        #endregion

        #region Bool

        /// <summary>
        /// Saves a boolean value to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the boolean is stored.</param>
        /// <param name="_Value">The boolean value to store.</param>
        /// <remarks>
        /// This method wraps the <see cref="Set(String, System.Object)"/> method to store a boolean value.
        /// </remarks>
        public static void SetBool(String _Key, bool _Value)
        {
            Set(_Key, _Value);
        }

        /// <summary>
        /// Retrieves a boolean value from PlayerPrefs, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the boolean is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored boolean value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the PlayerPrefs. If it does not, the default value is returned.
        /// </remarks>
        public static bool GetBool(String _Key, [DefaultValue("false")] bool _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<bool>(_Key);
            }
            return _DefaultValue;
        }

        /// <summary>
        /// Retrieves a boolean value from PlayerPrefs, or false if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the boolean is stored.</param>
        /// <returns>The stored boolean value, or false if the key does not exist.</returns>
        /// <remarks>
        /// This method is a shortcut for retrieving booleans with a default value of false.
        /// </remarks>
        public static bool GetBool(String _Key)
        {
            return GetBool(_Key, false);
        }

        #endregion

        #region Float

        /// <summary>
        /// Saves a float value to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the float is stored.</param>
        /// <param name="_Value">The float value to store.</param>
        /// <remarks>
        /// This method wraps the <see cref="Set(String, System.Object)"/> method to store a float value.
        /// </remarks>
        public static void SetFloat(String _Key, float _Value)
        {
            Set(_Key, _Value);
        }

        /// <summary>
        /// Retrieves a float value from PlayerPrefs, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the float is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored float value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the PlayerPrefs. If it does not, the default value is returned.
        /// </remarks>
        public static float GetFloat(String _Key, [DefaultValue("0.0f")] float _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<float>(_Key);
            }
            return PlayerPrefs.GetFloat(_Key, _DefaultValue);
        }

        /// <summary>
        /// Retrieves a float value from PlayerPrefs, or 0.0f if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the float is stored.</param>
        /// <returns>The stored float value, or 0.0f if the key does not exist.</returns>
        /// <remarks>
        /// This method is a shortcut for retrieving floats with a default value of 0.0f.
        /// </remarks>
        public static float GetFloat(String _Key)
        {
            return GetFloat(_Key, 0.0f);
        }

        #endregion

        #region String

        /// <summary>
        /// Saves a string value to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the string is stored.</param>
        /// <param name="_Value">The string value to store.</
        /// <remarks>
        /// This method wraps the <see cref="Set(String, System.Object)"/> method to store a string value.
        /// </remarks>
        public static void SetString(String _Key, string _Value)
        {
            Set(_Key, _Value);
        }

        /// <summary>
        /// Retrieves a string value from PlayerPrefs, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the string is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored string value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the PlayerPrefs. If it does not, the default value is returned.
        /// </remarks>
        public static string GetString(String _Key, [DefaultValue("")] string _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<string>(_Key);
            }
            return PlayerPrefs.GetString(_Key, _DefaultValue);
        }

        /// <summary>
        /// Retrieves a string value from PlayerPrefs, or an empty string if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the string is stored.</param>
        /// <returns>The stored string value, or an empty string if the key does not exist.</returns>
        /// <remarks>
        /// This method is a shortcut for retrieving strings with a default value of empty string.
        /// </remarks>
        public static string GetString(String _Key)
        {
            return GetString(_Key, "");
        }
        #endregion

        #region Vector2

        /// <summary>
        /// Saves a Vector2 value to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the Vector2 is stored.</param>
        /// <param name="_Value">The Vector2 value to store.</param>
        /// <remarks>
        /// This method wraps the <see cref="Set(String, System.Object)"/> method to store a Vector2 value.
        /// </remarks>
        public static void SetVector2(String _Key, Vector2 _Value)
        {
            Set(_Key, _Value);
        }

        /// <summary>
        /// Retrieves a Vector2 value from PlayerPrefs, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Vector2 is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored Vector2 value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the PlayerPrefs. If it does not, the default value is returned.
        /// </remarks>
        public static Vector2 GetVector2(String _Key, Vector2 _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<Vector2>(_Key);
            }
            return _DefaultValue;
        }

        /// <summary>
        /// Retrieves a Vector2 value from PlayerPrefs, or Vector2.zero if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Vector2 is stored.</param>
        /// <returns>The stored Vector2 value, or Vector2.zero if the key does not exist.</returns>
        /// <remarks>
        /// This method is a shortcut for retrieving Vector2 with a default value of Vector2.zero.
        /// </remarks>
        public static Vector2 GetVector2(String _Key)
        {
            return GetVector2(_Key, Vector2.zero);
        }
        #endregion

        #region Vector3

        /// <summary>
        /// Saves a Vector3 value to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the Vector3 is stored.</param>
        /// <param name="_Value">The Vector3 value to store.</param>
        /// <remarks>
        /// This method wraps the <see cref="Set(String, System.Object)"/> method to store a Vector3 value.
        /// </remarks>
        public static void SetVector3(String _Key, Vector3 _Value)
        {
            Set(_Key, _Value);
        }

        /// <summary>
        /// Retrieves a Vector3 value from PlayerPrefs, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Vector3 is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored Vector3 value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the PlayerPrefs. If it does not, the default value is returned.
        /// </remarks>
        public static Vector3 GetVector3(String _Key, Vector3 _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<Vector3>(_Key);
            }
            return _DefaultValue;
        }

        /// <summary>
        /// Retrieves a Vector3 value from PlayerPrefs, or Vector3.zero if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Vector3 is stored.</param>
        /// <returns>The stored Vector3 value, or Vector3.zero if the key does not exist.</returns>
        /// <remarks>
        /// This method is a shortcut for retrieving Vector3 with a default value of Vector3.zero.
        /// </remarks>
        public static Vector3 GetVector3(String _Key)
        {
            return GetVector3(_Key, Vector3.zero);
        }

        #endregion

        #region Vector4

        /// <summary>
        /// Saves a Vector4 value to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the Vector4 is stored.</param>
        /// <param name="_Value">The Vector4 value to store.</param>
        /// <remarks>
        /// This method wraps the <see cref="Set(String, System.Object)"/> method to store a Vector4 value.
        /// </remarks>
        public static void SetVector4(String _Key, Vector4 _Value)
        {
            Set(_Key, _Value);
        }

        /// <summary>
        /// Retrieves a Vector4 value from PlayerPrefs, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Vector4 is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored Vector4 value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the PlayerPrefs. If it does not, the default value is returned.
        /// </remarks>
        public static Vector4 GetVector4(String _Key, Vector4 _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<Vector4>(_Key);
            }
            return _DefaultValue;
        }

        /// <summary>
        /// Retrieves a Vector4 value from PlayerPrefs, or Vector4.zero if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Vector4 is stored.</param>
        /// <returns>The stored Vector4 value, or Vector4.zero if the key does not exist.</returns>
        /// <remarks>
        /// This method is a shortcut for retrieving Vector4 with a default value of Vector4.zero.
        /// </remarks>
        public static Vector4 GetVector4(String _Key)
        {
            return GetVector4(_Key, Vector4.zero);
        }

        #endregion

        #region Quaternion

        /// <summary>
        /// Saves a Quaternion value to PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key under which the Quaternion is stored.</param>
        /// <param name="_Value">The Quaternion value to store.</param>
        /// <remarks>
        /// This method wraps the <see cref="Set(String, System.Object)"/> method to store a Quaternion value.
        /// </remarks>
        public static void SetQuaternion(String _Key, Quaternion _Value)
        {
            Set(_Key, _Value);
        }

        /// <summary>
        /// Retrieves a Quaternion value from PlayerPrefs, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Quaternion is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored Quaternion value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the PlayerPrefs. If it does not, the default value is returned.
        /// </remarks>
        public static Quaternion GetQuaternion(String _Key, Quaternion _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<Quaternion>(_Key);
            }
            return _DefaultValue;
        }

        /// <summary>
        /// Retrieves a Quaternion value from PlayerPrefs, or Quaternion.zero if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Quaternion is stored.</param>
        /// <returns>The stored Quaternion value, or Quaternion.zero if the key does not exist.</returns>
        /// <remarks>
        /// This method is a shortcut for retrieving Quaternion with a default value of Quaternion.zero.
        /// </remarks>
        public static Quaternion GetQuaternion(String _Key)
        {
            return GetQuaternion(_Key, Quaternion.identity);
        }

        #endregion

        #region Save

        /// <summary>
        /// Indicates whether automatic saving of preferences to disk is enabled.
        /// </summary>
        /// <remarks>
        /// When set to true, preferences are automatically saved to disk after each modification.
        /// </remarks>
        public static bool AutoSave = false;

        /// <summary>
        /// Writes all modified PlayerPrefs data to disk.
        /// </summary>
        /// <remarks>
        /// This method ensures that any modified preferences are persisted to the disk immediately. It wraps the Unity `PlayerPrefs.Save` method.
        /// </remarks>
        public static void Save()
        {
            PlayerPrefs.Save();
        }

        #endregion

        #region Delete

        /// <summary>
        /// Removes a key and its corresponding value from PlayerPrefs.
        /// </summary>
        /// <param name="_Key">The key that should be removed.</param>
        /// <remarks>
        /// This method removes the key-value pair from PlayerPrefs, ensuring that the key is processed (hashed if necessary) before removal.
        /// </remarks>
        public static void DeleteKey(String _Key)
        {
            // This is the key stored in the PlayerPrefs.
            String var_Key = GetKeyName(_Key);

            // Remove the key from the PlayerPrefs.
            PlayerPrefs.DeleteKey(var_Key);      
        }

        #endregion
    }
}
