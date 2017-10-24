using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowAndHideImg : MonoBehaviour {

	public Image plantsMinimap;
	public Image sceneryMinimap;
	public Sprite plantBtnActive;
	public Sprite plantBtnInactive;
	public Sprite sceneryBtnActive;
	public Sprite sceneryBtnInactive;
	public Sprite camLockActive;
	public Sprite camLockInactive;
	public Button plantBtn;
	public Button sceneryBtn;
	public Button camLockBtn;


	public void ShowHidePlants()
	{
		plantsMinimap.enabled = !plantsMinimap.enabled;
		if (plantsMinimap.enabled) 
		{
			plantBtn.GetComponent<Image> ().sprite = plantBtnActive;
		} else 
		{
			plantBtn.GetComponent<Image> ().sprite = plantBtnInactive;

		}
	}
	public void ShowHideScenery()
	{
		sceneryMinimap.enabled = !sceneryMinimap.enabled;
		if (sceneryMinimap.enabled) 
		{
			sceneryBtn.GetComponent<Image> ().sprite = sceneryBtnActive;
		} else 
		{
			sceneryBtn.GetComponent<Image> ().sprite = sceneryBtnInactive;

		}
	}

	public void lockUnlockCam()
	{
		if (Camera.main.GetComponent<CameraController> ().selectedPlayer) 
		{
			camLockBtn.GetComponent<Image> ().sprite = camLockActive;
		} else 
		{
			camLockBtn.GetComponent<Image> ().sprite = camLockInactive;

		}
		
	}
}
