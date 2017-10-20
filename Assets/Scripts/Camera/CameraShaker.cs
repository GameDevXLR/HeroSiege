﻿///Daniel Moore (Firedan1176) - Firedan1176.webs.com/
///26 Dec 2015
///
///Shakes camera parent object
 
using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{

    public bool debugMode = false;//Test-run/Call ShakeCamera() on start

    public float shakeAmount;//The amount to shake this frame.
    public float shakeDuration;//The duration this frame.

    //Readonly values...
    float shakePercentage;//A percentage (0-1) representing the amount of shake to be applied when setting rotation.
    float startAmount;//The initial shake amount (to determine percentage), set when ShakeCamera is called.
    float startDuration;//The initial shake duration, set when ShakeCamera is called.

    public bool isRunning = false; //Is the coroutine running right now?

    public bool smooth = true;//Smooth rotation?
    [Range(1,2)]
    public float smoothAmount = 5f;//Amount to smooth

    private Quaternion rotationOffset;
    IEnumerator shakeCoroutine;

    private void Start()
    {
        rotationOffset = transform.localRotation;
        shakeCoroutine = Shake(GetComponent<CameraController>().target);
    }

    void Update()
    {

        if (debugMode) ShakeCamera();
    }


    void ShakeCamera()
    {

        startAmount = shakeAmount;//Set default (start) values
        startDuration = shakeDuration;//Set default (start) values

        if (!isRunning && shakeDuration > 0.01f) StartCoroutine(Shake(GetComponent<CameraController>().target));//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
    }

    public void ShakeCamera(float amount, float duration)
    {
        
        shakeAmount = amount;//Add to the current amount.
        startAmount = shakeAmount;//Reset the start amount, to determine percentage.
        if(duration > shakeDuration)
        {
            shakeDuration = duration;//Add to the current time.
            startDuration = shakeDuration;//Reset the start time.
        }

        if (!isRunning)
        { 
            StartCoroutine(Shake(GetComponent<CameraController>().target));//Only call the coroutine if it isn't currently running. Otherwise, just set the variables.
            GetComponent<CameraController>().isShaking = true;
        }
    }


    IEnumerator Shake(GameObject target)
    {
        isRunning = true;

        while (shakeDuration > 0.01f)
        {
            transform.LookAt(target.transform.position);
            Vector3 rotationAmount = Random.insideUnitSphere * shakeAmount;//A Vector3 to add to the Local Rotation
            rotationAmount.z = 0;//Don't change the Z; it looks funny.

            shakePercentage = shakeDuration / startDuration;//Used to set the amount of shake (% * startAmount).

            shakeAmount = startAmount * shakePercentage;//Set the amount of shake (% * startAmount).
            shakeDuration = Mathf.Lerp(shakeDuration, 0, Time.deltaTime);//Lerp the time, so it is less and tapers off towards the end.

            if (smooth)
                transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(rotationAmount), Time.deltaTime * smoothAmount);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(rotationAmount), Time.deltaTime); ;//Set the local rotation the be the rotation amount.

            yield return null;
        }
        transform.rotation = rotationOffset;//Set the local rotation to 0 when done, just to get rid of any fudging stuff.
        isRunning = false;
        GetComponent<CameraController>().isShaking = false;

    }

}