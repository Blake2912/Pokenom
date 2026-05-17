using Godot;
using Pokenom.Scripts.Core;

namespace Pokenom.Scripts.Gameplay.Levels;

public partial class SpawnPoint : Node2D
{
	public override void _EnterTree()
	{
		AddToGroup(LevelGroup.SPAWNPOINTS.ToString());
	}

	public override void _ExitTree()
	{
		RemoveFromGroup(LevelGroup.SPAWNPOINTS.ToString());
	}
	
}
