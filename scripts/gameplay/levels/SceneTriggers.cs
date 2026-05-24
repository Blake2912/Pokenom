using Godot;
using Pokenom.Scripts.Core;
using System;

namespace Pokenom.Scripts.Gameplay.Levels;
public partial class SceneTriggers : Area2D
{
	[ExportCategory("Target Scene Vars")]
	[Export]
	public LevelNames TargetLevelName;
	[Export]
	public int TargetLevelTrigger = 0;

	[ExportCategory("Current Scene Vars")]
	[Export]
	public int CurrentLevelTrigger = 0;
	[Export]
	public Vector2 EntryDirection;

	[Export]
	public Marker2D SpawnPoint;
	
	[Export]
	public bool Locked = false;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;

	}

	public void OnBodyEntered(Node2D body)
	{
		if (body.Name != "Player" || SceneManager.IsChanging)
		{
			return;
		}

		if (Locked)
		{
			CustomLogger.Warning("THE DOOR IS LOCKED");
		}

		SceneManager.ChangeLevel(levelName: TargetLevelName, trigger: TargetLevelTrigger);
	}

	public override void _EnterTree()
	{
		AddToGroup(LevelGroup.SCENETRIGGERS.ToString());
	}

	public override void _ExitTree()
	{
		RemoveFromGroup(LevelGroup.SCENETRIGGERS.ToString());
	}
}
