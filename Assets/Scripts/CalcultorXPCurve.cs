using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CalcultorXPCurve : MonoBehaviour 
{
	public int xpdeBase;
	public int[] xprecquiredByLvl;
	public int previousxp;

	// Use this for initialization
	void Start () 
	{
		Calculate ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public void Calculate()
	{
		for (int i = 0; i < 50; i++) 
		{
			if (i == 0) 
			{
				xprecquiredByLvl [i+1] = xpdeBase;
				previousxp = xpdeBase;
			}
			float j = 1f + ((float)i / 2f);
			float k  = (float)xpdeBase * j;
			xprecquiredByLvl [i+1] = (int)k + previousxp;
			previousxp = xprecquiredByLvl [i+1];
		}
	}
}
