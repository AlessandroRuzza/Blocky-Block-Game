using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtils
{
    static float halfScreenHeight, halfScreenWidth;
    public static float halfWidth { get{ return halfScreenWidth; }}
    public static float halfHeight { get{ return halfScreenHeight; }}

    public static void SetScreenDimension(){
        halfScreenHeight = Camera.main.orthographicSize;
        halfScreenWidth = Camera.main.orthographicSize * Camera.main.aspect;
    }
}
