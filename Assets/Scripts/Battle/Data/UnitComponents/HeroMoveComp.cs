using System;
using Battle.Windows;
using LSCore.BattleModule;
using UnityEngine;
using UnityEngine.Scripting;

namespace Battle.Data.Components
{
    [Preserve, Serializable]
    internal class HeroMoveComp : MoveComp
    {
        protected override void Init()
        {
            base.Init();
            ShouldLookAtTarget = false;
            var joystick = BattleWindow.Joystick;
            joystick.IsUsing.Changed += SetIsRunning;
        }
        
        protected override void Move()
        {
            CameraMover.Move();
            var joystick = BattleWindow.Joystick;
            var direction = joystick.Direction;
            rigidbody.position += direction * (speed * Time.fixedDeltaTime);
            var zAngle = Vector2.SignedAngle(Vector2.up, direction);
            rigidbody.rotation = Mathf.LerpAngle(rigidbody.rotation, zAngle, Time.deltaTime * 10);
        }
    }
}