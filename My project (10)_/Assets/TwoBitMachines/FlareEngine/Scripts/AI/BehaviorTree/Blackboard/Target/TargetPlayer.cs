﻿using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [AddComponentMenu ("")]
        public class TargetPlayer : Blackboard
        {
                [SerializeField] public PlayerFindType type;
                [SerializeField] public Vector2 offset;

                public override Vector2 GetTarget ( )
                {
                        Vector2 position = transform.position;
                        if (type == PlayerFindType.IsSinglePlayer)
                        {
                                return ThePlayer.Player.PlayerPosition (position) + offset;;
                        }
                        else if (type == PlayerFindType.FindNearestPlayer)
                        {
                                return NearestPlayerPosition (position);
                        }
                        else
                        {
                                return RandomPlayerPosition (position);
                        }
                }

                public Vector2 NearestPlayerPosition (Vector2 returnPosition)
                {
                        List<ThePlayer.Player> players = ThePlayer.Player.players;

                        if (players.Count > 0)
                        {
                                float distance = float.MaxValue;
                                Vector2 position = returnPosition;
                                for (int i = 0; i < players.Count; i++)
                                {
                                        if (players[i] == null) continue;
                                        float sqrMag = (returnPosition - (Vector2) players[i].transform.position).sqrMagnitude;
                                        if (sqrMag < distance)
                                        {
                                                distance = sqrMag;
                                                position = players[i].transform.position;
                                        }
                                }
                                return position + offset;
                        }
                        return returnPosition;
                }

                public Vector2 RandomPlayerPosition (Vector2 returnPosition)
                {
                        List<ThePlayer.Player> players = ThePlayer.Player.players;

                        if (players.Count > 0)
                        {
                                if (index < 0 || index >= players.Count)
                                {
                                        index = UnityEngine.Random.Range (0, players.Count);
                                }
                                if (index >= 0 && index < players.Count)
                                {
                                        return players[index] != null ? (Vector2) players[index].transform.position + offset : returnPosition;
                                }
                        }
                        return returnPosition;
                }

                public override Transform GetTransform ( )
                {
                        if (type == PlayerFindType.IsSinglePlayer)
                        {
                                return ThePlayer.Player.PlayerTransform ( );
                        }
                        else if (type == PlayerFindType.FindNearestPlayer)
                        {
                                return NearestPlayerTransform (transform.position);
                        }
                        else
                        {
                                return RandomPlayerTransform ( );
                        }
                }

                private Transform GetPlayerTransform ( )
                {
                        if (type == PlayerFindType.IsSinglePlayer)
                        {
                                return ThePlayer.Player.PlayerTransform ( );
                        }
                        else if (type == PlayerFindType.FindNearestPlayer)
                        {
                                return NearestPlayerTransform (transform.position);
                        }
                        else
                        {
                                return RandomPlayerTransform ( );
                        }
                }

                private Transform NearestPlayerTransform (Vector2 position)
                {
                        List<ThePlayer.Player> players = ThePlayer.Player.players;

                        if (players.Count > 0)
                        {
                                float distance = float.MaxValue;
                                Transform newTransform = null;
                                for (int i = 0; i < players.Count; i++)
                                {
                                        if (players[i] == null) continue;
                                        float sqrMag = (position - (Vector2) players[i].transform.position).sqrMagnitude;
                                        if (sqrMag < distance)
                                        {
                                                distance = sqrMag;
                                                newTransform = players[i].transform;
                                        }
                                }
                                return newTransform;
                        }
                        return null;
                }

                private Transform RandomPlayerTransform ( )
                {
                        List<ThePlayer.Player> players = ThePlayer.Player.players;

                        if (players.Count > 0)
                        {
                                if (index < 0 || index >= players.Count)
                                {
                                        index = UnityEngine.Random.Range (0, players.Count);
                                }
                                if (index >= 0 && index < players.Count)
                                {
                                        return players[index] != null ? players[index].transform : null;
                                }
                        }
                        return null;
                }

                public override void ResetIndex ( )
                {
                        index = -1;
                }

                public override void Set (Vector3 vector3)
                {
                        Transform playerTransform = GetPlayerTransform ( );
                        if (playerTransform != null) playerTransform.position = vector3;
                }

                public override void Set (Vector2 vector2)
                {
                        Transform playerTransform = GetPlayerTransform ( );
                        if (playerTransform != null) playerTransform.position = vector2;
                }
        }

        public enum PlayerFindType
        {
                IsSinglePlayer,
                FindRandomPlayer,
                FindNearestPlayer
        }

}