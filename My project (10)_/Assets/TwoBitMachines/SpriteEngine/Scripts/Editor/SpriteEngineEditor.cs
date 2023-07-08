using System.Collections.Generic;
using System.Linq;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite.Editors
{
        [CustomEditor (typeof (SpriteEngine), true)]
        [CanEditMultipleObjects]
        public class SpriteEngineEditor : Editor
        {
                public SpriteEngine main;
                public SerializedObject parent;
                public List<string> spriteNames = new List<string> ( );

                public SerializedProperty frames;
                public SerializedProperty property;
                public SerializedProperty sprite;
                public SerializedProperty frameIndex;
                public SerializedProperty spriteIndex;
                public SerializedProperty sprites;
                public bool spriteExists => sprite != null && frames != null;
                public static bool inspectorLocked => ActiveEditorTracker.sharedTracker.isLocked;

                //Play in Scene
                public Sprite originalSprite;
                public Sprite currentSprite;
                public SpritePacket tempSprite;
                public SpriteRenderer render;
                public int currentFrameIndex;
                public float timer;
                public bool propertyOpen;
                public string previousSprite;
                public string[] signals;
                public List<Sprite> tempSprites = new List<Sprite> ( );
                public List<Texture2D> tempTexture2D = new List<Texture2D> ( );
                public static Texture2D[] iconArray;

                private void OnEnable ( )
                {
                        main = target as SpriteEngine;
                        parent = serializedObject;
                        Layout.Initialize ( );
                        if (iconArray == null || iconArray.Length == 0)
                        {
                                iconArray = SpriteProperty.PropertyIcons ( );
                        }
                }

                private void OnDisable ( )
                {
                        StopPlayEditorOnly ( );
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );
                        Layout.VerticalSpacing (10);

                        serializedObject.Update ( );
                        {
                                SetCurrentSprite ( );
                                PlayButtons ( );
                                PreviewWindow.Display (this, parent, main);
                                signals = main.tree.signals.ToArray ( );

                                if (CreateSprite ( ))
                                {
                                        SetCurrentSprite ( ); // reset animation if sprites have been added or deleted
                                }
                                if (spriteExists)
                                {
                                        if (
                                                FoldOut.Bar (parent, Tint.SoftDark)
                                                .Label ("Sprites", Tint.White)
                                                .RightTab ("tabIndex", "DropCorner", 0, Tint.Blue, Tint.White, "Replace Sprites")
                                                .RightTab ("tabIndex", "Event", 1, Tint.Blue, Tint.White, "Sprite Options")
                                                .RightTab ("tabIndex", "Signal", 2, Tint.Blue, Tint.White, "Sprite Swap")
                                                .RightTab ("tabIndex", "Wrench", 3, Tint.Blue, Tint.White, "Properties")
                                                .FoldOut ("spritesFoldOut")
                                        )
                                        {
                                                InspectSprite.Display (parent, frames, frameIndex, sprite);
                                                SpriteProperty.Execute (property, frameIndex, ref propertyOpen);
                                                DragAndDrop ( );
                                                Property ( );
                                                Options ( );
                                        }
                                }
                                SpriteTreeEditor.TreeInspector (main.tree, parent.Get ("tree"), spriteNames, signals);
                                TransitionEditor.animationMenu = AddTransitionToAnimation;
                                TransitionEditor.Transition (parent, sprites, spriteNames.ToArray ( ), signals);
                                ShowCurrentState (main.currentAnimation);
                        }
                        serializedObject.ApplyModifiedProperties ( );
                        Layout.VerticalSpacing (10);
                        SpriteProperty.SetExtraProperty (main, spriteExists, frameIndex.intValue);
                        SetSpriteEditingProperty ( );
                        if (main.sort)
                        {
                                main.sort = false;
                                main.sprites = main.sprites.OrderBy (sp => sp.name).ToList ( );
                        }
                        if (GUI.changed && !EditorApplication.isPlaying)
                        {
                                Repaint ( );
                        }
                }

                private void SetCurrentSprite ( )
                {
                        sprites = parent.Get ("sprites");
                        spriteIndex = parent.Get ("spriteIndex");
                        spriteIndex.intValue = Mathf.Clamp (spriteIndex.intValue, 0, sprites.arraySize);
                        sprite = sprites.arraySize > 0 ? sprites.Element (spriteIndex.intValue) : null;
                        frames = sprite != null ? sprite.Get ("frame") : null;
                        frameIndex = sprite != null ? sprite.Get ("frameIndex") : null;
                        property = sprite != null ? sprite.Get ("property") : null;
                        sprites.CreateNameList (spriteNames);
                }

                public void PlayButtons ( )
                {
                        Bar.SetupTabBar (3, height : 22);
                        bool canPlay = main.sprites.Count != 0 && spriteIndex.intValue >= 0 && spriteIndex.intValue < main.sprites.Count;
                        if (Bar.TabButton ("Play", "RoundTopLeft", Tint.SoftDark, parent.Bool ("playInInspector"), "Play In Inspector") && canPlay)
                        {
                                parent.SetTrue ("playInInspector");
                                parent.SetFalse ("playInScene");
                                parent.SetTrue ("resetPlayFrame");
                                StopPlayEditorOnly ( );
                        }
                        if (Bar.TabButton ("PlayScene", "Square", Tint.SoftDark, parent.Bool ("playInScene"), "Play in Scene") && canPlay)
                        {
                                parent.SetFalse ("playInInspector");
                                parent.SetTrue ("playInScene");
                                parent.SetTrue ("resetPlayFrame");
                                InitializePlayEditorOnly ( );
                        }
                        if (Bar.TabButton ("Red", "RoundTopRight", Tint.SoftDark, false) && canPlay)
                        {
                                parent.SetFalse ("playInInspector");
                                parent.SetFalse ("playInScene");
                                parent.SetTrue ("resetPlayFrame");
                                StopPlayEditorOnly ( );
                        }
                }

                public bool CreateSprite ( )
                {
                        bool delete = parent.Bool ("deleteAsk");
                        FoldOut.Bar (parent, Tint.SoftDark, 4).BL ("open", "ArrowRight").
                        BR ("deleteAsk", "Yes", execute : delete).SR (8, execute : delete).BR ("delete", "Close", execute : delete).
                        BR ("deleteAsk", "Minus", execute: !delete).SR (8).BR (execute: !delete).SR (3).BR ("sort", "Sort");
                        {
                                Rect labelRect = new Rect (Bar.barStart) { width = Layout.longInfoWidth - 90 };
                                labelRect.DrawRect (texture: Skin.square, color: Color.white);
                                if (spriteNames.Count > 0)
                                {
                                        GUI.Label (labelRect.Adjust (5, labelRect.width - 4), spriteNames[spriteIndex.intValue], FoldOut.boldStyle);
                                }
                                if (spriteExists)
                                {
                                        sprite.FieldOnlyClear (labelRect, "name"); // change index string name
                                }
                        }
                        if (parent.Bool ("open"))
                        {
                                if (FoldOut.DropDownMenu (spriteNames, parent.Get ("shiftNames"), spriteIndex))
                                {
                                        parent.SetFalse ("open");
                                }
                                Repaint ( );
                        }
                        return AppendSprites ( );
                }

                private bool AppendSprites ( )
                {
                        if (parent.ReadBool ("delete") && sprites.arraySize > 1)
                        {
                                parent.SetFalse ("deleteAsk");
                                sprites.DeleteArrayElement (spriteIndex.intValue);
                                spriteIndex.intValue = Mathf.Clamp (spriteIndex.intValue, 0, sprites.arraySize - 1);
                                return true;
                        }
                        if (parent.ReadBool ("add") || sprites.arraySize == 0)
                        {
                                sprites.CreateNewElement ( ).FindPropertyRelative ("name").stringValue = "_New Sprite";
                                spriteIndex.intValue = sprites.arraySize - 1;
                                return true;
                        }
                        return false;
                }

                public void DragAndDrop ( )
                {
                        if (frames.arraySize == 0)
                        {
                                CreateDragAndDropArea ("Drop Sprites Here", Tint.White);
                                TransferSpritesFromTempList ( );
                        }
                        if (parent.Int ("tabIndex") == 0 && frames.arraySize > 0)
                        {
                                CreateDragAndDropArea ("Replace Sprites", Tint.Delete);
                                TransferSpritesFromTempList ( );
                        }
                }

                private void CreateDragAndDropArea (string message, Color color)
                {
                        tempSprites.Clear ( );
                        tempTexture2D.Clear ( );
                        Rect dropArea = Layout.CreateRectAndDraw (Layout.longInfoWidth, 88, xOffset: -11, texture : Skin.square, color : Tint.SoftDark);
                        {
                                Fields.DropAreaGUI<Sprite> (dropArea, tempSprites);
                                Fields.DropAreaGUI<UnityEngine.Texture2D> (dropArea, tempTexture2D);
                                Labels.Centered (dropArea, message, color, fontSize : 10, shiftY : 15);

                                Rect dropRect = Skin.TextureCentered (dropArea, Icon.Get ("DropCorner"), new Vector2 (22, 22), Tint.White, shiftY: -10);
                                if (frames.arraySize == 0 && dropRect.ContainsMouseDown ( ))
                                {
                                        frames.arraySize++;
                                        parent.SetFalse ("resetSprites");
                                        ActiveEditorTracker.sharedTracker.isLocked = false;
                                        Selection.activeGameObject = main.gameObject;
                                }
                                else if (dropArea.ContainsMouseDown (false))
                                {
                                        ToggleInspectorLock ( );
                                }
                                if (ActiveEditorTracker.sharedTracker.isLocked) Labels.Centered (dropArea, "Locked", color, fontSize : 10, shiftY : 28);
                        }
                }

                private void TransferSpritesFromTempList ( )
                {
                        if ((tempSprites.Count == 0 && tempTexture2D.Count == 0) || frames == null) return;

                        frames.arraySize = 0;
                        parent.SetFalse ("resetSprites");
                        ActiveEditorTracker.sharedTracker.isLocked = false;
                        Selection.activeGameObject = main.gameObject;

                        for (int i = 0; i < tempSprites.Count; i++)
                        {
                                frames.arraySize++;
                                frames.LastElement ( ).Get ("sprite").objectReferenceValue = tempSprites[i];
                                frames.LastElement ( ).Get ("rate").floatValue = 1f / 10f;
                        }
                        for (int i = 0; i < tempTexture2D.Count; i++)
                        {
                                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite> (AssetDatabase.GetAssetPath (tempTexture2D[i]));
                                if (sprite == null) continue;
                                frames.arraySize++;
                                frames.LastElement ( ).Get ("sprite").objectReferenceValue = sprite;
                                frames.LastElement ( ).Get ("rate").floatValue = 1f / 10f;
                        }
                }

                public void ToggleInspectorLock ( )
                {
                        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
                }

                public void Options ( )
                {
                        if (parent.Int ("tabIndex") == 2)
                        {
                                FoldOut.Box (Texture2D.whiteTexture, 1, Tint.SoftDark, yOffset: -1);
                                {
                                        parent.Field ("Sprite Swap", "spriteSwap");
                                }
                                Layout.VerticalSpacing (5);
                        }
                        if (sprite == null || frames == null)
                        {
                                return;
                        }

                        if (parent.Int ("tabIndex") == 1 && frames.arraySize > 0)
                        {
                                FoldOut.Box (Texture2D.whiteTexture, 4, Tint.SoftDark, yOffset: -1);
                                {
                                        sprite.FieldAndEnable ("Synchronize", "syncID", "canSync");
                                        Labels.FieldText ("Sync ID", rightSpacing : 18);
                                        sprite.Field ("Loop Start Index", "loopStartIndex");
                                        sprite.FieldToggleAndEnable ("Loop Once", "loopOnce");
                                }
                                Layout.VerticalSpacing (5);
                                if (sprite.Bool ("loopOnce"))
                                {
                                        Fields.EventField (sprite.Get ("onLoopOnce"), adjustX : 10, color : Tint.SoftDark);
                                }
                                else
                                {
                                        Layout.VerticalSpacing (4);
                                        Layout.CreateRectAndDraw (Layout.longInfoWidth, 42, xOffset: -11, texture : Texture2D.whiteTexture, color : Tint.SoftDark); // to keep all options height even
                                }
                        }
                }

                public void AddTransitionToAnimation (object obj)
                {
                        string spriteName = (string) obj;
                        parent.Update ( );
                        sprites = parent.Get ("sprites");
                        for (int i = 0; i < sprites.arraySize; i++)
                        {
                                if (sprites.Element (i).String ("name") == spriteName)
                                {
                                        sprites.Element (i).SetTrue ("hasTransition");
                                        break;
                                }
                        }
                        parent.ApplyModifiedProperties ( );
                }

                public void Property ( )
                {
                        if (frames == null || property == null)
                        {
                                return;
                        }

                        SpriteProperty.MatchArraySize (property, frames.arraySize);
                        SpriteProperty.SecureSameSize (property, frames.arraySize);

                        if (frames.arraySize != 0 && parent.Int ("tabIndex") == 3) /// CREATE PROPERTIES
                        {
                                SerializedProperty propertyIndex = parent.Get ("propertyIndex");
                                propertyIndex.intValue = -1;;
                                FoldOut.DropDownMenu (SpriteProperty.NameList ( ), parent.Get ("shiftProperty"), propertyIndex, iconArray, itemLimit : 4);

                                if (propertyIndex.intValue >= 0 && main != null)
                                {
                                        parent.Get ("tabIndex").intValue = -1;
                                        SpriteProperty.CreateProperty (propertyIndex.intValue, property);
                                        SpriteProperty.MatchArraySize (property, frames.arraySize);
                                }
                                Repaint ( );
                        }
                        SpriteProperty.DeleteArrayElement (property);
                }

                private void SetSpriteEditingProperty ( )
                {
                        if (main.playingInScene)
                        {
                                return;
                        }

                        if (propertyOpen && currentSprite != null)
                        {
                                if (render == null)
                                {
                                        render = main.gameObject.GetComponent<SpriteRenderer> ( );
                                }
                                if (render == null)
                                {
                                        return;
                                }
                                if (!main.settingRenderer)
                                {
                                        main.rendererSprite = render.sprite;
                                }
                                main.settingRenderer = true;
                                render.sprite = currentSprite;
                                if (previousSprite != currentSprite.name)
                                {
                                        SceneView.RepaintAll ( );
                                }
                                previousSprite = currentSprite.name;
                        }
                        else if (main.settingRenderer)
                        {
                                if (render == null)
                                {
                                        render = main.gameObject.GetComponent<SpriteRenderer> ( );
                                }
                                if (main.rendererSprite != null && render != null)
                                {
                                        render.sprite = main.rendererSprite;
                                }
                                main.settingRenderer = false;
                        }
                }

                public static void ShowCurrentState (string animation)
                {
                        if (Application.isPlaying)
                        {
                                FoldOut.BoxSingle (1, Tint.Blue);
                                EditorGUILayout.LabelField ("Current Animation:   " + animation);
                                Layout.VerticalSpacing (5);
                        }
                }

                #region Play In Scene
                public void InitializePlayEditorOnly ( )
                {
                        StopPlayEditorOnly ( );
                        render = main.gameObject.GetComponent<SpriteRenderer> ( );
                        Clock.Initialize ( );
                        timer = currentFrameIndex = 0;
                        tempSprite = null;
                        for (int i = 0; i < main.sprites.Count; i++)
                        {
                                if (main.spriteIndex != i) continue;
                                tempSprite = main.sprites[i];
                                originalSprite = render != null ? render.sprite : originalSprite;
                                tempSprite.SetProperties (0, firstFrame : true);
                                EditorApplication.update += RunAnimation;
                                break;
                        }
                }

                private void RunAnimation ( )
                {
                        if (EditorApplication.isPlayingOrWillChangePlaymode || BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling || render == null || tempSprite == null || tempSprite.frame.Count == 0)
                        {
                                StopPlayEditorOnly ( );
                                return;
                        }
                        if (Clock.TimerEditor (ref timer, tempSprite.frame[currentFrameIndex].rate))
                        {
                                currentFrameIndex = currentFrameIndex + 1 >= tempSprite.frame.Count ? 0 : currentFrameIndex + 1;
                                render.sprite = tempSprite.frame[currentFrameIndex].sprite;
                                tempSprite.SetProperties (currentFrameIndex);
                        }
                        for (int i = 0; i < tempSprite.property.Count; i++)
                        {
                                if (tempSprite.property[i].interpolate)
                                {
                                        float playRate = tempSprite.frame[currentFrameIndex].rate;
                                        tempSprite.property[i].Interpolate (currentFrameIndex, playRate, timer);
                                }
                        }
                }

                public void StopPlayEditorOnly ( )
                {
                        if (tempSprite != null)
                        {
                                tempSprite.ResetProperties ( );
                        }
                        if (render != null && originalSprite != null)
                        {
                                render.sprite = originalSprite;
                        }
                        EditorApplication.update -= RunAnimation;
                        originalSprite = null; //
                        tempSprite = null;
                }
                #endregion
        }
}