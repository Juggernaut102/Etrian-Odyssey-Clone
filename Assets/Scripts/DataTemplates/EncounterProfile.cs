using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterProfile", menuName = "Scriptable Objects/EncounterProfile")]
public class EncounterProfile : ScriptableObject
{
    [SerializeField] private List<EnemyProfile> enemiesInTroop = new List<EnemyProfile>();

    public List<EnemyProfile> EnemiesInTroop => enemiesInTroop;
}
