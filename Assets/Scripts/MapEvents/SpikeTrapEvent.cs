using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapEvent : MonoBehaviour, ISteppable
{
    [SerializeField] int damage = 5;

    public void OnStep()
    {
        List<CharacterRuntimeData> party = PartyManager.Instance.LivePartyData;
        foreach ( CharacterRuntimeData data in party )
        {
            data.TakeDamage(damage);
            Debug.Log($"Spike trap triggered! {data.EntityName} takes {damage} damage!");
        }

        // play animation, sound effects etc. for spike trap triggering here
    }
}
