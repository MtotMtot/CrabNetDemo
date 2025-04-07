using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Text;

namespace GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
        }

        public static void Sector1State(int _fromClient, Packet _packet)
        {
            bool Sector1State = _packet.ReadBool();

            Console.WriteLine($"Received Sector1state from host : {Sector1State}");
            if (Sector1State)
            {
                ServerSend.Sector1Clear(_fromClient);
            }
        }
    }
}