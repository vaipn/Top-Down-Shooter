using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour
{
    [Header("Intersection check")]
    [SerializeField] private LayerMask intersectionLayer;
    [SerializeField] private Collider[] intersectionCheckColliders;
    [SerializeField] private Transform intersectionCheckParent;


    [ContextMenu("Set static to environment layer")]
    private void AdjustLayerForStaticObjects()
    {
        foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>(true))
        {
            if (childTransform.gameObject.isStatic)
                childTransform.gameObject.layer = LayerMask.NameToLayer("Environment");
        }
    }

    public bool IntersectionDetected()
    {
        Physics.SyncTransforms();

        foreach (var collider in intersectionCheckColliders)
        {
            Collider[] hitColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, intersectionLayer);

            foreach (var hit in hitColliders)
            {
                IntersectionCheck intersectionCheck = hit.GetComponentInParent<IntersectionCheck>();

                if (intersectionCheck != null && intersectionCheckParent != intersectionCheck.transform)
                    return true;
            }
        }

        return false;
    }

    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEntrancePoint();

        AlignTo (entrancePoint, targetSnapPoint); // Alignment should be before snapping
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

	private void AlignTo(SnapPoint ownSnapPoint, SnapPoint targetSnapPoint)
	{
        // Calculate the rotation offset between the level part's current rotation and it's own snap point's rotation.
        // This helps in fine-tuning the alignment later.
        var rotationOffset = ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;

        // Set the level part's rotation to match the target snap point's rotation. This is the initial step to align
        // the orientations of the two parts.
        transform.rotation = targetSnapPoint.transform.rotation;

        // Rotate the level part by 180 degrees around the Y-axis. This is necessary because the snap points are
        // typically facing opposite directions, and this rotation aligns them to face each other correctly.
        transform.Rotate(0, 180, 0);

        // Apply the previously calculated offset. This step fine-tunes the alignment by adjusting the level part's 
        // rotation to account for any initial difference in orientation between the level part's own snap point and
        // the main body of the part.
        transform.Rotate(0, -rotationOffset, 0);
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
    public Enemy[] LevelPartEnemies() => GetComponentsInChildren<Enemy>(true);
}
