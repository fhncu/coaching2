#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
using UnityEngine;

namespace TwoBitMachines.Safire2DCamera
{

        [System.Serializable]
        public class WorldClamp
        {
                [SerializeField] public bool enable;
                [SerializeField] public SimpleBounds bounds = new SimpleBounds ( );
                [System.NonSerialized] private Camera camera;

                public void Initialize (Camera camera)
                {
                        bounds.Initialize ( );
                        this.camera = camera;
                }

                public void Clamp (Vector3 previousCamera)
                {
                        if (!enable) return;

                        Vector3 camCenter = camera.transform.position;
                        Vector2 velocity = camCenter - previousCamera;
                        float camSizeX = camera.Width ( );
                        float camSizeY = camera.Height ( );
                        float buffer = 2.5f;
                        float rate = 0.98f;

                        if (velocity.x < 0 && (camCenter.x - camSizeX) < bounds.left + buffer)
                                camCenter.x = Compute.Lerp (camCenter.x - velocity.x, bounds.left + camSizeX, rate);
                        else if (velocity.x > 0 && (camCenter.x + camSizeX) > bounds.right - buffer)
                                camCenter.x = Compute.Lerp (camCenter.x - velocity.x, bounds.right - camSizeX, rate);

                        if ((camCenter.x - camSizeX) < bounds.left) camCenter.x = bounds.left + camSizeX;
                        if ((camCenter.x + camSizeX) > bounds.right) camCenter.x = bounds.right - camSizeX;
                        if ((camCenter.y - camSizeY) < bounds.bottom) camCenter.y = bounds.bottom + camSizeY;
                        if ((camCenter.y + camSizeY) > bounds.top) camCenter.y = bounds.top - camSizeY;
                        camera.transform.position = camCenter;
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] public bool edit;
                [SerializeField, HideInInspector] public bool close;
                [SerializeField, HideInInspector] public bool foldOut;
                [SerializeField, HideInInspector] public bool view = true;
                public static void CustomInspector (SerializedProperty parent, Color barColor, Color labelColor)
                {
                        if (!parent.Bool ("edit")) return;

                        if (Follow.Open (parent, "World Clamp", barColor, labelColor, false, true, true))
                        {
                                GUI.enabled = parent.Bool ("enable");
                                SerializedProperty element = parent.Get ("bounds");
                                FoldOut.Box (2, Tint.Delete, 0);
                                {
                                        element.Field ("Position", "position");
                                        element.Field ("Size", "size");
                                }
                                Layout.VerticalSpacing (5);
                                GUI.enabled = true;
                        }
                }
                public static void DrawTrigger (Safire2DCamera main)
                {
                        if (!main.worldClamp.view || !main.worldClamp.enable) return;
                        SimpleBounds bounds = main.worldClamp.bounds;
                        SceneTools.DrawAndModifyBounds (ref bounds.position, ref bounds.size, Tint.Delete, 1f);
                }
                #pragma warning restore 0414
                #endif
                #endregion

        }
}