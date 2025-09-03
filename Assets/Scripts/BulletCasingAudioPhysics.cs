using UnityEngine;
using Random = UnityEngine.Random;

public class BulletCasingAudioPhysics : MonoBehaviour
{
    public float disableTimer = 5f;
    public AudioClip[] audioClips;
    private AudioSource _audioSource;
    private Rigidbody _rigidbody;
    private float _timer;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = audioClips[0];
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > disableTimer) _rigidbody.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground")) return;
        _audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        _audioSource.volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 20 * 4f);
        _audioSource.Play();
    }
}