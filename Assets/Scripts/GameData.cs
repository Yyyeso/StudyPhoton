using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class GameData : Singleton<GameData>
{
    public string NickName { get; set; } = null;

    public RpcController Player { get; set; }

    public bool IsMaster => PhotonNetwork.LocalPlayer.IsMasterClient;

    public Dictionary<string, RoomInfo> RoomList { get; set; } = new();
}

#region Enum
public enum CustomKey
{
    RoomName,
    RoomInfo,
    RoomMaster
}
#endregion