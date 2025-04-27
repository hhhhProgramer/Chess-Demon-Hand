using Godot;
using System;
using System.Collections.Generic;

namespace ChessDemonHand
{
	public partial class TurnManager : Node
	{
		[Export]
		private int NumberOfPlayers = 2;
		
		[Export]
		private PackedScene PlayerScene;
		
		private List<Player> _players = new List<Player>();
		private int _currentPlayerIndex = 0;
		
		private Timer _turnDelayTimer;
		
		[Signal]
		public delegate void TurnChangedEventHandler(Player currentPlayer);
		
		public override void _Ready()
		{
			if (PlayerScene == null)
			{
				GD.PrintErr("PlayerScene is not assigned in the editor!");
				return;
			}
			
			_turnDelayTimer = new Timer();
			_turnDelayTimer.WaitTime = 1.0f; // 5 segundos
			_turnDelayTimer.OneShot = true; // Solo se ejecuta una vez
			_turnDelayTimer.Connect("timeout", new Callable(this, nameof(OnTurnDelayTimeout)));
			AddChild(_turnDelayTimer); // Agregar el Timer al nodo
			CallDeferred(nameof(CreatePlayers));
		}
		
		private void CreatePlayers()
		{
			// Colores predefinidos para los jugadores
			Color[] playerColors = new Color[]
			{
				new Color(1, 0, 0, 1), // Rojo
				new Color(0, 0, 1, 1), // Azul
				new Color(0, 1, 0, 1), // Verde
				new Color(1, 1, 0, 1), // Amarillo
				new Color(1, 0, 1, 1), // Magenta
				new Color(0, 1, 1, 1)  // Cyan
			};
			
			for (int i = 0; i < NumberOfPlayers; i++)
			{
				// Crear nuevo jugador
				var player = PlayerScene.Instantiate<Player>();
				if (player == null)
				{
					GD.PrintErr($"Failed to instantiate player {i + 1}");
					continue;
				}
				
				player.PlayerName = $"Player {i + 1}";
				player.PlayerColor = playerColors[i % playerColors.Length];
				
				// Agregar a la escena y al manager
				GetParent().CallDeferred("add_child", player);
				AddPlayer(player);
				
				GD.Print($"Created player {player.PlayerName} with color {player.PlayerColor}");
			}
			
			CallDeferred(nameof(StartGame));
		}
		
		private void StartGame()
		{
			if (_players.Count == 0)
			{
				GD.PrintErr("No players were created!");
				return;
			}
			
			// Inicializar el primer turno
			UpdateCurrentPlayer();
		}
		
		public void EndTurn()
		{
			// Iniciar el Timer
			// Desactivar el turno de todos los jugadores
			foreach (var player in _players)
			{
				player.SetTurn(false);
			}
			_turnDelayTimer.Start();
		}
		
		private void UpdateCurrentPlayer()
		{
			// Activar el turno del jugador actual
			var currentPlayer = _players[_currentPlayerIndex];
			currentPlayer.SetTurn(true);
			
			// Emitir señal de cambio de turno
			EmitSignal(SignalName.TurnChanged, currentPlayer);
		}
		
		public Player GetCurrentPlayer()
		{
			return _players[_currentPlayerIndex];
		}
		
		public int GetCurrentPlayerIndex()
		{
			return _currentPlayerIndex;
		}
		
		public List<Player> GetPlayers()
		{
			return _players;
		}
		
		public void AddPlayer(Player player)
		{
			_players.Add(player);
			player.AddToGroup("players");
		}
		
		private void OnTurnDelayTimeout()
		{
			_currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
			UpdateCurrentPlayer();
		}
	}
} 
