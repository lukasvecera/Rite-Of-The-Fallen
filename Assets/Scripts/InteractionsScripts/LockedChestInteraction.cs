using UnityEngine;
using System.Collections;

public class LockedChestInteraction : MonoBehaviour, IInteractible
{
    [Header("Target (stejně jako u ChestInteraction)")]
    [Tooltip("Transform víka/křídel. Když je prázdné, použije se tento GameObject.")]
    public Transform chestTarget;

    [Header("Animation")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Key / Access")]
    public ItemSO requiredKey;
    public string playerTag = "Player";
    public bool oneTime = true;

    bool isOpen = false;
    bool isUnlocked = false;

    Transform _t;
    Quaternion _closedRotation, _openRotation;
    Coroutine _current;

    void Start()
    {
        _t = chestTarget ? chestTarget : transform;
        _closedRotation = _t.rotation;
        _openRotation   = Quaternion.Euler(_t.eulerAngles + new Vector3(openAngle, 0, 0));
    }

    public void Interact()
    {
        if (isOpen && oneTime) return;

        var inv = FindFirstObjectByType<PlayerInventory>();
        if (!inv) return;

        var selected = inv.GetSelectedItem();
        if (!isUnlocked)
        {
            if (selected != requiredKey)
            {
                return;
            }

            bool consumed = inv.ConsumeSelectedIf(requiredKey);
            if (!consumed) return;
            isUnlocked = true;
        }

        if (_current != null) StopCoroutine(_current);
        _current = StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        var target = _openRotation;
        isOpen = true;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.openChestSound);
        while (Quaternion.Angle(_t.rotation, target) > 0.01f)
        {
            _t.rotation = Quaternion.Lerp(_t.rotation, target, Time.deltaTime * openSpeed);
            yield return null;
        }
        _t.rotation = target;

        if (oneTime)
        {
            var col = GetComponent<Collider>();
            if (col) col.enabled = false;
            enabled = false;
        }
    }
}
