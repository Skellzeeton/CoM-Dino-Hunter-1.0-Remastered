// System
using System;
using System.IO;
using System.Runtime.CompilerServices;

// Unity
using UnityEngine;
using UnityEngine.Internal;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Hash;
using GUPS.AntiCheat.Core.Protected;
using GUPS.AntiCheat.Core.Storage;

// GUPS - AntiCheat
using GUPS.AntiCheat.Settings;
using System.Threading.Tasks;

// Allow the internal classes to be accessed by the test assembly.
[assembly: InternalsVisibleTo("GUPS.AntiCheat.Tests")]

namespace GUPS.AntiCheat.Protected.Storage.Prefs
{
    /// <summary>
    /// Provides a thread-safe, file-based implementation of protected player preferences.
    /// </summary>
    /// <remarks>
    /// This class allows secure storage and retrieval of player preferences using a file-based system. 
    /// It supports various data types, optional encryption, and automatic saving. 
    /// The implementation ensures thread safety through locking mechanisms and offers both synchronous 
    /// and asynchronous methods for interacting with the stored data.
    /// </remarks>
    public static class ProtectedFileBasedPlayerPrefs
    {
        #region Error Messages

        internal const String ERROR_NO_STORAGE = "The storage was not loaded yet, nothing to save.";
        internal const String ERROR_OWNER = "You are not the owner of the storage item.";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the custom file path for storing player preferences.
        /// Default is: Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + "playerprefs.dat"
        /// </summary>
        public static String FilePath { get; set; } = Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + "playerprefs.dat";

        /// <summary>
        /// Lock object for ensuring thread safety.
        /// </summary>
        private static object lockHandle = new object();

        /// <summary>
        /// The locally loaded storage container.
        /// </summary>
        private static StorageContainer storage = null;

        /// <summary>
        /// Gets or sets a value indicating whether changes are saved automatically. Default is true.
        /// </summary>
        public static bool AutoSave { get; set; } = true;

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
            // This is the key stored in the Storage.
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
        /// Checks if a specified key exists in the storage file.
        /// </summary>
        /// <param name="_Key">The key to check for existence.</param>
        /// <returns>True if the key exists, otherwise false.</returns>
        /// <remarks>
        /// This method ensures that the storage is loaded before checking for the key's existence. 
        /// The key is processed (hashed if necessary) before the check is performed. 
        /// Thread safety and data integrity are ensured during the operation.
        /// </remarks>
        public static bool HasKey(String _Key)
        {
            // Load the Storage if not already loaded.
            Load();

            // This is the key stored in the Storage.
            String var_Key = GetKeyName(_Key);

            // Verify if the key exists.
            return storage.Has(var_Key);
        }

        /// <summary>
        /// Asynchronously checks if a specified key exists in the storage file.
        /// </summary>
        /// <param name="_Key">The key to check for existence.</param>
        /// <returns>A task that represents the asynchronous operation. The result is true if the key exists, otherwise false.</returns>
        /// <remarks>
        /// This method performs the same functionality as <see cref="HasKey(string)"/> but executes it asynchronously 
        /// to avoid blocking the main thread. It ensures that the storage is loaded before performing the check.
        /// </remarks>
        public static Task<bool> HasKeyAsync(String _Key)
        {
            return Task.Run(() =>
            {
                return HasKey(_Key);
            });
        }

        #endregion

        #region Set

