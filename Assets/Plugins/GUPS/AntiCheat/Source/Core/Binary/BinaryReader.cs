// System
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

// Unity
using UnityEngine;

// Allow the internal classes to be accessed by the test assembly.
[assembly: InternalsVisibleTo("GUPS.AntiCheat.Tests")]

namespace GUPS.AntiCheat.Core.Binary
{
    /// <summary>
    /// Represents a binary reader for reading primitive data types from a buffer.
    /// </summary>
    /// <remarks>
    /// This class provides methods to read various data types from a byte buffer.
    /// </remarks>
    internal class BinaryReader
    {
        /// <summary>
        /// Represents a union of a float and uint value.
        /// </summary>
        /// <remarks>
        /// This structure allows for efficient conversion between float and uint
        /// without allocating additional memory.
        /// </remarks>
        [StructLayout(LayoutKind.Explicit)]
        private struct UIntFloat
        {
            /// <summary>
            /// The float value.
            /// </summary>
            /// <remarks>
            /// This field shares the same memory location as IntValue.
            /// </remarks>
            [FieldOffset(0)]
            public float FloatValue;

            /// <summary>
            /// The uint value.
            /// </summary>
            /// <remarks>
            /// This field shares the same memory location as FloatValue.
            /// </remarks>
            [FieldOffset(0)]
            public uint IntValue;
        }

        /// <summary>
        /// Represents a union of a double and ulong value.
        /// </summary>
        /// <remarks>
        /// This structure allows for efficient conversion between double and ulong
        /// without allocating additional memory.
        /// </remarks>
        [StructLayout(LayoutKind.Explicit)]
        private struct LongDouble
        {
            /// <summary>
            /// The double value.
            /// </summary>
            /// <remarks>
            /// This field shares the same memory location as LongValue.
            /// </remarks>
            [FieldOffset(0)]
            public double DoubleValue;

            /// <summary>
            /// The ulong value.
            /// </summary>
            /// <remarks>
            /// This field shares the same memory location as DoubleValue.
            /// </remarks>
            [FieldOffset(0)]
            public ulong LongValue;
        }

        /// <summary>
        /// The underlying buffer for reading data.
        /// </summary>
        /// <remarks>
        /// This buffer stores the raw byte data that will be read by the BinaryReader.
        /// </remarks>
        private Buffer buffer;

        /// <summary>
        /// The maximum byte length that can be read.
        /// </summary>
        /// <remarks>
        /// This constant defines the upper limit of bytes that can be processed by the reader.
        /// </remarks>
        private const int MAX_BYTE_LENGTH = 2147483647;

        /// <summary>
        /// The initial size of the string reader buffer.
        /// </summary>
        /// <remarks>
        /// This constant defines the initial capacity of the buffer used for reading strings.
        /// </remarks>
        private const int INITIAL_STRING_BUFFER_SIZE = 1024;

        /// <summary>
        /// Buffer used for reading strings.
        /// </summary>
        /// <remarks>
        /// This static buffer is reused across string read operations to improve performance.
        /// </remarks>
        private static byte[] stringReaderBuffer;

        /// <summary>
        /// The encoding used for reading strings.
        /// </summary>
        /// <remarks>
        /// This encoding is used to convert byte data to string characters.
        /// </remarks>
        private static Encoding encoding;

        /// <summary>
        /// Converter for float values.
        /// </summary>
        /// <remarks>
        /// This static field is used to efficiently convert between float and uint representations.
        /// </remarks>
        private static UIntFloat floatConverter;

        /// <summary>
        /// Converter for double values.
        /// </summary>
        /// <remarks>
        /// This static field is used to efficiently convert between double and ulong representations.
        /// </remarks>
        private static LongDouble doubleConverter;

        /// <summary>
        /// Gets the current position within the buffer.
        /// </summary>
        /// <remarks>
        /// This property returns the current read position in the underlying buffer.
        /// </remarks>
        public uint Position
        {
            get { return this.buffer.Position; }
        }

        /// <summary>
        /// Gets the current length of the buffer.
        /// </summary>
        /// <remarks>
        /// This property returns the total length of the underlying buffer.
        /// </remarks>
        public int Length
        {
            get { return this.buffer.Length; }
        }

        /// <summary>
        /// Creates a new BinaryReader object with an empty buffer.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the reader with a new, empty buffer and performs necessary static initialization.
        /// </remarks>
        public BinaryReader()
        {
            this.buffer = new Buffer();
            BinaryReader.Initialize();
        }

