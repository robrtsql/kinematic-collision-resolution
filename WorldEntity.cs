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

	void Start () {
		hitbox = GetComponent<Collider2D>();
		if (!world) { world = GameObject.FindGameObjectWithTag("World").GetComponent<World>(); }
		world.Register(this);
	}

	void OnDestroy()
	{
		world.Deregesister(this);
	}
}
