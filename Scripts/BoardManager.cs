using Godot;
using System;
using System.Collections.Generic;

namespace ChessDemonHand
{
	public partial class BoardManager : Node2D
	{
		[Export]
		private int BoardWidth = 5;
		
		[Export]
		private int BoardHeight = 5;
		
		private Vector2[,] _boardPositions;
		private Node2D _boardContainer;
		private float _cellSize;
		
		public override void _Ready()
		{
			// Get the viewport size
			var viewport = GetViewport();
			var screenSize = viewport.GetVisibleRect().Size;
			
			// Calculate cell size based on the smaller dimension to ensure the board fits
			_cellSize = Mathf.Min(screenSize.X / BoardWidth, screenSize.Y / BoardHeight);
			
			InitializeBoard();
			CreateBoardVisual();
		}
		
		private void InitializeBoard()
		{
			_boardPositions = new Vector2[BoardWidth, BoardHeight];
			_boardContainer = new Node2D();
			AddChild(_boardContainer);
			
			// Calculate board positions starting from (0,0)
			for (int x = 0; x < BoardWidth; x++)
			{
				for (int y = 0; y < BoardHeight; y++)
				{
					_boardPositions[x, y] = new Vector2(
						x * _cellSize,
						y * _cellSize
					);
				}
			}
		}
		
		private void CreateBoardVisual()
		{
			// Create board background
			var boardBackground = new ColorRect
			{
				Size = new Vector2(BoardWidth * _cellSize, BoardHeight * _cellSize),
				Color = new Color(0.2f, 0.2f, 0.2f)
			};
			_boardContainer.AddChild(boardBackground);
			
			// Create grid lines
			for (int i = 0; i <= BoardWidth; i++)
			{
				// Vertical lines
				var vLine = new Line2D
				{
					Width = 2,
					DefaultColor = Colors.White
				};
				vLine.AddPoint(new Vector2(i * _cellSize, 0));
				vLine.AddPoint(new Vector2(i * _cellSize, BoardHeight * _cellSize));
				_boardContainer.AddChild(vLine);
			}
			
			for (int i = 0; i <= BoardHeight; i++)
			{
				// Horizontal lines
				var hLine = new Line2D
				{
					Width = 2,
					DefaultColor = Colors.White
				};
				hLine.AddPoint(new Vector2(0, i * _cellSize));
				hLine.AddPoint(new Vector2(BoardWidth * _cellSize, i * _cellSize));
				_boardContainer.AddChild(hLine);
			}
		}
		
		public Vector2 GetCellPosition(int x, int y)
		{
			if (x < 0 || x >= BoardWidth || y < 0 || y >= BoardHeight)
				return Vector2.Zero;
				
			return _boardPositions[x, y];
		}
		
		public bool IsValidPosition(int x, int y)
		{
			return x >= 0 && x < BoardWidth && y >= 0 && y < BoardHeight;
		}
		
		public float GetCellSize()
		{
			return _cellSize;
		}
	}
} 
