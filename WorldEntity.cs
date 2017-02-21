using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType {
	Obstacle,
	Trigger
}

[RequireComponent(typeof (Collider2D))]
public class WorldEntity : MonoBehaviour {

	[SerializeField]
	private World world;

	[SerializeField]
	public EntityType type;

	[HideInInspector]
	public Collider2D hitbox;

	private void RegisterWithWorld() {
		if (!world) { world = GameObject.FindGameObjectWithTag("World").GetComponent<World>(); }
		world.Register(this);
	}

	void Start () {
		hitbox = GetComponent<Collider2D>();
	}

	void OnEnable() {
		RegisterWithWorld();
	}

	void OnDisable()
	{
		world.Deregesister(this);
	}
}
