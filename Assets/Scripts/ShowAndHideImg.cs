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

	public Transform bigMapBtn;
	public GameObject smallMapBtn;
	public RectTransform minimapArea;
	public bool isBig;
//	public Quaternion rotImgMinimap;


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
		if (Camera.main.GetComponent<CameraController> ().behavior.GetBool("Lock")) 
		{
			camLockBtn.GetComponent<Image> ().sprite = camLockActive;
		} else 
		{
			camLockBtn.GetComponent<Image> ().sprite = camLockInactive;

		}
		
	}

	public void ShowHideBigMap()
	{
		isBig = !isBig;
		if (!isBig) 
		{
			bigMapBtn.gameObject.SetActive (true);
			smallMapBtn.SetActive (false);
			minimapArea.localScale = new Vector3 (1, 1, 1);
			minimapArea.anchoredPosition = Vector3.zero;
//			bigMapBtn.rotation = Quaternion.Inverse (rotImgMinimap);
		} else 
		{
			bigMapBtn.gameObject.SetActive (false);
			smallMapBtn.SetActive (true);
			minimapArea.localScale = new Vector3 (2.45f,2.45f, 1f);
			minimapArea.anchoredPosition = new Vector3 (0, 160f, 0f);
//			bigMapBtn.rotation = rotImgMinimap;

		}
		
	}
}
