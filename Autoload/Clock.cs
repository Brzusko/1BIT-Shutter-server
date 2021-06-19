using Godot;
using System;


public class Clock : Node
{
    #region RPCs
    [Remote]
    public void RecivePing(ulong clientTime) {
        var id = GetTree().GetRpcSenderId();
        RpcId(id, "RecivePong", OS.GetSystemTimeMsecs(), clientTime);
    }
    #endregion
}
