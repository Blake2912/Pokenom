using Godot;
using Pokenom.Scripts.Core;
using System;

namespace Pokenom.Scripts.Gameplay.Levels;

public partial class Level : Node2D
{

	[ExportCategory("Level Vars")]
	[Export]
	public LevelNames LevelName;

	[ExportCategory("Camera Limits")]
	[Export]
	public int Top;
	[Export]
	public int Bottom;
	[Export]
	public int Left;
	[Export]
	public int Right;

	public override void _Ready()
	{
		CustomLogger.Debug($"Loading level {LevelName}");
	}
}
