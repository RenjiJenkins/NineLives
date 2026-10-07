using UnityEngine;

public abstract class AbilityData : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
    public float cooldown = 1f;

    public abstract void Activate(GameObject user);
}