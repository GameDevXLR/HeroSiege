using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperLuminalGames;
using UnityEngine.Networking;

public class CaptureTheCatapulte : CaptureThePoint
{

    public Animation anime;


    public override void captureThePoint(PointOwner target)
    {
        base.captureThePoint(target);
        anime.Play();
    }


    public override void stopCapture()
    {
        base.stopCapture();
        anime.Stop();
    }

}
