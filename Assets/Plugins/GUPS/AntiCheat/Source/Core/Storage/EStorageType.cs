// System
using System;

namespace GUPS.AntiCheat.Core.Storage
{
    /// <summary>
    /// Represents the storage types for reading different data types.
    /// </summary>
    [Serializable]
    public enum EStorageType : byte
    {
        /// <summary>
        /// Represents a byte (8-bit unsigned integer).
        /// </summary>
        /// <remarks>
        /// Use this for handling byte data.
        /// </remarks>
        Byte,

        /// <summary>
        /// Represents a byte array.
        /// </summary>
        /// <remarks>
        /// Use this for handling byte array data.
        /// </remarks>
        ByteArray,

        /// <summary>
        /// Represents a Boolean value.
        /// </summary>
        /// <remarks>
        /// Use this for handling true or false values.
        /// </remarks>
        Boolean,

        /// <summary>
        /// Represents a 16-bit signed integer.
        /// </summary>
        /// <remarks>
        /// Use this for handling short integer values.
        /// </remarks>
        Int16,

        /// <summary>
        /// Represents a 32-bit signed integer.
        /// </summary>
        /// <remarks>
        /// Use this for handling integer values.
        /// </remarks>
        Int32,

        /// <summary>
        /// Represents a 64-bit signed integer.
        /// </summary>
        /// <remarks>
        /// Use this for handling long integer values.
        /// </remarks>
        Int64,

        /// <summary>
        /// Represents a 16-bit unsigned integer.
        /// </summary>
        /// <remarks>
        /// Use this for handling unsigned short integer values.
        /// </remarks>
        UInt16,

        /// <summary>
        /// Represents a 32-bit unsigned integer.
        /// </summary>
        /// <remarks>
        /// Use this for handling unsigned integer values.
        /// </remarks>
        UInt32,

        /// <summary>
        /// Represents a 64-bit unsigned integer.
        /// </summary>
        /// <remarks>
        /// Use this for handling unsigned long integer values.
        /// </remarks>
        UInt64,

        /// <summary>
        /// Represents a single-precision floating-point number.
        /// </summary>
        /// <remarks>
        /// Use this for handling float values.
        /// </remarks>
        Single,

        /// <summary>
        /// Represents a double-precision floating-point number.
        /// </summary>
        /// <remarks>
        /// Use this for handling double values.
        /// </remarks>
        Double,

        /// <summary>
        /// Represents a decimal number.
        /// </summary>
        /// <remarks>
        /// Use this for handling high precision decimal values.
        /// </remarks>
        Decimal,

        /// <summary>
        /// Represents a single Unicode character.
        /// </summary>
        /// <remarks>
        /// Use this for handling character data.
        /// </remarks>
        Char,

        /// <summary>
        /// Represents a string.
        /// </summary>
        /// <remarks>
        /// Use this for handling textual data.
        /// </remarks>
        String,

        /// <summary>
        /// Represents a Unity color.
        /// </summary>
        /// <remarks>
        /// Use this for handling color data in Unity.
        /// </remarks>
        Color,

        /// <summary>
        /// Represents a Unity color with 32-bit color depth.
        /// </summary>
        /// <remarks>
        /// Use this for handling Color32 data in Unity.
        /// </remarks>
        Color32,

        /// <summary>
        /// Represents a 2D vector.
        /// </summary>
        /// <remarks>
        /// Use this for handling Vector2 data in Unity.
        /// </remarks>
        Vector2,

        /// <summary>
        /// Represents a 2D integer vector.
        /// </summary>
        /// <remarks>
        /// Use this for handling Vector2Int data in Unity.
        /// </remarks>
        Vector2Int,

        /// <summary>
        /// Represents a 3D vector.
        /// </summary>
        /// <remarks>
        /// Use this for handling Vector3 data in Unity.
        /// </remarks>
        Vector3,

        /// <summary>
        /// Represents a 3D integer vector.
        /// </summary>
        /// <remarks>
        /// Use this for handling Vector3Int data in Unity.
        /// </remarks>
        Vector3Int,

        /// <summary>
        /// Represents a 4D vector.
        /// </summary>
        /// <remarks>
        /// Use this for handling Vector4 data in Unity.
        /// </remarks>
        Vector4,

        /// <summary>
        /// Represents a quaternion for rotation.
        /// </summary>
        /// <remarks>
        /// Use this for handling quaternion rotations in Unity.
        /// </remarks>
        Quaternion,

        /// <summary>
        /// Represents a rectangle with position and size.
        /// </summary>
        /// <remarks>
        /// Use this for handling Rect data in Unity.
        /// </remarks>
        Rect,

        /// <summary>
        /// Represents a mathematical plane.
        /// </summary>
        /// <remarks>
        /// Use this for handling Plane data in Unity.
        /// </remarks>
        Plane,

        /// <summary>
        /// Represents a ray with an origin and direction.
        /// </summary>
        /// <remarks>
        /// Use this for handling Ray data in Unity.
        /// </remarks>
        Ray,

        /// <summary>
        /// Represents a 4x4 transformation matrix.
        /// </summary>
        /// <remarks>
        /// Use this for handling Matrix4x4 data in Unity.
        /// </remarks>
        Matrix4x4
    }
}
