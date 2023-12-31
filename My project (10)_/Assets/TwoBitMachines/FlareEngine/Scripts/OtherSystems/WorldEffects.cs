﻿using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu ("Flare Engine/WorldEffects")]
        public class WorldEffects : MonoBehaviour
        {
                [SerializeField] public List<WorldEffectPool> effect = new List<WorldEffectPool> ( );
                [System.NonSerialized] private Dictionary<string, WorldEffectPool> particleList = new Dictionary<string, WorldEffectPool> ( );

                [System.NonSerialized] private GameObject container;
                [System.NonSerialized] public static List<WorldEffects> effects = new List<WorldEffects> ( );
                public static WorldEffects get;

                private void Awake ( )
                {
                        get = this;
                        container = new GameObject ("Container");
                        container.transform.parent = this.transform;
                        for (int i = 0; i < effect.Count; i++)
                        {
                                effect[i].Initialize (container.transform);
                                particleList.Add (effect[i].gameObject.name, effect[i]);
                        }
                }

                private void OnEnable ( )
                {
                        if (!effects.Contains (this)) effects.Add (this);
                }

                private void OnDisable ( )
                {
                        if (effects.Contains (this)) effects.Remove (this);
                        get = null;
                }

                public static void ResetEffects ( )
                {
                        for (int i = 0; i < effects.Count; i++)
                        {
                                if (effects[i] == null) continue;
                                for (int j = 0; j < effects[i].effect.Count; j++)
                                {
                                        if (effects[i].effect[j] == null) continue;
                                        effects[i].effect[j].ResetAll ( );
                                }
                        }
                }

                public void Activate (ImpactPacket impact)
                {
                        if (particleList.TryGetValue (impact.name, out WorldEffectPool effect))
                        {
                                effect.Activate (impact, impact.bottomPosition);
                        }
                }

                public void ActivateWithDirection (ImpactPacket impact)
                {
                        if (particleList.TryGetValue (impact.name, out WorldEffectPool effect))
                        {
                                effect.Activate (impact, impact.bottomPosition, Quaternion.LookRotation (Vector3.forward, impact.direction));
                        }
                }

                public void ActivateWithInvertedDirection (ImpactPacket impact)
                {
                        if (particleList.TryGetValue (impact.name, out WorldEffectPool effect))
                        {
                                effect.Activate (impact, impact.bottomPosition, Quaternion.LookRotation (Vector3.forward, -impact.direction));
                        }
                }

                public void ActivateWithMirrorDirection (ImpactPacket impact)
                {
                        if (particleList.TryGetValue (impact.name, out WorldEffectPool effect))
                        {
                                Vector3 direction = impact.direction.x > 0 ? Vector3.zero : new Vector3 (0, 180f, 0);
                                effect.Activate (impact, impact.bottomPosition, Quaternion.Euler (direction));
                        }
                }
        }

        [System.Serializable]
        public class WorldEffectPool
        {
                [SerializeField] public GameObject gameObject;
                [System.NonSerialized] private List<GameObject> list = new List<GameObject> ( );
                [System.NonSerialized] private Transform parent;

                public static GameObject currentGameObject;

                public void Initialize (Transform parent)
                {
                        this.parent = parent;
                        list.Add (gameObject);
                }

                public void ResetAll ( )
                {
                        for (int i = 0; i < list.Count; i++)
                                if (list[i] != null)
                                {
                                        list[i].SetActive (false);
                                }
                }

                public void Activate (ImpactPacket impact, Vector3 position, Quaternion rotation)
                {
                        for (int i = 0; i < list.Count; i++)
                                if (list[i] != null && !list[i].activeInHierarchy)
                                {
                                        Transform transform = list[i].transform;
                                        transform.position = position;
                                        transform.rotation = rotation;
                                        list[i].SetActive (true);
                                        currentGameObject = transform.gameObject;
                                        return;
                                }

                        GameObject newEffect = MonoBehaviour.Instantiate (gameObject, position, rotation, parent);
                        newEffect.gameObject.SetActive (true);
                        list.Add (newEffect);
                        currentGameObject = newEffect;
                }

                public void Activate (ImpactPacket impact, Vector3 position)
                {
                        for (int i = 0; i < list.Count; i++)
                                if (list[i] != null && !list[i].activeInHierarchy)
                                {
                                        Transform transform = list[i].transform;
                                        transform.position = position;
                                        list[i].SetActive (true);
                                        currentGameObject = transform.gameObject;
                                        return;
                                }

                        GameObject newEffect = MonoBehaviour.Instantiate (gameObject, position, Quaternion.identity, parent);
                        newEffect.gameObject.SetActive (true);
                        list.Add (newEffect);
                        currentGameObject = newEffect;
                }

        }
}