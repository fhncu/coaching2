using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu ("Flare Engine/一Saving/WorldFloat")]
        public class WorldFloat : WorldVariable
        {
                [SerializeField] public string variableName = "name"; // name must be unique
                [SerializeField] private float currentValue = 10;
                [SerializeField] private float minValue = 0;
                [SerializeField] private float maxValue = 100;
                [SerializeField] private bool save = false;
                [SerializeField] private bool broadcastValue;
                [SerializeField] private bool isScriptableObject;
                [SerializeField] private bool saveManually = false;
                [SerializeField] public WorldFloatSO soReference;

                [SerializeField] public bool callDamageEffect;
                [SerializeField] public float positionOffset = 1f;
                [SerializeField] public string worldEffect = "";

                [SerializeField] private UnityEventEffect onMinValue = new UnityEventEffect ( );
                [SerializeField] private UnityEventEffect onMaxValue = new UnityEventEffect ( );
                [SerializeField] private UnityEventEffect onValueChanged = new UnityEventEffect ( );
                [SerializeField] private UnityEventFloat onLoadConditionTrue = new UnityEventFloat ( );
                [SerializeField] private UnityEventFloat onLoadConditionFalse = new UnityEventFloat ( );
                [SerializeField] private SaveFloat saveFloat = new SaveFloat ( );

                [System.NonSerialized] private float refreshValue;
                [System.NonSerialized] private float tempValue;
                [System.NonSerialized] private Collider2D colliderRef;
                public bool isSaved => save;
                public bool cantIncrement { get; private set; }

                public bool register => isScriptableObject && soReference != null;

                #region 
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] private bool foldOut = false;
                [SerializeField, HideInInspector] private bool eventFoldOut = false;
                [SerializeField, HideInInspector] private bool minFoldOut = false;
                [SerializeField, HideInInspector] private bool maxFoldOut = false;
                [SerializeField, HideInInspector] private bool changedFoldOut = false;
                [SerializeField, HideInInspector] private bool changedGOFoldOut = false;
                [SerializeField, HideInInspector] private bool shieldFoldOut = false;
                [SerializeField, HideInInspector] private bool damageFoldOut = false;
                [SerializeField, HideInInspector] private bool damageEffectFoldOut = false;
                [SerializeField, HideInInspector] private bool loadFoldOutTrue = false;
                [SerializeField, HideInInspector] private bool loadFoldOutFalse = false;
                [SerializeField, HideInInspector] private bool createSO = false;
                #pragma warning restore 0414
                #endif
                #endregion

                private void Start ( )
                {
                        tempValue = 0;
                        SetSOValue ( );
                        if (save)
                        {
                                RestoreValue ( );
                        }
                        if (IsTrue ( ))
                        {
                                onLoadConditionTrue.Invoke (currentValue);
                        }
                        else
                        {
                                onLoadConditionFalse.Invoke (currentValue);
                        }
                }

                public override void Register ( )
                {
                        if (register)
                        {
                                soReference.Register (this);
                        }
                }

                public override bool IncrementValue (Transform attacker, float value, Vector2 direction)
                {
                        if (cantIncrement) return false;

                        float newValue = currentValue + value;
                        if (colliderRef == null)
                        {
                                colliderRef = this.gameObject.GetComponent<Collider2D> ( );
                        }
                        ImpactPacket impact = ImpactPacket.impact.Set (worldEffect, this.transform, colliderRef, this.transform.position, attacker, direction, value);

                        if (newValue <= minValue)
                        {
                                bool success = currentValue > minValue;
                                currentValue = minValue;
                                SetSOValue ( );
                                if (success) onValueChanged.Invoke (impact);
                                if (success) onMinValue.Invoke (impact);
                                return success;
                        }
                        else if (newValue >= maxValue)
                        {
                                bool success = currentValue < maxValue;
                                currentValue = maxValue;
                                SetSOValue ( );
                                if (success) onValueChanged.Invoke (impact);
                                if (success) onMaxValue.Invoke (impact);
                                return success;
                        }
                        else
                        {
                                currentValue = newValue;
                                SetSOValue ( );
                                onValueChanged.Invoke (impact);
                                return true;
                        }
                }

                public ImpactPacket BasicImpact (Transform attacker, Vector2 direction)
                {
                        if (colliderRef == null)
                        {
                                colliderRef = this.gameObject.GetComponent<Collider2D> ( );
                        }
                        return ImpactPacket.impact.Set (worldEffect, this.transform, colliderRef, this.transform.position, attacker, direction, 0);
                }

                public void IncreaseTempValue (ItemEventData itemEventData)
                {
                        if (cantIncrement) return;
                        tempValue += itemEventData.genericFloat;
                }

                public void IncreaseTempValue (float value)
                {
                        if (cantIncrement) return;
                        tempValue += value;
                }

                public void Increment (float value) //                created this method so it can be used by onEvent
                {
                        IncrementValue (null, value, Vector2.zero);
                }

                public void Increment (ItemEventData itemEventData) // created this method so it can be used by onEvent
                {
                        itemEventData.success = IncrementValue (null, itemEventData.genericFloat, Vector2.zero);
                }

                public void CantIncrement (bool value)
                {
                        cantIncrement = value;
                }

                public void SetValue (float value)
                {
                        currentValue = Mathf.Clamp (value, minValue, maxValue);
                        SetSOValue ( );
                }

                public float GetValue ( )
                {
                        return currentValue + tempValue;
                }

                public void SetTrue ( )
                {
                        currentValue = 1f;
                        SetSOValue ( );
                }

                public void SetFalse ( )
                {
                        currentValue = 0;
                        SetSOValue ( );
                }

                public bool IsTrue ( )
                {
                        return currentValue > 0;
                }

                private void SetSOValue ( )
                {
                        if (register)
                        {
                                soReference.SetWorldValue (currentValue);
                        }
                        BroadcastValue ( );
                }

                private void BroadcastValue ( )
                {
                        if (!broadcastValue) return;
                        for (int i = 0; i < variables.Count; i++)
                        {
                                if (variables[i].Name ( ) == variableName)
                                {
                                        variables[i].InternalSet (currentValue);
                                }
                        }
                }

                public override void InternalSet (float newValue)
                {
                        currentValue = newValue;
                }

                public override string Name ( )
                {
                        return variableName;
                }

                public override void ClearTempValue ( )
                {
                        tempValue = 0;
                }

                public void SetRefreshValue ( )
                {
                        currentValue = refreshValue;
                        SetSOValue ( );
                }

                public void UpdateRefreshValue ( )
                {
                        refreshValue = currentValue;
                }

                #region Save
                public void RestoreValue ( )
                {
                        saveFloat.value = currentValue;
                        currentValue = Storage.Load<SaveFloat> (saveFloat, WorldManager.saveFolder, variableName).value;
                        refreshValue = currentValue;
                        SetSOValue ( );
                }

                public override void Save ( )
                {
                        if (save && !saveManually)
                        {
                                SaveNow ( );
                        }
                }

                public void SaveManually ( )
                {
                        SaveNow ( );
                }

                public void SaveTempValue ( )
                {
                        currentValue = Mathf.Clamp (currentValue + tempValue, minValue, maxValue);
                        tempValue = 0;
                        SetSOValue ( );
                        SaveManually ( );
                }

                public void SetValueAndSave (float value)
                {
                        currentValue = Mathf.Clamp (value, minValue, maxValue);
                        SetSOValue ( );
                        SaveNow ( );
                }

                public override void DeleteSavedData ( )
                {
                        Storage.Delete (WorldManager.saveFolder, variableName);
                }

                private void SaveNow ( )
                {
                        saveFloat.value = currentValue;
                        refreshValue = currentValue;
                        Storage.Save (saveFloat, WorldManager.saveFolder, variableName);
                }
                #endregion
        }

}