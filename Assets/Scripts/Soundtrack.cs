using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundtrack : MonoBehaviour
{
    public List<AudioClip> soundtracks;
    public float crossFadeTime = 0;
    private AudioSource _audioSource1;
    private AudioSource _audioSource2;
    private float startOfPlayingNewTrack;
    private bool isChanging = false;
    private int indexOfTrack = 0;

    void Start()
    {
        var _audioSources = GetComponents<AudioSource>();
        _audioSource1 = _audioSources[0];
        _audioSource2 = _audioSources[1];
        
        _audioSource1.PlayOneShot(soundtracks[indexOfTrack]);
        startOfPlayingNewTrack = Time.time;
    }

    void Update()
    {
        if(Time.time - startOfPlayingNewTrack >= soundtracks[indexOfTrack].length-crossFadeTime){
            isChanging = true;
            indexOfTrack = (indexOfTrack + 1)%soundtracks.Count;
            startOfPlayingNewTrack = Time.time;
            if(indexOfTrack%2 == 1){
                _audioSource1.volume = 0f;
                _audioSource1.PlayOneShot(soundtracks[indexOfTrack]);
            }else{
                _audioSource2.volume = 0f;
                _audioSource2.PlayOneShot(soundtracks[indexOfTrack]);
            }
        }
        if(isChanging){
            if(Time.time-startOfPlayingNewTrack >= crossFadeTime){
                isChanging = false;
            }
            if(indexOfTrack%2 == 1){
                _audioSource1.volume = Mathf.Clamp01((Time.time - startOfPlayingNewTrack)/crossFadeTime);
                _audioSource2.volume = 1-_audioSource1.volume;
            }else{
                _audioSource2.volume = Mathf.Clamp01((Time.time - startOfPlayingNewTrack)/crossFadeTime);
                _audioSource1.volume = 1-_audioSource2.volume;
            }
        }
    }
}
