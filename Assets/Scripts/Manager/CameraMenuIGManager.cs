using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMenuIGManager : MonoBehaviour {


    public Slider speedSlider;
    public Slider speedRotateSlider;
    public Slider angleSlider;
    public Slider distanceSlider;


    private void Start()
    {
        speedSlider.value = CameraController.instanceCamera.speed;
        speedRotateSlider.value = CameraController.instanceCamera.speedRotate;
        angleSlider.value = CameraController.instanceCamera.yRef;
        distanceSlider.value = CameraController.instanceCamera.distance;
    }


    public void setSpeed()
    {
        CameraController.instanceCamera.speed = speedSlider.value;
    }

    public void setSpeedRotate()
    {
        CameraController.instanceCamera.speedRotate = speedRotateSlider.value;
    }
    public void setAngle()
    {
        CameraController.instanceCamera.setAngle(angleSlider.value);
    }

    public void setDistance()
    {
        CameraController.instanceCamera.distance = distanceSlider.value;
    }
}
