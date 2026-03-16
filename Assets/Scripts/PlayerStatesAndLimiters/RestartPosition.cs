using UnityEngine;

public class RestartPosition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var root = other.transform.root;
        if (!root.CompareTag("Player")) return;

        var death = root.GetComponent<PlayerDeath>();
        if (death != null)
        {
            death.Die();
        }
    }
}
