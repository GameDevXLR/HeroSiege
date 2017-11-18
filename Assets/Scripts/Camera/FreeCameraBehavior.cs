using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraBehavior : CameraBehavior {
    

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        move();
        
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

    public void move()
    {

        float yValue = (Input.mousePosition.y - (Screen.height / 2)) / (Screen.height / 2);
        float xValue = (Input.mousePosition.x - (Screen.width / 2)) / (Screen.width / 2);
        float zoneDectectWidth = (float)cameraController.zoneDetectionMouse / Screen.width;
        float zoneDectectHeight = (float)cameraController.zoneDetectionMouse / Screen.height;

        float xValueDeplacement = 0;
        float zValueDeplacement = 0;

        if ((xValue <= 1 && xValue >= 1 - zoneDectectWidth)
            || (xValue >= -1 && xValue <= -1 + zoneDectectWidth)
            || (yValue <= 1 && yValue >= 1 - zoneDectectHeight)
            || (yValue >= -1 && yValue <= -1 + zoneDectectHeight))
        {
            if ((xValue > 0 && cameraController.cameraCible.transform.position.z <= cameraController.boundaries.getEst())
                || (xValue < 0 && cameraController.cameraCible.transform.position.z >= cameraController.boundaries.getOuest()))
            {
                xValueDeplacement = cameraController.speed * Time.deltaTime * xValue;
            }

            if ((yValue > 0 && cameraController.cameraCible.transform.position.x >= cameraController.boundaries.getNord())
                || (yValue < 0 && cameraController.cameraCible.transform.position.x <= cameraController.boundaries.getSud()))
            {
                zValueDeplacement = cameraController.speed * Time.deltaTime * yValue;
            }
        }

        if (xValueDeplacement != 0 || zValueDeplacement != 0)
        {
            // Debug.Log("CameraCible position : " + cameraCible.transform.position);
            Vector3 destination = cameraController.cameraCible.transform.position + cameraController.cameraCible.transform.TransformDirection(new Vector3(xValueDeplacement, 0, zValueDeplacement));
            destination.y = cameraController.cameraCible.transform.position.y;

            Vector3 hitPointDest = new Vector3();
            Vector3 hitPointDepart = new Vector3();

            if (Utils.hadDetectTheLayer(destination, cameraController.layer_mask, out hitPointDest) && Utils.hadDetectTheLayer(cameraController.cameraCible.transform.position, cameraController.layer_mask, out hitPointDepart))
            {
                destination.y = cameraController.cameraCible.transform.position.y - (hitPointDepart.y - hitPointDest.y);
            }
            cameraController.cameraCible.transform.localPosition = Vector3.Lerp(cameraController.cameraCible.transform.position, destination, cameraController.speed * Time.deltaTime);

        }

    }
}
