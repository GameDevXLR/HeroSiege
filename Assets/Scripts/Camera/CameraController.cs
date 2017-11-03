﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;
using UnityEngine.Events;



/**
 * 
 * class : CameraController
 * A script to controll the camera
 * 
 * */
public class CameraController : MonoBehaviour
{
	public static CameraController instanceCamera = null;
	// our personnage
	public GameObject target;
    public Transform helperCamPos;

    // selectedPlayer
    // true : camera lock in the perso
    // false : camera free from the perso
	public UnityEvent changeCamLockImg;
    public bool isLock = false;

	//speed move of the camera when move with mouse
    public float speed = 5;
    public float speedRotate = 3;
    public float angle = 90;

	// detection zone of the mouse in the border
    public int zoneDetectionMouse = 300;
    
    public float distance = 15;

    // zoom
    private float zoomfact = 1;
    public float vitesseZoom = 0.2f;
    public float limiteBasse = 0.5f;
    public float limiteHausse = 2;
    

	// y difference use to move the y value
	private float yvalueDiff;

	private bool isReady;
    public bool isAnotherPlayer;
    public bool isDead = false; //permet d'éviter de modifier trop souvent le postprocessing

    int layer_mask;

	Camera cameraCible;

    public CameraBoundaries boundaries;
    public PostProcessingBehaviour postProcessing;

    public bool isShaking = false;



    public string MouseScrollWheel = "Mouse ScrollWheel";
    public float ThirdPersonZoomSensitivity = 3f;
    public float ThirdPersonCameraMinDistance = 4f;
    public float ThirdPersonCameraMaxDistance = 20f;
    public int RotateCameraMouseButton = 1;
    public float ThirdPersonCameraSensitivity = 2f;
    public float ThirdPersonCameraMinPitch = 5f;
    public float ThirdPersonCameraMaxPitch = 70f;

    private Vector2 mousePos;
    
    public float yRef = 1.5f;
    public float xRef = 1;
    public float zRef = 0;
    public Vector3 vectCam;


    /// <summary>
    /// Limitation pour éviter de clicker sur la falaise
    /// </summary>
    public float yMinCamera = 15;
    /// <summary>
    ///  les différents styles de caméra possible de test
    ///  stratégique : en hauteur
    ///  thirdPerson : derrière le joueur
    ///  thirdPerson : derrière le joueur mais suis la rotation
    /// </summary>
    public enum StyleCam
    {
        strategique,
        thirdPerson,
        free
    }

    public StyleCam style = StyleCam.thirdPerson;

    //on s'assure en Awake que le script est bien unique. sinon on détruit le nouvel arrivant.
    void Awake(){
		if (instanceCamera == null) {
			instanceCamera = this;
            vectCam = new Vector3(xRef, yRef, zRef);


        } else if (instanceCamera != this) 
		{
			Destroy (gameObject);
		}
	}

	public void Initialize()
    {
        cameraCible = GetComponent<Camera>();
		isReady = true;
		layer_mask = Layers.Ground; // ground layer 10
        helperCamPos = target.transform.Find("ThirdPersonCamPosition");


    }

