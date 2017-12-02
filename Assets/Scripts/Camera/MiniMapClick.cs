using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class MiniMapClick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
   
    float minimapHeight;
    float minimapWidth;
    //private KeyCode centerBackKey = KeyCode.Space;
    public Camera cam;
    public GameObject ping;


    private void Start()
    {
       
        minimapHeight = GetComponent<RectTransform>().rect.height;
        minimapWidth = GetComponent<RectTransform>().rect.width;
        
    }



    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.instanceGM.playerObj.GetComponent<PlayerClicToMove>().isInMiniMap = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 localHit = transform.InverseTransformPoint(eventData.position);
        if (Input.GetKey(KeyCode.LeftControl))
        {
//            Debug.Log("controll");
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("activation + " + eventData.position);
                //Vector3 pingPos = new Vector3(eventData.position.x / Screen.width ,eventData.position.y / Screen.height,0);
                //GameManager.instanceGM.playerObj.GetComponent<PlayerManager>().recevePingPosition(pingPos);
                sendPing(localHit);
            }
                
        }

        else if (Input.GetMouseButtonDown(0))
        {
            CameraController.instanceCamera.initialiseMinimapState();
            moveCamera(localHit);
        }
        else if (Input.GetMouseButtonDown(1))
            movePLayer(localHit);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 localHit = transform.InverseTransformPoint(eventData.position);
        if (localHit.x < minimapWidth && localHit.x > 0 && localHit.y > -minimapHeight && localHit.y < 0)
        {
            if (Input.GetMouseButton(0))
                moveCamera(localHit);
            
            else if (Input.GetMouseButtonDown(1))
                movePLayer(localHit);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CameraController.instanceCamera.endMinimapState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instanceGM.playerObj.GetComponent<PlayerClicToMove>().isInMiniMap = false;
    }

    

    private void moveCamera(Vector3 localHit)
    {
        Vector3 vector = new Vector3(Math.Abs(localHit.x / minimapWidth), 1 - Math.Abs(localHit.y / minimapHeight), 0);
        Ray ray = cam.ViewportPointToRay(vector);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Camera.main.GetComponent<CameraController>().setTargetVect(hit.point);
        }
    }

    private void movePLayer(Vector3 localHit)
    {
        Vector3 vector = new Vector3(Math.Abs(localHit.x / minimapWidth), 1 - Math.Abs(localHit.y / minimapHeight), 0);
        Ray ray = cam.ViewportPointToRay(vector);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameManager.instanceGM.playerObj.GetComponent<PlayerClicToMove>().movePlayer(hit);
           

        }
    }

    private void sendPing(Vector3 localHit)
    {
        Vector3 vector = new Vector3(Math.Abs(localHit.x / minimapWidth), 1 - Math.Abs(localHit.y / minimapHeight), 0);
        Ray ray = cam.ViewportPointToRay(vector);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameManager.instanceGM.playerObj.GetComponent<PlayerManager>().recevePingPosition(hit.point);
//            Debug.Log("moau");
        }
    }



    //public void sendPing(Vector3 pingPos)
    //{


    //    Vector3 truePos = new Vector3(pingPos.x * Screen.width,pingPos.y * Screen.height,0);
    //    Debug.Log("position : " + truePos);
    //    //ping.transform.position = truePos;
    //    GameObject newPing = Instantiate(ping, truePos, ping.transform.rotation) as GameObject;

    //    newPing.transform.SetParent(transform);
    //}




}