using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HeroLeft.Sounds
{
    public class SoundTrakStart : MonoBehaviour
    {
        public AudioClip audioClip;

        void Start()
        {
            if(SoundManager.soundManager != null)
            SoundManager.soundManager.PlayTheme(audioClip);
        }
    }
}