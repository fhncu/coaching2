using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TwoBitMachines.FlareEngine
{
        [CreateAssetMenu (menuName = "FlareEngine/InputButtonSO")]
        public class InputButtonSO : ScriptableObject
        {
                [SerializeField] public string buttonName;
                [SerializeField] public InputType type;
                [SerializeField] public InputMouse mouse;
                [SerializeField] public KeyCode key = KeyCode.A;
                [SerializeField] public List<InputButtonSO> bindings = new List<InputButtonSO> ( );

                [System.NonSerialized] public bool inputHold;
                [System.NonSerialized] public bool inputPressed;
                [System.NonSerialized] public bool inputReleased;
                [SerializeField, HideInInspector] public bool foldOut;

                #region Read Button Values
                public bool Holding ( )
                {
                        if (inputHold)
                        {
                                return true;
                        }
                        if (type == InputType.Keyboard ? Input.GetKey (key) : Input.GetMouseButton ((int) mouse))
                        {
                                return true;
                        }
                        for (int i = 0; i < bindings.Count; i++)
                        {
                                if (bindings[i] != null && bindings[i] != this && bindings[i].HoldingBinding ( ))
                                {
                                        return true;
                                }
                        }
                        return false;
                }

                public bool Pressed ( )
                {
                        if (inputPressed)
                        {
                                return true;
                        }
                        if (type == InputType.Keyboard ? Input.GetKeyDown (key) : Input.GetMouseButtonDown ((int) mouse))
                        {
                                return true;
                        }
                        for (int i = 0; i < bindings.Count; i++)
                        {
                                if (bindings[i] != null && bindings[i] != this && bindings[i].PressedBinding ( ))
                                {
                                        return true;
                                }
                        }
                        return false;
                }

                public bool Released ( )
                {
                        if (inputReleased)
                        {
                                return true;
                        }
                        if (type == InputType.Keyboard ? Input.GetKeyUp (key) : Input.GetMouseButtonUp ((int) mouse))
                        {
                                return true;
                        }
                        for (int i = 0; i < bindings.Count; i++)
                        {
                                if (bindings[i] != null && bindings[i] != this && bindings[i].ReleasedBinding ( ))
                                {
                                        return true;
                                }
                        }
                        return false;
                }

                public bool HoldingBinding ( )
                {
                        return inputHold ? true : type == InputType.Keyboard ? Input.GetKey (key) : Input.GetMouseButton ((int) mouse);
                }

                public bool PressedBinding ( )
                {
                        return inputPressed ? true : type == InputType.Keyboard ? Input.GetKeyDown (key) : Input.GetMouseButtonDown ((int) mouse);
                }

                public bool ReleasedBinding ( )
                {
                        return inputReleased? true : type == InputType.Keyboard ? Input.GetKeyUp (key) : Input.GetMouseButtonUp ((int) mouse);
                }

                public bool Active (ButtonTrigger buttonTrigger)
                {
                        if (buttonTrigger == ButtonTrigger.OnPress)
                                return Pressed ( );
                        else if (buttonTrigger == ButtonTrigger.OnHold)
                                return Holding ( );
                        else if (buttonTrigger == ButtonTrigger.OnRelease)
                                return Released ( );
                        else if (buttonTrigger == ButtonTrigger.Always)
                                return true;
                        else if (buttonTrigger == ButtonTrigger.Never)
                                return false;
                        else
                                return Active ( );
                }

                public bool Active ( )
                {
                        return inputPressed || inputHold; // these can only be read if they've been set true externally
                }

                public bool Any (ButtonTrigger buttonTrigger)
                {
                        return Pressed ( ) || Holding ( ) || Released ( ) || (buttonTrigger == ButtonTrigger.Always);
                }
                #endregion

                #region Set Button Values
                public void ButtonPressed ( )
                {
                        inputPressed = true;
                        for (int i = 0; i < bindings.Count; i++)
                                if (bindings[i] != null) bindings[i].inputPressed = true;
                }

                public void ButtonHold ( )
                {
                        inputHold = true;
                        for (int i = 0; i < bindings.Count; i++)
                                if (bindings[i] != null) bindings[i].inputHold = true;
                }

                public void ButtonReleased ( )
                {
                        inputHold = false;
                        inputReleased = true; //cleared each frame by world manager
                        for (int i = 0; i < bindings.Count; i++)
                        {
                                if (bindings[i] == null) continue;
                                bindings[i].inputHold = false;
                                bindings[i].inputReleased = true;
                        }
                }

                #if ENABLE_INPUT_SYSTEM
                public void InputPerformed (InputAction.CallbackContext context)
                {
                        if (context.performed) inputPressed = true;
                }

                public void InputPerformedHold (InputAction.CallbackContext context)
                {
                        if (context.performed) inputHold = true;
                }

                public void InputCancelled (InputAction.CallbackContext context)
                {
                        if (context.canceled)
                        {
                                inputHold = false;
                                inputReleased = true;
                        }
                }
                #endif
                #endregion

                #region Override Button Values -- old input system only
                public void RestoreSavedValues ( )
                {
                        key = (KeyCode) PlayerPrefs.GetInt ("TwoBitMachinesButton" + buttonName, (int) key);
                        mouse = (InputMouse) PlayerPrefs.GetInt ("TwoBitMachinesMouse" + buttonName, (int) mouse);
                }

                public void OverrideKeyboardKey (KeyCode newKey)
                {
                        key = newKey;
                        PlayerPrefs.SetInt ("TwoBitMachinesButton" + buttonName, (int) key);
                }

                public void OverrideMouseKey (int newKey)
                {
                        mouse = (InputMouse) newKey;
                        PlayerPrefs.SetInt ("TwoBitMachinesMouse" + buttonName, newKey);
                }
                #endregion
        }

        public enum ButtonTrigger
        {
                OnHold,
                OnPress,
                OnRelease,
                Always,
                Never,
                Active
        }

        public enum InputMouse
        {
                Left,
                Right,
                Middle
        }

        public enum InputType
        {
                Keyboard,
                Mouse
        }

}