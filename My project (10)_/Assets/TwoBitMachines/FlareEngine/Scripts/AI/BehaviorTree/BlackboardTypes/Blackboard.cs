using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
       [AddComponentMenu ("")]
       public class Blackboard : MonoBehaviour
       {
              [SerializeField] public string dataName;
              [SerializeField, HideInInspector] public int index = 0;
              [SerializeField, HideInInspector] public BlackboardType blackboardType;

              public virtual GameObject GetNearestGameObjectTarget (Vector2 position) { return null; }
              public virtual GameObject GetRandomGameObjectTarget ( ) { return null; }
              public virtual GameObject GetGameObject ( ) { return null; }

              public virtual Transform GetNearestTransformTarget (Vector2 position) { return null; }
              public virtual Transform GetRandomTransformTarget ( ) { return null; }
              public virtual Transform GetTransform ( ) { return null; }

              public virtual Vector2 GetNearestTarget (Vector2 position) { return Vector2.zero; }
              public virtual Vector2 GetRandomTarget ( ) { return Vector2.zero; }
              public virtual Vector2 GetTarget ( ) { return Vector2.zero; }
              public virtual Vector3 GetVector ( ) { return Vector3.zero; }

              public virtual Object GetObject ( ) { return null; }
              public virtual void SetObject (Object newValue) { }

              public virtual Quaternion GetQuaternion ( ) { return Quaternion.identity; }
              public virtual void SetQuaternion (Quaternion newValue) { }

              public virtual Color GetColor ( ) { return Color.magenta; }
              public virtual void SetColor (Color newValue) { }

              public virtual int ListCount ( ) { return 0; }
              public virtual bool Contains (Vector2 position) { return false; }
              public virtual bool AddToList (GameObject gameObject) { return false; }
              public virtual bool AddToList (Transform transform) { return false; }
              public virtual bool AddToList (Vector3 vector3) { return false; }
              public virtual bool RemoveFromList (GameObject gameObject) { return false; }
              public virtual bool RemoveFromList (Transform transform) { return false; }
              public virtual bool RemoveFromList (Vector3 vector) { return false; }

              public virtual void RunPathFollower (ref Vector2 velocity) { }
              public virtual void CalculatePath (Blackboard target) { }
              public virtual bool TargetPlaneChanged (Vector2 position) { return false; }
              public virtual bool PathSafeToChange ( ) { return true; }
              public virtual bool OnASurface ( ) { return false; }
              public virtual bool AtFinalTarget ( ) { return false; }
              public virtual NodeState NextTarget (ref Vector3 target) { return NodeState.Success; }
              public virtual bool CanExit ( ) { return false; }
              public virtual void IgnoreBlock (bool value) { }
              public virtual void ResetState (WorldCollision world, Gravity gravity, AnimationSignals signals, Vector2 target) { }

              public virtual AITree GetTree ( ) { return null; }
              public virtual float GetValue ( ) { return 0; }
              public virtual float CellSize ( ) { return 1f; }
              public virtual void ResetIndex ( ) { }

              public virtual void Set (Transform transform) { }
              public virtual void Set (Collider2D collider2D) { }
              public virtual void Set (GameObject gameObject) { }
              public virtual void Set (Vector2 vector2) { }
              public virtual void Set (Vector3 vector3) { }
              public virtual void Set (float value) { }
              public virtual void Set (bool value) { }

              #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀ 
              #if UNITY_EDITOR
              #pragma warning disable 0414
              [SerializeField, HideInInspector] public bool foldOut;
              [SerializeField, HideInInspector] public bool delete;
              [SerializeField, HideInInspector] public bool deleteAsk;
              [SerializeField, HideInInspector] public List<string> refName = new List<string> ( );
              public virtual void OnSceneGUI (UnityEditor.Editor editor) { }
              public virtual void DrawWhenNotSelected ( ) { }
              private void OnDrawGizmos ( )
              {
                     this.hideFlags = HideFlags.HideInInspector;
              }
              #pragma warning restore 0414
              #endif
              #endregion
       }

       public enum BlackboardType
       {
              Target,
              Territory,
              Variable
       }

}