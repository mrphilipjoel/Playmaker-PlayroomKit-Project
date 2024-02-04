﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LayoutStyle
{
    Linear,
    Grid,
    Euclidean,
    Radial
}

public enum LayoutAxis3D
{
    X,
    Y,
    Z
}

public enum LayoutAxis2D
{
    X,
    Y
}

public enum Alignment
{
    Min,
    Center,
    Max
}

[ExecuteAlways]
public class LayoutGroup3D : MonoBehaviour
{
    [HideInInspector]
    public List<Transform> LayoutElements;
    [HideInInspector]
    public Vector3 ElementDimensions = Vector3.one;

    [HideInInspector]
    public float Spacing;
    [HideInInspector]
    public LayoutStyle Style;

    [HideInInspector]
    public int GridConstraintCount;
    [HideInInspector]
    public int SecondaryConstraintCount;

    [HideInInspector]
    public bool UseFullCircle;
    [HideInInspector]
    public float MaxArcAngle;
    [HideInInspector]
    public float Radius = 1f;
    [HideInInspector]
    public float StartAngleOffset;
    [HideInInspector]
    public bool AlignToRadius;
    [HideInInspector]
    public bool RadialSymmetricalAboutStart;
    [HideInInspector]
    public LayoutAxis3D PrimaryAlignmentAxis = LayoutAxis3D.Z;
    [HideInInspector]
    public LayoutAxis3D SecondaryAlignmentAxis = LayoutAxis3D.Y;

    [HideInInspector]
    public LayoutAxis3D LayoutAxis = LayoutAxis3D.X;
    [HideInInspector]
    public LayoutAxis3D SecondaryLayoutAxis = LayoutAxis3D.Y;
    [HideInInspector]
    public List<Quaternion> ElementRotations;
    [HideInInspector]
    public Vector3 StartPositionOffset;
    [HideInInspector]
    public float SpiralFactor;
    [HideInInspector]
    public Alignment PrimaryAlignment;
    [HideInInspector]
    public Alignment SecondaryAlignment;
    [HideInInspector]
    public Alignment TertiaryAlignment;

    [HideInInspector]
    public bool NeedsRebuild = false;

    [Tooltip("Forces the layoutgroup to rebuild every frame while in Play mode.  WARNING: This may have performance implications for very large / complex layouts, use with caution")]
    public bool ForceContinuousRebuild = false;
    [Tooltip("If true, child gameobjects that are not activeSelf will be ignored for the layout.  Otherwise, inactive child gameobjects will be included in the layout.")]
    public bool IgnoreDeactivatedElements = true;

    [HideInInspector]
    public bool DrawGizmos;
    [HideInInspector]
    public float GizmoScale = 1f;
    [HideInInspector]
    public Color GizmoPrimaryColor = Color.white;
    [HideInInspector]
    public Color GizmoSecondaryColor = Color.green;
    [HideInInspector]
    public Color GizmoTertiaryColor = Color.red;

    [HideInInspector]
    public bool ShowGizmoSettings;

    protected Dictionary<Transform, LayoutElement3D> LayoutElementsByTransform;

    public void RebuildLayout()
    {
        PopulateElementsFromChildren();

        // If we have no layout elements
        if(LayoutElements == null || LayoutElements.Count == 0)
        {
            NeedsRebuild = false;
            return;
        }

        switch(Style)
        {
            case LayoutStyle.Linear:
                LinearLayout();
                break;

            case LayoutStyle.Grid:
                GridLayout();
                break;

            case LayoutStyle.Euclidean:
                EuclideanLayout();
                break;

            case LayoutStyle.Radial:
                RadialLayout();
                break;
        }

        NeedsRebuild = false;
    }

    public void LinearLayout()
    {
        float totalLength = 0f;
        // nextPos represents the furthest edge of the last element in the LayoutAxis
        Vector3 nextPos = StartPositionOffset;

        for(int i = 0; i < LayoutElements.Count; i++)
        {
            Vector3 dimensions = GetDimensionsForElement(LayoutElements[i]);
            // The element's position should be half its dimension in the LayoutAxis, plus spacing
            Vector3 elemPos = Vector3.zero;
            Vector3 nextPosOffset = Vector3.zero;
            switch (LayoutAxis)
            {
                case LayoutAxis3D.X:
                    // If this isnt the first element in the layout
                    if(i != 0)
                    {
                        elemPos.x = Spacing + dimensions.x / 2f;
                        totalLength += Spacing;
                    }
                    
                    nextPosOffset.x = dimensions.x / 2f;
                    totalLength += dimensions.x;
                    break;
                case LayoutAxis3D.Y:
                    // If this isnt the first element in the layout
                    if (i != 0)
                    {
                        elemPos.y = Spacing + dimensions.y / 2f;
                        totalLength += Spacing;
                    }
                        
                    nextPosOffset.y = dimensions.y / 2f;
                    totalLength += dimensions.y;
                    break;
                case LayoutAxis3D.Z:
                    // If this isnt the first element in the layout
                    if (i != 0)
                    {
                        elemPos.z = Spacing + dimensions.z / 2f;
                        totalLength += Spacing;
                    }
                        
                    nextPosOffset.z = dimensions.z / 2f;
                    totalLength += dimensions.z;
                    break;
            }
            // Set elements local position 
            LayoutElements[i].localPosition = nextPos + elemPos;
            // Shift nextPos by half of the element's dimension in the LayoutAxis
            nextPos = nextPos + elemPos + nextPosOffset;
        }

        // Get vector pointing from first element to last element
        Vector3 alignmentOffset = LayoutElements[LayoutElements.Count - 1].localPosition - LayoutElements[0].localPosition;

        // Calculate alignment offset amount
        switch (PrimaryAlignment)
        {
            case Alignment.Min:
                alignmentOffset = Vector3.zero;
                break;
            case Alignment.Center:
                alignmentOffset = -(alignmentOffset / 2f);
                break;
            case Alignment.Max:
                alignmentOffset = -alignmentOffset;
                break;
        }

        // Apply alignment offset
        for (int i = 0; i < LayoutElements.Count; i++)
        {
            LayoutElements[i].localPosition = LayoutElements[i].localPosition + alignmentOffset;
        }
    }