        /// <summary>
        /// Creates a new BinaryReader object with the specified buffer.
        /// </summary>
        /// <param name="_Buffer">A buffer to construct the reader with, this buffer is NOT copied.</param>
        /// <remarks>
        /// This constructor initializes the reader with the provided buffer and performs necessary static initialization.
        /// </remarks>
        public BinaryReader(byte[] _Buffer)
        {
            this.buffer = new Buffer(_Buffer);
            BinaryReader.Initialize();
        }

        /// <summary>
        /// Initializes static members of the BinaryReader class, if not already initialized.
        /// </summary>
        static BinaryReader()
        {
            BinaryReader.Initialize();
        }

        /// <summary>
        /// Initializes static members of the BinaryReader class.
        /// </summary>
        /// <remarks>
        /// This method ensures that the string reader buffer and encoding are properly initialized.
        /// </remarks>
        private static void Initialize()
        {
            if (BinaryReader.encoding == null)
            {
                BinaryReader.stringReaderBuffer = new byte[INITIAL_STRING_BUFFER_SIZE];
                BinaryReader.encoding = new UTF8Encoding();
            }
        }

        /// <summary>
        /// Sets the current position of the reader's stream to the start of the stream.
        /// </summary>
        /// <remarks>
        /// This method resets the read position to the beginning of the buffer.
        /// </remarks>
        public void SeekZero()
        {
            this.buffer.SeekZero();
        }

        /// <summary>
        /// Replaces the current buffer with a new one.
        /// </summary>
        /// <param name="_Buffer">The new buffer to use.</param>
        /// <remarks>
        /// This method allows changing the underlying buffer without creating a new BinaryReader instance.
        /// </remarks>
        internal void Replace(byte[] _Buffer)
        {
            this.buffer.Replace(_Buffer);
        }

        /// <summary>
        /// Reads an object of the specified type from the binary reader.
        /// </summary>
        /// <param name="_Type">The type of object to read.</param>
        /// <returns>The object read from the binary reader.</returns>
        /// <remarks>
        /// This method supports reading various primitive types, Unity-specific types,
        /// and creates default instances for other value types.
        /// </remarks>
        public System.Object Read(Type _Type)
        {
            if (_Type == typeof(System.Byte))
                return this.ReadByte();
            else if (_Type == typeof(System.Boolean))
                return this.ReadBoolean();
            else if (_Type == typeof(System.Int16))
                return this.ReadInt16();
            else if (_Type == typeof(System.Int32))
                return this.ReadInt32();
            else if (_Type == typeof(System.Int64))
                return this.ReadInt64();
            else if (_Type == typeof(System.UInt16))
                return this.ReadUInt16();
            else if (_Type == typeof(System.UInt32))
                return this.ReadUInt32();
            else if (_Type == typeof(System.UInt64))
                return this.ReadUInt64();
            else if (_Type == typeof(System.Single))
                return this.ReadSingle();
            else if (_Type == typeof(System.Double))
                return this.ReadDouble();
            else if (_Type == typeof(System.Decimal))
                return this.ReadDecimal();
            else if (_Type == typeof(System.Char))
                return this.ReadChar();
            else if (_Type == typeof(System.String))
                return this.ReadString();
            else if (_Type == typeof(UnityEngine.Color))
                return this.ReadColor();
            else if (_Type == typeof(UnityEngine.Color32))
                return this.ReadColor32();
            else if (_Type == typeof(UnityEngine.Vector2))
                return this.ReadVector2();
            else if (_Type == typeof(UnityEngine.Vector2Int))
                return this.ReadVector2Int();
            else if (_Type == typeof(UnityEngine.Vector3))
                return this.ReadVector3();
            else if (_Type == typeof(UnityEngine.Vector2Int))
                return this.ReadVector3Int();
            else if (_Type == typeof(UnityEngine.Vector4))
                return this.ReadVector4();
            else if (_Type == typeof(UnityEngine.Quaternion))
                return this.ReadQuaternion();
            else if (_Type == typeof(UnityEngine.Rect))
                return this.ReadRect();
            else if (_Type == typeof(UnityEngine.Plane))
                return this.ReadPlane();
            else if (_Type == typeof(UnityEngine.Ray))
                return this.ReadRay();
            else if (_Type == typeof(UnityEngine.Matrix4x4))
                return this.ReadMatrix4x4();

            // Is a struct, create the default instance.
            if (_Type.IsValueType)
            {
                return Activator.CreateInstance(_Type);
            }

            // Is a class, return null.
            return null;
        }

