using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.TwoBitSprite
{
        [System.Serializable]
        public class SpritePacket
        {
                [SerializeReference] public List<ExtraProperty> property = new List<ExtraProperty> ( );
                [SerializeField] public List<Frame> frame = new List<Frame> ( );
                [SerializeField] public UnityEvent onLoopOnce;
                [SerializeField] public int loopStartIndex = 0;
                [SerializeField] public bool loopOnce;
                [SerializeField] public string name;

                [SerializeField] public bool canSync;
                [SerializeField] public int syncID;

                [SerializeField] public List<AnimationTransition> transition = new List<AnimationTransition> ( );
                [SerializeField] public bool hasTransition = false;
                [SerializeField] public bool hasChangedDirection = false; // set by editor if changed direction signal is being checked

                public bool useTransition => hasTransition && transition.Count > 0;
                public bool changedDirection => hasTransition && hasChangedDirection;

                public bool Transition (SpriteEngine spriteEngine, string currentAnimation, out SpritePacket transitionAnimation)
                {
                        for (int i = 0; i < transition.Count; i++)
                        {
                                if (transition[i].from != currentAnimation)
                                {
                                        continue;
                                }
                                if (spriteEngine.tree.SignalTrue (transition[i].condition))
                                {
                                        transitionAnimation = spriteEngine.GetSprite (transition[i].to);
                                        return transitionAnimation != null;
                                }
                        }
                        transitionAnimation = null;
                        return false;
                }

                public void SetProperties (int frameIndex, bool firstFrame = false)
                {
                        for (int i = 0; i < property.Count; i++)
                        {
                                property[i].SetState (frameIndex, firstFrame);
                        }
                }

                public void ResetProperties ( )
                {
                        for (int i = 0; i < property.Count; i++)
                        {
                                property[i].ResetToOriginalState ( );
                        }
                }

                public void InterpolateProperties (int frameIndex, float counter)
                {
                        for (int i = 0; i < property.Count; i++)
                        {
                                if (property[i].interpolate)
                                {
                                        property[i].Interpolate (frameIndex, frame[frameIndex].rate, counter);
                                }
                        }
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] private bool active;
                [SerializeField, HideInInspector] public int frameIndex = 0;
                [SerializeField, HideInInspector] private int signalIndex = 0;
                [SerializeField, HideInInspector] private float globalRate = 10;
                [SerializeField, HideInInspector] public bool transitionFoldOut;
                [SerializeField, HideInInspector] public bool addTransition;
                [SerializeField, HideInInspector] public bool deleteTransition;
                #pragma warning restore 0414
                #endif
                #endregion
        }

        [System.Serializable]
        public class Frame
        {
                public UnityEvent onEnterFrame;
                public Sprite sprite;
                public float rate;

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] private bool add;
                [SerializeField, HideInInspector] private bool delete;
                [SerializeField, HideInInspector] private bool eventFoldOut;
                #pragma warning restore 0414
                #endif
                #endregion
        }

        [System.Serializable]
        public class AnimationTransition
        {
                [SerializeField] public string from;
                [SerializeField] public string to;
                [SerializeField] public string condition;
        }

}