    public void GridLayout()
    {
        Vector3 pos = Vector3.zero;

        int primaryCount = GridConstraintCount;
        int secondaryCount = Mathf.CeilToInt((float)LayoutElements.Count / primaryCount);

        float pDim = ElementDimensions.x;
        float sDim = ElementDimensions.y;

        switch (LayoutAxis)
        {
            case LayoutAxis3D.X:
                pDim = ElementDimensions.x;
                break;
            case LayoutAxis3D.Y:
                pDim = ElementDimensions.y;
                break;
            case LayoutAxis3D.Z:
                pDim = ElementDimensions.z;
                break;
        }

        switch (SecondaryLayoutAxis)
        {
            case LayoutAxis3D.X:
                sDim = ElementDimensions.x;
                break;
            case LayoutAxis3D.Y:
                sDim = ElementDimensions.y;
                break;
            case LayoutAxis3D.Z:
                sDim = ElementDimensions.z;
                break;
        }

        // Calculate primary alignment offset
        Vector3 alignmentOffset = Vector3.zero;
        switch (PrimaryAlignment)
        {
            case Alignment.Min:
                break;
            case Alignment.Center:
                alignmentOffset -= GetVectorForAxis(LayoutAxis) * (GridConstraintCount - 1) * (pDim + Spacing) / 2f;
                break;
            case Alignment.Max:
                alignmentOffset -= GetVectorForAxis(LayoutAxis) * (GridConstraintCount - 1) * (pDim + Spacing);
                break;
        }

        // Calculate secondary alignment offset
        switch (SecondaryAlignment)
        {
            case Alignment.Min:
                break;
            case Alignment.Center:
                alignmentOffset -= GetVectorForAxis(SecondaryLayoutAxis) * (secondaryCount - 1) * (sDim + Spacing) / 2f;
                break;
            case Alignment.Max:
                alignmentOffset -= GetVectorForAxis(SecondaryLayoutAxis) * (secondaryCount - 1) * (sDim + Spacing);
                break;
        }

        int i = 0;

        for(int s = 0; s < secondaryCount; s++)
        {
            for(int p = 0; p < primaryCount; p++)
            {
                if(i < LayoutElements.Count)
                {
                    float pOffset = (float)p * (pDim + Spacing);
                    float sOffset = (float)s * (sDim + Spacing);

                    pos = GetVectorForAxis(LayoutAxis) * pOffset + GetVectorForAxis(SecondaryLayoutAxis) * sOffset;

                    LayoutElements[i++].localPosition = pos + StartPositionOffset + alignmentOffset;
                }
            }
        }
    }

    public void EuclideanLayout()
    {
        Vector3 pos = Vector3.zero;

        int i = 0;

        int primaryCount = GridConstraintCount;
        int secondaryCount = SecondaryConstraintCount;
        int tertiaryCount = Mathf.CeilToInt((float)LayoutElements.Count / (primaryCount * secondaryCount));

        // Bit mask to determine final driven axis (001 = X, 010 = Y, 100 = Z)
        int tertiaryAxisMask = 7;
        LayoutAxis3D tertiaryAxis = LayoutAxis3D.X;

        #region Determine Element Dimensions in Each Axis
        float pDim = 0f, sDim = 0f, tDim = 0f;

        switch (LayoutAxis)
        {
            case LayoutAxis3D.X:
                pDim = ElementDimensions.x;
                tertiaryAxisMask ^= 1;
                break;
            case LayoutAxis3D.Y:
                pDim = ElementDimensions.y;
                tertiaryAxisMask ^= 2;
                break;
            case LayoutAxis3D.Z:
                pDim = ElementDimensions.z;
                tertiaryAxisMask ^= 4;
                break;
        }

        switch (SecondaryLayoutAxis)
        {
            case LayoutAxis3D.X:
                sDim = ElementDimensions.x;
                tertiaryAxisMask ^= 1;
                break;
            case LayoutAxis3D.Y:
                sDim = ElementDimensions.y;
                tertiaryAxisMask ^= 2;
                break;
            case LayoutAxis3D.Z:
                sDim = ElementDimensions.z;
                tertiaryAxisMask ^= 4;
                break;
        }

        switch (tertiaryAxisMask)
        {
            case 1:
                tDim = ElementDimensions.x;
                break;
            case 2:
                tDim = ElementDimensions.y;
                break;
            case 4:
                tDim = ElementDimensions.z;
                break;
        }

        switch (tertiaryAxisMask)
        {
            case 1:
                tertiaryAxis = LayoutAxis3D.X;
                break;
            case 2:
                tertiaryAxis = LayoutAxis3D.Y;
                break;
            case 4:
                tertiaryAxis = LayoutAxis3D.Z;
                break;
        }
        #endregion

        
        Vector3 alignmentOffset = Vector3.zero;

        #region Calculate alignment offset vectors
        // Calculate primary alignment offset
        switch (PrimaryAlignment)
        {
            case Alignment.Min:
                break;
            case Alignment.Center:
                alignmentOffset -= GetVectorForAxis(LayoutAxis) * (primaryCount - 1) * (pDim + Spacing) / 2f;
                break;
            case Alignment.Max:
                alignmentOffset -= GetVectorForAxis(LayoutAxis) * (primaryCount - 1) * (pDim + Spacing);
                break;
        }
        // Calculate secondary alignment offset
        switch (SecondaryAlignment)
        {
            case Alignment.Min:
                break;
            case Alignment.Center:
                alignmentOffset -= GetVectorForAxis(SecondaryLayoutAxis) * (secondaryCount - 1) * (sDim + Spacing) / 2f;
                break;
            case Alignment.Max:
                alignmentOffset -= GetVectorForAxis(SecondaryLayoutAxis) * (secondaryCount - 1) * (sDim + Spacing);
                break;
        }
        // Calculate tertiary alignment offset
        switch (TertiaryAlignment)
        {
            case Alignment.Min:
                break;
            case Alignment.Center:
                alignmentOffset -= GetVectorForAxis(tertiaryAxis) * (tertiaryCount - 1) * (tDim + Spacing) / 2f;
                break;
            case Alignment.Max:
                alignmentOffset -= GetVectorForAxis(tertiaryAxis) * (tertiaryCount - 1) * (tDim + Spacing);
                break;
        }
        #endregion

        for (int t = 0; t < tertiaryCount; t++)
        {
            for (int s = 0; s < secondaryCount; s++)
            {
                for(int p = 0; p < primaryCount; p++)
                {
                    if (i < LayoutElements.Count)
                    {
                        float pOffset = (float)p * (pDim + Spacing);
                        float sOffset = (float)s * (sDim + Spacing);
                        float tOffset = (float)t * (tDim + Spacing);

                        switch(LayoutAxis)
                        {
                            case LayoutAxis3D.X:
                                pos.x = pOffset;
                                break;
                            case LayoutAxis3D.Y:
                                pos.y = pOffset;
                                break;
                            case LayoutAxis3D.Z:
                                pos.z = pOffset;
                                break;
                        }

                        switch (SecondaryLayoutAxis)
                        {
                            case LayoutAxis3D.X:
                                pos.x = sOffset;
                                break;
                            case LayoutAxis3D.Y:
                                pos.y = sOffset;
                                break;
                            case LayoutAxis3D.Z:
                                pos.z = sOffset;
                                break;
                        }

                        switch(tertiaryAxisMask)
                        {
                            case 1:
                                pos.x = tOffset;
                                break;
                            case 2:
                                pos.y = tOffset;
                                break;
                            case 4:
                                pos.z = tOffset;
                                break;
                        }

                        LayoutElements[i++].localPosition = pos + StartPositionOffset + alignmentOffset;
                    }
                }
            }
        }
    }

