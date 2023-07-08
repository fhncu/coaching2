using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu ("Flare Engine/一Saving/WorldString")]
        public class WorldString : WorldVariable
        {
                [SerializeField] public string variableName = "name"; // name must be unique
                [SerializeField] private string currentValue;
                [SerializeField] private bool isScriptableObject = false;
                [SerializeField] private WorldStringSO soReference;
                [SerializeField] private bool save = false;
                [SerializeField] private UnityEventString afterLoad = new UnityEventString ( );
                [SerializeField] private SaveString saveString = new SaveString ( );

                private bool sOAvailable => isScriptableObject && soReference != null;

                #region 
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] private bool foldOut = false;
                [SerializeField, HideInInspector] private bool eventFoldOut = false;
                [SerializeField, HideInInspector] private bool loadFoldOut = false;
                [SerializeField, HideInInspector] private bool createSO = false;
                #pragma warning restore 0414
                #endif
                #endregion

                public override void Register ( )
                {
                        if (sOAvailable) soReference.Register (this);
                }

                private void Start ( )
                {
                        SetSOValue ( );
                        if (save)
                        {
                                saveString.value = currentValue;
                                currentValue = Storage.Load<SaveString> (saveString, WorldManager.saveFolder, variableName).value;
                                SetSOValue ( );
                        }
                        afterLoad.Invoke (currentValue);
                }

                public override void Save ( )
                {
                        if (save)
                        {
                                saveString.value = currentValue;
                                Storage.Save (saveString, WorldManager.saveFolder, variableName);
                        }
                }

                public override void DeleteSavedData ( )
                {
                        Storage.Delete (WorldManager.saveFolder, variableName);
                }

                public void Refresh (string value)
                {
                        currentValue = value;
                }

                public void SetValue (string value)
                {
                        currentValue = value;
                        SetSOValue ( );
                }

                public void SetValueAndSave (string value)
                {
                        currentValue = value;
                        SetSOValue ( );
                        Save ( );
                }

                public string GetValue ( )
                {
                        return currentValue;
                }

                private void SetSOValue ( )
                {
                        if (sOAvailable)
                        {
                                soReference.SetWorldValue (currentValue);
                        }
                }
        }
}