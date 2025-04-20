using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXItem : MonoBehaviour
{
    public ParticleSystem ps;
    public Action OnFinished;

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
            OnFinished?.Invoke();
        }
    }

    private void OnDisable()
    {
        ps.Stop();
    }

    public void Spawn(Transform parent)
    {
        if (parent != null)
        {
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = new Vector3(0, parent.localPosition.y + 1.5f, 0);
        }
        if (ps.isPlaying)
        {
            gameObject.SetActive(false);
        }
       
        gameObject.SetActive(true);
    }
}
