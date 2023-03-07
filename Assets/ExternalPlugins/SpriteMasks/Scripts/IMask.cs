using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FastSpriteMask
{
    public interface IMask
    {
        Transform Transform { get; }
        Sprite Sprite { get; }

        MaskType Type { get; }
        float AlphaCutoff { get; }
    }
}
