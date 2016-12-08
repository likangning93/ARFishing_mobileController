using UnityEngine;
using System.Collections;

public class PhoneNetwork : Photon.PunBehaviour
{

    float accelMax = 0.0f;

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Label("Room Name: " + roomName);
        GUILayout.Label("Accelerometer: " + Input.acceleration.ToString());

        accelMax = Mathf.Max(accelMax, Input.acceleration.magnitude);
        GUILayout.Label("Max Accel: " + accelMax);

        GUILayout.Label("Gyro: " + Input.gyro.attitude.ToString());
    }

    string roomName;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            accelMax = 0.0f;
        }
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
