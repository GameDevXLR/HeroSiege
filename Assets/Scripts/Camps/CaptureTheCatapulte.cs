using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HyperLuminalGames;
using UnityEngine.Networking;

public class CaptureTheCatapulte : CaptureThePoint
{

    public Animation anime;
    public ParticleSystem areaParticles;
    public ParticleSystem cdParticles;

    public bool emissionGo;


    public override void captureThePoint(PointOwner target)
    {
        base.captureThePoint(target);
        ParticleSystem.EmissionModule em = cdParticles.emission;
        em.enabled = true;
        anime.Play();
    }


    public override void stopCapture()
    {
        base.stopCapture();
        ParticleSystem.EmissionModule em = cdParticles.emission;
        em.enabled = false;
        anime.Stop();
    }

}
