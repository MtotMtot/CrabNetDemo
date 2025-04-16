using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrabNet.RPC;

public class ClientSend : MonoBehaviour
{
    /// <summary>
    /// Send data via TCP to server.
    /// </summary>
    /// <param name="_packet"></param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    /// <summary>
    /// Send data via UDP to server.
    /// </summary>
    /// <param name="_packet"></param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets

    /// <summary>
    /// Welcome received packet.
    /// </summary>
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    /// <summary>
    /// Player movement for this client packet.
    /// </summary>
    public static void PlayerMovement()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(GameManager.players[Client.instance.myId].transform.position);
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(_packet);
        }
    }

    /// <summary>
    /// Player shoot for this client pakcet.
    /// </summary>
    public static void PlayerShoot()
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(GameManager.players[Client.instance.myId].playerCam.transform.position);
            _packet.Write(GameManager.players[Client.instance.myId].playerCam.transform.rotation);

            SendUDPData(_packet);
        }
    }

    /// <summary>
    /// Enemy damaged for enemy(id) from this client.
    /// </summary>
    /// <param name="_enemyId"></param>
    /// <param name="_damage"></param>
    public static void EnemyDamaged(int _enemyId, float _damage)
    {
        using (Packet _packet = new Packet((int)ClientPackets.enemyDamaged))
        {
            _packet.Write(_enemyId);
            _packet.Write(_damage);
            SendUDPData(_packet);
        }
    }

    public static void SendRPC(int targetId, string methodName, params object[] parameters)
    {
        using (Packet _packet = new Packet((int)ClientPackets.rpc))
        {
            _packet.Write(targetId);
            _packet.Write(methodName);
            _packet.Write(parameters.Length);

            foreach (object param in parameters)
            {
                RpcRegistry.WriteParameter(_packet, param);
            }

            if (RpcRegistry.IsMethodReliable(targetId, methodName))
            {
                SendTCPData(_packet);
            }
            else
            {
                SendUDPData(_packet);
            }
        }
    }

    public static void SendUnreliableRPC(int targetId, string rpcId, params object[] parameters)
    {
        using (Packet _packet = new Packet((int)ClientPackets.rpc))
        {
            // Write the target object ID
            _packet.Write(targetId);

            // Serialize the RPC data
            byte[] rpcData = RpcMessageHandler.SerializeRpcMessage(rpcId, parameters);
            
            // Write the RPC data length and the data itself
            _packet.Write(rpcData.Length);
            _packet.Write(rpcData);

            SendUDPData(_packet);
        }
    }
    #endregion
}