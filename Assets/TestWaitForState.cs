using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWaitForState : MonoBehaviour
{

    public void DoTheTest()
    {
        Debug.LogWarning("Do The Test was called.");
        Playroom.PlayroomKit.WaitForState("movedPieceStartXPos", FoundState);
        Playroom.PlayroomKit.WaitForState("testKey", FoundStringTest);
    }

    void FoundState()
    {
        Debug.LogWarning("The C# script found the 'movedPieceStartXPos' state.");
        Debug.LogWarning(Playroom.PlayroomKit.GetState<float>("movedPieceStartXPos"));
    }

    void FoundStringTest()
    {
        Debug.LogWarning("The C# script found the 'testKey' state: " + Playroom.PlayroomKit.GetState<string>("testKey"));
    }
}
