using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = PInvoke.WTSVirtualChannelOpenEx(PInvoke.WTS_CURRENT_SESSION, "Channel1", PInvoke.WTS_CHANNEL_OPTION_DYNAMIC);
            if (channel != IntPtr.Zero)
            {
                IntPtr ptr;
                uint bytes;

                if (PInvoke.WTSVirtualChannelQuery(
                    channel,
                    PInvoke.WTS_VIRTUAL_CLASS.WTSVirtualFileHandle,
                    out ptr,
                    out bytes))
                {
                    var handle = Marshal.ReadIntPtr(ptr);
                    IntPtr file;

                    if (PInvoke.DuplicateHandle(
                        PInvoke.GetCurrentProcess(),
                        handle,
                        PInvoke.GetCurrentProcess(),
                        out file,
                        0,
                        false,
                        PInvoke.DUPLICATE_SAME_ACCESS))
                    {
                        PInvoke.WTSFreeMemory(ptr);
                        ptr = IntPtr.Zero;

                        var data = File.ReadAllBytes(@"X:\1G-FILE.dat");

                        var ovl = new NativeOverlapped();
                        ovl.EventHandle = PInvoke.CreateEvent(IntPtr.Zero, true, false, null);

                        PInvoke.WriteFile(file, data, (uint)data.Length, out bytes, IntPtr.Zero);
                        if (PInvoke.GetOverlappedResult(file, ref ovl, out bytes, true))
                        {
                            var header = new PInvoke.CHANNEL_PDU_HEADER();

                            {
                                var buffer = new byte[Marshal.SizeOf<PInvoke.CHANNEL_PDU_HEADER>()];
                                PInvoke.ReadFile(file, buffer, (uint)buffer.Length, out bytes, IntPtr.Zero);
                                header = ByteArrayToStructure<PInvoke.CHANNEL_PDU_HEADER>(buffer);
                            }

                            {
                                var buffer = new byte[header.length];
                                if (PInvoke.ReadFile(file, buffer, (uint)buffer.Length, out bytes, IntPtr.Zero))
                                {
                                    Console.WriteLine(Encoding.UTF8.GetString(buffer));
                                }
                                else
                                {
                                    Console.WriteLine("ReadFile(): {0}", Marshal.GetLastWin32Error());
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("WriteFile(): {0}", Marshal.GetLastWin32Error());
                        }
                    }
                    else
                    {
                        Console.WriteLine("DuplicateHandle(): {0}", Marshal.GetLastWin32Error());
                    }
                }
                else
                {
                    Console.WriteLine("WTSVirtualChannelQuery(): {0}", Marshal.GetLastWin32Error());
                }

                PInvoke.WTSVirtualChannelClose(channel);
                channel = IntPtr.Zero;
            }
            else
            {
                Console.WriteLine("WTSVirtualChannelOpenEx(): {0}", Marshal.GetLastWin32Error());
            }

            Console.WriteLine("\nPress <any key> to continue.");
            Console.ReadKey(true);
        }

        static T ByteArrayToStructure<T>(byte[] data) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return result;
        }

        static byte[] StructureToByteArray<T>(T data) where T : struct
        {
            var result = new byte[Marshal.SizeOf(data)];
            var ptr = Marshal.AllocHGlobal(result.Length);
            Marshal.StructureToPtr(data, ptr, false);
            Marshal.Copy(ptr, result, 0, result.Length);
            Marshal.FreeHGlobal(ptr);

            return result;
        }
    }
}
