using UnityEngine;
using System.Collections;

public class PhoneNetwork : Photon.PunBehaviour
{

    float accelMax = 0.0f;
    Vector3 accelMaxDir;

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        GUILayout.Label("Room Name: " + roomName);
        GUILayout.Label("Accelerometer: " + Input.acceleration.ToString() + " m: " + Input.acceleration.magnitude);

        if (accelMax < Input.acceleration.magnitude)
        {
            accelMax = Input.acceleration.magnitude;
            accelMaxDir = Input.acceleration;
        }
        GUILayout.Label("Max Accel: " + accelMax + " Dir: " + accelMaxDir.ToString());

        GUILayout.Label("Gyro: " + Input.gyro.attitude.ToString());
    }

    string roomName;

    void Awake()
    {
        Input.gyro.enabled = true;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            accelMax = 0.0f;
            accelMaxDir = Vector3.zero;
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