        /// <summary>
        /// Reads a byte from the stream.
        /// </summary>
        /// <returns>The value read.</returns>
        /// <remarks>
        /// This method reads a single unsigned byte from the current position in the buffer.
        /// </remarks>
        public byte ReadByte()
        {
            return this.buffer.ReadByte();
        }

        /// <summary>
        /// Reads a signed byte from the stream.
        /// </summary>
        /// <returns>Value read.</returns>
        /// <remarks>
        /// This method reads a single signed byte from the current position in the buffer.
        /// </remarks>
        public sbyte ReadSByte()
        {
            return (sbyte)this.buffer.ReadByte();
        }

        /// <summary>
        /// Reads a signed 16 bit integer from the stream.
        /// </summary>
        /// <returns>Value read.</returns>
        /// <remarks>
        /// This method reads two bytes from the buffer and combines them into a 16-bit signed integer.
        /// </remarks>
        public short ReadInt16()
        {
            ushort num = 0;
            num = (ushort)(num | this.buffer.ReadByte());
            num = (ushort)(num | (ushort)(this.buffer.ReadByte() << 8));
            return (short)num;
        }

        /// <summary>
        /// Reads an unsigned 16 bit integer from the stream.
        /// </summary>
        /// <returns>Value read.</returns>
        /// <remarks>
        /// This method reads two bytes from the buffer and combines them into a 16-bit unsigned integer.
        /// </remarks>
        public ushort ReadUInt16()
        {
            ushort num = 0;
            num = (ushort)(num | this.buffer.ReadByte());
            return (ushort)(num | (ushort)(this.buffer.ReadByte() << 8));
        }

        /// <summary>
        /// Reads a signed 32bit integer from the stream.
        /// </summary>
        /// <returns>Value read.</returns>
        /// <remarks>
        /// This method reads four bytes from the buffer and combines them into a 32-bit signed integer.
        /// </remarks>
        public int ReadInt32()
        {
            uint num = 0u;
            num |= this.buffer.ReadByte();
            num = (uint)((int)num | this.buffer.ReadByte() << 8);
            num = (uint)((int)num | this.buffer.ReadByte() << 16);
            return (int)num | this.buffer.ReadByte() << 24;
        }

        /// <summary>
        /// Reads an unsigned 32 bit integer from the stream.
        /// </summary>
        /// <returns>Value read.</returns>
        /// <remarks>
        /// This method reads four bytes from the buffer and combines them into a 32-bit unsigned integer.
        /// </remarks>
        public uint ReadUInt32()
        {
            uint num = 0u;
            num |= this.buffer.ReadByte();
            num = (uint)((int)num | this.buffer.ReadByte() << 8);
            num = (uint)((int)num | this.buffer.ReadByte() << 16);
            return (uint)((int)num | this.buffer.ReadByte() << 24);
        }

        /// <summary>
        /// Reads a signed 64 bit integer from the stream.
        /// </summary>
        /// <returns>Value read.</returns>
        /// <remarks>
        /// This method reads eight bytes from the buffer and combines them into a 64-bit signed integer.
        /// </remarks>
        public long ReadInt64()
        {
            ulong num = 0uL;
            ulong num2 = this.buffer.ReadByte();
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 8;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 16;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 24;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 32;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 40;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 48;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 56;
            return (long)(num | num2);
        }

        /// <summary>
        /// Reads an unsigned 64 bit integer from the stream.
        /// </summary>
        /// <returns>Value read.</returns>
        /// <remarks>
        /// This method reads eight bytes from the buffer and combines them into a 64-bit unsigned integer.
        /// </remarks>
        public ulong ReadUInt64()
        {
            ulong num = 0uL;
            ulong num2 = this.buffer.ReadByte();
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 8;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 16;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 24;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 32;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 40;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 48;
            num |= num2;
            num2 = (ulong)this.buffer.ReadByte() << 56;
            return num | num2;
        }

        /// <summary>
        /// Reads a decimal from the stream.
        /// </summary>
        /// <returns>Value read.</returns>
        /// <remarks>
        /// This method reads four 32-bit integers from the buffer and constructs a decimal value from them.
        /// </remarks>
        public decimal ReadDecimal()
        {
            return new decimal(new int[4]
            {
                this.ReadInt32(),
                this.ReadInt32(),
                this.ReadInt32(),
                this.ReadInt32()
            });
        }

        /// <summary>
        /// Reads a float from the stream.
        /// </summary>
        /// <returns>The float value read from the stream.</returns>
        /// <remarks>
        /// Uses a static FloatConverter to convert the read uint to a float.
        /// </remarks>
        public float ReadSingle()
        {
            floatConverter.IntValue = this.ReadUInt32();
            return floatConverter.FloatValue;
        }

