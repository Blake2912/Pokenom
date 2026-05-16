using Godot;
using Pokenom.Scripts.Core;

namespace Pokenom.Scripts.Gameplay.Characters;

public partial class PlayerInput : CharacterInput
{
	[ExportCategory("Player Input")]
	[Export] 
	public double HoldThreshold = 0.1f;

	[Export]
	public double HoldTime = 0.0f;


	public override void _Ready()
	{
		CustomLogger.Debug("Loading Player input component...");
	}
}
