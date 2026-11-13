using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// main lumines game system.
    /// manage inputs, piece spawn, piece locked, and scan timeline
    /// 
    /// </summary>
    public class LuminesGame : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private BoardField _boardField;
        [SerializeField] private InputHandler _inputHandler;

        [Header("Gameplay Settings")] 
        [SerializeField] private float dasWindow;
        [SerializeField] private float dasInterval = 0.1f;
        [SerializeField] private float continueMoveInterval = 0.04f;
        
        private Piece _currentPiece;
        private Queue<Piece> _queue = new Queue<Piece>();


        private readonly float _controllerInputThreshold = 0.05f;

        private bool _isMoveInputEnter;
        private float _lastMoveInputTime;
        
        private void Awake()
        {
            
        }

        private void Update()
        {
            HandleDelayedAutoShift();
        }

        #region Input
        
        private void RegisterInputEvents()
        {
            _inputHandler.MovePiece += OnMovePiece;
            _inputHandler.RotateCw += OnRotateCw;
            _inputHandler.RotateCcw += OnRotateCcw;
        }

        private void OnRotateCcw()
        {
            TryRotateCcwCurrentPiece();
        }

        private void OnRotateCw()
        {
            TryRotateCwCurrentPiece();
        }
        
        private void OnMovePiece(Vector2 axis)
        {
            // controller noise prevent
            if (Mathf.Abs(axis.sqrMagnitude) < _controllerInputThreshold * _controllerInputThreshold)
            {
                CancelMoveInput();
                return;
            }

            // if current piece is moving, then ignore input
            // todo : we should handle the input buffer
            if (_currentPiece.IsMoving)
            {
                
            }
            
            // a valuable move input enter
            _isMoveInputEnter = true;
            _lastMoveInputTime = Time.time;
        }

        #endregion
        
        #region Gameplay

        /// <summary>
        /// DAS stuffs
        /// </summary>
        private void HandleDelayedAutoShift()
        {

            if (_inputHandler.MoveInput.x > _controllerInputThreshold)
            {
                TryMoveRightCurrentPiece();
            }
            else
            {
                TryMoveLeftCurrentPiece();
            }
        }

        private void CancelMoveInput()
        {
            _isMoveInputEnter = false;
            _lastMoveInputTime = -1f;
        }

        public void TryMoveLeftCurrentPiece()
        {
            _currentPiece.MoveLeft();
        }
        
        public void TryMoveRightCurrentPiece()
        {
            _currentPiece.MoveRight();
        }

        public void TryRotateCcwCurrentPiece()
        {
            _currentPiece.TryRotate(true);
        }
        
        public void TryRotateCwCurrentPiece()
        {
            _currentPiece.TryRotate(false);
        }
        
        #endregion

        #region Piece Management

        private void SpawnPiece()
        {
            
        }

        private void OnCurrentPieceLocked(Piece piece)
        {
            SpawnPiece();
            FillTheNexPieceQueue();
        }

        private void FillTheNexPieceQueue()
        {
            
        }

        #endregion
    }
}
