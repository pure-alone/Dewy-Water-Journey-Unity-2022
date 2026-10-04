using UnityEngine;

public sealed class DewyBootstrap : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectOfType<DewyApp>() == null)
            gameObject.AddComponent<DewyApp>();
    }
}
