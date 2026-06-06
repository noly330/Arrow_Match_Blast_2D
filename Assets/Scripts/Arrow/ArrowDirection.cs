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
}
