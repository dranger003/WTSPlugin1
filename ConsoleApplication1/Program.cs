using System;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = PInvoke.WTSVirtualChannelOpenEx(PInvoke.WTS_CURRENT_SESSION, "Channel1", PInvoke.WTS_CHANNEL_OPTION_DYNAMIC);
            if (channel!= IntPtr.Zero)
            {
                IntPtr ptr;
                uint bytes;

                if (PInvoke.WTSVirtualChannelQuery(
                    channel,
                    PInvoke.WTS_VIRTUAL_CLASS.WTSVirtualFileHandle,
                    out ptr,
                    out bytes))
                {
                    System.Diagnostics.Debug.Assert(bytes == 8);

                    var handle = Marshal.ReadIntPtr(ptr);
                    IntPtr file;

                    PInvoke.DuplicateHandle(
                        PInvoke.GetCurrentProcess(),
                        handle,
                        PInvoke.GetCurrentProcess(),
                        out file,
                        0,
                        false,
                        PInvoke.DUPLICATE_SAME_ACCESS
                    );

                    PInvoke.WTSFreeMemory(ptr);
                    ptr = IntPtr.Zero;
                }

                PInvoke.WTSVirtualChannelClose(channel);
                channel = IntPtr.Zero;
            }
        }
    }
}
