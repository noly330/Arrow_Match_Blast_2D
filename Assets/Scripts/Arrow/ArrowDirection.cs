using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrowDirection
{
    public static int GetRotationByDirection(char direction)
    {
        switch (direction)
        {
            case 'w':
                return 0;
            case 's':
                return 180;
            case 'a':
                return 90;
            case 'd':
                return -90;
            default:
                Debug.LogError($"无效方向: {direction}");
                return 0;
        }
    }

    public static Vector3 GetDirectionVector(char direction)
    {
        switch (direction)
        {
            case 'w':
                return new Vector3(0,1,0);
            case 's':
                return new Vector3(0,-1,0);
            case 'a':
                return new Vector3(-1,0,0);
            case 'd':
                return new Vector3(1,0,0);
            default:
                Debug.LogError("Invalid direction");
                return Vector2.zero;
        }
    }


}
