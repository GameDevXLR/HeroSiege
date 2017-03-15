using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertMessageManager : MonoBehaviour 
{
	public GameObject messPrefab;
	private Transform messagesBox;

	void Start()
	{
		messagesBox = GameObject.Find ("MessagesPanel").transform;
	}
	public void SendAnAlertMess(string message, Color color)
	{
		GameObject go = Instantiate (messPrefab, messagesBox, false);
		go.GetComponentInChildren<Text> ().text = message;
		go.GetComponent<Image> ().color = color;
	}
}
