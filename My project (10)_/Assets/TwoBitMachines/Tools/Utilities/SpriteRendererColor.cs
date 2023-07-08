using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines
{
        public class SpriteRendererColor : MonoBehaviour
        {
                [SerializeField] public SpriteRenderer rendererRef;
                [SerializeField] public Color color;

                public void ChangeColor ( )
                {
                        if (rendererRef != null)
                        {
                                rendererRef.color = color;
                        }
                }
        }
}