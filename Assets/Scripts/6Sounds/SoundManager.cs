using UnityEngine;

namespace HeroLeft.Sounds
{
    public class SoundManager : MonoBehaviour
    {
        public AudioSource themeSource;
        public static SoundManager soundManager;

        void Start()
        {
            if (soundManager == null)
            {
                soundManager = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlayTheme(AudioClip clip)
        {
            themeSource.clip = clip;
            themeSource.Play();
        }
    }
}