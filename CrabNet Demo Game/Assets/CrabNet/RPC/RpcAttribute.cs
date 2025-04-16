using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class RpcAttribute : Attribute
{
    public string RpcId { get; private set; }
    public bool IsReliable { get; private set; }

    public RpcAttribute(string rpcId = null, bool isReliable = true)
    {
        RpcId = rpcId;
        IsReliable = isReliable;
    }

    
}
