using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerCanalisage : NetworkBehaviour {

    public float canalisage = 3;
    private bool interruption = false;
    public IEnumerator coroutine;
    public float timeBetweenCheck =0.1f;
    public Slider slider;
    public Canvas canvas;

    
    private void Awake()
    {
        canvas = GameObject.Find("CanalisationCanvas").GetComponent<Canvas>();
        slider = GameObject.Find("CanalisationCanvas").GetComponent<Slider>();
        slider.value = 0;
        canvas.enabled = false;
    }


    public void Update()
    {
        if (checkInterruption())
        {
            interruption = true;
        }
    }
    
    public void LaunchCanalisage(ICanalisage canalise, float time)
    {
        canalisage = time;
        if (enabled)
        {
            interruption = true;
            StartCoroutine(check(canalise, timeBetweenCheck +0.1f));
        }
        else
        {
            StartCoroutine(check(canalise));
        }
    }

    IEnumerator check(ICanalisage canalise, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        interruption = false;
        enabled = true;
        canvas.enabled = true;
        slider.value = 0;
        float time = 0;
        while (time < canalisage && !interruption)
        {
            time += timeBetweenCheck;
            slider.value = time / canalisage;
            yield return new WaitForSeconds(timeBetweenCheck);
        }
        if (interruption)
        {
            canalise.interruption();
            
        }
        else
        {
            canalise.success();
        }
        enabled = false;
        interruption = false;
        slider.value = 0;
        canvas.enabled = false;
    }

    public bool checkInterruption()
    {
        return (
            ( 
                Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.sort1))
                || Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.sort2))
                || Input.GetKey(CommandesController.Instance.getKeycode(CommandesEnum.sort3))
            )
            || Input.GetMouseButtonDown(1)
            );
    }
    
}
