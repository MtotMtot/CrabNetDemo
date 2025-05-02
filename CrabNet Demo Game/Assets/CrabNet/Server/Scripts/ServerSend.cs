using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend : MonoBehaviour
{
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.serverClients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.serverClients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.serverClients[i].tcp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.serverClients[i].tcp.SendData(_packet);
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.serverClients[i].udp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.serverClients[i].udp.SendData(_packet);
            }
        }
    }


    #region Host Server Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
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

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="_toClient">The client that should spawn the player.</param>
    /// <param name="_player">The player to spawn.</param>
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);

            Debug.Log("Sent Spawn player...");
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public static void PlayerPosition(Player _player)

    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    /// <summary>Send player disconncted to all clients (but the one who disconnected) to properly handle player disconnecting, prevents null referencing</summary>
    /// <param name="_playerId">The player's ID.</param>
    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_playerId, _packet);
        }

    }

    /// <summary>Send player shoot to all clients except the client shooting.</summary>
    /// <param name="_playerId">The player's ID.</param>
    /// <param name="_camPos">The camera's position.</param>
    /// <param name="_camRot">The camera's rotation.</param>
    public static void PlayerShoot(int _playerId, Vector3 _camPos, Quaternion _camRot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerShoot))
        {
            _packet.Write(_playerId);
            _packet.Write(_camPos);
            _packet.Write(_camRot);

            SendUDPDataToAll(_playerId, _packet);
        }
    }

    /// <summary>Send Enemy target to all clients, prevents need to send enemy actions over network such as shooting or chasing, all handled locally.</summary>
    /// <param name="_enemyId">The enemy's ID.</param>
    /// <param name="_targetId">The target's ID.</param>
    public static void EnemyTarget(int _enemyId, int _targetId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyTarget))
        {
            _packet.Write(_enemyId);
            _packet.Write(_targetId);

            SendUDPDataToAll(_packet);
        }
    }

    /// <summary>Send enemy damaged from client to all players except the client who did damaged.</summary>
    /// <param name="_playerId">The player's ID.</param>
    /// <param name="_enemyId">The enemy's ID.</param>
    /// <param name="_damage">The damage amount.</param>
    public static void EnemyDamaged(int _playerId, int _enemyId, float _damage)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyDamaged))
        {
            _packet.Write(_enemyId);
            _packet.Write(_damage);
            SendUDPDataToAll(_playerId, _packet);
        }
    }

    /// <summary>Send enemy position to all clients except host.</summary>
    /// <param name="_hostId">The host's ID.</param>
    /// <param name="_enemyId">The enemy's ID.</param>
    /// <param name="_enemyPosition">The enemy's position.</param>
    public static void EnemyPosition(int _hostId, int _enemyId, Vector3 _enemyPosition)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyPosition))
        {
            _packet.Write(_enemyId);
            _packet.Write(_enemyPosition);
            SendUDPDataToAll(_hostId, _packet);
        }
    }

    /// <summary>Send enemy rotation to all clients except host.</summary>
    /// <param name="_hostId">The host's ID.</param>
    /// <param name="_enemyId">The enemy's ID.</param>
    /// <param name="_enemyRotation">The enemy's rotation.</param>
    public static void EnemyRotation(int _hostId, int _enemyId, Quaternion _enemyRotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyRotation))
        {
            _packet.Write(_enemyId);
            _packet.Write(_enemyRotation);
            SendUDPDataToAll(_hostId, _packet);
        }
    }

    /// <summary>Send enemy spawn to target client (most recent to connect).</summary>
    /// <param name="_playerId">The player's ID.</param>
    /// <param name="_id">The enemy's ID.</param>
    /// <param name="_spawnPos">The enemy's spawn position.</param>
    public static void SpawnEnemy(int _playerId, int _id, Vector3 _spawnPos)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            _packet.Write(_id);
            _packet.Write(_spawnPos);
            SendTCPData(_playerId, _packet);
        }
    }

    /// <summary>
    /// Send spawn boss to target client.
    /// </summary>
    /// <param name="_playerId"></param>
    /// <param name="_id"></param>
    /// <param name="_spawnPos"></param>
    public static void SpawnBoss(int _playerId, int _id, Vector3 _spawnPos)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnBoss))
        {
            _packet.Write(_id);
            _packet.Write(_spawnPos);
            SendTCPData(_playerId, _packet);
        }
    }

    /// <summary>
    /// Relay Sector 1 clear (from logic server) to all other clients except host.
    /// </summary>
    /// <param name="_exceptClient"></param>
    public static void RelaySector1Clear(int _exceptClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.relaySector1Clear))
        {
            SendTCPDataToAll(_exceptClient, _packet);
        }
    }

    /// <summary>
    /// Relay Sector 2 clear (from logic server) to all other clients except host.
    /// </summary>
    /// <param name="_exceptClient"></param>
    public static void RelaySector2Clear(int _exceptClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.relaySector2Clear))
        {
            SendTCPDataToAll(_exceptClient, _packet);
        }
    }

    #endregion

    #region LogicServer Packets

    /// <summary>
    /// Welcome received packet.
    /// </summary>
    /// <param name="_playerId">The player's ID.</param>
    /// <param name="_username">The player's username.</param>
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)LogicClientPackets.welcomeReceived))
        {
            _packet.Write(LogicClient.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            _packet.WriteLength();
            LogicClient.instance.tcp.SendData(_packet);
        }
    }

    /// <summary>
    /// Send sector 1 clear to LogicServer
    /// </summary>
    /// <param name="_state">The state of the sector 1.</param>
    public static void Sector1State(bool _state)
    {
        using (Packet _packet = new Packet((int)LogicClientPackets.Sector1State))
        {
            _packet.Write(_state);

            _packet.WriteLength();
            LogicClient.instance.udp.SendData(_packet);
        }
    }

    /// <summary>
    /// Send sector 2 clear to LogicServer
    /// </summary>
    /// <param name="_state">The state of the sector 2.</param>
    public static void Sector2State(bool _state)
    {
        using (Packet _packet = new Packet((int)LogicClientPackets.Sector2State))
        {
            _packet.Write(_state);

            _packet.WriteLength();
            LogicClient.instance.udp.SendData(_packet);
        }
    }
    #endregion
}
