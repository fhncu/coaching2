using System.Collections.Generic;

namespace TwoBitMachines.TwoBitSprite
{
        [System.Serializable]
        public class StateSignals
        {
                public SignalPacket[] all = new SignalPacket[]
                {
                        new SignalPacket ("velX"),
                        new SignalPacket ("velXLeft"),
                        new SignalPacket ("velXRight"),
                        new SignalPacket ("velXZero"),
                        new SignalPacket ("velY"),
                        new SignalPacket ("velYUp"),
                        new SignalPacket ("velYDown"),
                        new SignalPacket ("velYZero"),

                        new SignalPacket ("onGround"),
                        new SignalPacket ("jumping"),
                        new SignalPacket ("airJump"),
                        new SignalPacket ("airGlide"),
                        new SignalPacket ("highJump"),
                        new SignalPacket ("windJump"),
                        new SignalPacket ("running"),
                        new SignalPacket ("hover"),
                        new SignalPacket ("staticFlying"),
                        new SignalPacket ("ladderClimb"),
                        new SignalPacket ("ceilingClimb"),
                        new SignalPacket ("crouch"),
                        new SignalPacket ("crouchSlide"),
                        new SignalPacket ("crouchWalk"),

                        new SignalPacket ("friction"),
                        new SignalPacket ("sliding"),
                        new SignalPacket ("autoGround"),

                        new SignalPacket ("dashing"),
                        new SignalPacket ("dashX"),
                        new SignalPacket ("dashY"),
                        new SignalPacket ("dashDiagonal"),

                        new SignalPacket ("pushBack"),
                        new SignalPacket ("pushBackLeft"),
                        new SignalPacket ("pushBackRight"),

                        new SignalPacket ("pushBlock"),
                        new SignalPacket ("pullBlock"),
                        new SignalPacket ("pickAndThrowBlock"),
                        new SignalPacket ("pickingUpBlock"),
                        new SignalPacket ("holdingBlock"),
                        new SignalPacket ("throwingBlock"),

                        new SignalPacket ("cannonBlast"),

                        new SignalPacket ("rope"),
                        new SignalPacket ("ropeClimbing"),
                        new SignalPacket ("ropeHanging"),
                        new SignalPacket ("ropeHolding"),
                        new SignalPacket ("ropeSwinging"),

                        new SignalPacket ("inWater"),
                        new SignalPacket ("swimming"),
                        new SignalPacket ("floating"),

                        new SignalPacket ("wall"),
                        new SignalPacket ("wallLeft"),
                        new SignalPacket ("wallRight"),
                        new SignalPacket ("wallClimb"),
                        new SignalPacket ("wallHold"),
                        new SignalPacket ("wallSlide"),
                        new SignalPacket ("wallHang"),
                        new SignalPacket ("wallSlideJump"),
                        new SignalPacket ("wallCornerGrab"),
                        new SignalPacket ("autoCornerJump"),

                        new SignalPacket ("slopeSlide"),
                        new SignalPacket ("slopeSlideAuto"),

                        new SignalPacket ("recoil"),
                        new SignalPacket ("recoilLeft"),
                        new SignalPacket ("recoilRight"),
                        new SignalPacket ("recoilUp"),
                        new SignalPacket ("recoilDown"),
                        new SignalPacket ("recoilShake"),
                        new SignalPacket ("recoilSlide"),

                        new SignalPacket ("mouseDirectionLeft"),
                        new SignalPacket ("mouseDirectionRight"),

                        new SignalPacket ("meleeCombo"),
                        new SignalPacket ("meleeLeft"),
                        new SignalPacket ("meleeRight"),

                        new SignalPacket ("zipline"),
                        new SignalPacket ("vehicleMounted"),
                        new SignalPacket ("onVehicle"),
                        new SignalPacket ("sameDirection"),
                        new SignalPacket ("changedDirection"),
                        new SignalPacket ("alwaysTrue"),
                        new SignalPacket ("alwaysFalse")
                };

                public List<SignalPacket> extra = new List<SignalPacket> ( );

                public bool foldOutVelocity;
                public bool foldOutWall;
                public bool foldOutWorld;
                public bool foldOutRecoil;
                public bool foldOutAttack;
                public bool foldOutUtility;
                public bool foldOutExtra;
                public bool button;
                public string createSignal;
        }

        [System.Serializable]
        public class SignalPacket
        {
                public string name;
                public bool use = false;

                public SignalPacket (string name)
                {
                        this.name = name;
                }
        }

}