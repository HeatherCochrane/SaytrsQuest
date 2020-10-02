using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioHandler : MonoBehaviour
{
    [SerializeField]
    AudioSource playerSteps;

    [SerializeField]
    AudioSource playerHits;

    [SerializeField]
    AudioSource misc;

    [SerializeField]
    List<AudioClip> playerStep = new List<AudioClip>();

    [SerializeField]
    AudioClip playerJump;

    [SerializeField]
    AudioClip hitSound;

    [SerializeField]
    AudioClip damage;

    [SerializeField]
    AudioClip pickUp;

    [SerializeField]
    AudioClip drop;

    [SerializeField]
    AudioClip door;


    [SerializeField]
    GameObject audioGraphic;
    [SerializeField]
    Sprite on;
    [SerializeField]
    Sprite off;

    bool audioOff = false;

    [SerializeField]
    AudioSource atmosphere;

    [SerializeField]
    AudioClip pickup;

    [SerializeField]
    List<AudioClip> enemyKilled = new List<AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        setAudioOn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void enemyKill()
    {
        misc.clip = enemyKilled[Random.Range(0, enemyKilled.Count)];
        misc.Play();
    }

    public void playPickup()
    {
        playerHits.clip = pickup;
        playerHits.Play();
    }
    public void playJump()
    {
        playerHits.clip = playerJump;
        playerHits.Play();
    }
    public void openDoor()
    {
        misc.clip = door;
        misc.Play();
    }

    public void playDrop()
    {
        playerHits.clip = drop;
        playerHits.Play();
    }
    public void playPickUp()
    {
        playerHits.clip = pickUp;
        playerHits.Play();
    }

    public void takeDamage()
    {
        playerHits.clip = damage;
        playerHits.Play();
    }
    public void playHitNoise()
    {
        playerHits.clip = hitSound;
        playerHits.Play();
    }

    public void playPlayerStep()
    {
        playerSteps.Stop();
        playerSteps.volume = 0.4f;
        playerSteps.pitch = Random.Range(0.7f, 1.2f);
        playerSteps.clip = playerStep[Random.Range(0, playerStep.Count)];
        playerSteps.Play();
    }

    public void setAudioOn()
    {
        if (audioOff)
        {
            audioOff = false;
            audioGraphic.GetComponent<Image>().sprite = on;
        }
        else
        {
            audioOff = true;
            audioGraphic.GetComponent<Image>().sprite = off;
        }

        playerSteps.mute = audioOff;
        playerHits.mute = audioOff;
        misc.mute = audioOff;
        atmosphere.mute = audioOff;
    }

    public void endScreenAudio()
    {
        playerSteps.mute = true;
        playerHits.mute = true;
        misc.mute = true;
        atmosphere.mute = audioOff;

    }

    public void setAudioBack()
    {
        playerSteps.mute = audioOff;
        playerHits.mute = audioOff;
        misc.mute = audioOff;
        atmosphere.mute = audioOff;
    }
}