    public void RadialLayout()
    {
        Vector3 pos = Vector3.zero;
        float spiralSum = 0f;
        float spiralIncrement = SpiralFactor / LayoutElements.Count;

        if(UseFullCircle)
        {
            MaxArcAngle = 360f - 360f / LayoutElements.Count;
        }

        Vector3 primaryAxis = Vector3.right;
        Vector3 secondaryAxis = Vector3.forward;

        // If user provided a valid start direction
        if (SecondaryLayoutAxis != LayoutAxis)
        {
            // Determine primary and secondary axes based on provided start direction
            switch (SecondaryLayoutAxis)
            {
                case LayoutAxis3D.X:
                    primaryAxis = Vector3.right;
                    secondaryAxis = Vector3.Cross(GetVectorForAxis(LayoutAxis), primaryAxis);
                    break;
                case LayoutAxis3D.Y:
                    primaryAxis = Vector3.up;
                    secondaryAxis = Vector3.Cross(GetVectorForAxis(LayoutAxis), primaryAxis);
                    break;
                case LayoutAxis3D.Z:
                    primaryAxis = Vector3.forward;
                    secondaryAxis = Vector3.Cross(GetVectorForAxis(LayoutAxis), primaryAxis);
                    break;
            }
        }
        // If user provided an invalid start direction
        else
        {
            // Determine primary and secondary axis based on plane normal axis
            switch (LayoutAxis)
            {
                case LayoutAxis3D.X:
                    primaryAxis = Vector3.forward;
                    secondaryAxis = Vector3.down;
                    break;
                case LayoutAxis3D.Y:
                    primaryAxis = Vector3.right;
                    secondaryAxis = Vector3.back;
                    break;
                case LayoutAxis3D.Z:
                    primaryAxis = Vector3.right;
                    secondaryAxis = Vector3.up;
                    break;
            }
        }
        

        // If using symmetry, add angle offset
        float symmAngleOffset = RadialSymmetricalAboutStart && !UseFullCircle ? -MaxArcAngle / 2f : 0f;

        for (int i = 0; i < LayoutElements.Count; i++)
        {
            float angle = (LayoutElements.Count > 1) ? (float)i / (LayoutElements.Count - 1) * MaxArcAngle * Mathf.Deg2Rad : 0f;
            float x = Mathf.Cos(angle + Mathf.Deg2Rad * (StartAngleOffset + symmAngleOffset)) * (Radius + spiralSum);
            float y = Mathf.Sin(angle + Mathf.Deg2Rad * (StartAngleOffset + symmAngleOffset)) * (Radius + spiralSum);
            pos = primaryAxis * x + secondaryAxis * y;
            LayoutElements[i].localPosition = pos + StartPositionOffset;

            // Handle rotation of elements
            if (AlignToRadius)
            {
                AlignRadialElement(i, pos);
            }
            
            spiralSum += spiralIncrement;
        }
    }

    private void AlignRadialElement(int i, Vector3 LocalElemPos)
    {
        Vector3 dir = transform.TransformPoint(LocalElemPos) - transform.position;
        //LayoutElements[i].localRotation = Quaternion.identity;

        // Determine rotation that aligns child primary axis with radius and secondary axis with plane normal
        switch (PrimaryAlignmentAxis)
        {
            case LayoutAxis3D.X:
                switch (SecondaryAlignmentAxis)
                {
                    case LayoutAxis3D.X:
                        LayoutElements[i].right = dir;
                        break;
                    case LayoutAxis3D.Y:
                        LayoutElements[i].localRotation = MakeRotFromXY(LocalElemPos.normalized, GetVectorForAxis(LayoutAxis));
                        break;
                    case LayoutAxis3D.Z:
                        LayoutElements[i].localRotation = MakeRotFromXZ(LocalElemPos.normalized, GetVectorForAxis(LayoutAxis));
                        break;
                }
                break;
            case LayoutAxis3D.Y:
                switch (SecondaryAlignmentAxis)
                {
                    case LayoutAxis3D.X:
                        LayoutElements[i].localRotation = MakeRotFromYX(LocalElemPos.normalized, GetVectorForAxis(LayoutAxis));
                        break;
                    case LayoutAxis3D.Y:
                        LayoutElements[i].up = dir;
                        break;
                    case LayoutAxis3D.Z:
                        LayoutElements[i].localRotation = MakeRotFromYZ(LocalElemPos.normalized, GetVectorForAxis(LayoutAxis));
                        break;
                }
                break;
            case LayoutAxis3D.Z:
                switch (SecondaryAlignmentAxis)
                {
                    case LayoutAxis3D.X:
                        LayoutElements[i].localRotation = MakeRotFromZX(LocalElemPos.normalized, GetVectorForAxis(LayoutAxis));
                        break;
                    case LayoutAxis3D.Y:
                        LayoutElements[i].localRotation = MakeRotFromZY(LocalElemPos.normalized, GetVectorForAxis(LayoutAxis));
                        break;
                    case LayoutAxis3D.Z:
                        LayoutElements[i].forward = dir;
                        break;
                }
                break;
        }
    }

