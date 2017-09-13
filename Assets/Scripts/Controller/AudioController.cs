using UnityEngine;
using IMAV.Model;

namespace IMAV.Controller
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioController : MonoBehaviour
    {
        public AudioClip screenshot;
        public AudioClip save;
        public AudioClip startRecord;

        AudioSource audioPlayer;

        void Awake()
        {
            audioPlayer = GetComponent<AudioSource>();
        }

        public void PlayOneshot(AudioEnum au)
        {
            switch(au)
            {
                case AudioEnum.Screenshot: audioPlayer.PlayOneShot(screenshot);break;
                case AudioEnum.StartRecord: audioPlayer.PlayOneShot(startRecord); break;
                case AudioEnum.Save: audioPlayer.PlayOneShot(save); break;
            }
        }
    }
}