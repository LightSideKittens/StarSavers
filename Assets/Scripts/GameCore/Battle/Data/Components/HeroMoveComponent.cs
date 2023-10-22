using System;
using Battle.Windows;
using UnityEngine;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class HeroMoveComponent : MoveComponent
    {
        public override void Move()
        {
            var joystick = BattleWindow.Joystick;
            if (!joystick.IsUsing) return;

            var direction = joystick.Direction;
            rigidbody.position += direction * (speed * Time.fixedDeltaTime);
            var zAngle = Vector2.SignedAngle(Vector2.up, direction);
            rigidbody.rotation = zAngle;
            
        }
    }
}