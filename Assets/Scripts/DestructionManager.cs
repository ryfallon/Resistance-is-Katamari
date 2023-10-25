using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionManager : MonoBehaviour
{
    public ParticleSystem energyExp;
    public ParticleSystem bigExp;

    public void Play()
    {
        energyExp.Play(true);
        energyExp.GetComponent<AudioSource>().Play();
        bigExp.Play(true);
        bigExp.GetComponent<AudioSource>().Play();
    }
}
