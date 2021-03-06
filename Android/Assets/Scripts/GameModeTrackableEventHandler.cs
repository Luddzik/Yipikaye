﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class GameModeTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    
    public MazeGenerator generator;
    public GameObject gameUI;
    public GameObject gameSelection;
    public GameObject backButton;

    #region PRIVATE_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;

    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNTIY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;

        // Enable gravity:
        //transform.GetChild(0).GetComponent<GravityPull>().enabled = true;

        // Setup grid:
        //transform.GetChild(0).GetComponent<GridSystem>().SetupGrid();

        // Enable game objects
        transform.GetChild(0).gameObject.SetActive(true);
        if (!generator.mazeGenerated)
        {
            int pastRating = 0;
            if (generator.gameMode == 1)
                pastRating = PlayerPrefs.GetInt("Mode1Rating");
            else if (generator.gameMode == 2)
                pastRating = PlayerPrefs.GetInt("Mode2Rating");

            gameSelection.SetActive(true);
            gameSelection.GetComponent<StarManage>().SetStar(pastRating);
        }
        else
        {
            gameUI.SetActive(true);
            backButton.SetActive(false);
        }
        //if (!mazeGenerated)
        //{
        //    generator.StartGeneration();
        //    mazeGenerated = true;
        //}
    }


    protected virtual void OnTrackingLost()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable game objects
        if (transform.GetChild(0).gameObject != null)
            transform.GetChild(0).gameObject.SetActive(false);
        //gameUI.SetActive(false);
        if (backButton != null)
            backButton.SetActive(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;

        // Disable gravity:
        //transform.GetChild(0).GetComponent<GravityPull>().enabled = false;

        
    }

    #endregion // PRIVATE_METHODS
}
