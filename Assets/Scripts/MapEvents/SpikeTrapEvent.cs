using UnityEngine;

public class SpikeTrapEvent : MonoBehaviour, ISteppable
{
    [SerializeField] int damage = 5;

    public void OnStep()
    {
        Debug.Log($"Spike trap triggered! Takes {damage} damage!");
        // damage logic here
        // play animation, sound effects etc. for spike trap triggering here
    }
}
