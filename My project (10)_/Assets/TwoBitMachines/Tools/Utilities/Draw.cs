using UnityEngine;

namespace TwoBitMachines
{
        public static class Draw
        {
                public static void BasicSquare (Vector2 point, float size, Color color)
                {
                        Vector2 lT = new Vector2 (-1, 1);
                        Vector2 rT = new Vector2 (1, 1);
                        Vector2 lB = new Vector2 (-1, -1);
                        Vector2 rB = new Vector2 (1, -1);

                        UnityEngine.Debug.DrawLine (point + lT * size, point + rT * size, color);
                        UnityEngine.Debug.DrawLine (point + rT * size, point + rB * size, color);
                        UnityEngine.Debug.DrawLine (point + rB * size, point + lB * size, color);
                        UnityEngine.Debug.DrawLine (point + lB * size, point + lT * size, color);
                }

                public static void Circle (Vector2 center, float radius, float precision = 10f)
                {

                        Vector2 centerBottom = center + Vector2.down * radius;
                        float angle = 45f / precision;
                        float c = 2f * Mathf.PI * radius;
                        float quadrant = c / 4f;
                        float segment = quadrant / precision;
                        Vector2 startPoint = centerBottom;
                        for (int i = 0; i < precision * 4f; i++)
                        {
                                float curve = 1f + (i * 2f);
                                Vector2 direction = new Vector2 (Mathf.Cos (Mathf.Deg2Rad * angle * curve) * segment, Mathf.Sin (Mathf.Deg2Rad * angle * curve) * segment);
                                Debug.DrawLine (startPoint, startPoint + direction, Color.green);
                                startPoint += direction;
                        }
                        Debug.DrawLine (center, centerBottom, Color.green);

                }

                public static void CircleSector (Vector2 center, Vector2 direction, float radius, float angleP, float angleN, float sign, float precision = 10)
                {
                        Vector2 right = direction * radius;

                        Vector2 positiveDirection = Compute.RotateVector (right, angleP);
                        Vector2 negativeDirection = Compute.RotateVector (right, angleN);

                        if (sign < 0)
                        {
                                negativeDirection.x *= -1f;
                                positiveDirection.x *= -1f;
                        }

                        float totalAngle = Vector2.Angle (positiveDirection, negativeDirection);
                        float intervalAngle = Mathf.Abs (totalAngle) / precision;
                        Vector2 startDirection = negativeDirection;

                        for (int i = 0; i < precision; i++)
                        {
                                Vector2 currentEndPoint = center + startDirection;
                                startDirection = Compute.RotateVector (startDirection, intervalAngle * sign);
                                Debug.DrawLine (currentEndPoint, center + startDirection, Color.green);
                        }
                        Debug.DrawLine (center, center + negativeDirection, Color.green);
                        Debug.DrawLine (center, center + positiveDirection, Color.green);

                }

                public static void Circle (Vector2 center, float radius, Color color, float precision = 5)
                {
                        Vector2 centerBottom = center + Vector2.down * radius;
                        float angle = 45f / precision;
                        float c = 2f * Mathf.PI * radius;
                        float quadrant = c / 4f;
                        float segment = quadrant / precision;
                        Vector2 startPoint = centerBottom;
                        for (int i = 0; i < precision * 4f; i++)
                        {
                                float curve = 1f + (i * 2f);
                                Vector2 direction = new Vector2 (Mathf.Cos (Mathf.Deg2Rad * angle * curve) * segment, Mathf.Sin (Mathf.Deg2Rad * angle * curve) * segment);
                                Debug.DrawLine (startPoint, startPoint + direction, color);
                                startPoint += direction;
                        }
                        //Debug.DrawLine (center, centerBottom, color);

                }

                public static void Cross (Vector2 point, Vector2 size, Color color, float angle = 0)
                {
                        Vector2 up = Compute.RotateVector (new Vector2 (0, 1), angle);
                        Vector2 right = Compute.RotateVector (new Vector2 (1, 0), angle);
                        Vector2 left = Compute.RotateVector (new Vector2 (-1, 0), angle);
                        Vector2 down = Compute.RotateVector (new Vector2 (0, -1), angle);

                        UnityEngine.Debug.DrawLine (point, point + right * size.x, color);
                        UnityEngine.Debug.DrawLine (point, point + left * size.x, color);
                        UnityEngine.Debug.DrawLine (point, point + down * size.y, color);
                        UnityEngine.Debug.DrawLine (point, point + up * size.y, color);
                }

