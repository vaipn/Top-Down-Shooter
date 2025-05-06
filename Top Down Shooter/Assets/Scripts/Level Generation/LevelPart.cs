using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{


    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntrancePoint();

        SnapTo (entrancePoint, targetSnapPoint);
    }

    private void SnapTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
    {
        // Calculate the offset between the level part's transform position and it's own snap point's position.
        // This offset represents the distance and direction from the level part's pivot to its snap point

        var offset = transform.position - ownSnapPoint.transform.position;

        // Determine the new position for the level part. It's calculated by adding the previously computed offset
        // to the target snap point's position. This effectively moves the level part so that its snap point aligns
        // with the target snap point's position

        var newPosition = targetSnapPoint.transform.position + offset;

        transform.position = newPosition;
    }

    public SnapPoint GetEntrancePoint() => GetSnapPointOfType(SnapPointType.Enter);
    public SnapPoint GetExitPoint() => GetSnapPointOfType(SnapPointType.Exit);
    private SnapPoint GetSnapPointOfType(SnapPointType pointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();

        List<SnapPoint> filteredSnapPoints = new List<SnapPoint>();

        // Collect all snap points of the specified type
        foreach (SnapPoint snapPoint in snapPoints)
        {
            if (snapPoint.pointType == pointType)
                filteredSnapPoints.Add(snapPoint);
        }

        // if there are matching snap points, choose one at random
        if (filteredSnapPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, filteredSnapPoints.Count);
            return filteredSnapPoints[randomIndex];
        }

        return null;
    }
}
