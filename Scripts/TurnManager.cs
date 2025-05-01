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
		
		private List<Player> _players = new();
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
			// Obtener el BoardManager
			var boardManager = GetNode<BoardManager>("/root/TableGame/BoardManager");
			
			// Obtener el tamaño de las celdas
			float cellSize = boardManager.GetCellSize();
			
			// Obtener el ancho y alto del tablero
			int boardWidth = boardManager.BoardWidth; // Asegúrate de que sea público
			int boardHeight = boardManager.BoardHeight; // Asegúrate de que sea público

			// Definir posiciones para hasta 4 jugadores
			Vector2I[] playerPositions = new Vector2I[]
			{
				new((int)((boardWidth - 1) * cellSize / cellSize), 0), // Jugador 1: Superior derecha
				new(0, (int)((boardHeight - 1) * cellSize / cellSize)), // Jugador 2: Inferior izquierda
				new(0, 0), // Jugador 3: Superior izquierda
				new((int)((boardWidth - 1) * cellSize / cellSize), (int)((boardHeight - 1) * cellSize / cellSize)) // Jugador 4: Inferior derecha
			};

			// Colores predefinidos para los jugadores
			Color[] playerColors =
			[
				new(1, 0, 0, 1), // Rojo
				new(0, 0, 1, 1), // Azul
				new(0, 1, 0, 1), // Verde
				new(1, 1, 0, 1), // Amarillo
				new(1, 0, 1, 1), // Magenta
				new(0, 1, 1, 1)  // Cyan
			];

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
				player.CurrentPosition = playerPositions[i];
				
				// Agregar a la escena y al manager
				GetParent().CallDeferred("add_child", player);
				AddPlayer(player, playerPositions[i]);
				GD.Print($"Created player {player.PlayerName} with color {player.PlayerColor} at position {player.Position}");
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
			
			foreach (var player in _players)
			{
				player.UpdatePosition();
			}
			
			// Inicializar el primer turno
			UpdateCurrentPlayer();
		}
		
		public void EndTurn()
		{
			// Iniciar el Timer
			// Desactivar el turno de todos los jugadores

			if(CheckForCombat()){
				return;
			}

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
		
		public void AddPlayer(Player player, Vector2I position)
		{
			_players.Add(player);
			player.AddToGroup("players");

		}
		
		private void OnTurnDelayTimeout()
		{
			_currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
			UpdateCurrentPlayer();
		}
		
		private bool CheckForCombat()
		{
			// Get all players in the game
			var players = GetTree().GetNodesInGroup("players"); // Assuming players are added to a group called "players"
			var currentPlayer = GetCurrentPlayer();

			foreach (Player otherPlayer in players)
			{
				if (otherPlayer != currentPlayer && otherPlayer.CurrentPosition == currentPlayer.CurrentPosition && !otherPlayer.IsMyTurn)
				{
					GD.Print($"Combat initiated between {currentPlayer.PlayerName} and {otherPlayer.PlayerName}");
					StartCombat(otherPlayer); // Call StartCombat in TurnManager
					return true;
				}
			}

			return false;
		}

		public void StartCombat(Player otherPlayer)
		{
			var currentPlayer = GetCurrentPlayer();
			var combatMenu = GetNode<CombatMenu>("/root/TableGame/CombatMenu");
			combatMenu.ShowMenu(currentPlayer, otherPlayer);
		}
		
		public void RemovePlayer(Player player)
		{
			_players.Remove(player);
			player.QueueFree(); // Eliminar el nodo del juego
		}
		
		public void RestartGame()
		{
			// Lógica para reiniciar el juego
			GD.Print("Restarting the game...");
			// Aquí puedes reiniciar el estado del juego, volver a crear jugadores, etc.
		}
	}

} 
