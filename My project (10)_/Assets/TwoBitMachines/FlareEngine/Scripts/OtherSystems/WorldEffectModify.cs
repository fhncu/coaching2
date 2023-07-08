using TMPro;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [System.Serializable]
        public class WorldEffectModify
        {
                [SerializeField] public WorldEffectType type;
                [SerializeField] public EffectPosition position;
                [SerializeField] public float yOffset = 0f;
                [SerializeField] public bool useRandomX;
                [SerializeField] public bool useRandomY;
                [SerializeField] public bool useRandomRotation;
                [SerializeField] public float randomRotationMin = 0f;
                [SerializeField] public float randomRotationMax = 0f;
                [SerializeField] public float randomXOffsetMin = 0f;
                [SerializeField] public float randomXOffsetMax = 0f;
                [SerializeField] public float randomYOffsetMin = 0f;
                [SerializeField] public float randomYOffsetMax = 0f;
                [SerializeField] public bool checkForWalls = false;

                public void Activate (GameObject gameObject, ImpactPacket impact)
                {
                        Transform transform = gameObject.transform;
                        if (type == WorldEffectType.TextMeshPro)
                        {
                                TextMeshPro text = gameObject.GetComponent<TextMeshPro> ( );
                                if (text != null) text.SetText (impact.damageValue.ToString ( ));
                        }
                        else if (type == WorldEffectType.LetsWiggle)
                        {
                                LetsWiggle wiggle = gameObject.GetComponent<LetsWiggle> ( );
                                if (wiggle != null) wiggle.Activate (impact);
                        }

                        // position
                        if (position == EffectPosition.Bottom)
                        {
                                transform.position = impact.bottomPosition;
                        }
                        else if (position == EffectPosition.Center)
                        {
                                transform.position = impact.Center ( );
                        }
                        else
                        {
                                transform.position = impact.Top ( );
                        }
                        if (yOffset != 0)
                        {
                                transform.position += Vector3.up * yOffset;
                        }
                        if (useRandomX)
                        {
                                transform.position += Vector3.right * Random.Range (randomXOffsetMin, randomXOffsetMax);

                        }
                        if (useRandomY)
                        {
                                transform.position += Vector3.up * Random.Range (randomYOffsetMin, randomYOffsetMax);

                        }
                        if (useRandomRotation)
                        {
                                transform.rotation = Quaternion.Euler (0, 0, Random.Range (randomRotationMin, randomRotationMax));
                        }
                        if (checkForWalls)
                        {
                                if (Physics2D.OverlapPoint (transform.position, WorldManager.collisionMask))
                                {
                                        transform.position = impact.Center ( );
                                }
                        }
                }
        }

        public enum EffectPosition
        {
                Bottom,
                Center,
                Top
        }

        public enum WorldEffectType
        {
                Normal,
                TextMeshPro,
                LetsWiggle
        }
}