        /// <summary>
        /// Reads a double from the stream.
        /// </summary>
        /// <returns>The double value read from the stream.</returns>
        /// <remarks>
        /// Uses a static DoubleConverter to convert the read ulong to a double.
        /// </remarks>
        public double ReadDouble()
        {
            doubleConverter.LongValue = this.ReadUInt64();
            return doubleConverter.DoubleValue;
        }

        /// <summary>
        /// Reads a string from the stream. (max of 32k bytes)
        /// </summary>
        /// <returns>The string value read from the stream, or null if the length is -1.</returns>
        /// <remarks>
        /// Throws an IndexOutOfRangeException if the string length exceeds the maximum allowed length.
        /// </remarks>
        public string ReadString()
        {
            int num = this.ReadInt32();
            if (num == -1)
            {
                return null;
            }
            if (num == 0)
            {
                return "";
            }
            while (num > BinaryReader.stringReaderBuffer.Length)
            {
                if (BinaryReader.stringReaderBuffer.Length >= Int32.MaxValue / 2)
                {
                    throw new IOException("Required array size of " + num + " too large");
                }

                BinaryReader.stringReaderBuffer = new byte[BinaryReader.stringReaderBuffer.Length * 2];
            }
            this.buffer.ReadBytes(BinaryReader.stringReaderBuffer, num);
            char[] chars = BinaryReader.encoding.GetChars(BinaryReader.stringReaderBuffer, 0, num);
            return new string(chars);
        }

        /// <summary>
        /// Reads a char from the stream.
        /// </summary>
        /// <returns>The char value read from the stream.</returns>
        /// <remarks>
        /// Reads a single byte and casts it to a char.
        /// </remarks>
        public char ReadChar()
        {
            return (char)this.buffer.ReadByte();
        }

        /// <summary>
        /// Reads a boolean from the stream.
        /// </summary>
        /// <returns>The boolean value read from the stream.</returns>
        /// <remarks>
        /// Returns true if the read byte is 1, false otherwise.
        /// </remarks>
        public bool ReadBoolean()
        {
            return this.buffer.ReadByte() == 1;
        }

        /// <summary>
        /// Reads a number of bytes from the stream.
        /// </summary>
        /// <param name="_Count">Number of bytes to read.</param>
        /// <returns>A new array containing the bytes read from the stream.</returns>
        /// <remarks>
        /// Throws an IndexOutOfRangeException if the count is negative.
        /// </remarks>
        public byte[] ReadBytes(int _Count)
        {
            if (_Count < 0)
            {
                throw new IndexOutOfRangeException("BinaryReader ReadBytes " + _Count);
            }
            byte[] array = new byte[_Count];
            this.buffer.ReadBytes(array, (int)_Count);
            return array;
        }

        /// <summary>
        /// Reads a 32-bit byte count and an array of bytes of that size from the stream.
        /// </summary>
        /// <returns>The bytes read from the stream, or an empty array if the count is 0.</returns>
        /// <remarks>
        /// First reads an int32 to determine the number of bytes to read, then reads that many bytes.
        /// </remarks>
        public byte[] ReadBytesAndSize()
        {
            int num = this.ReadInt32();
            if (num == 0)
            {
                return new byte[0];
            }
            return this.ReadBytes(num);
        }

