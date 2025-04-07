using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    class ServerSend
    {
        /// <summary>Sends a TCP packet to a client.</summary>
        /// <param name="_toClient">The client ID of the recipient.</param>
        /// <param name="_packet">The packet to send.</param>
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        /// <summary>Sends a UDP packet to a client.</summary>
        /// <param name="_toClient">The client ID of the recipient.</param>
        /// <param name="_packet">The packet to send.</param>
        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].udp.SendData(_packet);
        }

        /// <summary>Sends a TCP packet to all clients.</summary>
        /// <param name="_packet">The packet to send.</param>
        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }

        /// <summary>Sends a TCP packet to all clients except one.</summary>
        /// <param name="_exceptClient">The client ID of the recipient to exclude.</param>
        /// <param name="_packet">The packet to send.</param>
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        /// <summary>Sends a UDP packet to all clients.</summary>
        /// <param name="_packet">The packet to send.</param>
        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }

        /// <summary>Sends a UDP packet to all clients except one.</summary>
        /// <param name="_exceptClient">The client ID of the recipient to exclude.</param>
        /// <param name="_packet">The packet to send.</param>
        private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].udp.SendData(_packet);
                }
            }
        }

        #region Packets
        /// <summary>Sends a welcome packet to a client.</summary>
        /// <param name="_toClient">The client ID of the recipient.</param>
        /// <param name="_msg">The message to send.</param>
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        /// <summary>Sends a sector 1 clear packet to a client.</summary>
        /// <param name="_toClient">The client ID of the recipient.</param>
        public static void Sector1Clear(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerPackets.Sector1Clear))
            {
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
        #endregion
    }
}