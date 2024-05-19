using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip waterSfx;
    [SerializeField] private AudioClip correctSfx;
    [SerializeField] private AudioClip popSfx;
    [SerializeField] private AudioClip pushSfx;
    [SerializeField] private AudioClip epicSfx;
    
    [Header("Sound Display")]
    [SerializeField] private TMP_Text soundDisplay;
    private AudioSource _audioSource;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _audioSource = GetComponent<AudioSource>();
            soundDisplay.text = VolumePercentage(_audioSource.volume) + " %";
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        } 
    }

    public void SliderSoundMaster(float value)
    {
        _audioSource.volume = value;
        soundDisplay.text = VolumePercentage(_audioSource.volume) + " %";
    }

    int VolumePercentage(float volumeValue)
    {
        return Mathf.RoundToInt(volumeValue * 100);
    }

    public void WaterSound()
    {
        _audioSource.clip = waterSfx;
        _audioSource.Play();
    }

    public void StopPlaying()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }

    public void CorrectSound()
    {
        _audioSource.clip = correctSfx;
        _audioSource.Play();
    }
    
}
