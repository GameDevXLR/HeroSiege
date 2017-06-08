using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class MouseManager : MonoBehaviour 
{

//	public LayerMask layer_mask;
	public List<Outline> curTargets;
	public Outline selectedObj;

	private Outline hoverTarget;
	private bool isclicking;
	// Use this for initialization
	void Start () 
	{

	}

	// Update is called once per frame
	void Update ()
	{
		if (hoverTarget) 
		{
			hoverTarget.eraseRenderer = true;
			hoverTarget = null;
			curTargets.Clear ();
		}
		if (selectedObj) 
		{
			selectedObj.eraseRenderer = false;
		}

		if (isclicking) 
		{
			isclicking = false;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (selectedObj) {
				selectedObj.eraseRenderer = true;
			}
			selectedObj = null;
			isclicking = true;
		}
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit[] hit=Physics.RaycastAll (ray);

		foreach (RaycastHit  r in hit) 
		{
			if (r.collider.GetComponent<Outline>()) 
			{
				if(!curTargets.Contains(r.collider.GetComponent<Outline>()))
				{
					curTargets.Add (r.collider.GetComponent<Outline> ());
					if (hoverTarget == null || Vector3.Distance (Camera.main.transform.position, hoverTarget.transform.position) > Vector3.Distance (Camera.main.transform.position, r.collider.transform.position)) 
					{
						hoverTarget = r.collider.GetComponent<Outline>();
					}
//					break;
				}
				if (isclicking) 
				{
					if (selectedObj) 
					{
						selectedObj.eraseRenderer = true;
						selectedObj = null;
					}
					selectedObj = hoverTarget;
					if (selectedObj) {
						selectedObj.eraseRenderer = false;
					}
				}
			}

		}
		if (hoverTarget) 
		{
			hoverTarget.eraseRenderer = false;
		}
	}
}
	

