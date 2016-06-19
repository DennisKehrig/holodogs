// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public partial class GestureManager : Singleton<GestureManager>
    {
        /// <summary>
        /// To select even when a hologram is not being gazed at,
        /// set the override focused object.
        /// If its null, then the gazed at object will be selected.
        /// </summary>
        public GameObject GestureHandler
        {
            get; set;
        }

        /// <summary>
        /// Gets the currently focused object, or null if none.
        /// </summary>
        public GameObject FocusedObject
        {
            get { return focusedObject; }
        }

        private GestureRecognizer gestureRecognizer;
        private GameObject focusedObject;

        void Start()
        {
            // Create a new GestureRecognizer. Sign up for tapped events.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.ManipulationTranslate);

            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;
            gestureRecognizer.ManipulationStartedEvent += GestureRecognizer_ManipulationStartedEvent;
            gestureRecognizer.ManipulationCompletedEvent += GestureRecognizer_ManipulationCompletedEvent;
            gestureRecognizer.ManipulationCanceledEvent += GestureRecognizer_ManipulationCanceledEvent;
            gestureRecognizer.ManipulationUpdatedEvent += GestureRecognizer_ManipulationUpdatedEvent;

            // Start looking for gestures.
            gestureRecognizer.StartCapturingGestures();
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            GameObject handler = GestureHandler != null ? GestureHandler : focusedObject;
            if (handler != null)
                handler.SendMessage("OnSelect");
        }

        public GameObject heldObject;
        private GameObject heldObjectHandler;

        private void GestureRecognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            heldObject = focusedObject;
            heldObjectHandler = GestureHandler != null ? GestureHandler : heldObject;
            if (heldObjectHandler != null)
                heldObjectHandler.SendMessage("OnManipulationStarted", cumulativeDelta);
        }

        private void GestureRecognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            if (heldObjectHandler != null)
                heldObjectHandler.SendMessage("OnManipulationUpdated", cumulativeDelta);
        }

        private void GestureRecognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            if (heldObjectHandler != null)
                heldObjectHandler.SendMessage("OnManipulationCompleted", cumulativeDelta);

            heldObject = null;
            heldObjectHandler = null;
        }

        private void GestureRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            if (heldObjectHandler != null)
                heldObjectHandler.SendMessage("OnManipulationCanceled", cumulativeDelta);

            heldObject = null;
            heldObjectHandler = null;
        }

        void LateUpdate()
        {
            GameObject oldFocusedObject = focusedObject;
            if (GazeManager.Instance.Hit && GazeManager.Instance.HitInfo.collider != null)
                // If gaze hits a hologram, set the focused object to that game object.
                // Also if the caller has not decided to override the focused object.
                focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
            else
                focusedObject = null;

            if (focusedObject != oldFocusedObject)
            {
                // If the currently focused object doesn't match the old focused object, cancel the current gesture.
                // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
                //gestureRecognizer.CancelGestures();
                //gestureRecognizer.StartCapturingGestures();
            }
        }

        void OnDestroy()
        {
            gestureRecognizer.StopCapturingGestures();
            gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
        }
    }
}