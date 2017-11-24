using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionDisplay : MonoBehaviour {

	public Transform herosPanelObj;
	public Transform[] HerosTransformPanels;
	public AudioClip nextHeroSnd;
	public AudioClip prevHeroSnd;

	public void HeroScrollUp()
	{
		GameManager.instanceGM.GetComponent<AudioSource> ().PlayOneShot (nextHeroSnd);
		for (int i = 0; i < HerosTransformPanels.Length; i++) 
		{
			int j = HerosTransformPanels[i].GetSiblingIndex ();

			if (j == HerosTransformPanels.Length-1) {
				HerosTransformPanels[i].SetAsFirstSibling();
				break;
			}
			
		}
		HideShowDetailsOnHero ();

		
	}

	public void HeroScrollDown()
	{
		GameManager.instanceGM.GetComponent<AudioSource> ().PlayOneShot (prevHeroSnd);

		for (int i = 0; i < HerosTransformPanels.Length; i++) 
		{
			int j = HerosTransformPanels[i].GetSiblingIndex ();

			if (j == 0) {
				HerosTransformPanels[i].SetAsLastSibling();
				break;
			}

		}
		HideShowDetailsOnHero ();
	}

	public void HideShowDetailsOnHero()
	{
		for (int i = 0; i < HerosTransformPanels.Length; i++) 
		{
			if (i == 1) {

				herosPanelObj.GetChild(i).GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 700);
				herosPanelObj.GetChild(i).GetChild (0).gameObject.SetActive(true);
				herosPanelObj.GetChild (i).GetChild (0).GetComponentInChildren<Button> ().onClick.Invoke ();
			} else 
			{
				herosPanelObj.GetChild(i).GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, 250);
				herosPanelObj.GetChild(i).GetChild (0).gameObject.SetActive(false);

			}
		}
	}

}
