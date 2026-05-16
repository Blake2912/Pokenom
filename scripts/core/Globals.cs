using Godot;
using System;

namespace Pokenom.Scripts.Core;

public partial class Globals : Node
{
	public static Globals Instance { get; set; }

	[ExportCategory("Gameplay")]
	[Export] public int GRID_SIZE = 16;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;

		Logger.Debug("Loading Globals...");
		
	}
}
