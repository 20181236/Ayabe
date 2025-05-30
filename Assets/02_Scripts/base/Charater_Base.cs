using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    public abstract ObjectType ObjectType { get; }
    public abstract void ApplyDamage(float damage, bool isExplosion, Vector3? explosionPos = null);
}
