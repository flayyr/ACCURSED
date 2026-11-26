using UnityEngine;
using FMODUnity;
using FMOD;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            //Debug.LogError("more than one audio amanger in scene");
            UnityEngine.Debug.LogError("more than one audio manager in scene");
        }
        instance = this;
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventinstance = RuntimeManager.CreateInstance(eventReference);
        return eventinstance;
    }

}
