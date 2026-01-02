using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        GameObject go = new GameObject("OneShotAudio");
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 1f;
        source.Play();

        Destroy(go, clip.length);
    }
}