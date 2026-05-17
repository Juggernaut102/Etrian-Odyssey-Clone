using UnityEngine;

// Singleton class used to manage game audio, such as background music and sound effects. This class can be expanded to include methods for playing specific sounds, adjusting volume, and handling audio transitions.
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    // Property to access the singleton instance of UiManager. If it doesn't exist, it will attempt to find one in the scene or create a new one from a prefab.

    public static AudioManager Instance 
    { 
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AudioManager>();
                if (instance == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("GameRoot");
                    if (prefab != null)
                    {
                        GameObject obj = Instantiate(prefab);
                        instance = obj.GetComponent<AudioManager>();
                    }
                    else
                    {
                        Debug.Log("Can't find GameRoot prefab in Resources folder! Please create one and add the AudioManager component to it.");
                    }
                }
            }
            return instance;
        }
    }

    [Header("Audio Source Speaker")]
    [SerializeField] private AudioSource bgmSpeaker;
    [SerializeField] private AudioSource sfxSpeaker;
    [SerializeField] private AudioSource uiSpeaker;


    [Header("Audio Clips")]
    [SerializeField] private AudioClip wallThudSound;
    [SerializeField] private AudioClip[] footstepSounds;

    private float thudCooldownTimer = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (thudCooldownTimer > 0f)
        {
            thudCooldownTimer -= Time.deltaTime; // Decrease cooldown timer over time
        }
    }

    /// <summary>
    /// Attempts to play the wall thud sound, respecting a cooldown duration.
    /// </summary>
    /// <param name="cooldownDuration">How long to wait before allowing this sound again.</param>
    public void PlayWallThud(float cooldownDuration)
    {
        if (sfxSpeaker != null && wallThudSound != null && thudCooldownTimer <= 0f)
        {
            sfxSpeaker.PlayOneShot(wallThudSound);
            thudCooldownTimer = cooldownDuration; // Reset cooldown timer
        }
    }

    public void PlayFootsteps()
    {
        if (sfxSpeaker != null && footstepSounds != null)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            sfxSpeaker.PlayOneShot(footstepSounds[randomIndex]);
        }
    }
}
