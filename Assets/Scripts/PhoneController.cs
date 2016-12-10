using UnityEngine;
using System.Collections;

public class PhoneController : MonoBehaviour {

    public float activationAcceleration;
    public Camera mainCamera;

    bool activated = false;
    int upRightDelayMax = 10;
    int upRightDelay;
    bool orbRetrieved;

    float[] accelSignal = new float[3];
    float reeledIn;

    bool listeningToAccelerometer;
    float speed;

	// Use this for initialization
	void Start () {
        Screen.orientation = ScreenOrientation.Portrait;
        orbRetrieved = true;
        listeningToAccelerometer = false;
    }

    void FixedUpdate()
    {
        upRightDelay--;
    }

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        if (!activated) return;

        Vector3 acceleration = Input.acceleration;

        // check acceleration. if phone is pointing up and moves faster than activationAcceleration
        float accelMagX = Mathf.Abs(acceleration.x);

        if (acceleration.y < -0.5f && accelMagX < activationAcceleration) upRightDelay = upRightDelayMax;

        if (listeningToAccelerometer)
        {
            mainCamera.backgroundColor = Color.red;
        }
        else if ((upRightDelay > 0) && orbRetrieved)
        {
            mainCamera.backgroundColor = Color.green;
        } else
        {
            mainCamera.backgroundColor = Color.black;
        }


        if (listeningToAccelerometer)
        {
            speed += acceleration.magnitude * Time.deltaTime;
            if (acceleration.magnitude < 1.0f) // if phone is "stopped," stop listening and toss a ball out
            {
                Cast(speed, acceleration);
                listeningToAccelerometer = false;
                speed = 0.0f;
                orbRetrieved = false;
            }
        }
        else if (accelMagX > activationAcceleration && upRightDelay > 0 && orbRetrieved)
        {
            speed += acceleration.magnitude * Time.deltaTime;
            listeningToAccelerometer = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mainCamera.backgroundColor = Color.red;
            Reel();
        }
    }

    public void Activate()
    {
        activated = true;
    }

    void Cast(float accelMag, Vector3 finalAngle)
    {
        // Use PUN RaiseEvent to kick off an event with acceleration strength:
        // https://doc.photonengine.com/en-us/pun/current/manuals-and-demos/rpcsandraiseevent
        byte evCode = 0;    // my event 0.
        accelSignal[0] = accelMag;
        accelSignal[1] = finalAngle.x; // right and left axis of phone
        accelSignal[2] = finalAngle.y; // up and down axis of phone

        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, accelSignal, reliable, null);
    }

    void Reel()
    {
        byte evCode = 1;
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, null, reliable, null);
    }

    // setup our OnEvent as callback:
    void Awake()
    {
        PhotonNetwork.OnEventCall += this.OnEvent;
    }

    // handle events:
    private void OnEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == 2) // signals cast readiness
        {
            orbRetrieved = (bool)content;
        }
    }
}
