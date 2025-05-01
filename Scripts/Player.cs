using Godot;
using System;
using System.Collections.Generic;

namespace ChessDemonHand
{
	public partial class Player : Node2D
	{
		[Export]
		public string PlayerName { get; set; }

		[Export]
		public Color PlayerColor { get; set; }

		public Vector2I CurrentPosition { get; set; }
		public MovementType CurrentMovementType { get; private set; }

		[Export]
		public bool IsMyTurn { get; private set; }

		private BoardManager _boardManager;
		private Sprite2D _playerSprite;
		private TurnManager _turnManager;
		private Popup _movementTypePopup;
		private VBoxContainer _optionsContainer;

		public override void _Ready()
		{
			_boardManager = GetNode<BoardManager>("/root/TableGame/BoardManager");
			_playerSprite = GetNode<Sprite2D>("Sprite2D");
			_playerSprite.Modulate = PlayerColor;
			_turnManager = GetNode<TurnManager>("/root/TableGame/TurnManager");
			// Create the Popup
			_movementTypePopup = new Popup();
			_optionsContainer = new VBoxContainer();

			// Add the VBoxContainer to the Popup
			_movementTypePopup.AddChild(_optionsContainer);

			// Add the Popup to the current node (Player)
			AddChild(_movementTypePopup);

			// Conectar los botones de movimiento
			foreach (MovementType type in Enum.GetValues(typeof(MovementType)))
			{
				if (type != CurrentMovementType)
				{
					Button button = new Button { Text = type.ToString() };

					var movementTypeCopy = type; // Evita problemas de captura en bucles
					button.Pressed += () => OnMovementTypeSelected(movementTypeCopy);
					_optionsContainer.AddChild(button);
				}
			}

			// Tipo de movimiento inicial: Torre
			CurrentMovementType = MovementType.Rook;
			GD.Print($"Player {PlayerName} ready with initial position {CurrentPosition} and movement type {CurrentMovementType}");
		}

		public void SetTurn(bool isMyTurn)
		{
			IsMyTurn = isMyTurn;
			GD.Print($"Player {PlayerName} turn set to {IsMyTurn}");
			// Aquí podrías agregar efectos visuales para indicar el turno
		}

		public override void _Input(InputEvent @event)
		{
			if (@event is InputEventMouseButton eventMouseButton)
			{
				if (eventMouseButton.ButtonIndex == MouseButton.Right && IsMyTurn)
				{
					// Check if the click is on the player
					GD.Print($"Mouse clicked for options player {PlayerName}");
					ShowMovementOptions(); // Show the movement options popup

					Rect2 playerRect = new Rect2(GlobalPosition - (_playerSprite.Texture.GetSize() / 2), _playerSprite.Texture.GetSize());
					if (playerRect.HasPoint(eventMouseButton.GlobalPosition))
					{
					}
				}
				else if (IsMyTurn)
				{
					GD.Print($"Mouse clicked for player {PlayerName}");
					Vector2I newPosition = new((int)(eventMouseButton.Position.X / _boardManager.GetCellSize()), (int)(eventMouseButton.Position.Y / _boardManager.GetCellSize()));
					GD.Print($"Calculated new position: {newPosition}");
					TryMove(newPosition);
				}
				else
				{
					GD.Print($"It's not player {PlayerName}'s turn");
				}
			}
		}

		public bool TryMove(Vector2I newPosition)
		{
			if (!IsMyTurn)
			{
				GD.Print($"Player {PlayerName} cannot move, it's not their turn");
				return false;
			}
			if (!IsValidMove(newPosition))
			{
				GD.Print($"Player {PlayerName} cannot move to {newPosition}, invalid move");
				return false;
			}

			CurrentPosition = newPosition;
			UpdatePosition();
			GD.Print($"Player {PlayerName} moved to {CurrentPosition}");

			// Llama a EndTurn en el TurnManager
			_turnManager.EndTurn();
			return true;
		}

		public bool TryChangeMovementType(MovementType newType)
		{
			if (!IsMyTurn)
			{
				GD.Print($"Player {PlayerName} cannot change movement type, it's not their turn");
				return false;
			}

			CurrentMovementType = newType;
			GD.Print($"Player {PlayerName} changed movement type to {CurrentMovementType}");
			return true;
		}

		private bool IsValidMove(Vector2I newPosition)
		{
			if (!_boardManager.IsValidPosition(newPosition.X, newPosition.Y))
				return false;

			var deltaX = Math.Abs(newPosition.X - CurrentPosition.X);
			var deltaY = Math.Abs(newPosition.Y - CurrentPosition.Y);

			switch (CurrentMovementType)
			{
				case MovementType.Knight:
					// Movimiento en L: 2 en una dirección y 1 en la otra
					return (deltaX == 2 && deltaY == 1) || (deltaX == 1 && deltaY == 2);

				case MovementType.Bishop:
					// Movimiento diagonal, máximo 2 casillas
					return deltaX == deltaY && deltaX <= 2;

				case MovementType.Rook:
					// Movimiento horizontal o vertical, máximo 2 casillas
					if (deltaX == 0 && deltaY <= 2 && deltaY > 0) // Movimiento vertical
						return true;
					if (deltaY == 0 && deltaX <= 2 && deltaX > 0) // Movimiento horizontal
						return true;
					return false;

				default:
					GD.Print($"Player {PlayerName} has an invalid movement type {CurrentMovementType}");
					return false;
			}
		}

		public void UpdatePosition()
		{
			Position = _boardManager.GetCellPosition(CurrentPosition.X, CurrentPosition.Y);
			GD.Print($"Player {PlayerName} position updated to {CurrentPosition}");
		}

		// Método para mostrar el Popup
		private void ShowMovementOptions()
		{
			GD.Print($"show options {PlayerName}");
			_movementTypePopup.Popup();
		}

		// Método para manejar la selección del tipo de movimiento
		private void OnMovementTypeSelected(MovementType newType)
		{
			TryChangeMovementType(newType);
			_movementTypePopup.Hide(); // Oculta el Popup después de seleccionar
			_turnManager.EndTurn();
		}
	}
}
