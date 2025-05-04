using UnityEngine;

public class SfxManager : Singleton<SfxManager>
{
    [SerializeField] private AudioClip _beepSFX;
    [SerializeField] private AudioClip _continousBeepSFX;
    [SerializeField] private AudioClip _engageEnemySFX;

    private AudioSource _source;
    private bool _isSfxOff = false;

    public bool IsSfxOff => _isSfxOff;

    private void Awake()
    {
        base.Awake();
        _source = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("SFX"))
        {
            if (PlayerPrefs.GetInt("SFX") == 0) _isSfxOff = false;
            else _isSfxOff = true;
        }
    }

    public void PlayBeepSFX()
    {
        if (_isSfxOff) return;
        _source.PlayOneShot(_beepSFX);
    }

    public void PlayContinousBeepSFX()
    {
        if (_isSfxOff) return;
        _source.PlayOneShot(_continousBeepSFX);
    }

    public void PlayEngageEnemySFX()
    {
        if (_isSfxOff) return;
        _source.PlayOneShot(_engageEnemySFX);
    }
}
