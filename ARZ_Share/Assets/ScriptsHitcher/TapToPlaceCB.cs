// @Author Nabil Lamriben ©2017
using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

public class TapToPlaceCB : MonoBehaviour, IInputClickHandler
{
 

    public string SavedAnchorFriendlyName = "SavedAnchorFriendlyName";
    public bool PlaceParentOnTap;
    public GameObject ParentGameObjectToPlace;

    public bool IsBeingPlaced;
    protected WorldAnchorManager anchorManager;
    protected SpatialMappingManager spatialMappingManager;

    protected virtual void Start()
    {
        anchorManager = WorldAnchorManager.Instance;
        if (anchorManager == null) {
            CONBUG.Instance.LOGitError("This script expects that you have a WorldAnchorManager component in your scene.");
        }

        spatialMappingManager = SpatialMappingManager.Instance;
        if (spatialMappingManager == null){
            CONBUG.Instance.LOGitError("This script expects that you have a SpatialMappingManager component in your scene.");
        }

        if (anchorManager != null && spatialMappingManager != null)
        {
            // If we are not starting out with actively placing the object, give it a World Anchor
            if (!IsBeingPlaced) {
                anchorManager.AttachAnchor(gameObject, SavedAnchorFriendlyName);
            }
        }
        else
        {
            // If we don't have what we need to proceed, we may as well remove ourselves.
            Destroy(this);
        }

        if (PlaceParentOnTap)
        {
            if (ParentGameObjectToPlace != null && !gameObject.transform.IsChildOf(ParentGameObjectToPlace.transform))
            {
                CONBUG.Instance.LOGitError("The specified parent object is not a parent of this object.");
            }

            DetermineParent();
        }
    }

    protected virtual void Update()
    {
        // If the user is in placing mode, update the placement to match the user's gaze.
        if (IsBeingPlaced)
        {
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, spatialMappingManager.LayerMask))
            {
                // Rotate this object to face the user.
                Quaternion toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;
                if (PlaceParentOnTap)
                {
                    // Place the parent object as well but keep the focus on the current game object
                    Vector3 currentMovement = hitInfo.point - gameObject.transform.position;
                    ParentGameObjectToPlace.transform.position += currentMovement;
                    ParentGameObjectToPlace.transform.rotation = toQuat;
                }
                else
                {
                    gameObject.transform.position = hitInfo.point;
                    gameObject.transform.rotation = toQuat;
                }
            }
        }
    }

    public virtual void OnInputClicked(InputClickedEventData eventData)
    {
        IsBeingPlaced = !IsBeingPlaced;
        if (IsBeingPlaced)
        {
            spatialMappingManager.DrawVisualMeshes = true;
            CONBUG.Instance.LOGit(gameObject.name + " : Removing existing world anchor if any.");
            anchorManager.RemoveAnchor(gameObject);
        }
        // If the user is not in placing mode, hide the spatial mapping mesh.
        else
        {
            spatialMappingManager.DrawVisualMeshes = false;
            // Add world anchor when object placement is done.
            anchorManager.AttachAnchor(gameObject, SavedAnchorFriendlyName);
        }
    }

    private void DetermineParent()
    {
        if (ParentGameObjectToPlace == null)
        {
            if (gameObject.transform.parent == null)
            {
                CONBUG.Instance.LOGitError("The selected GameObject has no parent.");
                PlaceParentOnTap = false;
            }
            else
            {
                CONBUG.Instance.LOGitError("No parent specified. Using immediate parent instead: " + gameObject.transform.parent.gameObject.name);
                ParentGameObjectToPlace = gameObject.transform.parent.gameObject;
            }
        }
    }
}

