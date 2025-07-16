using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    [Header("Audio Clips")]
    public AudioClip[] backgroundMusic;
    public AudioClip[] gameMusic;

    [Header("Common SFX")]
    public AudioClip teleportMusic;
    public AudioClip coinDrop;
    public AudioClip treasureChest;

    [Header("Panel")]
    public AudioClip buttonClick;
    public AudioClip[] victoryEffect;
    public AudioClip[] defeatEffect;

    [Header("Map 1")]
    public AudioClip map1;
    //mini wolf
    public AudioClip wolfAttack;
    public AudioClip wolfRange;
    //skeleton
    public AudioClip skeletonAttack;
    //Boss Wolf
    public AudioClip bossDoorClose;
    public AudioClip bossWolfRoar;
    public AudioClip bossWolfDash;
    public AudioClip bossWolfSlam;
    public AudioClip bossWolfDeath;

    [Header("Map 2")]
    public AudioClip map2;

    //Enemy
    public AudioClip beholderAttack;
    public AudioClip beholderDetect;

    //Boss
    //Âm thanh khi phát hiện player
    public AudioClip reaperDetect;
    //đánh cận chiến
    public AudioClip reaperMeleeAttack;
    //đánh tầm xa
    public AudioClip reaperThunderAttack;
    //chết
    public AudioClip reaperDeath;

    [Header("Map 3")]
    public AudioClip map3;
    //Trap
    public AudioClip[] boom;

    [Header("Map 4")]
    public AudioClip map4;
    //Enemy wolf
    public AudioClip meleeEnemyAttack;
    public AudioClip meleeEnemyDeath;

    //Enemy death4
    public AudioClip dead4CloseAttack;
    public AudioClip dead4Summon;
    public AudioClip dead4Storm;
    public AudioClip dead4Death;
    //Enemy DAttack
    public AudioClip dAttackSound;
    public AudioClip dAttackDeath;

    [Header("Map 5")]
    public AudioClip map5;

    //Enemy
    public AudioClip map5EnemyAttack;
    public AudioClip map5EnemyDeath;
    //Demon Bat
    public AudioClip demonBatDetect;
    public AudioClip demonBatAttack;
    public AudioClip demonBatDeath;
    //Mini Boss
    public AudioClip miniBossAttack1;
    public AudioClip miniBossAttack2;
    public AudioClip miniBossDeath;
    //Mini Boss 1
    public AudioClip miniBoss1Attack;
    public AudioClip miniBoss1Death;
    //Mini Boss Demon
    public AudioClip miniBossDemonAttack;
    public AudioClip miniBossDemonDeath;
    //Evil Wizard
    public AudioClip wizardAttack;
    public AudioClip wizardDeath;
    //Demon Red
    public AudioClip demonRedAttack1;
    public AudioClip demonRedAttack2;
    public AudioClip demonRedDeath;
    //Final Boss
    public AudioClip finalBossAttack1; // Holy Cross
    public AudioClip finalBossAttack2; // Moon Strike
    public AudioClip finalBossAttack3; // Skull Blast
    public AudioClip finalBossDeath;


    [Header("Player Actions")]
    public AudioClip playerFootstep;
    public AudioClip attack;
    public AudioClip attack2;
    public AudioClip dash;
    public AudioClip defend;
    public AudioClip jump;
    public AudioClip spell1;
    public AudioClip spell2;
    public AudioClip spell3;
    public AudioClip[] playerHurt;
    public AudioClip[] hurtEnemy;
    public AudioClip[] gainHealth;

    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Auto-setup AudioSources nếu chưa assign
            SetupAudioSources();

            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void SetupAudioSources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        if (musicSource == null && sources.Length > 0)
        {
            musicSource = sources[0];
            musicSource.loop = true;
            Debug.Log("Auto-assigned musicSource");
        }

        if (sfxSource == null)
        {
            if (sources.Length > 1)
            {
                sfxSource = sources[1];
            }
            else
            {
                // Tạo AudioSource mới cho SFX
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            sfxSource.loop = false;
            Debug.Log("Auto-assigned sfxSource");
        }
    }

    void LoadSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);

        // Apply loaded settings
        sfxSource.volume = sfxVolume;
        musicSource.volume = musicVolume;
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }



    public void PlayRandomSFX(AudioClip[] audioClips)
    {
        if (audioClips != null && audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Length);
            PlaySFX(audioClips[randomIndex]);
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayRandomMusic(AudioClip[] audioClips)
    {
        if (audioClips != null && audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Length);
            PlayMusic(audioClips[randomIndex]);
        }
    }
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null)
        {
            Debug.LogError("AudioManager: musicSource is null!");
            return;
        }

        if (clip == null)
        {
            Debug.LogError("AudioManager: music clip is null!");
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();

        Debug.Log($"🎵 Playing music: {clip.name}");
    }

}