        /// <summary>
        /// Reads a Unity Vector2 object.
        /// </summary>
        /// <returns>The Vector2 read from the stream.</returns>
        /// <remarks>
        /// Reads two float values to construct the Vector2.
        /// </remarks>
        public Vector2 ReadVector2()
        {
            return new Vector2(this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// Reads a Unity Vector2Int object.
        /// </summary>
        /// <returns>The Vector2Int read from the stream.</returns>
        /// <remarks>
        /// Reads two int values to construct the Vector2Int.
        /// </remarks>
        public Vector2Int ReadVector2Int()
        {
            return new Vector2Int(this.ReadInt32(), this.ReadInt32());
        }

        /// <summary>
        /// Reads a Unity Vector3 object.
        /// </summary>
        /// <returns>The Vector3 read from the stream.</returns>
        /// <remarks>
        /// Reads three float values to construct the Vector3.
        /// </remarks>
        public Vector3 ReadVector3()
        {
            return new Vector3(this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// Reads a Unity Vector3Int object.
        /// </summary>
        /// <returns>The Vector3Int read from the stream.</returns>
        /// <remarks>
        /// Reads three float values to construct the Vector3Int.
        /// </remarks>
        public Vector3Int ReadVector3Int()
        {
            return new Vector3Int(this.ReadInt32(), this.ReadInt32(), this.ReadInt32());
        }

        /// <summary>
        /// Reads a Unity Vector4 object.
        /// </summary>
        /// <returns>The Vector4 read from the stream.</returns>
        /// <remarks>
        /// Reads four float values to construct the Vector4.
        /// </remarks>
        public Vector4 ReadVector4()
        {
            return new Vector4(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// Reads a Unity Color object.
        /// </summary>
        /// <returns>The Color read from the stream.</returns>
        /// <remarks>
        /// Reads four float values to construct the Color (RGBA).
        /// </remarks>
        public Color ReadColor()
        {
            return new Color(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// Reads a Unity Color32 object.
        /// </summary>
        /// <returns>The Color32 read from the stream.</returns>
        /// <remarks>
        /// Reads four byte values to construct the Color32 (RGBA).
        /// </remarks>
        public Color32 ReadColor32()
        {
            return new Color32(this.ReadByte(), this.ReadByte(), this.ReadByte(), this.ReadByte());
        }

        /// <summary>
        /// Reads a Unity Quaternion object.
        /// </summary>
        /// <returns>The Quaternion read from the stream.</returns>
        /// <remarks>
        /// Reads four float values to construct the Quaternion (x, y, z, w).
        /// </remarks>
        public Quaternion ReadQuaternion()
        {
            return new Quaternion(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// Reads a Unity Rect object.
        /// </summary>
        /// <returns>The Rect read from the stream.</returns>
        /// <remarks>
        /// Reads four float values to construct the Rect (x, y, width, height).
        /// </remarks>
        public Rect ReadRect()
        {
            return new Rect(this.ReadSingle(), this.ReadSingle(), this.ReadSingle(), this.ReadSingle());
        }

        /// <summary>
        /// Reads a Unity Plane object.
        /// </summary>
        /// <returns>The Plane read from the stream.</returns>
        /// <remarks>
        /// Reads a Vector3 for the normal and a float for the distance to construct the Plane.
        /// </remarks>
        public Plane ReadPlane()
        {
            return new Plane(this.ReadVector3(), this.ReadSingle());
        }

        /// <summary>
        /// Reads a Unity Ray object.
        /// </summary>
        /// <returns>The Ray read from the stream.</returns>
        /// <remarks>
        /// Reads two Vector3 objects to construct the Ray (origin and direction).
        /// </remarks>
        public Ray ReadRay()
        {
            return new Ray(this.ReadVector3(), this.ReadVector3());
        }

        /// <summary>
        /// Reads a Unity Matrix4x4 object.
        /// </summary>
        /// <returns>The Matrix4x4 read from the stream.</returns>
        /// <remarks>
        /// Reads 16 float values to construct the Matrix4x4.
        /// </remarks>
        public Matrix4x4 ReadMatrix4x4()
        {
            Matrix4x4 result = default(Matrix4x4);
            result.m00 = this.ReadSingle();
            result.m01 = this.ReadSingle();
            result.m02 = this.ReadSingle();
            result.m03 = this.ReadSingle();
            result.m10 = this.ReadSingle();
            result.m11 = this.ReadSingle();
            result.m12 = this.ReadSingle();
            result.m13 = this.ReadSingle();
            result.m20 = this.ReadSingle();
            result.m21 = this.ReadSingle();
            result.m22 = this.ReadSingle();
            result.m23 = this.ReadSingle();
            result.m30 = this.ReadSingle();
            result.m31 = this.ReadSingle();
            result.m32 = this.ReadSingle();
            result.m33 = this.ReadSingle();
            return result;
        }

        /// <summary>
        /// Returns a string representation of the reader's buffer.
        /// </summary>
        /// <returns>A string representation of the buffer contents.</returns>
        /// <remarks>
        /// This method delegates to the underlying buffer's ToString method.
        /// </remarks>
        public override string ToString()
        {
            return this.buffer.ToString();
        }

        /// <summary>
        /// Gets the default value for a given type.
        /// </summary>
        /// <param name="_Type">The Type for which to get the default value.</param>
        /// <returns>The default value for the specified type.</returns>
        /// <remarks>
        /// For value types, this method creates a new instance.
        /// For reference types, it returns null.
        /// </remarks>
        private object GetDefaultValue(Type _Type)
        {
            if (_Type.IsValueType)
                return Activator.CreateInstance(_Type);

            return null;
        }
    }
}