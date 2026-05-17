using UnityEngine;

// Singleton class used to manage game audio, such as background music and sound effects. This class can be expanded to include methods for playing specific sounds, adjusting volume, and handling audio transitions.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source Speaker")]
    [SerializeField] private AudioSource bgmSpeaker;
    [SerializeField] private AudioSource sfxSpeaker;
    [SerializeField] private AudioSource uiSpeaker;


    [Header("Audio Clips")]
    [SerializeField] private AudioClip wallThudSound;

    private float thudCooldownTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Persist across scenes
        }
        else
        {
            Destroy(this.gameObject);
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
}
