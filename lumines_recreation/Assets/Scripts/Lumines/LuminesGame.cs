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
        [Header("Resource")] 
        [SerializeField] private Piece piecePrefab;
        
        [Header("Components")]
        [SerializeField] private BoardField boardField;
        [SerializeField] private InputHandler inputHandler;

        [Header("Gameplay Settings")] 
        [SerializeField] private float dasWindow = 0.1f;
        [SerializeField] private float dasBuffer = 0.1f;
        [SerializeField] private float continueMoveInterval = 0.04f;
        
        private Piece _currentPiece;
        private Queue<Piece> _queue = new Queue<Piece>();
        
        private readonly float _controllerInputThreshold = 0.05f;

        private bool _isMoveInputEnter;
        private float _lastMoveInputEnterTime = -1f;
        
        private void Awake()
        {
            RegisterInputEvents();
            InitGame();
        }

        private void InitGame()
        {
            _currentPiece = SpawnPiece();
            _currentPiece.transform.position = boardField.PieceSpawnPosition;
        }

        private void Update()
        {
            HandleDelayedAutoShift();
        }

        #region Input
        
        private void RegisterInputEvents()
        {
            inputHandler.MovePiece += OnMovePiece;
            inputHandler.RotateCw += OnRotateCw;
            inputHandler.RotateCcw += OnRotateCcw;
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
            Debug.Log($"Move - {axis}");
            
            // controller noise prevent
            if (Mathf.Abs(axis.sqrMagnitude) < _controllerInputThreshold * _controllerInputThreshold)
            {
                CancelMoveInput();
                _currentPiece?.EndDas();
                return;
            }

            // if current piece is moving, then input come in
            // todo : we should handle the input buffer
            if (_currentPiece.IsMoving)
            {
                // todo : move input buffer
                Debug.Log($"Piece is Moving, Skipped");
                
                return;
            }
            
            Debug.Log($"Move Valid - {axis}");
            
            // a valuable move input enter
            _isMoveInputEnter = true;
            _lastMoveInputEnterTime = Time.time;
            
            // move the piece and start das
            if (inputHandler.MoveInput.x > _controllerInputThreshold)
            {
                TryMoveRightCurrentPiece();
            }
            else
            {
                TryMoveLeftCurrentPiece();
            }
        }

        #endregion
        
        #region Gameplay

        /// <summary>
        /// DAS stuffs
        /// </summary>
        private void HandleDelayedAutoShift()
        {
            var interval = Time.time - _lastMoveInputEnterTime;
            if (_currentPiece == null || !_isMoveInputEnter)
            { 
                return;
            }

            // Check current time is enough for DAS movement
            if (Time.time - _lastMoveInputEnterTime <= dasBuffer)
            {
                Debug.LogError(interval);
                return;
            }

            // if current piece is starting DAS movement, return
            if (_currentPiece.IsDasMoving)
            {
                return;
            }
            
            var direction = (int)Mathf.Sign(inputHandler.MoveInput.x);
            _currentPiece?.StartDas(direction);
            
            // if (inputHandler.MoveInput.x > _controllerInputThreshold)
            // {
            //     TryMoveRightCurrentPiece();
            // }
            // else
            // {
            //     TryMoveLeftCurrentPiece();
            // }
        }

        private void CancelMoveInput()
        {
            _isMoveInputEnter = false;
            _lastMoveInputEnterTime = -1f;
        }

        public void TryMoveLeftCurrentPiece()
        {
            if(!_isMoveInputEnter)
                return;
                
            _currentPiece.MoveLeft();
        }
        
        public void TryMoveRightCurrentPiece()
        {
            if(!_isMoveInputEnter)
                return;
            
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

        private Piece SpawnPiece()
        {
            var piece = Instantiate(piecePrefab, boardField.PieceContainer, true);
            piece.Init(boardField);
            piece.transform.position = boardField.PieceSpawnPosition;
            return piece;
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
