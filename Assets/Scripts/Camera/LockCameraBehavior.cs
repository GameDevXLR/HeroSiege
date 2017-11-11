﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockCameraBehavior : StateMachineBehaviour {

    public CameraControllerV2 cameraController;
    public Vector3 mousePos;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        cameraController = CameraControllerV2.instanceCamera;
        
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float dir = findDirection();
        moveRotateAround(dir);
        cameraController.transform.position = Vector3.Lerp(cameraController.transform.position, cameraController.target.transform.position + cameraController.vectCam * cameraController.distance, cameraController.speed * Time.deltaTime);
        // Set the camera to look towards the Player model
        lookAt();
    }

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
    }

    private float findDirection()
    {

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            mousePos = Input.mousePosition;
        }
        else if (Input.GetKey(KeyCode.Mouse2))
        {
            return (mousePos.x - Input.mousePosition.x > 1) ? 1 : (mousePos.x - Input.mousePosition.x < -1) ? -1 : 0;
        }
        return 0;
    }


    public void moveRotateAround(float dir)
    {
        // fait rotate la camera autour de la target
        if (dir != 0)
        {
            cameraController.transform.RotateAround(cameraController.target.transform.position, Vector3.up, dir * cameraController.angle * cameraController.speedRotate * Time.deltaTime);

            // permet de replacer la caméra si la target bouge
            Vector3 vect = new Vector3(cameraController.transform.position.x - cameraController.target.transform.position.x, 1.5f, cameraController.transform.position.z - cameraController.target.transform.position.z).normalized;
            vect.y = 1.5f;
            cameraController.vectCam = vect;
        }
    }

    public void lookAt()
    {
        if (!cameraController.isShaking)
        {
            cameraController.transform.LookAt(cameraController.target.transform.position);
        }
    }
}
