using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace EmpireRush.Utils
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource uiSource;
        
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixerGroup masterMixerGroup;
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private AudioMixerGroup uiMixerGroup;
        
        [Header("Audio Clips")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] private List<AudioClip> buttonClickSounds;
        [SerializeField] private List<AudioClip> coinSounds;
        [SerializeField] private List<AudioClip> energySounds;
        [SerializeField] private List<AudioClip> businessSounds;
        [SerializeField] private List<AudioClip> spinWheelSounds;
        [SerializeField] private List<AudioClip> notificationSounds;
        
        [Header("Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;
        [SerializeField] private float uiVolume = 0.8f;
        [SerializeField] private bool muteOnFocusLoss = true;
        
        private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
        private bool isMuted = false;
        private float previousMasterVolume;
        
        public static event System.Action<float> OnMasterVolumeChanged;
        public static event System.Action<float> OnMusicVolumeChanged;
        public static event System.Action<float> OnSFXVolumeChanged;
        public static event System.Action<bool> OnMuteChanged;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeAudioManager()
        {
            // Create audio sources if they don't exist
            if (musicSource == null)
                musicSource = CreateAudioSource("MusicSource", musicMixerGroup);
                
            if (sfxSource == null)
                sfxSource = CreateAudioSource("SFXSource", sfxMixerGroup);
                
            if (uiSource == null)
                uiSource = CreateAudioSource("UISource", uiMixerGroup);
            
            // Setup audio sources
            SetupAudioSources();
            
            // Load audio clips into dictionary
            LoadAudioClips();
            
            // Load saved audio settings
            LoadAudioSettings();
            
            // Start background music
            PlayBackgroundMusic();
        }
        
        private AudioSource CreateAudioSource(string name, AudioMixerGroup mixerGroup)
        {
            GameObject audioSourceObj = new GameObject(name);
            audioSourceObj.transform.SetParent(transform);
            
            AudioSource source = audioSourceObj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixerGroup;
            source.playOnAwake = false;
            
            return source;
        }
        
        private void SetupAudioSources()
        {
            // Music source setup
            if (musicSource != null)
            {
                musicSource.loop = true;
                musicSource.volume = musicVolume;
                musicSource.outputAudioMixerGroup = musicMixerGroup;
            }
            
            // SFX source setup
            if (sfxSource != null)
            {
                sfxSource.loop = false;
                sfxSource.volume = sfxVolume;
                sfxSource.outputAudioMixerGroup = sfxMixerGroup;
            }
            
            // UI source setup
            if (uiSource != null)
            {
                uiSource.loop = false;
                uiSource.volume = uiVolume;
                uiSource.outputAudioMixerGroup = uiMixerGroup;
            }
        }
        
        private void LoadAudioClips()
        {
            audioClips.Clear();
            
            // Add clips to dictionary for easy access
            if (backgroundMusic != null)
                audioClips["background_music"] = backgroundMusic;
            
            AddClipsToDict("button_click", buttonClickSounds);
            AddClipsToDict("coin", coinSounds);
            AddClipsToDict("energy", energySounds);
            AddClipsToDict("business", businessSounds);
            AddClipsToDict("spin_wheel", spinWheelSounds);
            AddClipsToDict("notification", notificationSounds);
        }
        
        private void AddClipsToDict(string baseName, List<AudioClip> clips)
        {
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i] != null)
                    audioClips[$"{baseName}_{i}"] = clips[i];
            }
        }
        
        #region Public Audio Methods
        
        public void PlayButtonClick()
        {
            PlayRandomUISound(buttonClickSounds);
        }
        
        public void PlayCoinSound()
        {
            PlayRandomSFXSound(coinSounds);
        }
        
        public void PlayEnergySound()
        {
            PlayRandomSFXSound(energySounds);
        }
        
        public void PlayBusinessSound()
        {
            PlayRandomSFXSound(businessSounds);
        }
        
        public void PlaySpinWheelSound()
        {
            PlayRandomSFXSound(spinWheelSounds);
        }
        
        public void PlayNotificationSound()
        {
            PlayRandomUISound(notificationSounds);
        }
        
        public void PlayCustomSound(string clipName, AudioSource source = null)
        {
            if (audioClips.TryGetValue(clipName, out AudioClip clip))
            {
                if (source == null)
                    source = sfxSource;
                    
                source.PlayOneShot(clip);
            }
        }
        
        public void PlayCustomSound(AudioClip clip, AudioSource source = null)
        {
            if (clip != null)
            {
                if (source == null)
                    source = sfxSource;
                    
                source.PlayOneShot(clip);
            }
        }
        
        #endregion
        
        #region Private Audio Methods
        
        private void PlayRandomSFXSound(List<AudioClip> clips)
        {
            if (clips != null && clips.Count > 0 && sfxSource != null)
            {
                AudioClip clip = clips.GetRandomElement();
                if (clip != null)
                    sfxSource.PlayOneShot(clip);
            }
        }
        
        private void PlayRandomUISound(List<AudioClip> clips)
        {
            if (clips != null && clips.Count > 0 && uiSource != null)
            {
                AudioClip clip = clips.GetRandomElement();
                if (clip != null)
                    uiSource.PlayOneShot(clip);
            }
        }
        
        private void PlayBackgroundMusic()
        {
            if (musicSource != null && backgroundMusic != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }
        
        #endregion
        
        #region Volume Control
        
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            
            if (masterMixerGroup != null)
                masterMixerGroup.audioMixer.SetFloat("MasterVolume", LinearToDecibel(masterVolume));
            
            SaveAudioSettings();
            OnMasterVolumeChanged?.Invoke(masterVolume);
        }
        
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            
            if (musicMixerGroup != null)
                musicMixerGroup.audioMixer.SetFloat("MusicVolume", LinearToDecibel(musicVolume));
            else if (musicSource != null)
                musicSource.volume = musicVolume;
            
            SaveAudioSettings();
            OnMusicVolumeChanged?.Invoke(musicVolume);
        }
        
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            
            if (sfxMixerGroup != null)
                sfxMixerGroup.audioMixer.SetFloat("SFXVolume", LinearToDecibel(sfxVolume));
            else if (sfxSource != null)
                sfxSource.volume = sfxVolume;
            
            SaveAudioSettings();
            OnSFXVolumeChanged?.Invoke(sfxVolume);
        }
        
        public void SetUIVolume(float volume)
        {
            uiVolume = Mathf.Clamp01(volume);
            
            if (uiMixerGroup != null)
                uiMixerGroup.audioMixer.SetFloat("UIVolume", LinearToDecibel(uiVolume));
            else if (uiSource != null)
                uiSource.volume = uiVolume;
            
            SaveAudioSettings();
        }
        
        public void ToggleMute()
        {
            SetMute(!isMuted);
        }
        
        public void SetMute(bool mute)
        {
            isMuted = mute;
            
            if (isMuted)
            {
                previousMasterVolume = masterVolume;
                SetMasterVolume(0f);
            }
            else
            {
                SetMasterVolume(previousMasterVolume);
            }
            
            OnMuteChanged?.Invoke(isMuted);
        }
        
        private float LinearToDecibel(float linear)
        {
            if (linear <= 0f)
                return -80f;
            return 20f * Mathf.Log10(linear);
        }
        
        #endregion
        
        #region Settings Persistence
        
        private void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            uiVolume = PlayerPrefs.GetFloat("UIVolume", 0.8f);
            isMuted = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
            
            // Apply loaded settings
            SetMasterVolume(masterVolume);
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
            SetUIVolume(uiVolume);
            
            if (isMuted)
                SetMute(true);
        }
        
        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetFloat("UIVolume", uiVolume);
            PlayerPrefs.SetInt("AudioMuted", isMuted ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        #endregion
        
        #region Application Focus
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (muteOnFocusLoss)
            {
                if (hasFocus)
                {
                    // Resume audio
                    AudioListener.pause = false;
                }
                else
                {
                    // Pause audio
                    AudioListener.pause = true;
                }
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationFocus(!pauseStatus);
        }
        
        #endregion
        
        #region Getters
        
        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
        public float GetUIVolume() => uiVolume;
        public bool IsMuted() => isMuted;
        
        #endregion
    }
}