                public static void DrawRay (Vector2 point, Vector2 direction, Color color, float angle = 0)
                {
                        direction = Compute.RotateVector (direction, angle);
                        UnityEngine.Debug.DrawRay (point, direction, color);
                }

                public static void Grid2D (Vector2 position, Vector2 gridSize, Vector2 cellSize, Color color)
                {

                        if (cellSize.x == 0 || cellSize.y == 0) return;

                        float linesX = gridSize.x / cellSize.x + 1f;
                        float linesY = gridSize.y / cellSize.y + 1f;

                        for (int i = 0; i < linesX; i++)
                        {
                                Vector2 p = position + Vector2.right * cellSize.x * i;
                                Debug.DrawLine (p, p + Vector2.up * gridSize.y, color);
                        }

                        for (int i = 0; i < linesY; i++)
                        {
                                Vector2 p = position + Vector2.up * cellSize.y * i;
                                Debug.DrawLine (p, p + Vector2.right * gridSize.x, color);

                        }
                }

                public static void Square (Vector2 center, float size, Color color, float angle = 0)
                {

                        Vector2 lT = Compute.RotateVector (new Vector2 (-1, 1), angle);
                        Vector2 rT = Compute.RotateVector (new Vector2 (1, 1), angle);
                        Vector2 lB = Compute.RotateVector (new Vector2 (-1, -1), angle);
                        Vector2 rB = Compute.RotateVector (new Vector2 (1, -1), angle);

                        UnityEngine.Debug.DrawLine (center + lT * size, center + rT * size, color);
                        UnityEngine.Debug.DrawLine (center + rT * size, center + rB * size, color);
                        UnityEngine.Debug.DrawLine (center + rB * size, center + lB * size, color);
                        UnityEngine.Debug.DrawLine (center + lB * size, center + lT * size, color);
                }

                public static void SquareCenter (Vector2 center, Vector2 size, Color color)
                {
                        Vector2 bL = center - size * 0.5f;
                        Vector2 tL = bL + Vector2.up * size.y;
                        Vector2 tR = tL + Vector2.right * size.x;
                        Vector2 bR = tR + Vector2.down * size.y;

                        Debug.DrawLine (bL, tL, color);
                        Debug.DrawLine (tL, tR, color);
                        Debug.DrawLine (tR, bR, color);
                        Debug.DrawLine (bL, bR, color);
                }

                public static void Square (Vector2 bottomLeft, Vector2 size, Color color)
                {
                        Vector2 bL = bottomLeft;
                        Vector2 tL = bL + Vector2.up * size.y;
                        Vector2 tR = tL + Vector2.right * size.x;
                        Vector2 bR = tR + Vector2.down * size.y;

                        Debug.DrawLine (bL, tL, color);
                        Debug.DrawLine (tL, tR, color);
                        Debug.DrawLine (tR, bR, color);
                        Debug.DrawLine (bL, bR, color);

                }

                public static void Rectangle (Vector2 bL, Vector2 bR, Vector2 tR, Vector2 tL, Color color)
                {
                        Debug.DrawLine (bL, tL, color);
                        Debug.DrawLine (tL, tR, color);
                        Debug.DrawLine (tR, bR, color);
                        Debug.DrawLine (bL, bR, color);

                }

                public static void Square (Rect rect, Color color)
                {

                        Vector2 p = rect.position;
                        Vector2 r = Vector2.right * rect.width;
                        Vector2 u = Vector2.up * rect.height;

                        UnityEngine.Debug.DrawLine (p, p + u, color);
                        UnityEngine.Debug.DrawLine (p + u, p + u + r, color);
                        UnityEngine.Debug.DrawLine (p + u + r, p + r, color);
                        UnityEngine.Debug.DrawLine (p + r, p, color);
                }

        }

}