using System;

namespace CaptureMod.Voice
{
    public class MyCircularBuffer
    {
        public readonly float[] buffer;
        private readonly byte[][] Listeners;
        public readonly object lockObject;
        private int writePosition;
        private int readPosition;
        private int floatCount;
        private readonly int FloatPerRead;

        public MyCircularBuffer(int size, int floatPerRead)
        {
            buffer = new float[size];
            lockObject = new object();
            FloatPerRead = floatPerRead;
            Listeners = new byte[size / FloatPerRead][];
        }

        public int Write(float[] data, int offset, int count, byte [] listeners)
        {
            lock (lockObject)
            {
                Listeners[writePosition/FloatPerRead] = listeners;
                
                count = Math.Min(count, buffer.Length);
                var right = Math.Min(buffer.Length - writePosition, count);
                Buffer.BlockCopy(data, offset*4, buffer, writePosition*4, right*4);
                writePosition = (writePosition + right) % buffer.Length;
                var left = count - right;
                if (left > 0)
                {
                    Buffer.BlockCopy(data, (offset + right)*4, buffer, writePosition*4, left*4);
                    writePosition += left;
                }
                floatCount += count;
                if (floatCount > buffer.Length)
                {
                    readPosition = writePosition;
                    floatCount = buffer.Length;
                }
                
                return count;
            }
        }

        public int Read(float[] data, int offset, int count)
        {
            lock (this.lockObject)
            {
                if (count > this.floatCount)
                    count = this.floatCount;
                int num1 = 0;
                int length = Math.Min(this.buffer.Length - this.readPosition, count);
                Array.Copy(buffer, readPosition,  data, offset, length);
                int num2 = num1 + length;
                this.readPosition += length;
                this.readPosition %= this.buffer.Length;
                if (num2 < count)
                {
                    Array.Copy(buffer, readPosition, data, offset + num2, count - num2);
                    this.readPosition += count - num2;
                    num2 = count;
                }
                this.floatCount -= num2;
                return num2;
            }
        }
        
        public int Read(byte[] data, int offset, int ByteCount, out byte[] listeners)
        {
            byte[] b;
            int i;
            lock (lockObject)
            {
                listeners = Listeners[readPosition/FloatPerRead];
                for (i = offset; i < ByteCount && floatCount>0; i+=2, floatCount--, readPosition = (readPosition + 1) % MaxLength)
                {
                    b = BitConverter.GetBytes((short) (buffer[readPosition]*32768f));
                    //("="+buffer[readPosition]*32768f + " " + string.Join(",", b)).Log();
                    data[i] = b[0];
                    data[i + 1] = b[1];
                    
                }
            } 
            return i*2;
        }

        public int MaxLength => this.buffer.Length;

        public int Count
        {
            get
            {
                lock (this.lockObject)
                    return this.floatCount;
            }
        }

        public void Reset()
        {
            lock (this.lockObject)
                this.ResetInner();
        }

        private void ResetInner()
        {
            this.floatCount = 0;
            this.readPosition = 0;
            this.writePosition = 0;
        }

        public void Advance(int count)
        {
            lock (this.lockObject)
            {
                if (count >= this.floatCount)
                {
                    this.ResetInner();
                }
                else
                {
                    this.floatCount -= count;
                    this.readPosition += count;
                    this.readPosition %= this.MaxLength;
                }
            }
        }
    }
}