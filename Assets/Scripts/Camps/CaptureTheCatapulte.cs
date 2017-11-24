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
		RpcCaptureParticle ();
        anime.Play();
    }


    public override void stopCapture()
    {
        base.stopCapture();
        ParticleSystem.EmissionModule em = cdParticles.emission;
        em.enabled = false;
        anime.Stop();
    }


	[ClientRpc]
	public void RpcCaptureParticle()
	{
		ParticleSystem.EmissionModule em = cdParticles.emission;
		em.enabled = true;
		Invoke("unactivateEffect", 10);
	}

	public void unactivateEffect()
	{
		ParticleSystem.EmissionModule em = cdParticles.emission;
		em.enabled = false;

	}
	[ClientRpc]
	public void RpcActuPos(Vector3 newPos)
	{
		transform.position = newPos;
	}

}
