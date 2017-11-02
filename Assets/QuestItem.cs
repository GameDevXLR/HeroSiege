using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuestItem : NetworkBehaviour {


	public Text questDesc;
	public Text repReward;
	[SyncVar]public string questDescTxt;
	[SyncVar]public int repRewardInt;

	public override void OnStartClient ()
	{
		base.OnStartClient ();
		transform.SetParent(GameManager.instanceGM.GetComponent<QuestManager> ().QuestPanel.transform, false);
		transform.localScale = Vector3.one;
		questDesc.text = questDescTxt;
		repReward.text = repRewardInt.ToString ();
	}

}
