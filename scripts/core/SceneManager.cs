using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Pokenom.Scripts.Gameplay.Characters;
using Pokenom.Scripts.Gameplay.Levels;

namespace Pokenom.Scripts.Core;

public partial class SceneManager : Node
{
	public static SceneManager Instance { get; private set; }
	public static bool IsChanging { get; private set; }

	[ExportCategory("Scene Manager Vars")]
	[Export]
	public ColorRect FadeRect;

	[Export]
	public Level CurrentLevel;

	[Export]
	public Array<Level> AllLevels;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		IsChanging = false;
		CustomLogger.Debug("Loading SceneManager");

	}

	public static async void ChangeLevel(LevelNames levelName = LevelNames.smalltown, int trigger = 0, bool spawn = false)
	{
		while (IsChanging)
		{
			await Instance.ToSignal(Instance.GetTree(), "process_frame");
		}

		await Instance.GetLevel(levelName);

		IsChanging = true;

		if (spawn)
		{
			Instance.Spawn();
		}
		else
		{
			Instance.Switch(trigger);
		}

		await Instance.FadeIn();

		IsChanging = false;

	}

	public async Task GetLevel(LevelNames levelName)
	{
		if (CurrentLevel != null)
		{
			await Instance.FadeOut();
			GameManager.GetGameViewPort().RemoveChild(CurrentLevel);

		}

		CurrentLevel = AllLevels.FirstOrDefault(x => x.LevelName == levelName);

		if (CurrentLevel != null)
		{
			GameManager.GetGameViewPort().AddChild(CurrentLevel);
		}
		else
		{
			CurrentLevel = GD.Load<PackedScene>("res://scenes/levels/"+levelName+".tscn").Instantiate<Level>();
			AllLevels.Add(CurrentLevel);
			GameManager.GetGameViewPort().AddChild(CurrentLevel);
		}
	}

	public void Spawn()
	{
		var spawnPoints = CurrentLevel.GetTree().GetNodesInGroup(LevelGroup.SPAWNPOINTS.ToString());
		if (spawnPoints.Count <= 0)
		{
			throw new Exception("Spawn points are missing");
		}

		var spawnPoint = (SpawnPoint)spawnPoints[0];
		var player = GD.Load<PackedScene>("res://scenes/characters/player.tscn").Instantiate<Player>();

		GameManager.AddPlayer(player).Position = spawnPoint.Position;
	}

	public void Switch(int trigger)
	{
		var sceneTriggers = CurrentLevel.GetTree().GetNodesInGroup(LevelGroup.SCENETRIGGERS.ToString());
		if (sceneTriggers.Count <= 0)
		{
			throw new Exception("Scene Trigger are missing");
		}

		if (sceneTriggers.FirstOrDefault(st => ((SceneTriggers)st)?.CurrentLevelTrigger == trigger) is not SceneTriggers sceneTrigger)
		{
			throw new Exception ($"Missing scene trigger {trigger}");
		}

		GameManager.GetPlayer().Position = sceneTrigger.Position + sceneTrigger.EntryDirection * Globals.Instance.GRID_SIZE;
	}

	public async Task FadeOut()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(FadeRect, "color:a", 1.0, 0.25);
		await ToSignal(tween, "finished");
	}

		public async Task FadeIn()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(FadeRect, "color:a", 0.0, 0.25);
		await ToSignal(tween, "finished");
	}
}
