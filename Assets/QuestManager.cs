using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class QuestManager : NetworkBehaviour 
{
	public GameObject QuestPanel;
	public GameObject QuestItemPrefab;
	public List<GameObject> questItemList;

	[Server]
	public GameObject CreateQuestPanelItem(string questDescription, int questRewardAmount)
	{
		GameObject go = (GameObject)Instantiate (QuestItemPrefab, QuestPanel.transform);
		go.GetComponent<QuestItem> ().questDescTxt = questDescription;
		go.GetComponent<QuestItem> ().repRewardInt = questRewardAmount;
		questItemList.Add (go);
		NetworkServer.Spawn (go);
		return go.gameObject;
	}

	[Server]
	public void StartRandomQuest()
	{
		int i = Random.Range (0, 1);
		switch (i) 
		{
		case 0:
			break;


		default:
			break;
		}
	}

	#region Liste des quêtes


	#endregion
}
