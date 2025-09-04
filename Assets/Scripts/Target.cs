using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    private const string UpClip = "Up";
    private const string DownClip = "Down";
    public float timer;
    private Animator _animator;
    private bool _isUp;

    private void Awake()
    {
        _isUp = true;
        _animator = GetComponent<Animator>();
        Test();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Bullet")) return;
        if (other.rigidbody.linearVelocity.magnitude < 5f) return;
        if (!_isUp) return;
        _isUp = false;
        StartCoroutine(PlayAnimation());
    }

    private void Test()
    {
        _isUp = false;
        StartCoroutine(PlayAnimation());
    }
    
    
    private IEnumerator PlayAnimation()
    {
        _animator.Play(DownClip, 0, 0f);
        yield return new WaitUntil(() =>
        {
            var animInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return animInfo.normalizedTime >= 1f;
        });
        yield return new WaitForSeconds(timer);
        _animator.Play(UpClip, 0, 0f);
        yield return new WaitUntil(() =>
        {
            var animInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return animInfo.normalizedTime >= 1f;
        });
        _isUp = true;
    }
}