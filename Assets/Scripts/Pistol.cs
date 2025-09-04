using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Pistol : MonoBehaviour
{
    private const string SlideBackClip = "SlideBack";
    private const string SlideForwardClip = "SlideForwards";
    private const float MaxTriggerAngle = 50;
    public Animator slideAnimator;
    public Transform trigger;
    public ParticleSystem muzzleParticle;
    public AudioClip gunShot;
    public AudioClip slideRack;
    public Transform ejectPort;
    public GameObject casingPrefab;
    public Transform muzzlePos;
    public GameObject bulletPrefab;
    public int bulletCount;
    public AmmoCounter ammoCounter;
    public AudioClip reloadClip;
    public HapticImpulsePlayer leftHapticImpulse;
    public HapticImpulsePlayer rightHapticImpulse;
    private float _activateAxis;
    private AudioSource _audioSource;
    private bool _canShoot = true;
    private InteractorHandedness _handedness;
    private XRIDefaultInputActions _inputActions;
    private bool _isMagEmpty;
    private int _remainingBullets;

    private void Awake()
    {
        _inputActions = new XRIDefaultInputActions();
        _inputActions.Enable();
        _inputActions.XRILeftInteraction.Reload.performed += ReloadLeft;
        _inputActions.XRIRightInteraction.Reload.performed += ReloadRight;
        _audioSource = GetComponent<AudioSource>();
        _remainingBullets = bulletCount;
        ammoCounter.UpdateAmmoCounter(_remainingBullets);
    }

    public void Update()
    {
        if (_handedness != InteractorHandedness.None)
            _activateAxis = _handedness switch
            {
                InteractorHandedness.Left => _inputActions.XRILeftInteraction.ActivateValue.ReadValue<float>(),
                InteractorHandedness.Right => _inputActions.XRIRightInteraction.ActivateValue.ReadValue<float>(),
                _ => _activateAxis
            };
        var triggerRotation = Mathf.Lerp(0, MaxTriggerAngle, _activateAxis);
        trigger.localRotation = Quaternion.Euler(triggerRotation, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Controller")) return;
        var parent = other.transform.parent;
        if (parent == null) return;
        var renderers = parent.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in renderers) meshRenderer.enabled = false;
    }


    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Controller")) return;
        var parent = other.transform.parent;
        if (parent == null) return;
        var renderers = parent.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in renderers) meshRenderer.enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Controller")) return;
        var parent = other.transform.parent;
        if (parent == null) return;
        var renderers = parent.GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in renderers) meshRenderer.enabled = false;
    }

    private void ReloadLeft(InputAction.CallbackContext context)
    {
        if (_handedness != InteractorHandedness.Left || _remainingBullets == bulletCount) return;

        StartCoroutine(Reload());
    }

    private void ReloadRight(InputAction.CallbackContext context)
    {
        if (_handedness != InteractorHandedness.Right || _remainingBullets == bulletCount) return;

        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        if (_isMagEmpty)
        {
            _audioSource.PlayOneShot(slideRack);
            slideAnimator.Play(SlideForwardClip, 0, 0f);

            yield return new WaitUntil(() =>
            {
                var animInfo = slideAnimator.GetCurrentAnimatorStateInfo(0);
                return animInfo.IsName(SlideForwardClip) && animInfo.normalizedTime >= 1f;
            });
        }
        else
        {
            _audioSource.PlayOneShot(reloadClip);
        }

        _remainingBullets = bulletCount;
        ammoCounter.UpdateAmmoCounter(_remainingBullets);
        _isMagEmpty = false;
        _canShoot = true;
    }

    public void Selected(SelectEnterEventArgs selectEnterEventArgs)
    {
        _handedness = selectEnterEventArgs.interactorObject.handedness;
        ResetRotation();
    }

    public void SelectedExit()
    {
        _handedness = InteractorHandedness.None;
    }

    private void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    //TODO: replace it with custom trigger activation
    public void Activate()
    {
        if (_canShoot) StartCoroutine(PlaySlideAnimation());
    }


    private IEnumerator PlaySlideAnimation()
    {
        _canShoot = false;
        yield return new WaitUntil(() =>
        {
            var animInfo = slideAnimator.GetCurrentAnimatorStateInfo(0);
            return animInfo.normalizedTime >= 1f;
        });

        slideAnimator.Play(SlideBackClip, 0, 0f);
        _audioSource.PlayOneShot(gunShot);
        muzzleParticle.Play();
        var casing = Instantiate(casingPrefab, ejectPort.position, Quaternion.identity);
        casing.GetComponent<Rigidbody>().AddForce(new Vector3(0.05f, 0.05f, 0), ForceMode.Impulse);
        var bullet = Instantiate(bulletPrefab, muzzlePos.position, muzzlePos.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(muzzlePos.forward * 10f, ForceMode.Impulse);
        _remainingBullets--;
        ammoCounter.UpdateAmmoCounter(_remainingBullets);
        switch (_handedness)
        {
            case InteractorHandedness.Left:
                leftHapticImpulse.SendHapticImpulse(0.8f, 0.2f);
                break;
            case InteractorHandedness.Right:
                rightHapticImpulse.SendHapticImpulse(0.8f, 0.2f);
                break;
            case InteractorHandedness.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        yield return new WaitUntil(() =>
        {
            var animInfo = slideAnimator.GetCurrentAnimatorStateInfo(0);
            return animInfo.IsName(SlideBackClip) && animInfo.normalizedTime >= 1f;
        });


        if (_remainingBullets <= 0)
        {
            _canShoot = false;
            _isMagEmpty = true;
        }
        else
        {
            slideAnimator.Play(SlideForwardClip, 0, 0f);

            yield return new WaitUntil(() =>
            {
                var animInfo = slideAnimator.GetCurrentAnimatorStateInfo(0);
                return animInfo.IsName(SlideForwardClip) && animInfo.normalizedTime >= 1f;
            });
            _canShoot = true;
        }
    }
}