        /// <summary>
        /// Saves a protected value to the Storage File.
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
        /// Saves an object to the Storage File with optional encryption. Supports all types of <see cref="EStorageType"/>.
        /// </summary>
        /// <param name="_Key">The key under which the value is stored.</param>
        /// <param name="_Value">The value to store.</param>
        /// <remarks>
        /// If encryption is enabled in the global settings, the value is encrypted before being 
        /// stored. The value is serialized into a binary format and then converted to a base64 string.
        /// </remarks>
        public static void Set(String _Key, System.Object _Value)
        {
            // Load the Storage if not already loaded.
            Load();

            // This is the key stored in the Storage.
            String var_Key = GetKeyName(_Key);

            // Check if the storage already contains the key.
            if (storage.Has(var_Key))
            {
                // Overwrite the existing value.
                storage.Set(var_Key, _Value);
            }
            else
            {
                // Add the new value.
                storage.Add(var_Key, _Value);
            }

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// Retrieves a stored object from the Storage File by type. Supports all types of <see cref="EStorageType"/>.
        /// </summary>
        /// <param name="_Type">The type of the stored object.</param>
        /// <param name="_Key">The key under which the object is stored.</param>
        /// <returns>The retrieved object, or the default value of the type if the key does not exist.</returns>
        /// <remarks>
        /// The method checks for encryption, integrity, and ownership based on global settings.
        /// </remarks>
        public static System.Object Get(Type _Type, String _Key)
        {
            // Load the Storage if not already loaded.
            Load();

            // This is the key stored in the Storage.
            String var_Key = GetKeyName(_Key);

            // If the key does not exist return the default for the type.
            if (!storage.Has(var_Key))
            {
                // Is a struct, create the default instance.
                if (_Type.IsValueType)
                {
                    return Activator.CreateInstance(_Type);
                }

                // Is a class, return null.
                return null;
            }

            // Get the storage data.
            System.Object var_Value = storage.Get(var_Key);

            // Return the value.
            return var_Value;
        }

        /// <summary>
        /// Retrieves a stored object from the Storage File by generic type. Supports all types of <see cref="EStorageType"/>.
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
        /// Retrieves a stored object from the Storage File without specifying the type. Supports all types of <see cref="EStorageType"/>.
        /// </summary>
        /// <param name="_Key">The key under which the object is stored.</param>
        /// <returns>The retrieved object, or null if the key does not exist.</returns>
        /// <remarks>
        /// This method retrieves the stored object as a generic System.Object and handles any decryption or verification.
        /// </remarks>
        public static System.Object Get(String _Key)
        {
            // Load the Storage if not already loaded.
            Load();

            // This is the key stored in the Storage.
            String var_Key = GetKeyName(_Key);

            // If the key does not exist return the default for the type.
            if (!storage.Has(var_Key))
            {
                // Return null.
                return null;
            }

            // Get the storage data.
            System.Object var_Value = storage.Get(var_Key);

            // Return the value.
            return var_Value;
        }

        #endregion

        #region Int

        /// <summary>
        /// Saves an integer value to the Storage File.
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
        /// Retrieves an integer value from the Storage File, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the integer is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored integer value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the the Storage File. If it does not, the default value is returned. 
        /// It safely handles the retrieval of integer values, considering encryption and integrity checks if enabled.
        /// </remarks>
        public static int GetInt(String _Key, [DefaultValue("0")] int _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<Int32>(_Key);
            }
            return _DefaultValue;
        }

        /// <summary>
        /// Retrieves an integer value from the Storage File, or 0 if the key does not exist.
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
        /// Saves a boolean value to the Storage File.
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
        /// Retrieves a boolean value from the Storage File, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the boolean is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored boolean value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the the Storage File. If it does not, the default value is returned.
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
        /// Retrieves a boolean value from the Storage File, or false if the key does not exist.
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
        /// Saves a float value to the Storage File.
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
        /// Retrieves a float value from the Storage File, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the float is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored float value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the the Storage File. If it does not, the default value is returned.
        /// </remarks>
        public static float GetFloat(String _Key, [DefaultValue("0.0f")] float _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<float>(_Key);
            }
            return _DefaultValue;
        }

        /// <summary>
        /// Retrieves a float value from the Storage File, or 0.0f if the key does not exist.
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
        /// Saves a string value to the Storage File.
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
        /// Retrieves a string value from the Storage File, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the string is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored string value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the the Storage File. If it does not, the default value is returned.
        /// </remarks>
        public static string GetString(String _Key, [DefaultValue("")] string _DefaultValue)
        {
            if (HasKey(_Key))
            {
                return Get<string>(_Key);
            }
            return _DefaultValue;
        }

        /// <summary>
        /// Retrieves a string value from the Storage File, or an empty string if the key does not exist.
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
        /// Saves a Vector2 value to the Storage File.
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
        /// Retrieves a Vector2 value from the Storage File, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Vector2 is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored Vector2 value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the the Storage File. If it does not, the default value is returned.
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
        /// Retrieves a Vector2 value from the Storage File, or Vector2.zero if the key does not exist.
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
        /// Saves a Vector3 value to the Storage File.
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
        /// Retrieves a Vector3 value from the Storage File, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Vector3 is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored Vector3 value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the the Storage File. If it does not, the default value is returned.
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
        /// Retrieves a Vector3 value from the Storage File, or Vector3.zero if the key does not exist.
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
        /// Saves a Vector4 value to the Storage File.
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
        /// Retrieves a Vector4 value from the Storage File, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Vector4 is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored Vector4 value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the the Storage File. If it does not, the default value is returned.
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
        /// Retrieves a Vector4 value from the Storage File, or Vector4.zero if the key does not exist.
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
        /// Saves a Quaternion value to the Storage File.
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
        /// Retrieves a Quaternion value from the Storage File, or a default value if the key does not exist.
        /// </summary>
        /// <param name="_Key">The key under which the Quaternion is stored.</param>
        /// <param name="_DefaultValue">The default value to return if the key does not exist.</param>
        /// <returns>The stored Quaternion value, or the default value if the key does not exist.</returns>
        /// <remarks>
        /// This method first checks if the key exists in the the Storage File. If it does not, the default value is returned.
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
        /// Retrieves a Quaternion value from the Storage File, or Quaternion.zero if the key does not exist.
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

