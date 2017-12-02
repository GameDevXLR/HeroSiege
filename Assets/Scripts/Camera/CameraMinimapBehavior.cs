using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMinimapBehavior : CameraBehavior {


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            cameraController.transform.position = Vector3.Lerp(cameraController.transform.position, cameraController.targetVect + cameraController.vectCam * cameraController.distance, cameraController.speed * Time.deltaTime);
            // Set the camera to look towards the Player model
    }
}
