using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXItem : MonoBehaviour
{
    public ParticleSystem ps;

    public void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        ps.Play();
    }

    private void Update()
    {
        if (!ps.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        ps.Stop();
    }

    public void Spawm()
    {
       // Debug.Log("Spawm");
        if (ps.isPlaying)
        {
            gameObject.SetActive(false);
        }
       
        gameObject.SetActive(true);
    }
}
