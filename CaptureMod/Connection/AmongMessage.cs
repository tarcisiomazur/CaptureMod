﻿using System;
using System.Runtime.InteropServices;
using CaptureMod.Utils;

namespace CaptureMod.Connection
{
    
    
    [StructLayout(LayoutKind.Explicit)]
    public class VoipMessage : IDisposable
    {
        [FieldOffset(0)]
        public byte[] _byteMessage;
        [FieldOffset(0)]
        public float[] _floatMessage;

        [FieldOffset(8)]
        public int DataOffset = 0;
        [FieldOffset(12)]
        public bool Send;
        
        private static readonly int _ByteOffset = 12;
        private static readonly int _FloatOffset = 3;

        public int FloatOffset => _FloatOffset + DataOffset/4;
        public int FloatSize => (_byteMessage.Length-DataOffset-_ByteOffset)/4;
        
        public byte Source
        {
            get => _byteMessage[DataOffset];
            set => _byteMessage[DataOffset] = value;
        }

        public byte[] Listeners
        {
            get => _byteMessage.SubArray(DataOffset+2, ListenerCount);
        }

        public (byte[], long, long) Audio()
        {
            return (_byteMessage, DataOffset+_ByteOffset, _byteMessage.Length - _ByteOffset-DataOffset);
        }
        
        public (float[], long, long) FloatAudio()
        {
            return (_floatMessage, FloatOffset, FloatSize);
        }

        public float[] FloatBuffer
        {
            set
            {
                if (value.Length > _byteMessage.Length - _ByteOffset - DataOffset)
                {
                    var temp = new byte[value.Length + _ByteOffset+DataOffset];
                    System.Array.Copy(_byteMessage, 0, temp, 0, _ByteOffset+DataOffset);
                    _byteMessage = temp;
                }

                Buffer.BlockCopy(value, 0, _byteMessage, _ByteOffset+DataOffset, value.Length);
            }
        }

        public void AddListener(byte id)
        {
            _byteMessage[DataOffset+ListenerCount+2] = id;
            ListenerCount += 1;
        }
        
        public byte[] ByteArray => _byteMessage;

        public byte ListenerCount
        {
            get => _byteMessage[DataOffset+1];
            set => _byteMessage[DataOffset+1] = value;
        }
        public float[] FloatArray => _floatMessage;

        public VoipMessage(int len)
        {
            _byteMessage = new byte[len + _ByteOffset+DataOffset];
        }
        
        public VoipMessage(float[] buffer, long offset, long size)
        {
            _byteMessage = new byte[size*4 + _ByteOffset+DataOffset];
            Buffer.BlockCopy(buffer, 0, _byteMessage, _ByteOffset+DataOffset, (int) size*4);
        }
        
        public VoipMessage(byte[] buffer, long offset, long size)
        {
            _byteMessage = new byte[size + _ByteOffset+DataOffset];
            Buffer.BlockCopy(buffer, 0, _byteMessage, _ByteOffset+DataOffset, (int) size);
        }
        public VoipMessage(byte[] buffer, long offset)
        {
            DataOffset = (int) offset;
            _byteMessage = buffer;
        }

        public void Dispose()
        {

        }
        
    }

}