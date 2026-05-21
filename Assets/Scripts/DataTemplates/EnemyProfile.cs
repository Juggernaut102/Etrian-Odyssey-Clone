using UnityEngine;

[CreateAssetMenu(fileName = "EnemyProfile", menuName = "Scriptable Objects/EnemyProfile")]
public class EnemyProfile : ScriptableObject
{
    [Header("Base Identity")]
    [SerializeField] private string enemyName;
    [SerializeField] private Sprite battleSprite;

    [Header("Combat stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int attackPower;
    [SerializeField] private int xpReward;

    // Public properties to access the private fields
    public string EnemyName => enemyName;
    public Sprite BattleSprite => battleSprite;
    public int MaxHealth => maxHealth;
    public int AttackPower => attackPower;
    public int XpReward => xpReward;

}
