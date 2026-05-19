using RPLidar4Net.Api.Data;
using RPLidar4Net.Api.Helpers;
using System.Net;
using System.Net.Sockets;
using VL.Core.Import;
using VL.Lib.Basics.Resources;
using VL.Lib.Collections;
using NetSocket = System.Net.Sockets.Socket;

namespace VL.Devices.RPLidar.Advanced
{
    public enum RPLidarUDPState
    {
        Idle,
        ScanSimple,
        ScanDense,
        ScanHighQuality,
        NextPacketIsInfo,
    }

    public enum RPLidarUDPWorkMode
    {
        Legacy = 0x0,
        Unknown1 = 0x1,
        Unknown2 = 0x2,
    }

    public static class RPLidarUDPSocketHelper
    {
        public static bool IPEndPointEquals(IPEndPoint? input, IPEndPoint? input2)
        {
            if (ReferenceEquals(input, input2))
                return true;
            if (ReferenceEquals(input, null))
                return false;
            return input.Equals(input2);
        }

        public static byte[] FormatCommand(Command command, byte[] payload, bool includePayloadSize)
        {
            byte commandByte = CommandHelper.GetByte(command);

            var packetBytes = new List<byte>();
            packetBytes.Add(Constants.SYNC_BYTE);
            packetBytes.Add(commandByte);

            //Add payload
            if (payload != null)
            {
                if (includePayloadSize)
                    packetBytes.Add((byte)payload.Length);
                packetBytes.AddRange(payload);
                byte checksum = 0;
                foreach (var b in packetBytes)
                    checksum ^= b;
                packetBytes.Add(checksum);
            }

            var packet = packetBytes.ToArray();

            return packet;
        }

        public static byte[] RESPONSE_DESCRIPTOR_GET_INFO = new byte[]
        {
            0xA5,
            0x5A,
            0x14,
            0x0,
            0x0,
            0x0,
            0x4,
        };

        public static bool IsGetInfoResponse(Spread<byte> bytes) =>
            bytes.ToArray().SequenceEqual(RESPONSE_DESCRIPTOR_GET_INFO);

        public static InfoDataResponse ReadInfoDataResponse(byte[] bytes) =>
            InfoDataResponseHelper.ToInfoDataResponse(bytes);

        public static byte[] RESPONSE_DESCRIPTOR_SCAN_SCAN = new byte[]
        {
            0xA5,
            0x5A,
            0x05,
            0x0,
            0x0,
            0x40,
            0x81,
        };
        public static byte[] RESPONSE_DESCRIPTOR_SCAN_DENSE = new byte[]
        {
            0xA5,
            0x5A,
            0x54,
            0x00,
            0x00,
            0x40,
            0x85,
        };
        public static byte[] RESPONSE_DESCRIPTOR_SCAN_HIGHQL = new byte[]
        {
            0xA5,
            0x5A,
            0x0D,
            0x03,
            0x0,
            0x40,
            0x83,
        };

        public static bool IsScanResponse(byte[] bytes) =>
            bytes.SequenceEqual(RESPONSE_DESCRIPTOR_SCAN_SCAN);

        public static bool IsScanDenseResponse(byte[] bytes) =>
            bytes.SequenceEqual(RESPONSE_DESCRIPTOR_SCAN_DENSE);

        public static bool IsScanHightQlResponse(byte[] bytes) =>
            bytes.SequenceEqual(RESPONSE_DESCRIPTOR_SCAN_HIGHQL);

        public static byte[] FormattedExpressScanCommand(
            RPLidarUDPWorkMode wm = RPLidarUDPWorkMode.Legacy
        ) =>
            //FormatCommand(Command.ExpressScan, CommandHelper.GetExpressScanPayload((byte)wm), true);
            RPLidarUDPSocketHelper.FormatCommand(
                Command.ExpressScan,
                new byte[] { (byte)wm, 0x0, 0x0, 0x0, 0x0 },
                true
            );
    }

    [ProcessNode(Name = "RPLidar", Category = "RPLidar.UDPSocket")]
    public class RPLidarUDPSocketNode
    {
        ResourceProviderMonitor<NetSocket>? _provider;
        private bool _connect = false;
        private IPEndPoint? _remoteEndpoint;

        public IResourceProvider<NetSocket> Update(
            IPEndPoint remoteEndpoint,
            bool connect,
            bool enabled = true
        )
        {
            if (
                !RPLidarUDPSocketHelper.IPEndPointEquals(_remoteEndpoint, remoteEndpoint)
                || _connect != connect
            )
            {
                _connect = connect;
                _remoteEndpoint = remoteEndpoint;

                _provider = ResourceProvider
                    .New(() =>
                    {
                        var socket = new NetSocket(SocketType.Dgram, ProtocolType.Udp);
                        socket.ExclusiveAddressUse = false;
                        socket.SetSocketOption(
                            SocketOptionLevel.Socket,
                            SocketOptionName.ReuseAddress,
                            true
                        );
                        if (connect && remoteEndpoint != null)
                            socket.Connect(remoteEndpoint);
                        return socket;
                    })
                    .ShareInParallel()
                    .Monitor();
            }

            if (enabled)
                return _provider;
            return null;
        }

        public bool IsOpen => _provider?.SinkCount > 0;
    }
}