    public void PopulateElementsFromChildren()
    {
        if(LayoutElements == null)
        {
            LayoutElements = new List<Transform>();
        }

        if(ElementRotations == null)
        {
            ElementRotations = new List<Quaternion>();
        }

        if(LayoutElementsByTransform == null)
        {
            LayoutElementsByTransform = new Dictionary<Transform, LayoutElement3D>();
        }

        LayoutElements.Clear();
        LayoutElementsByTransform.Clear();

        foreach(Transform child in transform)
        {
            if(child.gameObject.activeSelf || !IgnoreDeactivatedElements)
            {
                
                // Check if this transform has a LayoutElement3D component
                LayoutElement3D elem = child.GetComponent<LayoutElement3D>();
                // If valid, add to map
                if(elem != null && !LayoutElementsByTransform.ContainsKey(child))
                {
                    // If this element should ignore the layout, dont add it to any lists
                    if(elem.IgnoreLayout)
                    {
                        continue;
                    }

                    LayoutElementsByTransform.Add(child, elem);
                }

                LayoutElements.Add(child);
            }
        }
    }

    public Vector3 GetDimensionsForElement(Transform Element)
    {
        if(LayoutElementsByTransform != null && LayoutElementsByTransform.ContainsKey(Element))
        {
            return LayoutElementsByTransform[Element].GetDimensions();
        }
        else
        {
            return ElementDimensions;
        }
    }    

    public void RecordRotations()
    {
        if (LayoutElements == null)
        {
            return;
        }

        if(ElementRotations == null)
        {
            ElementRotations = new List<Quaternion>();
        }

        if (HasChildCountChanged())
        {
            PopulateElementsFromChildren();
        }

        ElementRotations.Clear();

        for (int i = 0; i < LayoutElements.Count; i++)
        {
            ElementRotations.Add(LayoutElements[i].localRotation);
        }
    }

    public void RestoreRotations()
    {
        if(LayoutElements == null || ElementRotations == null || LayoutElements.Count != ElementRotations.Count)
        {
            return;
        }

        for(int i = 0; i < LayoutElements.Count; i++)
        {
            LayoutElements[i].localRotation = ElementRotations[i];
        }
    }

    public bool HasChildCountChanged()
    {
        if (LayoutElements != null)
        {
            if(IgnoreDeactivatedElements)
            {
                return GetNumActiveChildren() != LayoutElements.Count;
            }
            else
            {
                return transform.childCount != LayoutElements.Count;
            }
        }

        return false;
    }

    public int GetNumActiveChildren()
    {
        int num = 0;
        foreach(Transform child in transform)
        {
            if(child.gameObject.activeSelf)
            {
                num++;
            }
        }
        return num;
    }

    public Vector3 GetVectorForAxis(LayoutAxis3D Axis3D)
    {
        Vector3 axis = Vector3.zero;
        // Calculate alignment direction
        switch (Axis3D)
        {
            case LayoutAxis3D.X:
                axis = Vector3.right;
                break;
            case LayoutAxis3D.Y:
                axis = Vector3.up;
                break;
            case LayoutAxis3D.Z:
                axis = Vector3.forward;
                break;
        }

        return axis;
    }

    public Vector3 GetVectorForAxis(LayoutAxis2D Axis2D)
    {
        Vector3 axis = Vector3.zero;
        switch (Axis2D)
        {
            case LayoutAxis2D.X:
                axis = Vector3.right;
                break;
            case LayoutAxis2D.Y:
                axis = Vector3.up;
                break;
        }

        return axis;
    }

    public void Update()
    {
        if(NeedsRebuild || HasChildCountChanged())
        {
            RebuildLayout();
        }
        else if(Application.IsPlaying(gameObject) && ForceContinuousRebuild)
        {
            RebuildLayout();
        }
    }

    public void OnTransformChildrenChanged()
    {
        NeedsRebuild = true;
    }

    private void OnDrawGizmosSelected()
    {
        if(!DrawGizmos)
        {
            return;
        }

        switch (Style)
        {
            case LayoutStyle.Linear:
                DrawLinearGizmos();
                break;
            case LayoutStyle.Grid:
                DrawGridGizmos();
                break;
            case LayoutStyle.Euclidean:
                DrawEuclideanGizmos();
                break;
            case LayoutStyle.Radial:
                DrawRadialGizmos();
                break;
        }
    }

