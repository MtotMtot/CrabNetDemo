using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    /// <summary>
    /// Welcome packet received from server, send welcome received.
    /// </summary>
    /// <param name="_packet"></param>
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    /// <summary>
    /// Spawn player packet from server, instantiate player prefab with: id, username, position, rotation.
    /// </summary>
    /// <param name="_packet"></param>
    public static void SpawnPlayer(Packet _packet)
    {
        Debug.Log("Received Spawn player!");
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    /// <summary>
    /// Player position for player(id) from server.
    /// </summary>
    /// <param name="_packet"></param>
    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
    }

    /// <summary>
    /// Player rotation for player(id) from server.
    /// </summary>
    /// <param name="_packet"></param>
    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;
    }

    /// <summary>
    /// Player disconnect for player(id) from server.
    /// </summary>
    /// <param name="_packet"></param>
    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    /// <summary>
    /// Player shoot for player(id) from server.
    /// </summary>
    /// <param name="_packet"></param>
    public static void PlayerShoot(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _camPos = _packet.ReadVector3();
        Quaternion _camRot = _packet.ReadQuaternion();
        GameManager.players[_id].playerCam.transform.position = _camPos;
        GameManager.players[_id].playerCam.transform.rotation = _camRot;
        GameManager.players[_id].rayCastShoot.Shoot();
    }

    /// <summary>
    /// Enemy for target for enemy(id), targeting: (player id) from server.
    /// </summary>
    /// <param name="_packet"></param>
    public static void EnemyTarget(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        int _targetId = _packet.ReadInt();
        EnemyManager.enemies[_enemyId].GetComponent<EnemyAI>().targetId = _targetId;
    }

    /// <summary>
    /// Enemy damaged from player (id) for enemy(id) from server.
    /// </summary>
    /// <param name="_packet"></param>
    public static void EnemyDamaged(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        float _damage = _packet.ReadFloat();
        EnemyManager.enemies[_enemyId].GetComponent<EnemyAI>().TakeDamage(_damage);
    }

    /// <summary>
    /// Enemy position for enemy(id) from server.
    /// </summary>
    /// <param name="_packet"></param>
    public static void EnemyPosition(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        Vector3 _enemyPosition = _packet.ReadVector3();
        EnemyManager.enemies[_enemyId].GetComponent<EnemyAI>().transform.position = _enemyPosition;
    }

    /// <summary>
    /// Enemy rotation for enemy(id) from server.
    /// </summary>
    /// <param name="_packet"></param>
    public static void EnemyRotation(Packet _packet)
    {
        int _enemyId = _packet.ReadInt();
        Quaternion _enemyRotation = _packet.ReadQuaternion();
        EnemyManager.enemies[_enemyId].GetComponent<EnemyAI>().transform.rotation = _enemyRotation;
    }

    /// <summary>
    /// Spawn enemy with (id, position) from server.
    /// </summary>
    /// <param name="_packet"></param>
    public static void SpawnEnemy(Packet _packet)
    {
        Debug.Log("Received spawn enemy packet from Server");
        int _enemyId = _packet.ReadInt();
        Vector3 _spawnPos = _packet.ReadVector3();
        EnemyManager.instance.SpawnEnemy(_enemyId, _spawnPos);
    }

    public static void SpawnBoss(Packet _packet)
    {
        Debug.Log("Received spawn Boss packet from Server");
        int _enemyId = _packet.ReadInt();
        Vector3 _spawnPos = _packet.ReadVector3();
        EnemyManager.instance.SpawnBoss(_enemyId, _spawnPos);
    }

    public static void Sector1Clear(Packet _packet)
    {
        Debug.Log("received Sector 1 Clear from Server");
    }

    public static void Sector2Clear(Packet _packet)
    {
        Debug.Log("received Sector 1 Clear from Server");
    }


}