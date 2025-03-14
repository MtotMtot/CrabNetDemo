using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    /// <summary>
    /// Received welcome packet from client, send that client into game and initialize their data.
    /// </summary>
    /// <param name="_fromClient"></param>
    /// <param name="_packet"></param>
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.serverClients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.serverClients[_fromClient].SendIntoGame(_username);
    }

    /// <summary>
    /// Receive player movement from client
    /// </summary>
    /// <param name="_fromClient"></param>
    /// <param name="_packet"></param>
    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.serverClients[_fromClient].player.SetInput(_position, _rotation);
    }

    /// <summary>
    /// Receive player shooting from client
    /// </summary>
    /// <param name="_fromClient"></param>
    /// <param name="_packet"></param>
    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _camPos = _packet.ReadVector3();
        Quaternion _camRot = _packet.ReadQuaternion();

        Server.serverClients[_fromClient].player.Shoot(_camPos, _camRot);
    }

    /// <summary>
    /// Receive enemy damaged from client
    /// </summary>
    /// <param name="_fromClient"></param>
    /// <param name="_packet"></param>
    public static void EnemyDamaged(int _fromClient, Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        float _damage = _packet.ReadFloat();
        Server.serverClients[_fromClient].player.SetEnemyDamaged(_enemyId, _damage);
    }
}
