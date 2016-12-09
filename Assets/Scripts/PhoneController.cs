using UnityEngine;
using System.Collections;

public class PhoneController : MonoBehaviour {

    public float activationAcceleration;
    public Camera mainCamera;
    public GameObject showAfterCast;

    bool activated = false;
    int upRightDelayMax = 30;
    int upRightDelay;

    int postCastRechargeMax = 300;
    int postCastRecharge;

    Vector3 sphereScale;

    float[] accelSignal = new float[1];
    byte[] accelByteSignal = new byte[4];
    byte[] reelSignal = new byte[1];

	// Use this for initialization
	void Start () {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    void FixedUpdate()
    {
        if (postCastRecharge > -1)
        {
            showAfterCast.transform.localScale = sphereScale * ((float)postCastRecharge / (float)postCastRechargeMax);
        }
        postCastRecharge--;
        upRightDelay--;
    }

	// Update is called once per frame
	void Update () {
        if (!activated) return;

        Vector3 acceleration = Input.acceleration;

        // check acceleration. if phone is pointing up and moves faster than activationAcceleration
        float accelMag = acceleration.magnitude;

        if (acceleration.y < -0.7f && accelMag < activationAcceleration) upRightDelay = upRightDelayMax;

        if (upRightDelay > 0)
        {
            mainCamera.backgroundColor = Color.green;
        } else
        {
            mainCamera.backgroundColor = Color.black;
        }

        if (accelMag > activationAcceleration && upRightDelay > 0 && postCastRecharge < 0)
        {
            sphereScale = new Vector3(accelMag, accelMag, accelMag);
            postCastRecharge = postCastRechargeMax;
            Cast(accelMag);
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

    void Cast(float accelMag)
    {
        // Use PUN RaiseEvent to kick off an event with acceleration strength:
        // https://doc.photonengine.com/en-us/pun/current/manuals-and-demos/rpcsandraiseevent
        byte evCode = 0;    // my event 0.
        accelSignal[0] = accelMag;

        // create a byte array and copy the floats into it...
        System.Buffer.BlockCopy(accelSignal, 0, accelByteSignal, 0, 4);

        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, accelSignal, reliable, null);
    }

    void Reel()
    {
        byte evCode = 1;
        reelSignal[0] = 1;
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, accelSignal, reliable, null);
    }
}
