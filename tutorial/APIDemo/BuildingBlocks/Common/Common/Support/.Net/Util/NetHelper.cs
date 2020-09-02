////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 4/17/2009 4:44:50 PM 
// Description: NetHelper.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace Common.Support.Net.Util
{
    /// <summary>
    ///		Class that holds the Pack information
    /// </summary>
    public class IcmpPacket
    {
        public Byte Type;    // type of message
        public Byte SubCode;    // type of sub code
        public UInt16 CheckSum;   // ones complement checksum of struct
        public UInt16 Identifier;      // identifier
        public UInt16 SequenceNumber;     // sequence number  
        public Byte[] Data;

    } 
    static public class NetHelper
    {
        public  enum  PortStatus
        {
            Used,
            Unused,            
            Unknown
        }
        static public PortStatus PingPort(string hostname, int portNo)
        {
            IPAddress ipa = (IPAddress)Dns.GetHostAddresses(hostname)[0];
            try
            {
                using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.Connect(ipa, portNo);
                    return (sock.Connected)?PortStatus.Used: PortStatus.Unused;
                }
            }
            catch (SocketException ex)
            {
                // Port is unused and could not establish connection 
                return (ex.ErrorCode == 10061) ? PortStatus.Unused : PortStatus.Unknown;
            }
            catch
            {
                return PortStatus.Unknown;
            }
        }

        static public string GetLocalhostName()
        {
            return Dns.GetHostName();
        }
        static public IPHostEntry GetLocalIP()
        {
            return GetIPHost(GetLocalhostName());
        }
        
        static public IPHostEntry GetIPHost(string hostName)
        {
            return Dns.GetHostEntry(hostName);           
        }

        public static bool PingHost(string host)
        {
            return PingHost(host, 1000);
        }

        public static bool PingHost(string host,int timeout)
        {
            using (Ping pingSender = new Ping())
            {
                PingOptions options = new PingOptions();

                // Use the default Ttl value which is 128,
                // but change the fragmentation behavior.
                options.DontFragment = true;

                // Create a buffer of 32 bytes of data to be transmitted.
                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                PingReply reply = pingSender.Send(Dns.GetHostAddresses(host)[0], timeout, buffer, options);
                return reply.Status == IPStatus.Success;
            }
        }
        
        static readonly Regex ipRegex = new Regex("\\b\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\b");
    }
}
