using System.Collections;
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
    /// <summary>
    /// Instance de la camera
    /// </summary>
    public static CameraController instanceCamera;
    /// <summary>
    /// Object ciblé par la camera
    /// </summary>
    public GameObject target;

    public Animator behavior;

    /// <summary>
    /// Permet d'adapter l'image de Lock de la caméra 
    /// </summary>
    public UnityEvent changeCamLockImg;
    /// <summary>
    /// Défini si la caméra est en mode Lock ou non.
    /// </summary>
    public bool isLock = false;

    /// <summary>
    /// Défini la vitese de la caméra en mode libre
    /// </summary>
    public float speed = 5;
    /// <summary>
    /// Défini la vitesse de rotation de la caméra
    /// </summary>
    public float speedRotate = 3;
    /// <summary>
    /// Défini l'angle de rotation de la caméra
    /// </summary>
    public float angle = 90;

    /// <summary>
    /// Définie la zone de détection de la souris sur les bords en mode libre
    /// </summary>
    public int zoneDetectionMouse = 300;

    /// <summary>
    /// Défini la distance de la caméra par rapport à la cible ou à la layer ground
    /// </summary>
    public float distance = 15;
    public string MouseScrollWheel = "Mouse ScrollWheel";
    public float zoomSensitivity = 3f;
    public float minDistance = 5f;
    public float maxDistance = 20f;

    /// <summary>
    /// Permet de ne pas utiliser la caméra si elle n'est pas prête
    /// </summary>
    private bool isReady;
    /// <summary>
    /// Permet de savoir si la caméra film un autre object que le player
    /// </summary>
    public bool isAnotherPlayer;
    /// <summary>
    /// Permet de savoir si le perso est mort et permet d'éviter de modifier trop souvent le postprocessing
    /// </summary>
    public bool isDead = false; //permet d'éviter de modifier trop souvent le postprocessing

    /// <summary>
    /// Layer utilisé pour la caméra libre
    /// </summary>
    public int layer_mask;

    /// <summary>
    /// Camera cible pour la camera libre
    /// </summary>
	public Camera cameraCible;

    /// <summary>
    /// Les limites mises pour la camera libre
    /// </summary>
    public CameraBoundaries boundaries;

    /// <summary>
    /// Postprocessing utilisé lorsque le joueur meurt
    /// </summary>
    public PostProcessingBehaviour postProcessing;

    public bool isShaking = false;

    private Vector2 mousePos;

    public float yRef = 1.5f;
    public float xRef = 1;
    public float zRef = 0;
    public Vector3 vectCam;


    /// <summary>
    /// Limitation pour éviter de clicker sur la falaise
    /// </summary>
    public float yMinCamera = 15;

    //on s'assure en Awake que le script est bien unique. sinon on détruit le nouvel arrivant.
    void Awake()
    {
        if (instanceCamera == null)
        {
            instanceCamera = this;
            vectCam = new Vector3(xRef, yRef, zRef);
            enabled = false;
            

        }
        else if (instanceCamera != this)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        transform.position = Vector3.Lerp(transform.position, GameManager.instanceGM.playerObj.transform.position + vectCam * distance, speed * Time.deltaTime);
        // Set the camera to look towards the Player model
        transform.LookAt(GameManager.instanceGM.playerObj.transform);
        behavior.SetBool("isReady", true);
        cameraCible = GetComponent<Camera>();
        layer_mask = Layers.Ground; // ground layer 10
        enabled = true;
        
        behavior.SetBool("Lock", isLock);
        setSaturation(isDead);

    }

    void Update()
    {

        if (GameManager.instanceGM.playerObj && !isDead && GameManager.instanceGM.playerObj.GetComponent<PlayerIGManager>().isDead)
        {
            isDead = true;
            setSaturation(isDead);
        }
        else if (isDead && !GameManager.instanceGM.playerObj.GetComponent<PlayerIGManager>().isDead)
        {
            
            isDead = false;
            setSaturation(isDead);

        }
        if (!GameManager.instanceGM.isInTchat)
        {
            SelectNextCameraDistance();
            if (switchToOtherPlayer())
            {
                behavior.SetBool("Lock", true);
                isAnotherPlayer = true;
                if (!isLock)
                {
                    isLock = true;
                    changeCamLockImg.Invoke();
                }
            }
            else if (Input.GetKeyDown(CommandesController.Instance.getKeycode(CommandesEnum.CameraCenter)))
            {
                if (isAnotherPlayer)
                {
                    revertTargetToPlayer();
                    isAnotherPlayer = false;
                }
                behavior.SetBool("Lock", true);
                vectCam = new Vector3(xRef, yRef, zRef);
            }
            else if (Input.GetKeyUp(CommandesController.Instance.getKeycode(CommandesEnum.CameraCenter)))
            {
                if (!isLock)
                {
                    behavior.SetBool("Lock", false);
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

    public void MoveCameraTo(Vector3 vect)
    {
        if (vect != Vector3.zero)
        {
            gameObject.transform.position = vect + new Vector3(1, 2f, 0) * distance;
        }

    }

    public void areLocking()
    {
        isLock = !isLock;
        behavior.SetBool("Lock", isLock);
    }

    public void areLocking(bool islock)
    {
        behavior.SetBool("Lock", isLock);
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
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer2)))
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1Players.Count > 1)
            {
                target = GameManager.instanceGM.team1Players[1];
            }
            else if (GameManager.instanceGM.team2Players.Count > 1)
                target = GameManager.instanceGM.team2Players[1];
            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer3)))
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1Players.Count > 2)
            {
                target = GameManager.instanceGM.team1Players[2];
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
            else if (GameManager.instanceGM.team2Players.Count > 3)
                target = GameManager.instanceGM.team2Players[3];
            return true;
        }
        else if (Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.SwitchPlayer5)))
        {
            if (GameManager.instanceGM.isTeam1 && GameManager.instanceGM.team1Players.Count > 4)
            {
                target = GameManager.instanceGM.team1Players[4];
            }
            else if (GameManager.instanceGM.team2Players.Count > 4)
                target = GameManager.instanceGM.team2Players[4];
            return true;
        }
        return false;
    }



    public void changeToThird()
    {
        behavior.SetBool("Lock", true);
    }
    


    // If the user scrolls up on their mousewheel then zoom in, if they scroll down then zoom out
    private void SelectNextCameraDistance()
    {
        var mouseScroll = Input.GetAxis(MouseScrollWheel);
        if (!mouseScroll.Equals(0f))
        {
            var distanceChange = distance - mouseScroll * zoomSensitivity;
            distance = Mathf.Clamp(distanceChange, minDistance, maxDistance);
        }
    }

    public void setAngle(float y)
    {
        yRef = y;
        vectCam.y = yRef;
    }


    public void setSaturation(bool isdead)
    {
        var colograding = postProcessing.profile.colorGrading.settings;
        colograding.basic.saturation = (isdead)? 0:1;
        postProcessing.profile.colorGrading.settings = colograding;
    }

}



