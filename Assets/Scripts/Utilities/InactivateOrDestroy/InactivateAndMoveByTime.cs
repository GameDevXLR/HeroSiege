using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InactivateAndMoveByTime : InactivateByTime
{

    public Transform parent;

    public override void inactivate()
    {
        base.inactivate();
        gameObject.transform.SetParent(parent, false);
    }
}
