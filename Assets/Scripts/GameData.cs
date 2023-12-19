using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class GameData : Singleton<GameData>
{
    #region Player Info
    public string NickName { get; set; } = null;
    public PhotonView PV { get; set; }
    public RpcController Player { get; set; }

    public bool IsMaster => PhotonNetwork.LocalPlayer.IsMasterClient;
    #endregion
    public int MaxPlayers => 6;
    public Dictionary<string, RoomInfo> RoomList { get; set; } = new();
}

#region Enum
public enum CustomKey
{
    RoomName,
    RoomMaster
}
#endregion