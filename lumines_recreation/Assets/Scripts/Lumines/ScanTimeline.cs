using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// A timeline continue moving from left to right based on BPM
    /// </summary>
    public class ScanTimeline : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Transform transTimeline;
        [SerializeField] private Transform transStart;
        [SerializeField] private Transform transEnd;
        [SerializeField] private BoardField boardField;
        
        [Header("Timeline Settings")]
        [SerializeField, ReadOnly] private float bpm = 100f;

        private bool _isMoving;

        public float Bpm => bpm;
        
        public void InitScanTimeline(float newBpm, BoardField newBoardField)
        {
            bpm = newBpm;
            boardField = newBoardField;
            
            ResetTimelineToStartPosition();
        }

        public void StartTimeline()
        {
            _isMoving = true;
        }

        public void StopTimeline()
        {
            _isMoving = false;
        }
        
        private void Update()
        {
            UpdateTimelineMoveStartToEndHorizontally();
        }
        
        private void UpdateTimelineMoveStartToEndHorizontally()
        {
            if (!_isMoving)
            {
                return;
            }
            
            var fullDistance = transEnd.position.x - transStart.position.x;
            // calculate movement by bpm
            var beatPerSecond = bpm / 60f;
            var fullMeasureDuration = beatPerSecond * 4f;
            var deltaDistance = Time.deltaTime / fullMeasureDuration * fullDistance;
            
            transTimeline.position += deltaDistance * Vector3.right;

            // timeline scan over the end position
            if (transTimeline.position.x >= transEnd.position.x)
            {
                ResetTimelineToStartPosition();
            }
        }

        [Button]
        public void ResetTimelineToStartPosition()
        {
            transTimeline.position = 
                new Vector3(transStart.position.x, transTimeline.position.y, transTimeline.position.z);
        }
    }
}