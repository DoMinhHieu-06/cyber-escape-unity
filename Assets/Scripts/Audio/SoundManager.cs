using UnityEngine;

namespace CyberEscape.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Audio Clips")]
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip slideClip;
        [SerializeField] private AudioClip laneChangeClip;
        [SerializeField] private AudioClip coinClip;
        [SerializeField] private AudioClip powerUpClip;
        [SerializeField] private AudioClip shieldBreakClip;
        [SerializeField] private AudioClip gameOverClip;

        private AudioSource sfxSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudio();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudio()
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;

            // Generate procedural audio clips if not assigned in inspector
            if (jumpClip == null) jumpClip = GenerateToneClip("Jump", 440f, 660f, 0.15f);
            if (slideClip == null) slideClip = GenerateToneClip("Slide", 500f, 250f, 0.2f);
            if (laneChangeClip == null) laneChangeClip = GenerateToneClip("LaneChange", 600f, 600f, 0.08f);
            if (coinClip == null) coinClip = GenerateArpeggioClip("Coin", new float[] { 987f, 1318f }, 0.12f);
            if (powerUpClip == null) powerUpClip = GenerateArpeggioClip("PowerUp", new float[] { 523f, 659f, 784f, 1046f }, 0.25f);
            if (shieldBreakClip == null) shieldBreakClip = GenerateToneClip("ShieldBreak", 300f, 120f, 0.3f);
            if (gameOverClip == null) gameOverClip = GenerateToneClip("GameOver", 220f, 80f, 0.6f);
        }

        public void PlayJumpSound() => PlayClip(jumpClip);
        public void PlaySlideSound() => PlayClip(slideClip);
        public void PlayLaneChangeSound() => PlayClip(laneChangeClip);
        public void PlayCoinSound() => PlayClip(coinClip);
        public void PlayPowerUpSound() => PlayClip(powerUpClip);
        public void PlayShieldBreakSound() => PlayClip(shieldBreakClip);
        public void PlayGameOverSound() => PlayClip(gameOverClip);

        private void PlayClip(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, 0.7f);
            }
        }

        // Procedural Audio Synthesizer
        private AudioClip GenerateToneClip(string clipName, float startFreq, float endFreq, float duration)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float progress = (float)i / samples;
                float freq = Mathf.Lerp(startFreq, endFreq, progress);
                float envelope = 1f - progress; // Linear decay
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * envelope * 0.4f;
            }

            AudioClip clip = AudioClip.Create(clipName, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip GenerateArpeggioClip(string clipName, float[] freqs, float duration)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            float[] data = new float[samples];
            int noteSamples = samples / freqs.Length;

            for (int i = 0; i < samples; i++)
            {
                int noteIndex = Mathf.Min(i / noteSamples, freqs.Length - 1);
                float t = (float)i / sampleRate;
                float progress = (float)i / samples;
                float envelope = 1f - (progress * 0.7f);
                data[i] = Mathf.Sin(2f * Mathf.PI * freqs[noteIndex] * t) * envelope * 0.4f;
            }

            AudioClip clip = AudioClip.Create(clipName, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
