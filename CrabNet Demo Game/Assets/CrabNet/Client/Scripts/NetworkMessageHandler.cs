using UnityEngine;
using CrabNet.RPC;

namespace CrabNet.Client
{
    public class NetworkMessageHandler : MonoBehaviour
    {
        private void HandleRpcMessage(byte[] data, object target)
        {
            // Deserialize the RPC message
            RpcMessageHandler.RpcMessage rpcMessage = RpcMessageHandler.DeserializeRpcMessage(data);
            
            // Check if deserialization was successful
            if (rpcMessage.RpcId == null)
            {
                Debug.LogError("Failed to deserialize RPC message");
                return;
            }

            // Invoke the RPC method
            bool success = RpcRegistry.InvokeRpcMethod(target.NetworkId, rpcMessage.RpcId, rpcMessage.Parameters);
            
            if (!success)
            {
                Debug.LogError($"Failed to invoke RPC method: {rpcMessage.RpcId}");
            }
        }

        // Example of sending an RPC
        public void SendRpc(string rpcId, params object[] parameters)
        {
            // Check if the method is registered and get its reliability setting
            bool isReliable = RpcRegistry.IsMethodReliable(target.NetworkId, rpcId);

            // Serialize the RPC message
            byte[] rpcData = RpcMessageHandler.SerializeRpcMessage(rpcId, parameters);

            // TODO: Send the message using your network layer
            // Example:
            // if (isReliable)
            //     NetworkManager.Instance.SendReliable(rpcData);
            // else
            //     NetworkManager.Instance.SendUnreliable(rpcData);
        }
    }
} 