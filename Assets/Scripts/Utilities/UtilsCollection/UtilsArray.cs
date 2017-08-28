using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilsArray {

	public static int getFirstInactiveObject(List<GameObject> list)
    {
        int result = 0;
        while (result < list.Count && list[result].activeInHierarchy)
        {
            result++;
        }
        return result;
    }

    public static string[] getSubArray(string[] array,  int firstElement, int range)
    {
        return new List<string>(array).GetRange(firstElement, range).ToArray();
    }
}
