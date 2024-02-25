using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class GetVector3Distance : FsmStateAction
{
    public FsmVector3 point1;
    public FsmVector3 point2;

    [UIHint(UIHint.Variable)]
    public FsmFloat distance;

    public FsmBool everyFrame;

    public override void OnEnter()
    {
        distance.Value = Vector3.Distance(point1.Value, point2.Value);
        if (!everyFrame.Value)
        {
            Finish();
        }
        
    }

    public override void OnUpdate()
    {
        distance.Value = Vector3.Distance(point1.Value, point2.Value);
    }
}
