using Godot;
using System;

namespace ChessDemonHand
{
	public partial class GameGUI : Control
	{
		private TurnManager _turnManager;
		private Label _currentPlayerLabel;
		private Label _movementTypeLabel;
		private Label _turnNumberLabel;
		private VBoxContainer _playerInfoContainer;
		
		public override void _Ready()
		{
			_turnManager = GetNode<TurnManager>("/root/TableGame/TurnManager");
			_currentPlayerLabel = GetNode<Label>("PanelContainer/VBoxContainer/CurrentPlayerLabel");
			_movementTypeLabel = GetNode<Label>("PanelContainer/VBoxContainer/MovementTypeLabel");
			_turnNumberLabel = GetNode<Label>("PanelContainer/VBoxContainer/TurnNumberLabel");
			_playerInfoContainer = GetNode<VBoxContainer>("PanelContainer/VBoxContainer/PlayerInfoContainer");
			
			// Conectar señales
			_turnManager.TurnChanged += OnTurnChanged;
			
			// Posicionar el GUI en la parte superior derecha
			AnchorRight = 1.0f;
			AnchorBottom = 0.3f;

		}
		
		private void OnTurnChanged(Player currentPlayer)
		{
			UpdateGUI(currentPlayer);
		}
		
		private void UpdateGUI(Player currentPlayer)
		{
			_currentPlayerLabel.Text = $"Current Player: {currentPlayer.PlayerName}";
			_movementTypeLabel.Text = $"Movement Type: {currentPlayer.CurrentMovementType}";
			_turnNumberLabel.Text = $"Turn: {_turnManager.GetCurrentPlayerIndex() + 1}";
			
			UpdatePlayerInfo();
		}
		
		private void UpdatePlayerInfo()
		{
			// Limpiar información anterior
			foreach (var child in _playerInfoContainer.GetChildren())
			{
				child.QueueFree();
			}
			
			// Agregar información de cada jugador
			foreach (var player in _turnManager.GetPlayers())
			{
				var playerInfo = new Label();
				playerInfo.Text = $"{player.PlayerName}: {player.CurrentMovementType}";
				playerInfo.Modulate = player.PlayerColor;
				_playerInfoContainer.AddChild(playerInfo);
			}
		}
	}
} 