    private void DrawRadialGizmos()
    {
        float lineLength = Radius > 0 ? Radius : 1f;
        float handleScale = lineLength * 0.1f;

        Vector3 origin = transform.TransformPoint(StartPositionOffset);
        Vector3 primaryAxis = Vector3.right;
        Vector3 secondaryAxis = Vector3.forward;
        // If user provided a valid start direction
        if (SecondaryLayoutAxis != LayoutAxis)
        {
            // Determine primary and secondary axes based on provided start direction
            switch (SecondaryLayoutAxis)
            {
                case LayoutAxis3D.X:
                    primaryAxis = Vector3.right;
                    secondaryAxis = Vector3.Cross(GetVectorForAxis(LayoutAxis), primaryAxis);
                    break;
                case LayoutAxis3D.Y:
                    primaryAxis = Vector3.up;
                    secondaryAxis = Vector3.Cross(GetVectorForAxis(LayoutAxis), primaryAxis);
                    break;
                case LayoutAxis3D.Z:
                    primaryAxis = Vector3.forward;
                    secondaryAxis = Vector3.Cross(GetVectorForAxis(LayoutAxis), primaryAxis);
                    break;
            }
        }
        // If user provided an invalid start direction
        else
        {
            // Determine primary and secondary axis based on plane normal axis
            switch (LayoutAxis)
            {
                case LayoutAxis3D.X:
                    primaryAxis = Vector3.forward;
                    secondaryAxis = Vector3.down;
                    break;
                case LayoutAxis3D.Y:
                    primaryAxis = Vector3.right;
                    secondaryAxis = Vector3.back;
                    break;
                case LayoutAxis3D.Z:
                    primaryAxis = Vector3.right;
                    secondaryAxis = Vector3.up;
                    break;
            }
        }

        // Draw plane normal axis
        Vector3 worldNormal = transform.TransformDirection(GetVectorForAxis(LayoutAxis));
        Gizmos.color = GizmoPrimaryColor;
        DrawGizmoArrow(origin, origin + worldNormal * lineLength * GizmoScale, Vector3.Cross(worldNormal, transform.TransformDirection(primaryAxis)));
        Gizmos.DrawSphere(origin, handleScale);

        // Adjust start angle if using symmetry
        float startAngle = StartAngleOffset;
        if(RadialSymmetricalAboutStart && !UseFullCircle)
        {
            startAngle -= MaxArcAngle / 2f;
        }

        // Draw start direction
        float x = Mathf.Cos(Mathf.Deg2Rad * startAngle) * (lineLength);
        float y = Mathf.Sin(Mathf.Deg2Rad * startAngle) * (lineLength);
        Vector3 startPoint = primaryAxis * x + secondaryAxis * y;

        Gizmos.color = GizmoSecondaryColor;
        Gizmos.DrawLine(origin, transform.TransformPoint(startPoint + StartPositionOffset));

        // Draw arrow arc
        float maxAngle = UseFullCircle ? 360f : MaxArcAngle;
        float arcPointsPerDegree = 0.075f;
        int arcPoints = Mathf.Max(2, Mathf.FloorToInt(arcPointsPerDegree * maxAngle));
        float spiralSum = 0f;
        float spiralIncrement = SpiralFactor / arcPoints;

        Vector3 arcPos = startPoint + StartPositionOffset;

        for (int i = 1; i < arcPoints; i++)
        {
            float angle = (float)i / (arcPoints - 1) * maxAngle * Mathf.Deg2Rad;
            x = Mathf.Cos(angle + Mathf.Deg2Rad * startAngle) * (lineLength + spiralSum);
            y = Mathf.Sin(angle + Mathf.Deg2Rad * startAngle) * (lineLength + spiralSum);
            Vector3 pos = primaryAxis * x + secondaryAxis * y + StartPositionOffset;

            DrawGizmoArrow(transform.TransformPoint(arcPos), transform.TransformPoint(pos), worldNormal, 0.5f * Mathf.Clamp01(GizmoScale));

            spiralSum += spiralIncrement;
            arcPos = pos;
        }

        // Draw end direction
        if (!UseFullCircle)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * (startAngle + MaxArcAngle)) * (lineLength + spiralSum);
            y = Mathf.Sin(Mathf.Deg2Rad * (startAngle + MaxArcAngle)) * (lineLength + spiralSum);
            Vector3 endPoint = primaryAxis * x + secondaryAxis * y;

