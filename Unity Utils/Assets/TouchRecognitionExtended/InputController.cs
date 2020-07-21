using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace AUZ_UTIL
{
    public enum SwipeDirection
    {
        UP = 0,
        RIGHT = 1,
        DOWN = 2,
        LEFT = 3
    }

//[System.Serializable]
    public class TouchData
    {
        public bool HasStarted = false;
        public Vector2 DistanceDiff => CurrentTouchPosition - LastTouchPosition;
        public float TimeDiff => CurrentTouchTimer - LastTouchTimer;

        public Vector2 LastTouchPosition;
        public float LastTouchTimer;

        public Vector2 CurrentTouchPosition;
        public float CurrentTouchTimer;

        public TouchData()
        {
            Debug.Log("initialising touch data");
            Reset();
        }

        public void Reset()
        {
            HasStarted = false;
            CurrentTouchPosition = LastTouchPosition = Vector2.zero;
            CurrentTouchTimer = 0;
            LastTouchTimer = 0;
        }
    }

    public class InputController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<SwipeDirection, Vector2> OnSwipe;

        public TouchData CustomTouchData;
        float _minFrameDelay = 3;
        float _minSwipeDistance = 0.6f;

        private Camera _mainCam;

        private void Awake()
        {
            _mainCam = Camera.main;
            Debug.Log("running awake");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //Debug.Log("beginning drag");
            CustomTouchData = new TouchData();
            CustomTouchData.CurrentTouchPosition = _mainCam.ScreenToWorldPoint(eventData.position);
            CustomTouchData.LastTouchPosition = CustomTouchData.CurrentTouchPosition;
            CustomTouchData.HasStarted = true;
        }

        private void FixedUpdate()
        {
            if (CustomTouchData == null || !CustomTouchData.HasStarted) //touch has not started
                return;

            Vector3 tPosition = Vector3.zero;
#if UNITY_EDITOR
            tPosition = Input.mousePosition;
#else
        if (Input.touchCount>0) tPosition = Input.touches[0].position;
        else return; //back if no touch is detected, it will be processed OnEndDrag
#endif

            //Get current touch status
            CustomTouchData.CurrentTouchTimer += Time.fixedDeltaTime;
            CustomTouchData.CurrentTouchPosition = _mainCam.ScreenToWorldPoint(tPosition);

            //check if enough time has passed
            if (CustomTouchData.TimeDiff < Time.fixedDeltaTime * _minFrameDelay)
            {
                //Debug.Log($"============== {CustomTouchData.DistanceDiff.sqrMagnitude}");
                return;
            }

            //make calculation
            ProcessTouch();
        }

        private void SaveCurrentTouch()
        {
            //save current frame
            CustomTouchData.LastTouchTimer = CustomTouchData.CurrentTouchTimer;
            CustomTouchData.LastTouchPosition = CustomTouchData.CurrentTouchPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //Debug.Log("ending drag");
            if (CustomTouchData.DistanceDiff.sqrMagnitude >= _minSwipeDistance) ProcessTouch();
            CustomTouchData.Reset();
        }

        private void ProcessTouch()
        {
            Vector2 diff = CustomTouchData.DistanceDiff;

            if (CustomTouchData.DistanceDiff.sqrMagnitude < _minSwipeDistance) //stopped
            {
                //Debug.Log($"stopped:: {CustomTouchData.DistanceDiff.sqrMagnitude}");
                return;
            }

            float timeDiff = CustomTouchData.TimeDiff;
            SwipeDirection swipeDir = GetDirection(diff.normalized);

            float speed = (diff.sqrMagnitude * 100) / (timeDiff + 0.1f);

            //Debug.Log($"processing touch::: {swipeDir} : {speed}");
            SaveCurrentTouch();
            OnSwipe?.Invoke(swipeDir, new Vector2(diff.sqrMagnitude, speed));
        }

        private SwipeDirection GetDirection(Vector2 diff)
        {
            float angleInDegree = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            angleInDegree = angleInDegree < 0 ? 360 + angleInDegree : angleInDegree;

            SwipeDirection swipeDir = SwipeDirection.UP;
            if (angleInDegree >= 40 && angleInDegree < 140)
            {
                swipeDir = SwipeDirection.UP;
            }
            else if (angleInDegree >= 140 && angleInDegree < 230)
            {
                swipeDir = SwipeDirection.LEFT;
            }
            else if (angleInDegree >= 230 && angleInDegree < 320)
            {
                swipeDir = SwipeDirection.DOWN;
            }
            else if (angleInDegree >= 320 || (angleInDegree >= 0 && angleInDegree < 40))
            {
                swipeDir = SwipeDirection.RIGHT;
            }

            return swipeDir;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}