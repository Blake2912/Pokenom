using Godot;

namespace Pokenom.Scripts.Gameplay.Characters;

public partial class CharacterInput : Node
{
	[Signal]
	public delegate void WalkEventHandler();

	[Signal]
	public delegate void TurnEventHandler();

	[ExportCategory("Common Inputs")]
	[Export]
	public Vector2 Direction = Vector2.Zero;
	[Export]
	public Vector2 TargetPosition = Vector2.Zero;

}
