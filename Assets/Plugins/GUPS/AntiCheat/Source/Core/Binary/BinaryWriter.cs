// System
using System;
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
    /// Represents a binary writer for writing primitive data types to a buffer.
    /// </summary>
    /// <remarks>
    /// This class provides methods for writing various data types to a binary buffer.
    /// </remarks>
    internal class BinaryWriter
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
        /// The maximum length of a string that can be written.
        /// </summary>
        private const int MAX_STRING_LENGTH = 65535;

        /// <summary>
        /// The maximum length of a byte array that can be written.
        /// </summary>
        private const int MAX_BYTE_LENGTH = 2147483647;

        /// <summary>
        /// The internal buffer used for writing data.
        /// </summary>
        private Buffer buffer;

        /// <summary>
        /// The encoding used for writing strings.
        /// </summary>
        private static Encoding encoding;

        /// <summary>
        /// A buffer used for writing strings.
        /// </summary>
        private static byte[] stringWriteBuffer;

        /// <summary>
        /// A union structure for converting between float and int.
        /// </summary>
        private static UIntFloat floatConverter;

        /// <summary>
        /// A union structure for converting between double and long.
        /// </summary>
        private static LongDouble doubleConverter;

        /// <summary>
        /// Gets the current position of the internal buffer.
        /// </summary>
        /// <remarks>
        /// This property returns the current write position in the buffer.
        /// </remarks>
        public uint Position
        {
            get
            {
                return this.buffer.Position;
            }
        }

        /// <summary>
        /// Initializes a new instance of the BinaryWriter class.
        /// </summary>
        /// <remarks>
        /// This constructor creates a new BinaryWriter with an empty buffer.
        /// </remarks>
        public BinaryWriter()
        {
            this.buffer = new Buffer();
            if (BinaryWriter.encoding == null)
            {
                BinaryWriter.encoding = new UTF8Encoding();
                BinaryWriter.stringWriteBuffer = new byte[MAX_STRING_LENGTH];
            }
        }

        /// <summary>
        /// Initializes a new instance of the BinaryWriter class with a specified buffer.
        /// </summary>
        /// <param name="_Buffer">A buffer to write into. This is not copied.</param>
        /// <remarks>
        /// This constructor creates a new BinaryWriter with the specified buffer.
        /// </remarks>
        public BinaryWriter(byte[] _Buffer)
        {
            this.buffer = new Buffer(_Buffer);
            if (BinaryWriter.encoding == null)
            {
                BinaryWriter.encoding = new UTF8Encoding();
                BinaryWriter.stringWriteBuffer = new byte[MAX_STRING_LENGTH];
            }
        }

        /// <summary>
        /// Returns a copy of the internal array of bytes the writer is using.
        /// </summary>
        /// <returns>A copy of data used by the writer.</returns>
        /// <remarks>
        /// This method copies only the bytes used.
        /// </remarks>
        public byte[] ToArray()
        {
            byte[] array = new byte[this.buffer.AsArraySegment().Count];
            Array.Copy(this.buffer.AsArraySegment().Array, array, this.buffer.AsArraySegment().Count);
            return array;
        }

        /// <summary>
        /// Returns the internal array of bytes the writer is using.
        /// </summary>
        /// <returns>The internal buffer.</returns>
        /// <remarks>
        /// This method returns a reference to the internal buffer, not a copy.
        /// </remarks>
        public byte[] AsArray()
        {
            return this.AsArraySegment().Array;
        }

        /// <summary>
        /// Returns the internal buffer as an ArraySegment.
        /// </summary>
        /// <returns>An ArraySegment representing the internal buffer.</returns>
        /// <remarks>
        /// This method is internal and provides access to the buffer as an ArraySegment.
        /// </remarks>
        internal ArraySegment<byte> AsArraySegment()
        {
            return this.buffer.AsArraySegment();
        }

        /// <summary>
        /// Writes a value of a specified type to the stream.
        /// </summary>
        /// <param name="_Type">The Type of the value to write.</param>
        /// <param name="_Value">The value to write.</param>
        /// <remarks>
        /// This method determines the appropriate Write method to call based on the type of the value.
        /// </remarks>
        public void Write(Type _Type, System.Object _Value)
        {
            if (_Type == typeof(System.Byte))
                this.Write((System.Byte)_Value);
            else if (_Type == typeof(System.Boolean))
                this.Write((System.Boolean)_Value);
            else if (_Type == typeof(System.Int16))
                this.Write((System.Int16)_Value);
            else if (_Type == typeof(System.Int32))
                this.Write((System.Int32)_Value);
            else if (_Type == typeof(System.Int64))
                this.Write((System.Int64)_Value);
            else if (_Type == typeof(System.UInt16))
                this.Write((System.UInt16)_Value);
            else if (_Type == typeof(System.UInt32))
                this.Write((System.UInt32)_Value);
            else if (_Type == typeof(System.UInt64))
                this.Write((System.UInt64)_Value);
            else if (_Type == typeof(System.Single))
                this.Write((System.Single)_Value);
            else if (_Type == typeof(System.Double))
                this.Write((System.Double)_Value);
            else if (_Type == typeof(System.Decimal))
                this.Write((System.Decimal)_Value);
            else if (_Type == typeof(System.Char))
                this.Write((System.Char)_Value);
            else if (_Type == typeof(System.String))
                this.Write((System.String)_Value);
            else if (_Type == typeof(UnityEngine.Color))
                this.Write((UnityEngine.Color)_Value);
            else if (_Type == typeof(UnityEngine.Color32))
                this.Write((UnityEngine.Color32)_Value);
            else if (_Type == typeof(UnityEngine.Vector2))
                this.Write((UnityEngine.Vector2)_Value);
            else if (_Type == typeof(UnityEngine.Vector2Int))
                this.Write((UnityEngine.Vector2Int)_Value);
            else if (_Type == typeof(UnityEngine.Vector3))
                this.Write((UnityEngine.Vector3)_Value);
            else if (_Type == typeof(UnityEngine.Vector3Int))
                this.Write((UnityEngine.Vector3Int)_Value);
            else if (_Type == typeof(UnityEngine.Vector4))
                this.Write((UnityEngine.Vector4)_Value);
            else if (_Type == typeof(UnityEngine.Quaternion))
                this.Write((UnityEngine.Quaternion)_Value);
            else if (_Type == typeof(UnityEngine.Rect))
                this.Write((UnityEngine.Rect)_Value);
            else if (_Type == typeof(UnityEngine.Plane))
                this.Write((UnityEngine.Plane)_Value);
            else if (_Type == typeof(UnityEngine.Ray))
                this.Write((UnityEngine.Ray)_Value);
            else if (_Type == typeof(UnityEngine.Matrix4x4))
                this.Write((UnityEngine.Matrix4x4)_Value);
        }

        /// <summary>
        /// Writes a char value to the stream.
        /// </summary>
        /// <param name="_Value">The char value to write.</param>
        /// <remarks>
        /// This method writes a single byte representing the char value.
        /// </remarks>
        public void Write(char _Value)
        {
            this.buffer.WriteByte((byte)_Value);
        }

        /// <summary>
        /// Writes a byte value to the stream.
        /// </summary>
        /// <param name="_Value">The byte value to write.</param>
        /// <remarks>
        /// This method writes a single byte to the stream.
        /// </remarks>
        public void Write(byte _Value)
        {
            this.buffer.WriteByte(_Value);
        }

        /// <summary>
        /// Writes a sbyte value to the stream.
        /// </summary>
        /// <param name="_Value">The sbyte value to write.</param>
        /// <remarks>
        /// This method writes a single byte representing the sbyte value.
        /// </remarks>
        public void Write(sbyte _Value)
        {
            this.buffer.WriteByte((byte)_Value);
        }

        /// <summary>
        /// Writes a short value to the stream.
        /// </summary>
        /// <param name="_Value">The short value to write.</param>
        /// <remarks>
        /// This method writes two bytes representing the short value.
        /// </remarks>
        public void Write(short _Value)
        {
            this.buffer.WriteByte2((byte)(_Value & 0xFF), (byte)(_Value >> 8 & 0xFF));
        }

        /// <summary>
        /// Writes a ushort value to the stream.
        /// </summary>
        /// <param name="_Value">The ushort value to write.</param>
        /// <remarks>
        /// This method writes two bytes representing the ushort value.
        /// </remarks>
        public void Write(ushort _Value)
        {
            this.buffer.WriteByte2((byte)(_Value & 0xFF), (byte)(_Value >> 8 & 0xFF));
        }

        /// <summary>
        /// Writes an int value to the stream.
        /// </summary>
        /// <param name="_Value">The int value to write.</param>
        /// <remarks>
        /// This method writes four bytes representing the int value.
        /// </remarks>
        public void Write(int _Value)
        {
            this.buffer.WriteByte4((byte)(_Value & 0xFF), (byte)(_Value >> 8 & 0xFF), (byte)(_Value >> 16 & 0xFF), (byte)(_Value >> 24 & 0xFF));
        }

        /// <summary>
        /// Writes a uint value to the stream.
        /// </summary>
        /// <param name="_Value">The uint value to write.</param>
        /// <remarks>
        /// This method writes four bytes representing the uint value.
        /// </remarks>
        public void Write(uint _Value)
        {
            this.buffer.WriteByte4((byte)(_Value & 0xFF), (byte)(_Value >> 8 & 0xFF), (byte)(_Value >> 16 & 0xFF), (byte)(_Value >> 24 & 0xFF));
        }

        /// <summary>
        /// Writes a long value to the stream.
        /// </summary>
        /// <param name="_Value">The long value to write.</param>
        /// <remarks>
        /// This method writes eight bytes representing the long value.
        /// </remarks>
        public void Write(long _Value)
        {
            this.buffer.WriteByte8((byte)(_Value & 0xFF), (byte)(_Value >> 8 & 0xFF), (byte)(_Value >> 16 & 0xFF), (byte)(_Value >> 24 & 0xFF),
                                     (byte)(_Value >> 32 & 0xFF), (byte)(_Value >> 40 & 0xFF), (byte)(_Value >> 48 & 0xFF), (byte)(_Value >> 56 & 0xFF));
        }

        /// <summary>
        /// Writes a ulong value to the stream.
        /// </summary>
        /// <param name="_Value">The ulong value to write.</param>
        /// <remarks>
        /// This method writes eight bytes representing the ulong value.
        /// </remarks>
        public void Write(ulong _Value)
        {
            this.buffer.WriteByte8((byte)(_Value & 0xFF), (byte)(_Value >> 8 & 0xFF), (byte)(_Value >> 16 & 0xFF), (byte)(_Value >> 24 & 0xFF),
                                     (byte)(_Value >> 32 & 0xFF), (byte)(_Value >> 40 & 0xFF), (byte)(_Value >> 48 & 0xFF), (byte)(_Value >> 56 & 0xFF));
        }

        /// <summary>
        /// Writes a float value to the stream.
        /// </summary>
        /// <param name="_Value">The float value to write.</param>
        /// <remarks>
        /// This method converts the float to an int and writes it to the stream.
        /// </remarks>
        public void Write(float _Value)
        {
            BinaryWriter.floatConverter.FloatValue = _Value;
            this.Write(BinaryWriter.floatConverter.IntValue);
        }

        /// <summary>
        /// Writes a double value to the stream.
        /// </summary>
        /// <param name="_Value">The double value to write.</param>
        /// <remarks>
        /// This method converts the double to a long and writes it to the stream.
        /// </remarks>
        public void Write(double _Value)
        {
            BinaryWriter.doubleConverter.DoubleValue = _Value;
            this.Write(BinaryWriter.doubleConverter.LongValue);
        }

        /// <summary>
        /// Writes a decimal value to the stream.
        /// </summary>
        /// <param name="_Value">The decimal value to write.</param>
        /// <remarks>
        /// This method writes the decimal as four int values.
        /// </remarks>
        public void Write(decimal _Value)
        {
            int[] bits = decimal.GetBits(_Value);
            this.Write(bits[0]);
            this.Write(bits[1]);
            this.Write(bits[2]);
            this.Write(bits[3]);
        }

        /// <summary>
        /// Writes a reference to a string value to the stream.
        /// </summary>
        /// <param name="_Value">The string to write.</param>
        /// <remarks>
        /// If the string is null, writes -1 to the stream. Otherwise, writes the string length followed by its bytes.
        /// Throws an exception if the string exceeds the maximum allowed length.
        /// </remarks>
        public void Write(string _Value)
        {
            if (_Value == null)
            {
                this.Write((int)-1);
            }
            else
            {
                int byteCount = BinaryWriter.encoding.GetByteCount(_Value);
                if (byteCount >= MAX_STRING_LENGTH)
                {
                    throw new IndexOutOfRangeException("Serialize(string) too long: " + _Value.Length + "! Maximal length: " + MAX_STRING_LENGTH);
                }
                this.Write((int)byteCount);
                int bytes = BinaryWriter.encoding.GetBytes(_Value, 0, _Value.Length, BinaryWriter.stringWriteBuffer, 0);
                this.buffer.WriteBytes(BinaryWriter.stringWriteBuffer, (int)bytes);
            }
        }

        /// <summary>
        /// Writes a reference to a boolean value to the stream.
        /// </summary>
        /// <param name="_Value">The boolean to write.</param>
        /// <remarks>
        /// Writes 1 for true and 0 for false.
        /// </remarks>
        public void Write(bool _Value)
        {
            if (_Value)
            {
                this.buffer.WriteByte(1);
            }
            else
            {
                this.buffer.WriteByte(0);
            }
        }

        /// <summary>
        /// Writes a reference to a byte array to the stream.
        /// </summary>
        /// <param name="_Buffer">The byte array to write.</param>
        /// <param name="_Count">The number of bytes to write from the array.</param>
        /// <remarks>
        /// Logs an error if the byte array exceeds the maximum allowed length.
        /// </remarks>
        public void Write(byte[] _Buffer, int _Count)
        {
            if (_Count > MAX_BYTE_LENGTH)
            {
                Debug.LogError("BinaryWriter Write: buffer is too large (" + _Count + ") bytes. The maximum buffer size is 2000M bytes.");
            }
            else
            {
                this.buffer.WriteBytes(_Buffer, (int)_Count);
            }
        }

        /// <summary>
        /// Writes a reference to a byte array to the stream starting from an offset.
        /// </summary>
        /// <param name="_Buffer">The byte array to write.</param>
        /// <param name="_Offset">The offset in the array to start writing from.</param>
        /// <param name="_Count">The number of bytes to write from the array starting at the offset.</param>
        /// <remarks>
        /// Logs an error if the byte array exceeds the maximum allowed length.
        /// </remarks>
        public void Write(byte[] _Buffer, int _Offset, int _Count)
        {
            if (_Count > MAX_BYTE_LENGTH)
            {
                Debug.LogError("BinaryWriter Write: buffer is too large (" + _Count + ") bytes. The maximum buffer size is 2000M bytes.");
            }
            else
            {
                this.buffer.WriteBytesAtOffset(_Buffer, (int)_Offset, (int)_Count);
            }
        }

        /// <summary>
        /// Writes a 32-bit count followed by a byte array of that length to the stream.
        /// </summary>
        /// <param name="_Buffer">The byte array to write.</param>
        /// <param name="_Count">The number of bytes to write from the array.</param>
        /// <remarks>
        /// Writes the byte count first. If the count exceeds the maximum allowed length, logs an error.
        /// </remarks>
        public void WriteBytesAndSize(byte[] _Buffer, int _Count)
        {
            if (_Buffer == null || _Count == 0)
            {
                this.Write((int)0);
            }
            else if (_Count > MAX_BYTE_LENGTH)
            {
                Debug.LogError("BinaryWriter WriteBytesAndSize: buffer is too large (" + _Count + ") bytes. The maximum buffer size is 2000M bytes.");
            }
            else
            {
                this.Write((int)_Count);
                this.buffer.WriteBytes(_Buffer, (int)_Count);
            }
        }

        /// <summary>
        /// Writes a byte array to the stream, preceded by its length.
        /// </summary>
        /// <param name="_Buffer">The byte array to write.</param>
        /// <remarks>
        /// If the array is null or exceeds the maximum length, appropriate action is taken.
        /// </remarks>
        public void WriteBytesFull(byte[] _Buffer)
        {
            if (_Buffer == null)
            {
                this.Write((int)0);
            }
            else if (_Buffer.Length > MAX_BYTE_LENGTH)
            {
                Debug.LogError("BinaryWriter WriteBytes: buffer is too large (" + _Buffer.Length + ") bytes. The maximum buffer size is 2000M bytes.");
            }
            else
            {
                this.Write((int)_Buffer.Length);
                this.buffer.WriteBytes(_Buffer, (int)_Buffer.Length);
            }
        }

        /// Writes a reference to a Vector2 value to the stream.
        /// </summary>
        /// <param name="_Value">The Vector2 object to write.</param>
        /// <remarks>
        /// Writes the x and y components of the Vector2 object.
        /// </remarks>
        public void Write(Vector2 _Value)
        {
            this.Write(_Value.x);
            this.Write(_Value.y);
        }

        /// Writes a reference to a Vector2Int value to the stream.
        /// </summary>
        /// <param name="_Value">The Vector2Int object to write.</param>
        /// <remarks>
        /// Writes the x and y components of the Vector2Int object.
        /// </remarks>
        public void Write(Vector2Int _Value)
        {
            this.Write(_Value.x);
            this.Write(_Value.y);
        }

        /// <summary>
        /// Writes a reference to a Vector3 value to the stream.
        /// </summary>
        /// <param name="_Value">The Vector3 object to write.</param>
        /// <remarks>
        /// Writes the x, y, and z components of the Vector3 object.
        /// </remarks>
        public void Write(Vector3 _Value)
        {
            this.Write(_Value.x);
            this.Write(_Value.y);
            this.Write(_Value.z);
        }

        /// <summary>
        /// Writes a reference to a Vector3Int value to the stream.
        /// </summary>
        /// <param name="_Value">The Vector3Int object to write.</param>
        /// <remarks>
        /// Writes the x, y, and z components of the Vector3Int object.
        /// </remarks>
        public void Write(Vector3Int _Value)
        {
            this.Write(_Value.x);
            this.Write(_Value.y);
            this.Write(_Value.z);
        }

        /// <summary>
        /// Writes a reference to a Vector4 value to the stream.
        /// </summary>
        /// <param name="_Value">The Vector4 object to write.</param>
        /// <remarks>
        /// Writes the x, y, z, and w components of the Vector4 object.
        /// </remarks>
        public void Write(Vector4 _Value)
        {
            this.Write(_Value.x);
            this.Write(_Value.y);
            this.Write(_Value.z);
            this.Write(_Value.w);
        }

        /// <summary>
        /// Writes a reference to a Color value to the stream.
        /// </summary>
        /// <param name="_Value">The Color object to write.</param>
        /// <remarks>
        /// Writes the r, g, b, and a components of the Color object.
        /// </remarks>
        public void Write(Color _Value)
        {
            this.Write(_Value.r);
            this.Write(_Value.g);
            this.Write(_Value.b);
            this.Write(_Value.a);
        }

        /// <summary>
        /// Writes a reference to a Color32 value to the stream.
        /// </summary>
        /// <param name="_Value">The Color32 object to write.</param>
        /// <remarks>
        /// Writes the r, g, b, and a components of the Color32 object as bytes.
        /// </remarks>
        public void Write(Color32 _Value)
        {
            this.Write(_Value.r);
            this.Write(_Value.g);
            this.Write(_Value.b);
            this.Write(_Value.a);
        }

        /// <summary>
        /// Writes a reference to a Quaternion value to the stream.
        /// </summary>
        /// <param name="_Value">The Quaternion object to write.</param>
        /// <remarks>
        /// Writes the x, y, z, and w components of the Quaternion object.
        /// </remarks>
        public void Write(Quaternion _Value)
        {
            this.Write(_Value.x);
            this.Write(_Value.y);
            this.Write(_Value.z);
            this.Write(_Value.w);
        }

        /// <summary>
        /// Writes a reference to a Rect value to the stream.
        /// </summary>
        /// <param name="_Value">The Rect object to write.</param>
        /// <remarks>
        /// Writes the xMin, yMin, width, and height components of the Rect object.
        /// </remarks>
        public void Write(Rect _Value)
        {
            this.Write(_Value.xMin);
            this.Write(_Value.yMin);
            this.Write(_Value.width);
            this.Write(_Value.height);
        }

        /// <summary>
        /// Writes a reference to a Plane value to the stream.
        /// </summary>
        /// <param name="_Value">The Plane object to write.</param>
        /// <remarks>
        /// Writes the normal vector and the distance of the Plane object.
        /// </remarks>
        public void Write(Plane _Value)
        {
            this.Write(_Value.normal);
            this.Write(_Value.distance);
        }

        /// <summary>
        /// Writes a reference to a Ray value to the stream.
        /// </summary>
        /// <param name="_Value">The Ray object to write.</param>
        /// <remarks>
        /// Writes the direction and origin components of the Ray object.
        /// </remarks>
        public void Write(Ray _Value)
        {
            this.Write(_Value.origin);
            this.Write(_Value.direction);
        }

        /// <summary>
        /// Writes a reference to a Matrix4x4 value to the stream.
        /// </summary>
        /// <param name="_Value">The Matrix4x4 object to write.</param>
        /// <remarks>
        /// Writes all 16 components (m00 to m33) of the Matrix4x4 object.
        /// </remarks>
        public void Write(Matrix4x4 _Value)
        {
            this.Write(_Value.m00);
            this.Write(_Value.m01);
            this.Write(_Value.m02);
            this.Write(_Value.m03);
            this.Write(_Value.m10);
            this.Write(_Value.m11);
            this.Write(_Value.m12);
            this.Write(_Value.m13);
            this.Write(_Value.m20);
            this.Write(_Value.m21);
            this.Write(_Value.m22);
            this.Write(_Value.m23);
            this.Write(_Value.m30);
            this.Write(_Value.m31);
            this.Write(_Value.m32);
            this.Write(_Value.m33);
        }
    }
}
