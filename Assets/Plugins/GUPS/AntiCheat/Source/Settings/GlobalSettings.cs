// System
using System;
using System.Collections.Generic;

// Unity
using UnityEngine;

// GUPS - AntiCheat - Core
using GUPS.AntiCheat.Core.Hash;
using GUPS.AntiCheat.Core.Random;

// GUPS - AntiCheat
using GUPS.AntiCheat.Monitor.Android;

namespace GUPS.AntiCheat.Settings
{
    /// <summary>
    /// The global settings for the anti cheat monitor.
    /// </summary>
    public class GlobalSettings : ScriptableObject
    {
        /// <summary>
        /// A shared random provider used to generate random values.
        /// </summary>
        public static IRandomProvider RandomProvider { get; } = new PseudoRandom();

        /// <summary>
        /// Set this property to true to enable verification of the integrity of the player preferences. Set it to false if you do not wish to verify 
        /// integrity. The integrity check relies on a hash that is calculated from the data type, value, and owner, and is stored in the signature.
        /// </summary>
        public static bool PlayerPreferences_Verify_Integrity = false;

        /// <summary>
        /// Set this property to true to encrypt the player preference key. If set to false, the key will not be encrypted. When encryption is enabled, 
        /// the key is stored as a hash instead of its original name.
        /// </summary>
        public static bool PlayerPreferences_Hash_Key = false;

        /// <summary>
        /// Assign a key to encrypt the player preference value. This key will be used for encryption. If a key is not assigned, the value will remain unencrypted.
        /// If you change the key, the already written values will not be readable anymore, keep that in mind.
        /// </summary>
        public static string PlayerPreferences_Value_Encryption_Key = string.Empty;

        /// <summary>
        /// Set this property to true to permit anybody to read the stored player preference. If set to false, only the owner who created the player preference can 
        /// access it. By default, the owner is identified using the device's unique identifier from Unity, accessed via <see cref="UnityEngine.SystemInfo.deviceUniqueIdentifier"/>. 
        /// This feature is useful for sharing player preferences between different users or restricting access to them. For example if a user copy and paste it between devices.
        /// </summary>
        public static bool PlayerPreferences_Allow_Read_Any_Owner = true;
    }
}
