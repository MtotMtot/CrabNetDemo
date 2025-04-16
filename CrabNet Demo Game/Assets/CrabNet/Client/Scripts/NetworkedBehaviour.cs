using UnityEngine;
using CrabNet.RPC;

namespace CrabNet.Client
{
    public abstract class NetworkedBehaviour : MonoBehaviour
    {
        public int NetworkId { get; private set; }

        protected virtual void Awake()
        {
            // You should set the NetworkId when the object is spawned
            // This could come from your GameManager or network instantiation system
        }

        public void Initialize(int networkId)
        {
            NetworkId = networkId;
            RpcRegistry.RegisterObject(networkId, this);
        }

        protected virtual void OnDestroy()
        {
            RpcRegistry.UnregisterObject(NetworkId);
        }

        protected void InvokeRpc(string methodName, params object[] parameters)
        {
            ClientSend.SendRPC(NetworkId, methodName, parameters);
        }
    }
} 