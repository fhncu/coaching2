using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
      [System.Serializable]
      public class PathNode
      {
            [SerializeField] public int gridX;
            [SerializeField] public int gridY;
            [SerializeField] public int height;
            [SerializeField] public Vector2 position;

            [SerializeField] public bool air;
            [SerializeField] public bool wall;
            [SerializeField] public bool exact;
            [SerializeField] public bool block;
            [SerializeField] public bool ladder;
            [SerializeField] public bool moving;
            [SerializeField] public bool ground;
            [SerializeField] public bool bridge;
            [SerializeField] public bool isFall;
            [SerializeField] public bool ceiling;
            [SerializeField] public bool bigDrop;
            [SerializeField] public bool isOccupied;
            [SerializeField] public bool leftCorner;
            [SerializeField] public bool rightCorner;
            [SerializeField] public bool edgeOfCorner;
            [SerializeField] public bool jumpThroughGround;

            public bool path => ground || jumpThroughGround || ladder || wall || moving || ceiling || bridge;
            public bool onGround => ground || jumpThroughGround;
            public bool uniCorner => leftCorner && rightCorner;
            [System.NonSerialized] public TargetPathfinding unit;

            public bool Same (PathNode other)
            {
                  if (other == null)
                        return false;
                  return gridX == other.gridX && gridY == other.gridY;
            }

            public bool Same (Vector2 coordinates)
            {
                  return gridX == coordinates.x && gridY == coordinates.y;
            }

            public bool SameY (PathNode other)
            {
                  return gridY == other.gridY;
            }

            public bool SameX (PathNode other)
            {
                  return gridX == other.gridX;
            }

            public bool Above (PathNode other)
            {
                  return gridY > other.gridY;
            }

            public bool Below (PathNode other)
            {
                  return gridY < other.gridY;
            }

            public int DirectionX (PathNode other)
            {
                  return (int) Mathf.Sign (gridX - other.gridX);
            }

            public float SqrMagnitude (PathNode other)
            {
                  return (Coorddinates ( ) - other.Coorddinates ( )).sqrMagnitude;
            }

            public Vector2 Coorddinates ( )
            {
                  return new Vector2 (gridX, gridY);
            }

            public float DistanceX (Vector2 position)
            {
                  return Mathf.Abs (this.position.x - position.x);
            }

            public float DistanceX (PathNode node)
            {
                  return Mathf.Abs (this.position.x - node.position.x);
            }

            public bool NextToX (PathNode node)
            {
                  return gridY == node.gridY && Mathf.Abs (gridX - node.gridX) <= 1.1f;
            }

            public bool NextToY (PathNode node)
            {
                  return gridX == node.gridX && Mathf.Abs (gridY - node.gridY) <= 1.1f;
            }

            public bool NextToGridX (PathNode node)
            {
                  return Mathf.Abs (gridX - node.gridX) < 1.1f;
            }

            public bool DistanceXOne (PathNode node)
            {
                  float distance = Mathf.Abs (gridX - node.gridX);
                  return distance > 0 && distance < 1.1f;
            }

            public float DistanceY (Vector2 position)
            {
                  return Mathf.Abs (this.position.y - position.y);
            }

            public float DistanceY (PathNode node)
            {
                  return Mathf.Abs (this.position.y - node.position.y);
            }

            public PathNode Shift (Pathfinding map, int shiftX, int shiftY)
            {
                  return map.Node (gridX + shiftX, gridY + shiftY);
            }

            public PathNode ShiftX (Pathfinding map, int shiftX)
            {
                  return map.Node (gridX + shiftX, gridY);
            }

            public PathNode ShiftX (Pathfinding map, PathNode directionNode)
            {
                  return map.Node (gridX + DirectionX (directionNode), gridY);
            }

            public PathNode ShiftY (Pathfinding map, int shiftY)
            {
                  return map.Node (gridX, gridY + shiftY);
            }
      }

      public struct PathNodeStruct
      {
            public int index;
            public int gridX;
            public int gridY;
            public int height;
            public bool air;
            public bool wall;
            public bool block;
            public bool exact;
            public bool ladder;
            public bool moving;
            public bool ground;
            public bool bridge;
            public bool ceiling;
            public bool bigDrop;
            public bool edgeDrop;
            public bool leftCorner;
            public bool rightCorner;
            public bool jumpThroughGround;

            public bool Path ( ) { return ground || jumpThroughGround || ladder || wall || moving || ceiling || bridge; }

      }

}