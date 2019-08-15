﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;

namespace Microsoft.Protocols.TestTools.StackSdk.FileAccessService.WSP
{

    /// <summary>
    /// Helper class provides utility methods which are used through 
    /// out Adapter implementation
    /// and response validation
    /// </summary>
    public class Helper
    {
        #region  Utility Methods
        /// <summary>
        /// Read unsigned Int from a byte array
        /// </summary>
        /// <param name="bytes">Byte array</param>
        /// <param name="startingIndex">Starting Index to read</param>
        /// <returns>unsigned int</returns>
        public static uint GetUInt(byte[] bytes, ref int startingIndex)
        {
            byte[] tempArray = new byte[Constant.SIZE_OF_UINT];
            for (int i = 0; i < Constant.SIZE_OF_UINT; i++)
            {
                tempArray[i] = bytes[i + startingIndex];
            }
            startingIndex += Constant.SIZE_OF_UINT;
            // Values are retrieved in little indian format
            return BitConverter.ToUInt32(tempArray, 0);

        }

        /// <summary>
        /// Appends bytes to the already existing Array
        /// </summary>
        /// <param name="mainBlob">destination array where 
        /// new bytes are to be appended</param>
        /// <param name="index">starting index (from where to append)</param>
        /// <param name="bytesTobeAppended">source bytes</param>
        public static void CopyBytes(byte[] mainBlob,
            ref int index, byte[] bytesTobeAppended)
        {
            if (mainBlob.Length - index >= bytesTobeAppended.Length)
            {
                for (int i = 0; i < bytesTobeAppended.Length; i++)
                {
                    mainBlob[i + index] = bytesTobeAppended[i];
                }
                index += bytesTobeAppended.Length;
            }
            else
            {
                throw new IndexOutOfRangeException(
                    "Source buffer is not suffcient for the copy Operation");
            }
        }

        /// <summary>
        /// Retrieves a data as byte array from a containing BLOB
        /// </summary>
        /// <param name="bytes">Parent BLOB object</param>
        /// <param name="index">Index from where the data
        /// should be retrieved</param>
        /// <param name="dataWidth">Length of the data field</param>
        /// <returns></returns>
        public static byte[] GetData(byte[] bytes, ref int index, int dataWidth)
        {
            byte[] tempArray = new byte[dataWidth];
            for (int i = 0; i < dataWidth; i++)
            {
                tempArray[i] = bytes[i + index];
            }
            index += dataWidth;
            return tempArray;
        }

        #endregion

        # region Requirements Validation Methods

        /// <summary>
        /// A short cut for Contracts.Requires using a condition,
        /// requirement id and description. 
        /// Every requires which captures a requirement should use this.
        /// </summary>
        /// <param name="condition">Condition to be verified</param>
        /// <param name="requirementId">Requirement Id</param>
        /// <param name="description">Requirement Description or text</param>
        public static void Requires(bool condition,
            int requirementId, string description)
        {
            //Contracts.Requires(condition, 
            //    MakeReqId(requirementId, description));
        }

        /// <summary>
        /// A short cut for Contracts.Requires using a requirement id and description. 
        /// Every requires which captures a requirement should use this.
        /// </summary>
        public static void RequiresCapture(int requirementId,
            string description)
        {
            //Requirements.Capture(MakeReqId(requirementId, description));
        }

        # endregion

        /// <summary>
        /// Fetches 4 byte unsigned integer from a BLOB without traversing
        /// </summary>
        /// <param name="bytes">BLOB to read 4 bytes from</param>
        /// <param name="startingIndex">offset in the BLOB</param>
        /// <returns>4 byte unsigned integer</returns>
        public static uint GetUIntWithOutAdvancing(byte[] bytes,
            int startingIndex)
        {
            byte[] tempArray = new byte[Constant.SIZE_OF_UINT];
            for (int i = 0; i < Constant.SIZE_OF_UINT; i++)
            {
                tempArray[i] = bytes[i + startingIndex];
            }
            return BitConverter.ToUInt32(tempArray, 0);
        }

        /// <summary>
        /// Returns the Storage SIZE of a given BaseStorageVariant type
        /// </summary>
        /// <param name="type">StorageType</param>
        /// <returns>size in bytes</returns>
        static public ushort GetSize(StorageType type)
        {
            ushort size = 0;
            switch (type)
            {
                case StorageType.VT_EMPTY:
                    break;
                case StorageType.VT_NULL:
                    break;
                case StorageType.VT_I1:
                case StorageType.VT_UI1:
                    size = 1; // Take 1 Byte
                    break;
                case StorageType.VT_I2:
                case StorageType.VT_UI2:
                case StorageType.VT_BOOL:
                    size = 2; // Take 2 Bytes
                    break;
                case StorageType.VT_I4:
                case StorageType.VT_UI4:
                case StorageType.VT_INT:
                case StorageType.VT_UINT:
                case StorageType.VT_ERROR:
                case StorageType.VT_R4:
                    size = 4; // Take 4 byte
                    break;
                case StorageType.VT_I8:
                case StorageType.VT_UI8:
                case StorageType.VT_CY:
                case StorageType.VT_R8:
                case StorageType.VT_DATE:
                case StorageType.VT_CLSID:
                case StorageType.VT_FILETIME:
                    size = 8;
                    break;

                case StorageType.VT_DECIMAL:
                    size = 12;
                    break;
                case StorageType.VT_VARIANT:
                    size = 16;
                    break;
                default:
                    break;
            }
            return size;
        }

        public static byte[] ToBytes(IWspObject message)
        {
            var buffer = new WspBuffer();

            message.ToBytes(buffer);

            return buffer.GetBytes();
        }

        public static void FromBytes<T>(ref T t, byte[] bytes) where T : struct, IWspObject
        {
            var buffer = new WspBuffer(bytes);

            t.FromBytes(buffer);
        }

        public static UInt32 CalculateCheckSum(WspMessageHeader_msg_Values msg, byte[] messageBlob)
        {
            UInt32 checksum = 0;

            if (messageBlob.Length % 4 != 0)
            {
                Array.Resize
                    (ref messageBlob,
                    messageBlob.Length
                    + (4 - messageBlob.Length % 4));
            }

            int index = 0;

            while (index != messageBlob.Length)
            {
                checksum += Helper.GetUInt(messageBlob, ref index);
            }

            checksum = checksum ^ 0x59533959;
            checksum -= (UInt32)msg;

            return checksum;
        }
    }
}
