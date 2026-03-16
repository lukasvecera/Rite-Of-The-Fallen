using UnityEngine;

public class HideHeadInFirstPerson : MonoBehaviour
{
    public Transform headBone;

    void Start()
    {
        if (headBone != null)
        {
            headBone.localScale = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("HideHeadInFirstPerson: headBone is not set.");
        }
    }
}
