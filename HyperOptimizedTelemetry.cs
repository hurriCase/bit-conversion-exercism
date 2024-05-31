using System;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

public static class TelemetryBuffer
{
    public static byte[] ToBuffer(long reading)
    {
        byte[] bytes = new byte[9];
        if (reading < int.MinValue || reading > uint.MaxValue)
        {
            BitConverter.GetBytes(Convert.ToInt64(reading)).CopyTo(bytes, 1);
            bytes[0] = 256 - sizeof(long);
        }
        else if (reading > int.MaxValue)
        {
            BitConverter.GetBytes(Convert.ToUInt32(reading)).CopyTo(bytes, 1);
            bytes[0] = sizeof(uint);
        }
        else if (reading < short.MinValue || reading > ushort.MaxValue)
        {
            BitConverter.GetBytes(Convert.ToInt32(reading)).CopyTo(bytes, 1);
            bytes[0] = 256 - sizeof(int);
        }        
        else if (reading < 0)
        {
            BitConverter.GetBytes(Convert.ToInt16(reading)).CopyTo(bytes, 1);
            bytes[0] = 256 - sizeof(short);
        }
        else if (reading >= 0)
        {
            BitConverter.GetBytes(Convert.ToUInt16(reading)).CopyTo(bytes, 1);
            bytes[0] = sizeof(ushort);
        }

        return bytes;
    }
    public static long FromBuffer(byte[] buffer) =>
        buffer[0] switch
        {
            2 => BitConverter.ToUInt16(buffer[1..]),
            4 => BitConverter.ToUInt32(buffer[1..]),
            256 - sizeof(short) => BitConverter.ToInt16(buffer[1..]),
            256 - sizeof(int) => BitConverter.ToInt32(buffer[1..]),
            256 - sizeof(long) => BitConverter.ToInt64(buffer[1..]),
            _ => 0
        };        
}
