using System;
using System.Text;
using UnityEngine;
using System.IO;

namespace CrabNet.RPC
{
    public class RpcMessageHandler
    {
        // Structure for RPC message
        public struct RpcMessage
        {
            public string RpcId { get; set; }
            public object[] Parameters { get; set; }
        }

        public static RpcMessage DeserializeRpcMessage(byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    // Read RPC ID
                    int rpcIdLength = reader.ReadInt32();
                    byte[] rpcIdBytes = reader.ReadBytes(rpcIdLength);
                    string rpcId = Encoding.UTF8.GetString(rpcIdBytes);

                    // Read parameter count
                    int paramCount = reader.ReadInt32();
                    object[] parameters = new object[paramCount];

                    // Read parameters
                    for (int i = 0; i < paramCount; i++)
                    {
                        byte typeCode = reader.ReadByte();
                        parameters[i] = ReadParameter(reader, typeCode);
                    }

                    return new RpcMessage
                    {
                        RpcId = rpcId,
                        Parameters = parameters
                    };
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize RPC message: {e.Message}");
                return default;
            }
        }

        private static object ReadParameter(BinaryReader reader, byte typeCode)
        {
            switch (typeCode)
            {
                case 0: // Int32
                    return reader.ReadInt32();
                case 1: // Float
                    return reader.ReadSingle();
                case 2: // String
                    int strLength = reader.ReadInt32();
                    byte[] strBytes = reader.ReadBytes(strLength);
                    return Encoding.UTF8.GetString(strBytes);
                case 3: // Boolean
                    return reader.ReadBoolean();
                case 4: // Vector3
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    float z = reader.ReadSingle();
                    return new Vector3(x, y, z);
                // Add more types as needed
                default:
                    throw new ArgumentException($"Unsupported parameter type code: {typeCode}");
            }
        }

        public static byte[] SerializeRpcMessage(string rpcId, params object[] parameters)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                // Write RPC ID
                byte[] rpcIdBytes = Encoding.UTF8.GetBytes(rpcId);
                writer.Write(rpcIdBytes.Length);
                writer.Write(rpcIdBytes);

                // Write parameter count
                writer.Write(parameters.Length);

                // Write parameters
                foreach (object param in parameters)
                {
                    WriteParameter(writer, param);
                }

                return ms.ToArray();
            }
        }

        private static void WriteParameter(BinaryWriter writer, object param)
        {
            switch (param)
            {
                case int intValue:
                    writer.Write((byte)0);
                    writer.Write(intValue);
                    break;
                case float floatValue:
                    writer.Write((byte)1);
                    writer.Write(floatValue);
                    break;
                case string strValue:
                    writer.Write((byte)2);
                    byte[] strBytes = Encoding.UTF8.GetBytes(strValue);
                    writer.Write(strBytes.Length);
                    writer.Write(strBytes);
                    break;
                case bool boolValue:
                    writer.Write((byte)3);
                    writer.Write(boolValue);
                    break;
                case Vector3 vector3Value:
                    writer.Write((byte)4);
                    writer.Write(vector3Value.x);
                    writer.Write(vector3Value.y);
                    writer.Write(vector3Value.z);
                    break;
                // Add more types as needed
                default:
                    throw new ArgumentException($"Unsupported parameter type: {param.GetType()}");
            }
        }
    }
} 