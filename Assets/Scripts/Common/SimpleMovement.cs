using UnityEngine;

namespace StarSavers
{
    [RequireComponent(typeof(WASDEvents))]
    public class SimpleMovement : MonoBehaviour
    {
        private struct DirectionData
        {
            private SimpleMovement movement;
            private Vector2 vector;
            private float curveTime;

            public void Init(SimpleMovement movement, Vector2 vector)
            {
                this.movement = movement;
                this.vector = vector;
            }

            public void Update()
            {
                curveTime += Time.deltaTime;
                
                if (curveTime > movement.animationLoopTime)
                {
                    curveTime = 0;
                }
                
                movement.transform.position += ((Vector3)vector - (Vector3)vector * movement.curve.Evaluate(curveTime)) * (movement.speed * Time.deltaTime);
            }
        }
        
        private DirectionData forward;
        private DirectionData forwardLeft;
        private DirectionData forwardRight;
        private DirectionData back;
        private DirectionData backLeft;
        private DirectionData backRight;
        private DirectionData left;
        private DirectionData right;
        
        [SerializeField] private AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        [SerializeField] private float speed = 1;
        [SerializeField] private float animationLoopTime = 1;
        
        private Vector2 forwardVector = new Vector2(0, 1);
        private Vector2 forwardLeftVector = new Vector2(-1, 1);
        private Vector2 forwardRightVector = new Vector2(1, 1);
        
        private Vector2 backVector = new Vector2(0, -1);
        private Vector2 backLeftVector = new Vector2(-1, -1);
        private Vector2 backRightVector = new Vector2(1, -1);
        
        private Vector2 leftVector = new Vector2(-1, 0);
        private Vector2 rightVector = new Vector2(1, 0);

        private void Awake()
        {
            forward.Init(this, forwardVector);
            forwardLeft.Init(this, forwardLeftVector);
            forwardRight.Init(this, forwardRightVector);
            
            back.Init(this, backVector);
            backLeft.Init(this, backLeftVector);
            backRight.Init(this, backRightVector);
            
            left.Init(this, leftVector);
            right.Init(this, rightVector);
            
            WASDEvents.Forward += forward.Update;
            WASDEvents.ForwardLeft += forwardLeft.Update;
            WASDEvents.ForwardRight += forwardRight.Update;
            
            WASDEvents.Back += back.Update;
            WASDEvents.BackLeft += backLeft.Update;
            WASDEvents.BackRight += backRight.Update;
            
            WASDEvents.Left += left.Update;
            WASDEvents.Right += right.Update;
        }
    }
}