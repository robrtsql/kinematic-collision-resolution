using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Collider2D))]
public class WorldObject : MonoBehaviour {

    [SerializeField]
    private GameObject world;
    private World worldComponent;

    [HideInInspector]
    public Collider2D hitbox;

	void Start () {
        hitbox = GetComponent<Collider2D>();
        worldComponent = world.GetComponent<World>();
        worldComponent.Register(this);
	}

    void OnDestroy()
    {
        worldComponent.Deregesister(this);
    }
}
