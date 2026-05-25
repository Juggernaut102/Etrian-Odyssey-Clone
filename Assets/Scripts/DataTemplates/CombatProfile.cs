using UnityEngine;

[CreateAssetMenu(fileName = "CombatProfile", menuName = "Scriptable Objects/CombatProfile")]
public abstract class CombatProfile : ScriptableObject
{
    [Header("Base Identity")]
    [SerializeField] protected string entityName;

    [Header("Shared Combat Stats")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int attackPower;
    [SerializeField] protected int speed;   // This can be used to determine turn order in battle

    // Public properties to access the private fields
    public string EntityName => entityName;
    public int MaxHealth => maxHealth;
    public int AttackPower => attackPower;
    public int Speed => speed;
}
