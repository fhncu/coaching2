#region
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
        [System.Serializable]
        public class Walk
        {
                [SerializeField] public float speed = 10f;
                [SerializeField] public float smooth = 1f;
                [SerializeField] public float impedeChange; // prevent player from changing direction while in air

                [SerializeField] public RunType runType;
                [SerializeField] public bool run;
                [SerializeField] public bool smoothIntoRun;
                [SerializeField] public bool buttonIsLeftRight;
                [SerializeField] public float speedBoost = 1f;
                [SerializeField] public float tapTime;
                [SerializeField] public float lerpRunTime;
                [SerializeField] public float runThreshold;
                [SerializeField] public float runJumpBoost;
                [SerializeField] public string runButton;
                [SerializeField] public string groundHitWE;
                [SerializeField] public string notOnGroundWE;
                [SerializeField] public string walkingOnGroundWE;
                [SerializeField] public string directionChangedWE;
                [SerializeField] public UnityEventEffect onGroundHit;
                [SerializeField] public UnityEventEffect onNotOnGround;
                [SerializeField] public UnityEventEffect onWalkingOnGround;
                [SerializeField] public UnityEventEffect onDirectionChanged;

                [System.NonSerialized] public bool isRunning;
                [System.NonSerialized] private float inputX;
                [System.NonSerialized] public float runSmoothInVelocity;
                [System.NonSerialized] private float smoothIntoRunCounter;
                [System.NonSerialized] private float lerpStop;
                [System.NonSerialized] private float thresholdCounter;
                [System.NonSerialized] private float firstTapInputX;
                [System.NonSerialized] private float onGroundCounter;
                [System.NonSerialized] private bool stoppedRunning;

                [System.NonSerialized] public float externalVelX = 0;
                [System.NonSerialized] private bool toggleMode = false;
                [System.NonSerialized] private bool doubleTapMode = false;
                [System.NonSerialized] private bool switchTimeActive = false;

                [System.NonSerialized] private float tapTimer = 0;
                [System.NonSerialized] private float tapCounter = 0;
                [System.NonSerialized] private float switchCounter = 0;
                [System.NonSerialized] private float velocityX;

                public void Reset ( )
                {
                        lerpStop = 0;
                        velocityX = 0;
                        switchCounter = 0;
                        firstTapInputX = 0;
                        thresholdCounter = 0;
                        runSmoothInVelocity = 0;
                        smoothIntoRunCounter = 0;

                        isRunning = false;
                        toggleMode = false;
                        doubleTapMode = false;
                        stoppedRunning = false;
                        switchTimeActive = false;
                }

                public void Execute (AbilityManager player, WorldCollision world, ref Vector2 velocity)
                {
                        GetUserInput (player, velocity);
                        HorizontalMove (ref velocity);
                        PlayerRun (player, world, ref velocity);
                        ImpedeChange (player, ref velocity);

                        player.speed = speed;
                        player.inputX = inputX;
                        velocityX = velocity.x; // need to hold a reference to velocity x at start of frame, hover will use this
                        player.velocityX = velocity.x;
                        player.jumpBoost = isRunning && runJumpBoost > 0 ? runJumpBoost : 1f;
                        int previousDirection = player.playerDirection;

                        if (velocity.x != 0 && velocity.x < 0)
                        {
                                player.playerDirection = -1;
                        }
                        if (velocity.x != 0 && velocity.x > 0)
                        {
                                player.playerDirection = 1;
                        }
                        if (previousDirection != player.playerDirection)
                        {
                                onDirectionChanged.Invoke (ImpactPacket.impact.Set (directionChangedWE, world.transform, world.box.collider, player.world.position, null, -Vector2.right * player.playerDirection, 0));
                        }
                        if (!player.world.wasOnGround && player.world.onGround)
                        {
                                onGroundHit.Invoke (ImpactPacket.impact.Set (groundHitWE, world.transform, world.box.collider, player.world.position, null, world.box.up, 0));
                        }
                        if (player.world.wasOnGround && !player.world.onGround)
                        {
                                onNotOnGround.Invoke (ImpactPacket.impact.Set (notOnGroundWE, world.transform, world.box.collider, player.world.position, null, world.box.down, 0));
                        }
                        if (Clock.TimerExpired (ref onGroundCounter, 0.15f) && player.world.onGround && velocity.x != 0)
                        {
                                onWalkingOnGround.Invoke (ImpactPacket.impact.Set (walkingOnGroundWE, world.transform, world.box.collider, player.world.position, null, -Vector2.right * player.playerDirection, 0));
                                onGroundCounter = 0;
                        }
                }

                private void GetUserInput (AbilityManager player, Vector2 velocity)
                {
                        bool left = player.inputs.Holding ("Left");
                        bool right = player.inputs.Holding ("Right");

                        #region Debug
                        #if UNITY_EDITOR
                        if (WorldManager.viewDebugger)
                        {
                                if (useRightOnly) right = true;
                                if (useRightOnly) left = false;
                                if (useLeftOnly) left = true;
                                if (useLeftOnly) right = false;
                        }
                        #endif
                        #endregion

                        inputX = (left ? -1f : 0) + (right ? 1f : 0);

                }

                public void HorizontalMove (ref Vector2 velocity)
                {
                        if (smooth > 0 && smooth < 1f)
                        {
                                velocity.x = Compute.Lerp (velocityX, inputX * speed + externalVelX, smooth);
                                velocity.x = Mathf.Abs (velocity.x) < 0.2f ? 0 : velocity.x; // clamp or vel will continue to decrease without stopping
                        }
                        else
                        {
                                velocity.x = inputX * speed + externalVelX;
                        }
                        externalVelX = 0;
                }

                private void PlayerRun (AbilityManager player, WorldCollision world, ref Vector2 velocity)
                {
                        if (!run) return;

                        bool checkForRun = true;
                        if (RunInputReleased (player, world, velocity)) //stop running
                        {
                                RunDeactivate (world, velocity);
                                checkForRun = false;
                        }
                        if (stoppedRunning && smoothIntoRun && !isRunning)
                        {
                                RunToWalkSmoothIn (ref velocity);
                        }
                        if (checkForRun && RunInputActive (player, world)) // dont re-enter run on same frame as exit
                        {
                                ApplyRunVelocity (player, ref velocity);
                        }
                }

                private bool RunInputReleased (AbilityManager player, WorldCollision world, Vector2 velocity)
                {
                        if (!toggleMode && (velocity.x == 0 || world.onWall))
                        {
                                return true;
                        }
                        if (runType == RunType.Hold && !player.inputs.Holding (runButton))
                        {
                                return true;
                        }
                        if (doubleTapMode && !player.inputs.Holding (runButton))
                        {
                                return true;
                        }
                        if (toggleMode)
                        {
                                if (player.inputs.Pressed (runButton) || world.onWall)
                                {
                                        return true;
                                }
                                if (velocity.x == 0 && !switchTimeActive)
                                {
                                        switchTimeActive = true;
                                        switchCounter = 0;
                                }
                                if (switchTimeActive && Clock.Timer (ref switchCounter, 0.2f) && inputX == 0)
                                {
                                        return true;
                                }
                        }
                        return false;
                }

                private void RunDeactivate (WorldCollision world, Vector2 velocity)
                {
                        if (velocity.x != 0)
                        {
                                tapCounter = tapTimer = 0;
                        }
                        if (isRunning)
                        {
                                stoppedRunning = true;
                                lerpStop = 0;
                        }
                        if (world.leftWall || world.rightWall)
                        {
                                stoppedRunning = false; // let player continue at normal speed if hit wall, or it will feel stuck for a moment during smoothIn.
                                runSmoothInVelocity = 0;
                        }

                        isRunning = false;
                        toggleMode = false;
                        doubleTapMode = false;
                        switchTimeActive = false;
                        switchCounter = 0;
                        thresholdCounter = 0;
                        smoothIntoRunCounter = 0;
                }

                private void RunToWalkSmoothIn (ref Vector2 velocity)
                {
                        if (runSmoothInVelocity != 0)
                        {
                                velocity.x = Compute.Lerp (runSmoothInVelocity, inputX * speed, lerpRunTime * 0.75f, ref lerpStop);
                        }
                        if (lerpStop >= lerpRunTime * 0.75f || runSmoothInVelocity == 0 || velocity.x == 0)
                        {
                                stoppedRunning = false;
                        }
                }

                private bool RunInputActive (AbilityManager player, WorldCollision world)
                {
                        if (runType == RunType.Hold)
                        {
                                if ((player.ground || isRunning) && player.inputs.Holding (runButton))
                                {
                                        return true;
                                }
                        }
                        else if (runType == RunType.TimeThreshold)
                        {
                                if (!isRunning && !world.onGround)
                                {
                                        thresholdCounter = 0;
                                }
                                if (inputX != 0 && Clock.TimerExpired (ref thresholdCounter, runThreshold))
                                {
                                        return true;
                                }
                        }
                        else if (runType == RunType.Toggle)
                        {
                                if (toggleMode)
                                {
                                        return true;
                                }
                                if (player.ground && player.inputs.Pressed (runButton))
                                {
                                        switchCounter = 0;
                                        toggleMode = true;
                                        return true;
                                }
                        }
                        else
                        {
                                if (doubleTapMode)
                                {
                                        return true;
                                }

                                if (player.ground && player.inputs.Pressed (runButton))
                                {
                                        if (tapCounter == 0)
                                        {
                                                tapTimer = Time.time + tapTime;
                                                firstTapInputX = inputX;
                                                tapCounter = 1;
                                        }
                                        else if (Time.time <= tapTimer)
                                        {
                                                if (buttonIsLeftRight && firstTapInputX != inputX)
                                                {
                                                        tapCounter = firstTapInputX = 0;
                                                        doubleTapMode = false;
                                                }
                                                else
                                                {
                                                        tapCounter = 0;
                                                        doubleTapMode = true;
                                                        return true;
                                                }
                                        }
                                }

                                if (tapCounter == 1 && Time.time > tapTimer)
                                {
                                        tapCounter = firstTapInputX = 0;
                                        doubleTapMode = false;
                                }
                        }
                        return false;
                }

                private void ApplyRunVelocity (AbilityManager player, ref Vector2 velocity)
                {
                        if (smoothIntoRun)
                        {
                                velocity.x = runSmoothInVelocity = Compute.Lerp (velocity.x, inputX * speed * speedBoost, lerpRunTime, ref smoothIntoRunCounter);
                        }
                        else
                        {
                                velocity.x = inputX * speed * speedBoost;
                        }
                        isRunning = true;
                        player.signals.Set ("running");
                }

                private void ImpedeChange (AbilityManager player, ref Vector2 velocity)
                {
                        if (player.ground)
                        {
                                player.velocityOnGround = velocity.x;
                        }
                        if (impedeChange > 0 && !player.ground)
                        {
                                if (player.airMomentumActive && inputX == 0)
                                {
                                        return;
                                }
                                player.velocityOnGround = Mathf.MoveTowards (player.velocityOnGround, velocity.x, Time.deltaTime * (5f - impedeChange) * 10f); // 5 is the max value for airResistance, invert value
                                float scale = run ? speedBoost : 1f;
                                velocity.x = Mathf.Clamp (player.velocityOnGround, -speed * scale, speed * scale);
                        }
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] private bool runFoldOut;
                [SerializeField, HideInInspector] private bool useRightOnly;
                [SerializeField, HideInInspector] private bool useLeftOnly;
                [SerializeField, HideInInspector] private bool foldOut;
                [SerializeField, HideInInspector] private bool eventFoldOut;
                [SerializeField, HideInInspector] private bool groundFoldOut;
                [SerializeField, HideInInspector] private bool notGroundFoldOut;
                [SerializeField, HideInInspector] private bool directionFoldOut;
                [SerializeField, HideInInspector] private bool walkingOnGroundFoldOut;
                [SerializeField, HideInInspector] private bool editTransitions;

                public static void OnInspector (SerializedProperty parent, string[] inputList, Color barColor, Color labelColor)
                {
                        if (FoldOut.Bar (parent, barColor).Label ("Walk", labelColor).FoldOut ( ))
                        {
                                FoldOut.Box (3, FoldOut.boxColorLight, extraHeight : 5, yOffset: -2);
                                {
                                        parent.Field ("Speed", "speed");
                                        parent.Slider ("Smooth", "smooth", min : 0.95f);
                                        parent.Slider ("Damp Air Change", "impedeChange", 0, 5f);
                                }

                                if (FoldOut.FoldOutButton (parent.Get ("eventFoldOut")))
                                {
                                        Fields.EventFoldOutEffect (parent.Get ("onGroundHit"), parent.Get ("groundHitWE"), parent.Get ("groundFoldOut"), "Ground Hit", color : FoldOut.boxColorLight);
                                        Fields.EventFoldOutEffect (parent.Get ("onNotOnGround"), parent.Get ("notOnGroundWE"), parent.Get ("notGroundFoldOut"), "Not On Ground", color : FoldOut.boxColorLight);
                                        Fields.EventFoldOutEffect (parent.Get ("onWalkingOnGround"), parent.Get ("walkingOnGroundWE"), parent.Get ("walkingOnGroundFoldOut"), "Walking On Ground", color : FoldOut.boxColorLight);
                                        Fields.EventFoldOutEffect (parent.Get ("onDirectionChanged"), parent.Get ("directionChangedWE"), parent.Get ("directionFoldOut"), "Direction Changed", color : FoldOut.boxColorLight);
                                }

                                if (FoldOut.Bar (parent, FoldOut.boxColorLight).Label ("Run", FoldOut.titleColor, false).BRE ("run").FoldOut ("runFoldOut"))
                                {
                                        int runType = parent.Enum ("runType");
                                        int height = runType == 2 ? 2 : 0;

                                        FoldOut.Box (4 + height, FoldOut.boxColorLight, yOffset: -2);
                                        {
                                                GUI.enabled = parent.Bool ("run");
                                                {
                                                        parent.FieldAndDropDownList (inputList, "Type", "runType", "runButton", execute : runType != 3);
                                                        parent.FieldDouble ("Type", "runType", "runThreshold", execute : runType == 3);
                                                        parent.Field ("Speed Boost", "speedBoost");
                                                        parent.Field ("Jump Boost", "runJumpBoost");
                                                        parent.FieldAndEnable ("Ease into Run", "lerpRunTime", "smoothIntoRun");
                                                        Labels.FieldText ("Ease Time", rightSpacing : Layout.boolWidth + 4);
                                                        parent.Field ("Tap Threshold", "tapTime", execute : runType == 2);
                                                        parent.FieldToggle ("Button Is Left,Right", "buttonIsLeftRight", execute : runType == 2);
                                                }
                                                GUI.enabled = true;
                                        }
                                        Layout.VerticalSpacing (3);
                                }

                                #region Debug
                                #if UNITY_EDITOR
                                if (WorldManager.viewDebugger)
                                {
                                        FoldOut.Box (2, FoldOut.boxColorLight);
                                        {
                                                parent.FieldToggle ("Editor Only, Right", "useRightOnly");
                                                parent.FieldToggle ("Editor Only, Left", "useLeftOnly");
                                        }
                                        Layout.VerticalSpacing (5);
                                }
                                #endif
                                #endregion
                        }
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }

        public enum RunType
        {
                Hold,
                Toggle,
                DoubleTap,
                TimeThreshold,
        }
}