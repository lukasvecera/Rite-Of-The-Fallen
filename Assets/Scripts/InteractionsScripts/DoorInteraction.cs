using UnityEngine;
using System.Collections;

public class DoorInteraction : MonoBehaviour, IInteractible
{
    [Header("Target")]
    [Tooltip("Transform křídla dveří, které se otáčí. Když je prázdné, použije se tento GameObject.")]
    public Transform doorTarget;

    [Header("Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Access control")]
    public bool requireProximity = false;
    public float proximityRadius = 2.0f;
    public string playerTag = "Player";

    [HideInInspector] public bool isOpen = false;
    [HideInInspector] public bool isPlayerClose = false;

    Transform _t;
    Quaternion _closedRotation, _openRotation;
    Coroutine _currentCoroutine;

    void Start()
    {
        _t = doorTarget ? doorTarget : transform;
        _closedRotation = _t.rotation;
        _openRotation   = Quaternion.Euler(_t.eulerAngles + new Vector3(0f, openAngle, 0f));
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(ToggleDoor());
    }

    bool CanInteract()
    {
        if (!requireProximity) return true;
        if (isPlayerClose) return true;

        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (!player) return true;
        return Vector3.Distance(player.transform.position, transform.position) <= proximityRadius;
    }

    IEnumerator ToggleDoor()
    {
        AudioManager.Instance.PlaySFX(isOpen ? AudioManager.Instance.doorCloseSound : AudioManager.Instance.doorOpenSound);
        var target = isOpen ? _closedRotation : _openRotation;
        isOpen = !isOpen;

        while (Quaternion.Angle(_t.rotation, target) > 0.01f)
        {
            _t.rotation = Quaternion.Lerp(_t.rotation, target, Time.deltaTime * openSpeed);
            yield return null;
        }
        _t.rotation = target;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
            isPlayerClose = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            isPlayerClose = false;
    }
}
