using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {


	public int myBestScore;
	public int myActualScore;
	string myName = "NewPlayer";
	dreamloLeaderBoard leaderBoard;
	List<dreamloLeaderBoard.Score> allScores;
	public Dictionary<string, int> scoreDico;
//	bool isFirstGame = true;
	public GameObject endScorePanel;
	public GameObject playerScoreDisplayPrefab;
	Text endGameDisplayerTxt;
	string bestplayerTxt;
	// Use this for initialization
	void Start () 
	{
		leaderBoard = GetComponent<dreamloLeaderBoard>();
		leaderBoard.LoadScores ();
		myName = PlayerPrefs.GetString ("PlayerNN", "NewPlayer");
		myBestScore = PlayerPrefs.GetInt ("PlayerScore", 0);
		StartCoroutine (DelayTheActuOfScore ());
	}
	IEnumerator DelayTheActuOfScore()
	{
		yield return new WaitForSecondsRealtime (4f);
//		if (allScores == null) 
//		{
//			yield return new WaitForSecondsRealtime (2f);
//			if (allScores == null) {
//				yield return new WaitForSecondsRealtime (3f);
//				if (allScores == null) {
//					Debug.Log ("pc en mousse ou sans internet??");
//				}
//			}
//		}
		StartProcedure ();

	}

	public void InitializeThisInGame()
	{
		myActualScore = 0;
		endScorePanel = GameObject.Find ("EndScorePanel");
		endScorePanel.SetActive (false);
	}

	public void ShowEndGamePanel()
	{
		StartCoroutine (EndGameScoreProcedure());
	}
	IEnumerator EndGameScoreProcedure()
	{
		leaderBoard.LoadScores ();
		myName = PlayerPrefs.GetString ("PlayerNN", "NewPlayer");

		yield return new WaitForSecondsRealtime(3f);
		allScores = leaderBoard.ToListHighToLow ();
		yield return allScores;
		endScorePanel.SetActive (true);
		endScorePanel.transform.Find ("MyScorePanel").GetChild (1).GetComponent<Text> ().text = myName;
		endScorePanel.transform.Find ("MyScorePanel").GetChild (0).GetComponent<Text> ().text = myActualScore.ToString();
		endGameDisplayerTxt = endScorePanel.transform.Find ("MyScorePanel").GetChild (3).GetComponent<Text> ();
		if (myActualScore > myBestScore) 
		{
			myBestScore = myActualScore;
			endGameDisplayerTxt.text = "CONGRATULATION! It's your highest score!";
		}
		endScorePanel.transform.Find ("MyScorePanel").GetChild (2).GetComponent<Text> ().text = myBestScore.ToString();
		PlayerPrefs.SetInt ("PlayerScore", myBestScore);


		int j = 0;
		int maxDisplayed = 4;
		foreach (dreamloLeaderBoard.Score score in allScores) 
		{
			if (j> maxDisplayed) 
			{
				continue;
			}
//				StopCoroutine (EndGameScoreProcedure());
				GameObject go = GameObject.Instantiate (playerScoreDisplayPrefab, endScorePanel.transform, false);
			go.transform.Find ("PlayerName").GetComponent<Text> ().text = score.playerName;
			go.transform.Find ("PlayerScore").GetComponent<Text> ().text = score.score.ToString();
			if (score.playerName == myName && myBestScore == score.score) 
			{
				go.GetComponent<Image> ().color = Color.green;
				if (j == 1 ) 
				{
					endGameDisplayerTxt.text = "You are the best Worldwide!!!";
					yield break;
				}
				if(myActualScore<myBestScore)
				{
					endGameDisplayerTxt.text = "You did better last time, you'll do better next time!";
				}
			}
			j ++;

		}
	
	}

	public void AddMyScoreToLeaderBoard()
	{
		if (myActualScore >= myBestScore) {
			leaderBoard.AddScore (myName, myActualScore);
		}
	}

	public void StartProcedure(){
		StartCoroutine (GetTheScores ());
		
	}
	IEnumerator GetTheScores()
	{
		myName = PlayerPrefs.GetString ("PlayerNN");

		int i = 0;
		allScores = leaderBoard.ToListHighToLow ();
		yield return allScores;
		Text disclaimer;
		disclaimer = GameObject.Find ("DisclaimerMessage").GetComponent<Text> ();
		bestplayerTxt =  "The best score ever is: " + allScores [0].score + ". It was achieved by " + allScores [0].playerName;
		disclaimer.text= bestplayerTxt;
		yield return new WaitForSecondsRealtime (5f);
		if (!disclaimer) 
		{
			yield break;
		}
		if (myBestScore > 0) 
		{
			disclaimer.text = "Your best score is " + myBestScore.ToString () + ". Well done! Harder mods mean more points.";
		}
		yield return new WaitForSecondsRealtime (7f);
		if (!disclaimer) 
		{
			yield break;
		}
		disclaimer.text = "You can clic on 'One Lane Game' to play only on one side in coop with your friends.";
		yield return new WaitForSecondsRealtime (9f);
		if (!disclaimer) 
		{
			yield break;
		}
		disclaimer.text = "Press Space to center the view on your hero, L to lock/unlock the camera view";
		yield return new WaitForSecondsRealtime (11f);
		if (!disclaimer) 
		{
			yield break;
		}
		disclaimer.text = "Capturing the jungle camp means: twice more problems for the enemy team!";
		yield return new WaitForSecondsRealtime (15f);
		if (!disclaimer) 
		{
			yield break;
		}
		disclaimer.text = bestplayerTxt;
	}


//
//		foreach (dreamloLeaderBoard.Score score in allScores) 
//		{
//			if (i == 0) 
//			{
//				Debug.Log ("The best score ever is: " + score.score + ". It was achieved by " + score.playerName);
//			}
//			i++;
//			if (score.playerName == myName) 
//			{
//				isFirstGame = false;
//				myBestScore = score.score;
//			}
//			if (i == allScores.Count) 
//			{
//				if (isFirstGame) {
//					Debug.Log ("premiere partie: aucuns score enregistré avec ce pseudo.");
//				} else 
//				{
//					Debug.Log ("On a récup un score avec ton pseudo associé.");
//				}
//			}
//	}
}
