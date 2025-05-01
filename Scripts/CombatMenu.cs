using ChessDemonHand;
using Godot;

public partial class CombatMenu : Node
{
	private Player _player1;
	private Player _player2;

	public void ShowMenu(Player player1, Player player2)
	{
		_player1 = player1;
		_player2 = player2;

		// Mostrar opciones de piedra, papel o tijeras
		// Implementa la lógica para mostrar el menú y capturar la elección
	}

	public void PlayerChoice(Player player, string choice)
	{
		// Guardar la elección del jugador
		// Lógica para eliminar una opción aleatoriamente y determinar el ganador
		// Llamar a EliminatePlayer si es necesario
	}

	private void EliminatePlayer(Player loser)
	{
		// Eliminar al jugador perdedor
		var turnManager = GetNode<TurnManager>("/root/TableGame/TurnManager");
		turnManager.RemovePlayer(loser);

		// Verificar si solo queda un jugador
		if (turnManager.GetPlayers().Count == 1)
		{
			// Declarar al ganador y reiniciar el juego
			var winner = turnManager.GetCurrentPlayer();
			GD.Print($"{winner.PlayerName} wins!");
			turnManager.RestartGame();
		}
	}
}
