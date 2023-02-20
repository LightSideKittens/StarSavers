using System;
using UnityEngine;

[Serializable]
public abstract class BaseWallet
{
    [SerializeField] protected int value;
    public abstract void Earn();
    public abstract bool TrySpend();
}