using Godot;
using Pokenom.Scripts.Utilites;
using System;

namespace Pokenom.Scripts.Gameplay.Characters;

public partial class Player : CharacterBody2D
{
	[Export]
	public StateMachine StateMachine;

	public override void _Ready()
	{
		StateMachine.Customer = this;
		StateMachine.ChangeState(StateMachine.GetNode<State>("Roam"));
	}	
}
