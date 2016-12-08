using UnityEngine;
using System.Collections;

public class PhoneController : MonoBehaviour {

    public float activationAcceleration;
    public Camera mainCamera;
    public GameObject showAfterCast;

    bool activated = false;
    int upRightDelayMax = 30;
    int upRightDelay;

	// Use this for initialization
	void Start () {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    void FixedUpdate()
    {
        showAfterCast.transform.localScale *= 0.99f;
        upRightDelay--;
    }

	// Update is called once per frame
	void Update () {
        //if (!activated) return;

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

        if (accelMag > activationAcceleration && upRightDelay > 0)
        {
            showAfterCast.transform.localScale = new Vector3(accelMag, accelMag, accelMag);
            // Use PUN RaiseEvent to kick off an event with acceleration strength:
            // https://doc.photonengine.com/en-us/pun/current/manuals-and-demos/rpcsandraiseevent

        }
    }

    public void Activate()
    {
        activated = true;
    }
}
