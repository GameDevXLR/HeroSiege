using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class MiniMapClick : MonoBehaviour, IPointerDownHandler, IDragHandler
{
   
    float minimapHeight;
    float minimapWidth;
    public Camera cam;


    private void Start()
    {
       
        minimapHeight = GetComponent<RectTransform>().rect.height;
        minimapWidth = GetComponent<RectTransform>().rect.width;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 localHit = transform.InverseTransformPoint(eventData.position);
        if (Input.GetMouseButtonDown(0))
            moveCamera(localHit);
        if (Input.GetMouseButtonDown(1))
            movePLayer(localHit);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 localHit = transform.InverseTransformPoint(eventData.position);
        if (Input.GetMouseButton(0))
        {
            
            if (localHit.x < minimapWidth && localHit.x > 0 && localHit.y > -minimapHeight && localHit.y < 0)
            {
                moveCamera(localHit);
            }
        }
            
    }

    private void moveCamera(Vector3 localHit)
    {
        Vector3 vector = new Vector3(Math.Abs(localHit.x / minimapWidth), 1 - Math.Abs(localHit.y / minimapHeight), 0);
        Ray ray = cam.ViewportPointToRay(vector);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Camera.main.GetComponent<CameraController>().MoveCameraTo(hit.point);

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




}