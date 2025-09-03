using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    private const string UpClip = "Up";
    private const string DownClip = "Down";
    public float timer;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    //TODO: add sound and what not
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Bullet")) return;
        if (other.rigidbody.linearVelocity.magnitude < 5f) return; //Destroy the bullet?
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        _animator.Play(DownClip, 0, 0f);
        yield return new WaitForSeconds(timer);
        _animator.Play(UpClip, 0, 0f);
    }
}