    void Update()
    {
		if (!isReady) 
		{
			return;
		}

		if (GameManager.instanceGM.playerObj && !isDead && GameManager.instanceGM.playerObj.GetComponent<PlayerIGManager>().isDead)
        {
			var colograding = postProcessing.profile.colorGrading.settings;
            colograding.basic.saturation = 0;
            postProcessing.profile.colorGrading.settings = colograding;
            isDead = true;
        }
        else if(isDead && !GameManager.instanceGM.playerObj.GetComponent<PlayerIGManager>().isDead ) 
        {
            var colograding = postProcessing.profile.colorGrading.settings;
            colograding.basic.saturation = 1;
            postProcessing.profile.colorGrading.settings = colograding;
            isDead = false;

        }
        if(!GameManager.instanceGM.isInTchat)
        {
            SelectNextCameraDistance();
            if (switchToOtherPlayer())
            {
                style = StyleCam.thirdPerson;
                isAnotherPlayer = true;
                helperCamPos = target.transform.Find("ThirdPersonCamPosition");
            }                
            else if(Input.GetKeyDown(CommandesController.Instance.getKeycode(CommandesEnum.CameraCenter)))
            {
                if (isAnotherPlayer)
                {
                    revertTargetToPlayer();
                    isAnotherPlayer = false;
                }
                style = StyleCam.thirdPerson;
                vectCam = new Vector3(xRef, yRef, zRef);
            }
            else if (Input.GetKeyUp(CommandesController.Instance.getKeycode(CommandesEnum.CameraCenter)))
            {
                if (!isLock)
                {
                    style = StyleCam.free;

                }
                    
            }
            else if (Input.GetKeyUp(CommandesController.Instance.getKeycode(CommandesEnum.CameraLock)))
            {
                if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.CameraCenter)))
                {
                    isLock = !isLock;
                }
                else
                {
                    areLocking();
                }
                changeCamLockImg.Invoke();
            }

        }
    }

    void LateUpdate()
    {
		if (!isReady) 
		{
			return;
		}
         move();
		
    }

	public void move()
	{
        float dir;

        if (target != null) {
            switch (style) {
                case StyleCam.thirdPerson:
                    // Set the position of the camera based on the desired rotation towards and distance from the Player model
                    //gameObject.transform.position = Vector3.Lerp(target.transform.position, target.transform.position + new Vector3(1, 1.5f, 0) * distance, speed * Time.deltaTime);
                    dir = findDirection();
                    moveRotateAround(dir);
                    transform.position = Vector3.Lerp(transform.position, target.transform.position + vectCam * distance, speed * Time.deltaTime);
                    // Set the camera to look towards the Player model
                    lookAt();
                    break;
                case StyleCam.free:
                    UtilsScreenMovement.moveScreenWithMouse(cameraCible, boundaries, zoneDetectionMouse, speed, layer_mask);
                    dir = findDirection();
                    moveRotate(dir);
                    // Set the camera to look towards the Player model
                    break;
            }
        }
	}
      
    public void MoveCameraTo(Vector3 vect)
    {
		if (vect != Vector3.zero)
        {
			gameObject.transform.position = vect+ new Vector3(1, 2f, 0) * distance;
        }
        
    }

    public void areLocking()
    {
        if(style != StyleCam.free)
        {
            style = StyleCam.free;
            isLock = false;
        }
        else
        {
            style = StyleCam.thirdPerson;
            isLock = true;
        }
    }

    public void areLocking(bool islock)
    {
        style = (islock) ? StyleCam.thirdPerson : StyleCam.free;
        this.isLock = islock;
    }

    public void setCameraTarget(GameObject target)
    {
        this.target = target;
        areLocking(true);

    }


    public void revertTargetToPlayer()
    {
        setCameraTarget(GameManager.instanceGM.playerObj);
    }

    public bool switchToOtherPlayer()
    {

        
        if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer1)))
        {

            if (GameManager.instanceGM.isTeam1)
            {
                target = GameManager.instanceGM.team1Players[0];
            }
            else
                target = GameManager.instanceGM.team2Players[0];

            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer2)) )
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1Players.Count > 1)
            {
                target = GameManager.instanceGM.team1Players[1];
            }
            else if (GameManager.instanceGM.team2Players.Count > 1)
                target = GameManager.instanceGM.team2Players[1] ;
            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer3)) )
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1Players.Count > 2)
            {
                target =GameManager.instanceGM.team1Players[2];
            }
            else if (GameManager.instanceGM.team2Players.Count > 2)
                target = GameManager.instanceGM.team2Players[2];
            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer4)))
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1Players.Count > 3)
            {
                target = GameManager.instanceGM.team1Players[3];
            }
            else if ( GameManager.instanceGM.team2Players.Count > 3)
                target = GameManager.instanceGM.team2Players[3];
            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer5)))
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1Players.Count > 4)
            {
                target = GameManager.instanceGM.team1Players[4];
            }
            else if ( GameManager.instanceGM.team2Players.Count > 4)
                target = GameManager.instanceGM.team2Players[4];
            return true;
        }
        return false;
    }

    

    public void changeToThird()
    {
        style = StyleCam.thirdPerson;
        //        transform.SetParent(null);
    }

    public void moveAround()
    {
        // Calcule de l'angle de la caméra entre l'object helper et la caméra avec pour point de référence la target
        Vector2 vect2Target = new Vector2(target.transform.position.x - helperCamPos.position.x, target.transform.position.z - helperCamPos.position.z);
        Vector2 vect2Came = new Vector2(target.transform.position.x -  transform.position.x, target.transform.position.z - transform.position.z);
        float angle = Vector2.Angle(vect2Came, vect2Target);

        // permet de savoir de quel côté doit tourner la caméra pour le chemin le plus rapide
        float dir = Mathf.Sign(Vector3.Cross(vect2Target, vect2Came).z);
        if (dir == 0 && angle != 0)
        {
            dir = 1;
        }

        // fait rotate la camera autour de la target
        transform.RotateAround(target.transform.position, Vector3.up, dir *  angle * Time.deltaTime * speedRotate);

        // permet de replacer la caméra si la target bouge
        Vector3 vect = new Vector3(transform.position.x - target.transform.position.x, 1.5f, transform.position.z - target.transform.position.z).normalized;
        vect.y = 1.5f;
        gameObject.transform.position = target.transform.position + vect * distance;

        // permet d'avoir le visuel sur la caméra
        lookAt();
    }



    public void moveRotateAround(float dir)
    {
        // fait rotate la camera autour de la target
        if (dir != 0)
        {
            transform.RotateAround(target.transform.position, Vector3.up, dir * angle * speedRotate * Time.deltaTime);

            // permet de replacer la caméra si la target bouge
            Vector3 vect = new Vector3(transform.position.x - target.transform.position.x, 1.5f, transform.position.z - target.transform.position.z).normalized;
            vect.y = 1.5f;
            vectCam = vect;
        }
    }

   
    public void moveRotate(float dir)
    {
        //transform.Rotate(new Vector3(0, 1, 0), Space.World);
        if(dir != 0)
        {
            
            transform.rotation =  Quaternion.AngleAxis(dir * angle * speedRotate * Time.deltaTime , Vector3.up) * transform.rotation ;
            // permet de replacer la caméra si la target bouge
            Vector3 vect = new Vector3(transform.position.x - target.transform.position.x, 1.5f, transform.position.z - target.transform.position.z).normalized;
                vect.y = 1.5f;
                vectCam = vect;
        }
    }

    public void lookAt()
    {
        if (!isShaking)
        {
            transform.LookAt(target.transform.position);
        }
    }



    // If the user scrolls up on their mousewheel then zoom in, if they scroll down then zoom out
    private void SelectNextCameraDistance()
    {
        var mouseScroll = Input.GetAxis(MouseScrollWheel);
        if (!mouseScroll.Equals(0f))
        {
            var distanceChange = distance - mouseScroll * ThirdPersonZoomSensitivity;
            distance = Mathf.Clamp(distanceChange, ThirdPersonCameraMinDistance, ThirdPersonCameraMaxDistance);
        }
    }

    private float findDirection()
    {
        
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            mousePos = Input.mousePosition;
        }
        else if (Input.GetKey(KeyCode.Mouse2))
        {
            return (mousePos.x - Input.mousePosition.x > 1) ? 1 : (mousePos.x - Input.mousePosition.x < -1) ? -1 : 0 ;
        }
        return 0;
    }

    public void setAngle(float y)
    {
        yRef = y;
        vectCam.y = yRef;
    }

    


}



