using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testSliderText : MonoBehaviour {

    public Slider slid;
    public Text description;
    public string title;


	// Use this for initialization
	void Start () {
        setText();
        slid.onValueChanged.AddListener(delegate { setText(); });
	}

    void setText()
    {
        description.text = title + " : " + slid.value.ToString("#.00");
    }
}
