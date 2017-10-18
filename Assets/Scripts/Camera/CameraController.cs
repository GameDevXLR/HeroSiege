using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PostProcessing;

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

	// selectedPlayer
	// true : camera lock in the perso
	// false : camera free from the perso
    public bool selectedPlayer = true; 

	//speed move of the camera when move with mouse
    public int speed = 5;

	// detection zone of the mouse in the border
    public int zoneDetectionMouse = 300;
    
    public int distance = 15;

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


    public Quaternion cameraRotation = Quaternion.Euler(40, 0, 0);

    //on s'assure en Awake que le script est bien unique. sinon on détruit le nouvel arrivant.
    void Awake(){
		if (instanceCamera == null) {
			instanceCamera = this;
            
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
        
    }

    void Update()
    {
		if (!isReady) 
		{
			return;
		}

        if (!isDead && GameManager.instanceGM.playerObj.GetComponent<PlayerIGManager>().isDead)
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
            if (switchToOtherPlayer())
            {
                selectedPlayer = true;
                isAnotherPlayer = true;
            }
            else if (Input.GetKeyUp(CommandesController.Instance.getKeycode(CommandesEnum.CameraLock)))
            {
                 LockUnlockCamera();
             }
                
            else if(Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.CameraCenter)) && isAnotherPlayer)
            {
                revertTargetToPlayer();
                isAnotherPlayer = false;
            }
            else if (!Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.CameraCenter))
                && !selectedPlayer)
            {
                UtilsScreenMovement.moveScreenWithMouse(cameraCible, boundaries, zoneDetectionMouse, speed, layer_mask);
            }

        }
    }

    void LateUpdate()
    {
		if (!isReady) 
		{
			return;
		}
		if (selectedPlayer || 
            Input.GetKey (CommandesController.Instance.getKeycode(CommandesEnum.CameraCenter)))
        {
			CenterBackCameraOnTarget ();
		} 
    }

	public void CenterBackCameraOnTarget()
	{
		if (target != null) {
            //transform.position = target.transform.position + offset;
            // Set the position of the camera based on the desired rotation towards and distance from the Player model
            gameObject.transform.position = target.transform.position + new Vector3(1, 1, 0) * distance;
            // Set the camera to look towards the Player model
            gameObject.transform.LookAt(target.transform.position);
        }
	}
      
    public void MoveCameraTo(Vector3 vect)
    {
		if (vect != Vector3.zero)
        {
            gameObject.transform.position = target.transform.position + new Vector3(1, 1, 0) * distance;
        }
        
    }

    public void LockUnlockCamera()
    {
        selectedPlayer = !selectedPlayer;
    }

    public void LockUnlockCamera(bool islock)
    {
        selectedPlayer = islock;
    }

    public void setCameraTarget(GameObject target)
    {
        this.target = target;
        LockUnlockCamera(true);

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
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team1ID[0]);
            }
            else
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team2ID[0]);

            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer2)) )
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1ID.Count > 1)
            {
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team1ID[1]);
            }
            else if (GameManager.instanceGM.team2ID.Count > 1)
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team2ID[1] );
            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer3)) )
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1ID.Count > 2)
            {
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team1ID[2]);
            }
            else if (GameManager.instanceGM.team2ID.Count > 2)
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team2ID[2]);
            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer4)))
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1ID.Count > 3)
            {
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team1ID[3]);
            }
            else if ( GameManager.instanceGM.team2ID.Count > 3)
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team2ID[3]);
            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer5)))
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1ID.Count > 4)
            {
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team1ID[4]);
            }
            else if ( GameManager.instanceGM.team2ID.Count > 4)
                target = NetworkServer.FindLocalObject(GameManager.instanceGM.team2ID[4]);
            return true;
        }
        return false;
    }
}



