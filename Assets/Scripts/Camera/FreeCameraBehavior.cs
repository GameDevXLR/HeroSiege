using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraBehavior : CameraBehavior {



    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UtilsScreenMovement.moveScreenWithMouse(cameraController.cameraCible, cameraController.boundaries, cameraController.zoneDetectionMouse, cameraController.speed, cameraController.layer_mask);
        
        rotate(findDirection());
    }

    public override void rotate(float dir)
    {
        // fait rotate la camera autour de la target
        if (dir != 0)
        {

            cameraController.transform.rotation = Quaternion.AngleAxis(dir * cameraController.angle * cameraController.speedRotate * Time.deltaTime, Vector3.up) * cameraController.transform.rotation;
            // permet de replacer la caméra si la target bouge
            Vector3 vect = new Vector3(cameraController.transform.position.x - cameraController.target.transform.position.x, 1.5f, cameraController.transform.position.z - cameraController.target.transform.position.z).normalized;
            vect.y = 1.5f;
            cameraController.vectCam = vect;
        }
    }
}
