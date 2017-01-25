using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider2D))]
public class WorldEntity : MonoBehaviour {

    [SerializeField]
    private World world;

    [HideInInspector]
    public Collider2D hitbox;

	void Start () {
        hitbox = GetComponent<Collider2D>();
        world.Register(this);
	}

    void OnDestroy()
    {
        world.Deregesister(this);
    }
}
