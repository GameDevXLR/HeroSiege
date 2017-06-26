using UnityEngine;
using System.Collections;

public class JieTu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	public int jietuID = 0;
	public string jietuname ;
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			jietuID++;
			jietuname ="screen_" +jietuID +".png";
			Application.CaptureScreenshot(jietuname,0);
			print(jietuname);
		}
	}
}
