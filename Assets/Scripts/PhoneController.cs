using UnityEngine;
using System.Collections;

public class PhoneController : MonoBehaviour {

    public float activationAcceleration;
    public Camera mainCamera;

    bool activated = false;
    int upRightDelayMax = 10;
    int upRightDelay;
    bool orbRetrieved;

    bool inCastingCone = false; // boolean for whether or not the phone's angle is within the "casting" cone of orientation

    float[] accelSignal = new float[3];
    float reeledIn;

    bool listeningToAccelerometer;
    float speed;

    bool mouseDown = false;
    Vector3 mouseDown_pos = new Vector3();

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

        /** accelerometer stuff **/
        Vector3 acceleration = Input.acceleration;

        // check acceleration. if phone is pointing up and moves faster than activationAcceleration
        float accelMagX = Mathf.Abs(acceleration.x);
        bool inCastingConeNow = acceleration.y < -0.5f;
        if (inCastingConeNow && accelMagX < activationAcceleration) upRightDelay = upRightDelayMax;

        // ping the Hololens if the phone's orientation enters/exits the casting cone.
        if (inCastingCone != inCastingConeNow)
        {
            inCastingCone = inCastingConeNow;
            byte evCode = 3;    // my event 3.
            bool reliable = true;
            PhotonNetwork.RaiseEvent(evCode, inCastingConeNow, reliable, null);
        }


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

        // handler for "in swing"
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
        // handler for "starting swing"
        else if (accelMagX > activationAcceleration && upRightDelay > 0 && orbRetrieved)
        {
            speed += acceleration.magnitude * Time.deltaTime;
            listeningToAccelerometer = true;
        }


        /** reeling stuff **/

        if (!mouseDown && Input.GetMouseButtonDown(0) && !orbRetrieved)
        {
            mouseDown_pos = Input.mousePosition;
            mouseDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouseUp_pos = Input.mousePosition;
            if ((mouseUp_pos - mouseDown_pos).magnitude / (float) Screen.height > 0.2f &&
                mouseUp_pos.y < mouseDown_pos.y)
            {
                mainCamera.backgroundColor = Color.red;
                Reel();
            }
            mouseDown = false;
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
        Vibration.Vibrate(50);
    }

    void Reel()
    {
        byte evCode = 1;
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, null, reliable, null);
        Vibration.Vibrate(50);
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

        if (eventcode == 4) // fish has bit, or fish has escaped
        {
            Vibration.Vibrate(200);
        }
    }
}