        #region Delete

        /// <summary>
        /// Removes a key and its corresponding value from the Storage.
        /// </summary>
        /// <param name="_Key">The key that should be removed.</param>
        /// <remarks>
        /// This method removes the key-value pair from the Storage, ensuring that the key is processed (hashed if necessary) before removal.
        /// </remarks>
        public static void DeleteKey(String _Key)
        {
            // Load the Storage if not already loaded.
            Load();

            // This is the key stored in the Storage.
            String var_Key = GetKeyName(_Key);

            // Remove the key.
            storage.Remove(var_Key);

            // Auto save if activated.
            if (AutoSave)
            {
                Save();
            }
        }

        #endregion

        #region Load

        /// <summary>
        /// Loads the storage container from the file system, if not already loaded.
        /// </summary>
        /// <remarks>
        /// This method ensures that the storage container is initialized only once. If the storage file exists, 
        /// it reads the data into the storage container. Additionally, it checks for ownership verification 
        /// if the global setting `PlayerPreferences_Allow_Read_Any_Owner` is disabled.
        /// </remarks>
        /// <exception cref="Exception">Thrown if the storage owner does not match the device identifier.</exception>
        public static void Load()
        {
            lock (lockHandle)
            {
                // Load the storage container only once.
                if (storage == null)
                {
                    if(String.IsNullOrEmpty(GlobalSettings.PlayerPreferences_Value_Encryption_Key))
                    {
                        // Create a new storage container with the device identifier as the owner. Without encryption.
                        storage = new StorageContainer(UnityEngine.SystemInfo.deviceUniqueIdentifier);
                    } 
                    else
                    {
                        // Get the UTF8 bytes of the encryption key.
                        byte[] var_EncryptionKeyBytes = System.Text.Encoding.UTF8.GetBytes(GlobalSettings.PlayerPreferences_Value_Encryption_Key);

                        // Create a new storage container with the device identifier as the owner. With encryption.
                        storage = new StorageContainer(UnityEngine.SystemInfo.deviceUniqueIdentifier, var_EncryptionKeyBytes);
                    }
                }
                else
                {
                    return;
                }

                // If the storage container exists as a file, load it.
                if (System.IO.File.Exists(FilePath))
                {
                    using (FileStream var_FileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        storage.Read(var_FileStream);
                    }
                }

                // Check if ownership verification is required.
                if (!GlobalSettings.PlayerPreferences_Allow_Read_Any_Owner)
                {
                    // Verify that the storage owner matches the device identifier.
                    if (storage.Owner != UnityEngine.SystemInfo.deviceUniqueIdentifier)
                    {
                        throw new Exception(ERROR_OWNER);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronously loads the storage container from the file system, if not already loaded.
        /// </summary>
        /// <remarks>
        /// This method performs the same functionality as <see cref="Load"/> but executes it asynchronously 
        /// to avoid blocking the main thread. It ensures that the storage container is initialized only once 
        /// and performs ownership verification if required.
        /// </remarks>
        /// <returns>A task representing the asynchronous load operation.</returns>
        /// <exception cref="Exception">Thrown if the storage owner does not match the device identifier.</exception>
        public static async Task LoadAsync()
        {
            await Task.Run(() =>
            {
                Load();
            });
        }

        #endregion

        #region Save

        /// <summary>
        /// Saves the current storage container to the file system.
        /// </summary>
        /// <remarks>
        /// This method writes the data in the storage container to a file at the specified <see cref="FilePath"/>. 
        /// If no storage container is initialized, an exception is thrown. The method ensures thread safety by 
        /// locking during the save operation.
        /// </remarks>
        /// <exception cref="Exception">Thrown if there is no storage container to save.</exception>
        public static void Save()
        {
            lock (lockHandle)
            {
                // No storage to save.
                if (storage == null)
                {
                    throw new Exception(ERROR_NO_STORAGE);
                }

                // Save the storage container to the file.
                using (FileStream var_FileStream = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    storage.Write(var_FileStream);
                }
            }
        }

        /// <summary>
        /// Asynchronously saves the current storage container to the file system.
        /// </summary>
        /// <remarks>
        /// This method performs the same functionality as <see cref="Save"/> but executes it asynchronously 
        /// to avoid blocking the main thread. The operation ensures thread safety during the save process.
        /// </remarks>
        /// <returns>A task representing the asynchronous save operation.</returns>
        /// <exception cref="Exception">Thrown if there is no storage container to save.</exception>
        public static async Task SaveAsync()
        {
            await Task.Run(() =>
            {
                Save();
            });
        }

        #endregion
    }
}