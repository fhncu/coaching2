using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        [CustomEditor (typeof (Firearm)), CanEditMultipleObjects]
        public class FirearmEditor : UnityEditor.Editor
        {
                private Firearm main;
                private Transform transformReference;
                private GameObject objReference;
                private SerializedObject parent;

                private Color barColor = Tint.BoxTwo;
                private object[] buttons;
                private string[] buttonNames;

                private void OnEnable ( )
                {
                        main = target as Firearm;
                        transformReference = main.transform;
                        objReference = main.gameObject;
                        parent = serializedObject;
                        Layout.Initialize ( );

                        string path = "Assets/TwoBitMachines/FlareEngine/AssetsFolder/Inputs";
                        string[] guids = AssetDatabase.FindAssets ("t:scriptableObject", new [] { path });
                        buttons = new object[guids.Length];
                        buttonNames = new string[buttons.Length];
                        for (int i = 0; i < guids.Length; i++)
                        {
                                buttons[i] = AssetDatabase.LoadAssetAtPath (AssetDatabase.GUIDToAssetPath (guids[i]), typeof (InputButtonSO));
                                buttonNames[i] = (buttons[i] as InputButtonSO).buttonName;
                        }
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );
                        Layout.VerticalSpacing (10);
                        parent.Update ( );
                        {
                                Firearm ( );
                                DefaultProjectile ( );
                                Rotation ( );
                                LineOfSight ( );
                                ChargedProjectile ( );

                        }
                        parent.ApplyModifiedProperties ( );

                        Layout.VerticalSpacing (10);
                }

                public void Firearm ( )
                {
                        if (FoldOut.Bar (parent).Label ("Firearm").FoldOut ( ))
                        {
                                FoldOut.Box (6, FoldOut.boxColor, extraHeight : 3, yOffset: -2);
                                {
                                        parent.Field ("Name", "toolName");
                                        parent.Field ("Fire Point", "firePoint");

                                        parent.FieldAndDropDownList (buttonNames, "Button", "buttonTrigger", "buttonName");
                                        string buttonName = parent.String ("buttonName");;

                                        if (buttons != null)
                                        {
                                                for (int i = 0; i < buttons.Length; i++)
                                                {
                                                        InputButtonSO button = buttons[i] as InputButtonSO;
                                                        if (buttonName == button.buttonName)
                                                        {
                                                                parent.Get ("input").objectReferenceValue = button;
                                                                break;
                                                        }
                                                }
                                        }
                                        parent.Field ("Local Position", "localPosition");
                                        parent.FieldAndEnable ("Stop Velocity", "stopTime", "stopVelocity");
                                        Labels.FieldText ("Time", rightSpacing : 18);
                                        parent.FieldToggleAndEnable ("Off Near Wall", "turnOffNearWall", labelOffset : 0, toggleOffset : 1);
                                }

                                if (FoldOut.FoldOutButton (parent.Get ("mainFoldOut"), yOffset: -2))
                                {
                                        Fields.EventFoldOut (parent.Get ("onActivated"), parent.Get ("eventFoldOut"), "On Activated");
                                }
                        }
                }

                public void DefaultProjectile ( )
                {
                        SerializedProperty projectile = parent.Get ("defaultProjectile");
                        if (FoldOut.Bar (parent).Label ("Projectile").FoldOut ("projectileFoldOut"))
                        {
                                SerializedProperty animation = projectile.Get ("waitForAnimation");
                                int extra = animation.Bool ("wait") ? 1 : 0;
                                FoldOut.Box (4 + extra, FoldOut.boxColor, yOffset: -2, extraHeight : 3);
                                {
                                        projectile.Field ("Default Projectile", "projectile");
                                        parent.Field ("Inventory", "projectileInventory");
                                        projectile.Field ("Auto Discharge", "autoDischarge");
                                        animation.FieldAndEnable ("Shoot Animation", "weaponAnimation", "wait");
                                        Labels.FieldText ("Signal Name", rightSpacing : Layout.boolWidth, execute : animation.String ("weaponAnimation") == "");
                                }
                                if (extra > 0)
                                {
                                        animation.FieldDouble ("Extra Animation", "spriteEngine", "extraSignal");
                                }
                                if (FoldOut.FoldOutButton (projectile.Get ("eventsFoldOut"), yOffset: -2))
                                {
                                        Fields.EventFoldOut (projectile.Get ("onFireSuccess"), projectile.Get ("fireFoldOut"), "On Fire Success");
                                        Fields.EventFoldOut (projectile.Get ("onOutOfAmmo"), projectile.Get ("outOfAmmoFoldOut"), "On Out Of Ammo");
                                }
                                Recoil ("Recoil", projectile.Get ("recoil"), null);
                        }
                }

                public void Rotation ( )
                {
                        SerializedProperty rotate = parent.Get ("rotate");

                        if (FoldOut.Bar (parent).Label ("Rotation").FoldOut ("rotateFoldOut"))
                        {
                                int index = rotate.Enum ("rotate");

                                if (index == 4)
                                {
                                        FoldOut.Box (2, FoldOut.boxColor);
                                        rotate.Field ("Rotate With", "rotate");
                                        rotate.Field ("Fixed Direction", "fixedDirection");
                                        Layout.VerticalSpacing (5);
                                        return;
                                }
                                if (index == 5)
                                {
                                        FoldOut.Box (1, FoldOut.boxColor);
                                        rotate.Field ("Rotate With", "rotate");
                                        Layout.VerticalSpacing (5);
                                        return;
                                }

                                int orientation = rotate.Enum ("orientation");
                                FoldOut.Box (2, FoldOut.boxColor, yOffset: -2);
                                rotate.Field ("Rotate With", "rotate");
                                rotate.Field ("Point Towards", "orientation", execute : orientation < 2);
                                rotate.FieldDouble ("Point Towards", "orientation", "transformDirection", execute : orientation > 1);
                                Layout.VerticalSpacing (3);

                                if (index == 0)
                                {
                                        FoldOut.Box (3, FoldOut.boxColor);
                                        if (orientation != 0)
                                        {
                                                GUI.enabled = false;
                                                rotate.Slider ("Top Limit", "maxLimit", 0, 180);
                                                rotate.Get ("maxLimit").floatValue = 180;
                                                GUI.enabled = true;
                                        }
                                        else
                                        {
                                                rotate.Slider ("Top Limit", "maxLimit", 0, 180);
                                        }
                                        rotate.Slider ("Bottom Limit", "minLimit", -180, 0);

                                        if (rotate.Float ("maxLimit") < rotate.Float ("minLimit"))
                                        {
                                                rotate.Get ("maxLimit").floatValue = rotate.Float ("minLimit");
                                        }
                                        rotate.Field ("Angle Offset", "angleOffset");
                                        Layout.VerticalSpacing (5);
                                }
                                if (index == 1)
                                {
                                        FoldOut.Box (3, FoldOut.boxColor);
                                        rotate.FieldDouble ("Left,  Right", "left", "right");
                                        rotate.FieldDouble ("Up,  Down", "up", "down");
                                        rotate.Field ("Diagonal", "diagonal");
                                        Layout.VerticalSpacing (5);
                                }
                                if (index == 2)
                                {
                                        AutoSeek (rotate);
                                }
                                if (index == 3)
                                {
                                        FoldOut.Box (1, FoldOut.boxColor);
                                        rotate.Field ("Fixed Angle", "fixedAngle");
                                        Layout.VerticalSpacing (5);
                                }

                        }
                }

                public void LineOfSight ( )
                {
                        if (FoldOut.Bar (parent).Label ("Aim").FoldOut ("lineOfSightFoldOut"))
                        {
                                SerializedProperty lineOfSight = parent.Get ("lineOfSight");

                                int type = lineOfSight.Enum ("lineOfSight");

                                if (type == 0)
                                {
                                        FoldOut.Box (1, FoldOut.boxColor, yOffset: -2);
                                        lineOfSight.Field ("Line Of Sight", "lineOfSight");
                                        Layout.VerticalSpacing (3);
                                }
                                else if (type == 1)
                                {
                                        FoldOut.Box (7, FoldOut.boxColor, yOffset: -2, extraHeight : 3);
                                        lineOfSight.Field ("Line Of Sight", "lineOfSight");
                                        lineOfSight.Field ("Layer", "layer");
                                        lineOfSight.Field ("Beam", "beam");
                                        lineOfSight.Field ("Beam End", "beamEnd");
                                        lineOfSight.Field ("Max Length", "maxLength");

                                        lineOfSight.Field ("Target", "targetLayer");
                                        lineOfSight.FieldToggle ("Auto Shoot", "autoShoot");
                                        bool eventOpen = FoldOut.FoldOutButton (lineOfSight.Get ("eventsFoldOut"), yOffset: -2);

                                        Fields.EventFoldOut (lineOfSight.Get ("onTargetHit"), lineOfSight.Get ("onTargetHitFoldOut"), "On Target Hit", execute : eventOpen);
                                        Fields.EventFoldOut (lineOfSight.Get ("onBeamHit"), lineOfSight.Get ("onBeamHitFoldOut"), "On Beam Hit", execute : eventOpen);
                                        Fields.EventFoldOut (lineOfSight.Get ("onNothingHit"), lineOfSight.Get ("onNothingHitFoldOut"), "On Nothing Hit", execute : eventOpen);

                                }
                                else if (type == 2)
                                {
                                        bool rType = (int) lineOfSight.Enum ("reticleType") == 0;
                                        FoldOut.Box (rType ? 4 : 3, FoldOut.boxColor, yOffset: -2);
                                        lineOfSight.Field ("Line Of Sight", "lineOfSight");
                                        lineOfSight.Field ("Aim Reticle", "reticle");
                                        lineOfSight.Field ("Follow Type", "reticleType");
                                        if ((int) lineOfSight.Enum ("reticleType") == 0)
                                        {
                                                lineOfSight.Field ("Distance", "reticleDistance");
                                        }
                                        Layout.VerticalSpacing (3);
                                }
                        }
                }

                public void ChargedProjectile ( )
                {
                        SerializedProperty charge = parent.Get ("chargedProjectile");
                        if (FoldOut.Bar (charge).Label ("Charge").SR (5).BRE ("canCharge").FoldOut ("chargeFoldOut"))
                        {
                                GUI.enabled = charge.Bool ("canCharge");
                                {
                                        SerializedProperty animation = charge.Get ("waitForAnimation");
                                        int extra = animation.Bool ("wait") ? 1 : 0;
                                        if (Application.isPlaying && charge.Bool ("canCharge"))
                                        {
                                                FoldOut.Box (6 + extra, FoldOut.boxColor, yOffset: -2, extraHeight : 3);
                                                GUILayout.Label ("Fully Charged:   " + charge.Get ("percentCharged").floatValue.ToString ( ));
                                        }
                                        else
                                        {
                                                FoldOut.Box (5 + extra, FoldOut.boxColor, yOffset: -2, extraHeight : 3);
                                        }
                                        charge.Field ("Projectile", "projectile");
                                        charge.FieldDouble ("Charge Time", "chargeMaxTime", "chargeMinTime");
                                        Labels.FieldDoubleText ("Max", "Min");
                                        charge.Field ("Discharge Time", "dischargeTime");
                                        charge.Field ("Cooldown Time", "coolDownTime");
                                        animation.FieldAndEnable ("Shoot Animation", "weaponAnimation", "wait");
                                        Labels.FieldText ("Signal Name", rightSpacing : Layout.boolWidth, execute : animation.String ("weaponAnimation") == "");
                                        if (extra > 0)
                                                animation.FieldDouble ("Extra Animation", "spriteEngine", "extraSignal");

                                        if (FoldOut.FoldOutButton (charge.Get ("eventsFoldOut"), yOffset: -2))
                                        {
                                                Fields.EventFoldOut (charge.Get ("onCharging"), charge.Get ("onChargingFoldOut"), "On Charging");
                                                Fields.EventFoldOut (charge.Get ("onChargingComplete"), charge.Get ("onChargingCompleteFoldOut"), "On Charging Complete");
                                                Fields.EventFoldOut (charge.Get ("onDischarging"), charge.Get ("onDischargingFoldOut"), "On Discharging");
                                                Fields.EventFoldOut (charge.Get ("onDischargingComplete"), charge.Get ("onDischargingCompleteFoldOut"), "On Discharging Complete");
                                                Fields.EventFoldOut (charge.Get ("onDischargingFailed"), charge.Get ("onDischargingFailedFoldOut"), "On Discharging Failed");
                                                Fields.EventFoldOut (charge.Get ("onCoolingDown"), charge.Get ("onCoolDownFoldOut"), "On Cooling Down");
                                        }

                                        Recoil ("Recoil", charge.Get ("recoil"), charge, extraHeight : 1);
                                }
                                GUI.enabled = true;
                        }
                }

                public void AutoSeek (SerializedProperty parent)
                {
                        FoldOut.Box (4, FoldOut.boxColor, extraHeight : 3);
                        SerializedProperty autoSeek = parent.Get ("autoSeek");
                        autoSeek.Field ("Target Layer", "layer");
                        autoSeek.Field ("Search Radius", "maxRadius");
                        autoSeek.Field ("Search Rate", "searchRate");
                        autoSeek.FieldToggle ("Auto Shoot", "autoShoot");
                        bool eventOpen = FoldOut.FoldOutButton (autoSeek.Get ("eventsFoldOut"));
                        Fields.EventFoldOut (autoSeek.Get ("onFoundTarget"), autoSeek.Get ("onFoundTargetFoldOut"), "On Found New Target", execute : eventOpen);
                }

                public void Recoil (string title, SerializedProperty recoil, SerializedProperty discharge, int extraHeight = 0)
                {
                        if (FoldOut.Bar (recoil, FoldOut.boxColor).Label ("Recoil", FoldOut.titleColor).BRE ("recoil").FoldOut ( ))
                        {
                                FoldOut.Box (5 + extraHeight, FoldOut.boxColor, yOffset: -2);
                                GUI.enabled = GUI.enabled && recoil.Bool ("recoil");
                                {
                                        recoil.Field ("Type", "type");
                                        if (discharge != null) discharge.Field ("On Discharge", "recoilOnDischarge");
                                        recoil.Field ("Recoil When", "condition");
                                        recoil.Field ("Distance", "recoilDistance");
                                        recoil.Field ("Time", "recoilTime");
                                        recoil.FieldToggle ("Has Gravity", "hasGravity");
                                }
                                GUI.enabled = true;
                                Layout.VerticalSpacing (3);
                        }
                }

        }
}