            Gizmos.color = GizmoTertiaryColor;
            Gizmos.DrawLine(origin, transform.TransformPoint(endPoint + StartPositionOffset));
        }
    }

    private void DrawLinearGizmos()
    {
        Vector3 origin = transform.TransformPoint(StartPositionOffset);
        Vector3 dir = transform.TransformDirection(GetVectorForAxis(LayoutAxis));

        float lineLength = 1f * GizmoScale;

        Gizmos.color = GizmoPrimaryColor;
        Gizmos.DrawSphere(origin, 0.05f * GizmoScale);

        switch (PrimaryAlignment)
        {
            case Alignment.Min:
                DrawGizmoArrow(origin, origin + dir * lineLength, GetOrthogonalVector(dir));
                break;
            case Alignment.Center:
                DrawGizmoArrow(origin, origin + dir * lineLength / 2f, GetOrthogonalVector(dir));
                DrawGizmoArrow(origin, origin - dir * lineLength / 2f, GetOrthogonalVector(dir));
                break;
            case Alignment.Max:
                DrawGizmoArrow(origin, origin - dir * lineLength, GetOrthogonalVector(dir));
                break;
        }
    }

    private void DrawGridGizmos()
    {
        Vector3 origin = transform.TransformPoint(StartPositionOffset);
        Vector3 primaryDir = transform.TransformDirection(GetVectorForAxis(LayoutAxis));
        Vector3 secondaryDir = transform.TransformDirection(GetVectorForAxis(SecondaryLayoutAxis));

        float lineLength = 1f * GizmoScale;

        Gizmos.color = GizmoPrimaryColor;
        Gizmos.DrawSphere(origin, 0.05f * GizmoScale);

        
        switch (PrimaryAlignment)
        {
            case Alignment.Min:
                DrawGizmoArrow(origin, origin + primaryDir * lineLength, GetOrthogonalVector(primaryDir));
                break;
            case Alignment.Center:
                DrawGizmoArrow(origin, origin + primaryDir * lineLength / 2f, GetOrthogonalVector(primaryDir));
                DrawGizmoArrow(origin, origin - primaryDir * lineLength / 2f, GetOrthogonalVector(primaryDir));
                break;
            case Alignment.Max:
                DrawGizmoArrow(origin, origin - primaryDir * lineLength, GetOrthogonalVector(primaryDir));
                break;
        }

        Gizmos.color = GizmoSecondaryColor;
        switch (SecondaryAlignment)
        {
            case Alignment.Min:
                DrawGizmoArrow(origin, origin + secondaryDir * lineLength, GetOrthogonalVector(secondaryDir));
                break;
            case Alignment.Center:
                DrawGizmoArrow(origin, origin + secondaryDir * lineLength / 2f, GetOrthogonalVector(secondaryDir));
                DrawGizmoArrow(origin, origin - secondaryDir * lineLength / 2f, GetOrthogonalVector(secondaryDir));
                break;
            case Alignment.Max:
                DrawGizmoArrow(origin, origin - secondaryDir * lineLength, GetOrthogonalVector(secondaryDir));
                break;
        }
    }

    private void DrawEuclideanGizmos()
    {
        Vector3 origin = transform.TransformPoint(StartPositionOffset);
        Vector3 primaryDir = transform.TransformDirection(GetVectorForAxis(LayoutAxis));
        Vector3 secondaryDir = transform.TransformDirection(GetVectorForAxis(SecondaryLayoutAxis));
        Vector3 tertiaryDir = Vector3.Cross(primaryDir, secondaryDir);

        float lineLength = 1f * GizmoScale;

        Gizmos.color = GizmoPrimaryColor;
        Gizmos.DrawSphere(origin, 0.05f * GizmoScale);


        switch (PrimaryAlignment)
        {
            case Alignment.Min:
                DrawGizmoArrow(origin, origin + primaryDir * lineLength, GetOrthogonalVector(primaryDir));
                break;
            case Alignment.Center:
                DrawGizmoArrow(origin, origin + primaryDir * lineLength / 2f, GetOrthogonalVector(primaryDir));
                DrawGizmoArrow(origin, origin - primaryDir * lineLength / 2f, GetOrthogonalVector(primaryDir));
                break;
            case Alignment.Max:
                DrawGizmoArrow(origin, origin - primaryDir * lineLength, GetOrthogonalVector(primaryDir));
                break;
        }

        Gizmos.color = GizmoSecondaryColor;
        switch (SecondaryAlignment)
        {
            case Alignment.Min:
                DrawGizmoArrow(origin, origin + secondaryDir * lineLength, GetOrthogonalVector(secondaryDir));
                break;
            case Alignment.Center:
                DrawGizmoArrow(origin, origin + secondaryDir * lineLength / 2f, GetOrthogonalVector(secondaryDir));
                DrawGizmoArrow(origin, origin - secondaryDir * lineLength / 2f, GetOrthogonalVector(secondaryDir));
                break;
            case Alignment.Max:
                DrawGizmoArrow(origin, origin - secondaryDir * lineLength, GetOrthogonalVector(secondaryDir));
                break;
        }

        Gizmos.color = GizmoTertiaryColor;
        switch (TertiaryAlignment)
        {
            case Alignment.Min:
                DrawGizmoArrow(origin, origin + tertiaryDir * lineLength, GetOrthogonalVector(tertiaryDir));
                break;
            case Alignment.Center:
                DrawGizmoArrow(origin, origin + tertiaryDir * lineLength / 2f, GetOrthogonalVector(tertiaryDir));
                DrawGizmoArrow(origin, origin - tertiaryDir * lineLength / 2f, GetOrthogonalVector(tertiaryDir));
                break;
            case Alignment.Max:
                DrawGizmoArrow(origin, origin - tertiaryDir * lineLength, GetOrthogonalVector(tertiaryDir));
                break;
        }
    }


    /// <summary>
    /// Makes a Quaternion from X and Y directions where X takes priority.
    /// </summary>
    /// <param name="X">The x.</param>
    /// <param name="Y">The y.</param>
    /// <returns></returns>
    public static Quaternion MakeRotFromXY(Vector3 X, Vector3 Y)
    {
        Y.Normalize();

        Vector3 right = Vector3.Normalize(X);
        Vector3 forward = Vector3.Normalize(Vector3.Cross(right, Y));
        Vector3 up = Vector3.Cross(forward, right);
        var m00 = right.x;
        var m01 = right.y;
        var m02 = right.z;
        var m10 = up.x;
        var m11 = up.y;
        var m12 = up.z;
        var m20 = forward.x;
        var m21 = forward.y;
        var m22 = forward.z;


        float num8 = (m00 + m11) + m22;
        var quaternion = new Quaternion();
        if (num8 > 0f)
        {
            var num = (float)Math.Sqrt(num8 + 1f);
            quaternion.w = num * 0.5f;
            num = 0.5f / num;
            quaternion.x = (m12 - m21) * num;
            quaternion.y = (m20 - m02) * num;
            quaternion.z = (m01 - m10) * num;
            return quaternion;
        }
        if ((m00 >= m11) && (m00 >= m22))
        {
            var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
            var num4 = 0.5f / num7;
            quaternion.x = 0.5f * num7;
            quaternion.y = (m01 + m10) * num4;
            quaternion.z = (m02 + m20) * num4;
            quaternion.w = (m12 - m21) * num4;
            return quaternion;
        }
        if (m11 > m22)
        {
            var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
            var num3 = 0.5f / num6;
            quaternion.x = (m10 + m01) * num3;
            quaternion.y = 0.5f * num6;
            quaternion.z = (m21 + m12) * num3;
            quaternion.w = (m20 - m02) * num3;
            return quaternion;
        }
        var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
        var num2 = 0.5f / num5;
        quaternion.x = (m20 + m02) * num2;
        quaternion.y = (m21 + m12) * num2;
        quaternion.z = 0.5f * num5;
        quaternion.w = (m01 - m10) * num2;
        return quaternion;
    }

    /// <summary>
    /// Makes a Quaternion from X and Z directions where X takes priority.
    /// </summary>
    /// <param name="X">The x.</param>
    /// <param name="Z">The z.</param>
    /// <returns></returns>
    public static Quaternion MakeRotFromXZ(Vector3 X, Vector3 Z)
    {
        X.Normalize();

        Vector3 right = Vector3.Normalize(X);
        Vector3 up = Vector3.Normalize(Vector3.Cross(Z, right));
        Vector3 forward = Vector3.Cross(right, up);
        var m00 = right.x;
        var m01 = right.y;
        var m02 = right.z;
        var m10 = up.x;
        var m11 = up.y;
        var m12 = up.z;
        var m20 = forward.x;
        var m21 = forward.y;
        var m22 = forward.z;


        float num8 = (m00 + m11) + m22;
        var quaternion = new Quaternion();
        if (num8 > 0f)
        {
            var num = (float)Math.Sqrt(num8 + 1f);
            quaternion.w = num * 0.5f;
            num = 0.5f / num;
            quaternion.x = (m12 - m21) * num;
            quaternion.y = (m20 - m02) * num;
            quaternion.z = (m01 - m10) * num;
            return quaternion;
        }
        if ((m00 >= m11) && (m00 >= m22))
        {
            var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
            var num4 = 0.5f / num7;
            quaternion.x = 0.5f * num7;
            quaternion.y = (m01 + m10) * num4;
            quaternion.z = (m02 + m20) * num4;
            quaternion.w = (m12 - m21) * num4;
            return quaternion;
        }
        if (m11 > m22)
        {
            var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
            var num3 = 0.5f / num6;
            quaternion.x = (m10 + m01) * num3;
            quaternion.y = 0.5f * num6;
            quaternion.z = (m21 + m12) * num3;
            quaternion.w = (m20 - m02) * num3;
            return quaternion;
        }
        var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
        var num2 = 0.5f / num5;
        quaternion.x = (m20 + m02) * num2;
        quaternion.y = (m21 + m12) * num2;
        quaternion.z = 0.5f * num5;
        quaternion.w = (m01 - m10) * num2;
        return quaternion;
    }

    /// <summary>
    /// Makes a Quaternion from Y and X directions where Y takes priority.
    /// </summary>
    /// <param name="Y">The y.</param>
    /// <param name="X">The x.</param>
    /// <returns></returns>
    public static Quaternion MakeRotFromYX(Vector3 Y, Vector3 X)
    {

        Vector3 up = Vector3.Normalize(Y);
        Vector3 forward = Vector3.Normalize(Vector3.Cross(X, up));
        Vector3 right = Vector3.Cross(up, forward);
        var m00 = right.x;
        var m01 = right.y;
        var m02 = right.z;
        var m10 = up.x;
        var m11 = up.y;
        var m12 = up.z;
        var m20 = forward.x;
        var m21 = forward.y;
        var m22 = forward.z;


        float num8 = (m00 + m11) + m22;
        var quaternion = new Quaternion();
        if (num8 > 0f)
        {
            var num = (float)Math.Sqrt(num8 + 1f);
            quaternion.w = num * 0.5f;
            num = 0.5f / num;
            quaternion.x = (m12 - m21) * num;
            quaternion.y = (m20 - m02) * num;
            quaternion.z = (m01 - m10) * num;
            return quaternion;
        }
        if ((m00 >= m11) && (m00 >= m22))
        {
            var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
            var num4 = 0.5f / num7;
            quaternion.x = 0.5f * num7;
            quaternion.y = (m01 + m10) * num4;
            quaternion.z = (m02 + m20) * num4;
            quaternion.w = (m12 - m21) * num4;
            return quaternion;
        }
        if (m11 > m22)
        {
            var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
            var num3 = 0.5f / num6;
            quaternion.x = (m10 + m01) * num3;
            quaternion.y = 0.5f * num6;
            quaternion.z = (m21 + m12) * num3;
            quaternion.w = (m20 - m02) * num3;
            return quaternion;
        }
        var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
        var num2 = 0.5f / num5;
        quaternion.x = (m20 + m02) * num2;
        quaternion.y = (m21 + m12) * num2;
        quaternion.z = 0.5f * num5;
        quaternion.w = (m01 - m10) * num2;
        return quaternion;
    }

    /// <summary>
    /// Makes a Quaternion from Y and Z directions where Y takes priority.
    /// </summary>
    /// <param name="Y">The y.</param>
    /// <param name="Z">The z.</param>
    /// <returns></returns>
    public static Quaternion MakeRotFromYZ(Vector3 Y, Vector3 Z)
    {

        Vector3 up = Vector3.Normalize(Y);
        Vector3 right = Vector3.Normalize(Vector3.Cross(up, Z));
        Vector3 forward = Vector3.Cross(right, up);
        var m00 = right.x;
        var m01 = right.y;
        var m02 = right.z;
        var m10 = up.x;
        var m11 = up.y;
        var m12 = up.z;
        var m20 = forward.x;
        var m21 = forward.y;
        var m22 = forward.z;


        float num8 = (m00 + m11) + m22;
        var quaternion = new Quaternion();
        if (num8 > 0f)
        {
            var num = (float)Math.Sqrt(num8 + 1f);
            quaternion.w = num * 0.5f;
            num = 0.5f / num;
            quaternion.x = (m12 - m21) * num;
            quaternion.y = (m20 - m02) * num;
            quaternion.z = (m01 - m10) * num;
            return quaternion;
        }
        if ((m00 >= m11) && (m00 >= m22))
        {
            var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
            var num4 = 0.5f / num7;
            quaternion.x = 0.5f * num7;
            quaternion.y = (m01 + m10) * num4;
            quaternion.z = (m02 + m20) * num4;
            quaternion.w = (m12 - m21) * num4;
            return quaternion;
        }
        if (m11 > m22)
        {
            var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
            var num3 = 0.5f / num6;
            quaternion.x = (m10 + m01) * num3;
            quaternion.y = 0.5f * num6;
            quaternion.z = (m21 + m12) * num3;
            quaternion.w = (m20 - m02) * num3;
            return quaternion;
        }
        var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
        var num2 = 0.5f / num5;
        quaternion.x = (m20 + m02) * num2;
        quaternion.y = (m21 + m12) * num2;
        quaternion.z = 0.5f * num5;
        quaternion.w = (m01 - m10) * num2;
        return quaternion;
    }

    /// <summary>
    /// Makes a Quaternion from Z and X directions where Z takes priority.
    /// </summary>
    /// <param name="Z">The z.</param>
    /// <param name="X">The x.</param>
    /// <returns></returns>
    public static Quaternion MakeRotFromZX(Vector3 Z, Vector3 X)
    {

        Vector3 forward = Vector3.Normalize(Z);
        Vector3 up = Vector3.Normalize(Vector3.Cross(forward, X));
        Vector3 right = Vector3.Cross(up, forward);
        var m00 = right.x;
        var m01 = right.y;
        var m02 = right.z;
        var m10 = up.x;
        var m11 = up.y;
        var m12 = up.z;
        var m20 = forward.x;
        var m21 = forward.y;
        var m22 = forward.z;


        float num8 = (m00 + m11) + m22;
        var quaternion = new Quaternion();
        if (num8 > 0f)
        {
            var num = (float)Math.Sqrt(num8 + 1f);
            quaternion.w = num * 0.5f;
            num = 0.5f / num;
            quaternion.x = (m12 - m21) * num;
            quaternion.y = (m20 - m02) * num;
            quaternion.z = (m01 - m10) * num;
            return quaternion;
        }
        if ((m00 >= m11) && (m00 >= m22))
        {
            var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
            var num4 = 0.5f / num7;
            quaternion.x = 0.5f * num7;
            quaternion.y = (m01 + m10) * num4;
            quaternion.z = (m02 + m20) * num4;
            quaternion.w = (m12 - m21) * num4;
            return quaternion;
        }
        if (m11 > m22)
        {
            var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
            var num3 = 0.5f / num6;
            quaternion.x = (m10 + m01) * num3;
            quaternion.y = 0.5f * num6;
            quaternion.z = (m21 + m12) * num3;
            quaternion.w = (m20 - m02) * num3;
            return quaternion;
        }
        var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
        var num2 = 0.5f / num5;
        quaternion.x = (m20 + m02) * num2;
        quaternion.y = (m21 + m12) * num2;
        quaternion.z = 0.5f * num5;
        quaternion.w = (m01 - m10) * num2;
        return quaternion;
    }

    /// <summary>
    /// Makes a Quaternion from Z and Y directions where Z takes priority.
    /// </summary>
    /// <param name="Z">The z.</param>
    /// <param name="Y">The y.</param>
    /// <returns></returns>
    public static Quaternion MakeRotFromZY(Vector3 Z, Vector3 Y)
    {
        Z.Normalize();

        Vector3 forward = Vector3.Normalize(Z);
        Vector3 right = Vector3.Normalize(Vector3.Cross(Y, forward));
        Vector3 up = Vector3.Cross(forward, right);
        var m00 = right.x;
        var m01 = right.y;
        var m02 = right.z;
        var m10 = up.x;
        var m11 = up.y;
        var m12 = up.z;
        var m20 = forward.x;
        var m21 = forward.y;
        var m22 = forward.z;


        float num8 = (m00 + m11) + m22;
        var quaternion = new Quaternion();
        if (num8 > 0f)
        {
            var num = (float)Math.Sqrt(num8 + 1f);
            quaternion.w = num * 0.5f;
            num = 0.5f / num;
            quaternion.x = (m12 - m21) * num;
            quaternion.y = (m20 - m02) * num;
            quaternion.z = (m01 - m10) * num;
            return quaternion;
        }
        if ((m00 >= m11) && (m00 >= m22))
        {
            var num7 = (float)Math.Sqrt(((1f + m00) - m11) - m22);
            var num4 = 0.5f / num7;
            quaternion.x = 0.5f * num7;
            quaternion.y = (m01 + m10) * num4;
            quaternion.z = (m02 + m20) * num4;
            quaternion.w = (m12 - m21) * num4;
            return quaternion;
        }
        if (m11 > m22)
        {
            var num6 = (float)Math.Sqrt(((1f + m11) - m00) - m22);
            var num3 = 0.5f / num6;
            quaternion.x = (m10 + m01) * num3;
            quaternion.y = 0.5f * num6;
            quaternion.z = (m21 + m12) * num3;
            quaternion.w = (m20 - m02) * num3;
            return quaternion;
        }
        var num5 = (float)Math.Sqrt(((1f + m22) - m00) - m11);
        var num2 = 0.5f / num5;
        quaternion.x = (m20 + m02) * num2;
        quaternion.y = (m21 + m12) * num2;
        quaternion.z = 0.5f * num5;
        quaternion.w = (m01 - m10) * num2;
        return quaternion;
    }

    private void DrawGizmoArrow(Vector3 Start, Vector3 End, Vector3 PlaneNormal, float ArrowHeadSizeRatio = 0.1f)
    {
        Vector3 dir = End - Start;
        float length = dir.magnitude;

        Vector3 arrowHeadL = RotateVectorAroundAxis(-dir.normalized * length * ArrowHeadSizeRatio, PlaneNormal, 30f);
        Vector3 arrowHeadR = RotateVectorAroundAxis(-dir.normalized * length * ArrowHeadSizeRatio, PlaneNormal, -30f);

        Gizmos.DrawLine(Start, End);
        Gizmos.DrawLine(End, End + arrowHeadL);
        Gizmos.DrawLine(End, End + arrowHeadR);
        Gizmos.DrawLine(End + arrowHeadL, End + arrowHeadR);
    }

    private Vector3 RotateVectorAroundAxis(Vector3 VectorToRotate, Vector3 RotationAxis, float Angle)
    {
        return Quaternion.AngleAxis(Angle, RotationAxis) * VectorToRotate;
    }

    private Vector3 GetOrthogonalVector(Vector3 v)
    {
        if (Mathf.Abs(v.x) >= 1f / Mathf.Sqrt(3f))
        {
            return Vector3.Normalize(new Vector3(v.y, -v.x, 0.0f));
        }
        else
        {
            return Vector3.Normalize(new Vector3(0.0f, v.z, -v.y));
        }
    }
}
