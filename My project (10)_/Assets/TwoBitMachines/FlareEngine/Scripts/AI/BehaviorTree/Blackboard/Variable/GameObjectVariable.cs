using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [AddComponentMenu ("")]
        public class GameObjectVariable : Blackboard
        {
                public GameObject value;

                public override Vector2 GetTarget ( )
                {
                        return value != null ? value.transform.position : this.transform.position;
                }

                public override void Set (GameObject gameObject)
                {
                        value = gameObject;
                }

                public override void Set (Vector3 vector3)
                {
                        if (value != null) value.transform.position = vector3;
                }

                public override void Set (Vector2 vector2)
                {
                        if (value != null) value.transform.position = vector2;
                }
        }
}