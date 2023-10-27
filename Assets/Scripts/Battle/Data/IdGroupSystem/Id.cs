using UnityEngine;

public class Id : ScriptableObject
{
    public static implicit operator string(Id id) => id.name;
}