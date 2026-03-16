using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockLift : MonoBehaviour
{
    [Header("Co se má hýbat")]
    public Transform platform;

    [Header("Pohyb")]
    public float moveDistance = 1f;
    public float moveSpeed = 2f;
    public string playerTag = "Player";

    [Header("Hráč na plošině")]
    public float liftOffsetY = 0.15f;

    private Vector3 startPos;
    private bool isDown = false;
    private bool isMoving = false;

    private readonly List<CharacterController> riders = new List<CharacterController>();

    private void Start()
    {
        if (platform == null)
            platform = transform;

        startPos = platform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        var cc = other.GetComponent<CharacterController>();
        if (cc != null && !riders.Contains(cc))
            riders.Add(cc);

        if (!isMoving)
        {
            StartCoroutine(MoveRoutine(moveDown: !isDown));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        var cc = other.GetComponent<CharacterController>();
        if (cc != null)
            riders.Remove(cc);

    }

    private IEnumerator MoveRoutine(bool moveDown)
    {
        isMoving = true;

        Vector3 target = moveDown
            ? startPos + Vector3.down * moveDistance
            : startPos;

        foreach (var cc in riders)
        {
            if (cc != null)
                cc.Move(Vector3.up * liftOffsetY);
        }

        while (Vector3.SqrMagnitude(platform.position - target) > 0.000001f)
        {
            Vector3 newPos = Vector3.MoveTowards(platform.position, target, moveSpeed * Time.deltaTime);
            Vector3 delta = newPos - platform.position;

            platform.position = newPos;

            foreach (var cc in riders)
            {
                if (cc != null)
                    cc.Move(delta);
            }

            yield return null;
        }

        platform.position = target;
        isDown = moveDown;
        isMoving = false;
    }
}
