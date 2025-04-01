using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Range = UnityEngine.SocialPlatforms.Range;

public class SoundItem : MonoBehaviour
{
    [SerializeField] private bool applyList;
    [SerializeField] private bool isReadonPlay;
    [SerializeField] private AudioClipData _audioClipData;
    [SerializeField] private AudioClip _audioClip;
    
    public AudioClipType audioType;
    private AudioSource _audioSource;
    
    private void OnValidate()
    {
        if (_audioClipData == null)
        {
            this.name = _audioClip.name;
        }
        else
        {
            this.name = _audioClipData.name;
        }
    }

    private void Awake()
    {
        if (applyList)
        {
            _audioClip = _audioClipData.GetRandomAudioClip();
        }
        else
        {
            isReadonPlay = false;
        }
        
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Spawm();
    }

    private void Start()
    {
        _audioSource.clip = _audioClip;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        _audioSource.Stop();
    }

    public void Spawm()
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();
        
        if (isReadonPlay)
        {
            ReadonClip();
        }
        _audioSource.Play();
    }

    private void ReadonClip()
    {
        _audioSource.clip = _audioClipData.GetRandomAudioClip();
    }
}
