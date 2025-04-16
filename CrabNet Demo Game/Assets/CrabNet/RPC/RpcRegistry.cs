using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CrabNet.RPC
{
    public class RpcRegistry : MonoBehaviour
    {
        private static Dictionary<(int objectId, string methodName), MethodInfo> registeredMethods = new Dictionary<(int objectId, string methodName), MethodInfo>();
        private static Dictionary<MethodInfo, RpcAttribute> methodAttributes = new Dictionary<MethodInfo, RpcAttribute>();
        private static Dictionary<int, object> networkObjects = new Dictionary<int, object>();

        public static void RegisterObject(int networkId, object target)
        {
            if (networkObjects.ContainsKey(networkId))
            {
                Debug.LogWarning($"Object with network ID {networkId} is already registered.");
                return;
            }

            networkObjects[networkId] = target;
            RegisterRpcMethods(networkId, target);
        }

        public static void UnregisterObject(int networkId)
        {
            if (!networkObjects.ContainsKey(networkId))
            {
                return;
            }

            // Remove all methods for this object
            var methodsToRemove = new List<(int objectId, string methodName)>();
            foreach (var key in registeredMethods.Keys)
            {
                if (key.objectId == networkId)
                {
                    methodsToRemove.Add(key);
                }
            }

            foreach (var key in methodsToRemove)
            {
                var method = registeredMethods[key];
                methodAttributes.Remove(method);
                registeredMethods.Remove(key);
            }

            networkObjects.Remove(networkId);
        }

        private static void RegisterRpcMethods(int networkId, object target)
        {
            Type type = target.GetType();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | 
                                                 BindingFlags.Instance | BindingFlags.Static);

            foreach (MethodInfo method in methods)
            {
                RpcAttribute rpcAttribute = method.GetCustomAttribute<RpcAttribute>();
                if (rpcAttribute != null)
                {
                    string rpcId = rpcAttribute.RpcId ?? method.Name;
                    registeredMethods[(networkId, rpcId)] = method;
                    methodAttributes[method] = rpcAttribute;
                    Debug.Log($"Registered RPC method: {rpcId} for object {networkId}");
                }
            }
        }

        public static void InvokeRpcMethod(int targetId, string methodName, object[] parameters)
        {
            if (!registeredMethods.TryGetValue((targetId, methodName), out MethodInfo method))
            {
                Debug.LogError($"No RPC method '{methodName}' registered for object {targetId}");
                return;
            }

            if (!networkObjects.TryGetValue(targetId, out object target))
            {
                Debug.LogError($"Target object {targetId} not found");
                return;
            }

            try
            {
                method.Invoke(target, parameters);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error invoking RPC method {methodName}: {e.Message}");
            }
        }

        public static bool IsMethodReliable(int targetId, string methodName)
        {
            if (registeredMethods.TryGetValue((targetId, methodName), out MethodInfo method) &&
                methodAttributes.TryGetValue(method, out RpcAttribute attr))
            {
                return attr.IsReliable;
            }
            return true; // Default to reliable
        }

        public static void HandleRpcPacket(Packet packet)
        {
            int targetId = packet.ReadInt();
            string methodName = packet.ReadString();
            int paramCount = packet.ReadInt();
            
            object[] parameters = new object[paramCount];
            for (int i = 0; i < paramCount; i++)
            {
                parameters[i] = ReadParameter(packet);
            }

            InvokeRpcMethod(targetId, methodName, parameters);
        }

        private static object ReadParameter(Packet packet)
        {
            byte typeCode = packet.ReadByte();
            switch (typeCode)
            {
                case 0: return packet.ReadInt();
                case 1: return packet.ReadFloat();
                case 2: return packet.ReadString();
                case 3: return packet.ReadBool();
                case 4: return packet.ReadVector3();
                case 5: return packet.ReadQuaternion();
                default:
                    throw new ArgumentException($"Unsupported parameter type code: {typeCode}");
            }
        }

        public static void WriteParameter(Packet packet, object param)
        {
            switch (param)
            {
                case int intValue:
                    packet.Write((byte)0);
                    packet.Write(intValue);
                    break;
                case float floatValue:
                    packet.Write((byte)1);
                    packet.Write(floatValue);
                    break;
                case string strValue:
                    packet.Write((byte)2);
                    packet.Write(strValue);
                    break;
                case bool boolValue:
                    packet.Write((byte)3);
                    packet.Write(boolValue);
                    break;
                case Vector3 vector3Value:
                    packet.Write((byte)4);
                    packet.Write(vector3Value);
                    break;
                case Quaternion quaternionValue:
                    packet.Write((byte)5);
                    packet.Write(quaternionValue);
                    break;
                default:
                    throw new ArgumentException($"Unsupported parameter type: {param.GetType()}");
            }
        }
    }
} 