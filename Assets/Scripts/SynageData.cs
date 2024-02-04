
using UnityEngine;
[CreateAssetMenu(fileName = "SynageData", menuName = "ScriptableObject/Data/Synage")]
public class SynageData : ScriptableObject
{
    public float[] upAttackDamage;
    public float[] upReflection;
    public float[] upAttackSpeed;
    public float[] upAttackRange;
    public float[] downResistance;

    [TextArea]
    public string synageInfo;

}
