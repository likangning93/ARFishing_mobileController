using UnityEngine;
using System.Collections;

public class PhoneNetwork : Photon.PunBehaviour
{

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Label("Room Name: " + roomName);
    }

    string roomName;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    // TODO-2.a: the same as 1.b
    //   and join a room
    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        roomName = PhotonNetwork.room.name;
        GetComponent<PhoneController>().Activate();
    }
}
