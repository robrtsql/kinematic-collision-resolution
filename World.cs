using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContactType
{
	None,
	Above,
	Below,
	Left,
	Right,
	Inside
}

public struct Contact
{
	public WorldEntity entity;
	public ContactType type;

	public Contact(WorldEntity entity, ContactType type)
	{
		this.entity = entity;
		this.type = type;
	}
}

public class World : MonoBehaviour {

	private HashSet<WorldEntity> obstacles;
	private HashSet<WorldEntity> triggers;

	void Start () {
	}

	void OnEnable() {
		Debug.Log("World enable");
		obstacles = new HashSet<WorldEntity>();
		triggers = new HashSet<WorldEntity>();
	}

	public void Register(WorldEntity entity)
	{
		switch (entity.type)
		{
			case EntityType.Obstacle:
				obstacles.Add(entity);
				break;
			case EntityType.Trigger:
				triggers.Add(entity);
				break;
			default:
				Debug.LogError("Failed to register WorldEntity: unrecognized or uninitialized EntityType");
				break;
		}
	}

	public void Deregesister(WorldEntity entity)
	{
		switch (entity.type)
		{
			case EntityType.Obstacle:
				obstacles.Remove(entity);
				break;
			case EntityType.Trigger:
				triggers.Remove(entity);
				break;
			default:
				Debug.LogError("Failed to register WorldEntity: unrecognized or uninitialized EntityType");
				Debug.LogError("Fallback behavior: assume is 'trigger'");
				triggers.Remove(entity);
				break;
		}
	}

	/**
	 * Tries to move the given object to the given destination. Returns the location where
	 * it was allowed to move, and populates the collision list with all of the collisions
	 * it had.
	 */
	public Vector3 Move(Collider2D mover, Vector3 destination, List<Contact> contacts)
	{
		var resolvedDestination = destination;

		var enumerator = obstacles.GetEnumerator();
		while (enumerator.MoveNext())
		{
			resolvedDestination = ResolveIntersectionIfAny(mover, resolvedDestination, enumerator.Current);
		}

		enumerator = obstacles.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var contact = FindContact(mover, resolvedDestination, enumerator.Current);
			if (contact.type != ContactType.None && contact.entity != null)
			{
				contacts.Add(contact);
			}
		}
		enumerator = triggers.GetEnumerator();
		while (enumerator.MoveNext())
		{
			var contact = FindContact(mover, resolvedDestination, enumerator.Current);
			if (contact.type != ContactType.None && contact.entity != null)
			{
				contacts.Add(contact);
			}
		}

		return resolvedDestination;
	}

	public Vector3 ResolveIntersectionIfAny(Collider2D mover, Vector3 destination, WorldEntity entity)
	{
		var moverBounds = mover.bounds;
		var entityBounds = entity.hitbox.bounds;

		bool isLeft = moverBounds.max.x <= entityBounds.min.x;
		bool isRight = moverBounds.min.x >= entityBounds.max.x;
		bool isAbove = moverBounds.min.y >= entityBounds.max.y;
		bool isBelow = moverBounds.max.y <= entityBounds.min.y;

		bool isColliding = !isLeft && !isRight && !isAbove && !isBelow;
		bool isHorizontallyAligned = !isAbove && !isBelow;
		bool isVerticallyAligned = !isLeft && !isRight;

		var destinationBounds = new Bounds(destination + new Vector3(mover.offset.x, mover.offset.y), moverBounds.size);

		bool isDestLeft = destinationBounds.max.x <= entityBounds.min.x;
		bool isDestRight = destinationBounds.min.x >= entityBounds.max.x;
		bool isDestAbove = destinationBounds.min.y >= entityBounds.max.y;
		bool isDestBelow = destinationBounds.max.y <= entityBounds.min.y;

		bool isDestColliding = !isDestLeft && !isDestRight && !isDestAbove && !isDestBelow;

		if (isDestColliding)
		{
			Vector3 newResolvedDestination = new Vector3(destination.x, destination.y);
			if (isHorizontallyAligned)
			{
				if (isLeft)
				{
					newResolvedDestination.x = entityBounds.min.x - moverBounds.extents.x - mover.offset.x;
					return newResolvedDestination;
				} else if (isRight)
				{
					newResolvedDestination.x = entityBounds.max.x + moverBounds.extents.x - mover.offset.x;
					return newResolvedDestination;
				}
			} else if (true || isVerticallyAligned)
			{
				if (isBelow)
				{
					newResolvedDestination.y = entityBounds.min.y - moverBounds.extents.y - mover.offset.y;
					return newResolvedDestination;
				}
				else if (isAbove)
				{
					newResolvedDestination.y = entityBounds.max.y + moverBounds.extents.y - mover.offset.y;
					return newResolvedDestination;
				}
			}
		}
		return destination;
	}

	private Contact FindContact(Collider2D mover, Vector3 resolvedDestination, WorldEntity entity)
	{
		var moverBounds = new Bounds(resolvedDestination + new Vector3(mover.offset.x, mover.offset.y), mover.bounds.size);
		var entityBounds = entity.hitbox.bounds;

		bool isLeft = moverBounds.max.x <= entityBounds.min.x;
		bool isRight = moverBounds.min.x >= entityBounds.max.x;
		bool isAbove = moverBounds.min.y >= entityBounds.max.y;
		bool isBelow = moverBounds.max.y <= entityBounds.min.y;
		bool isIntersecting = !isLeft && !isRight && !isAbove && !isBelow;
		if (isIntersecting)
		{
			return new Contact(entity, ContactType.Inside);
		}

		if (moverBounds.min.y == entityBounds.max.y && !isLeft && !isRight)
		{
			return new Contact(entity, ContactType.Above);
		}
		if (moverBounds.max.y == entityBounds.min.y && !isLeft && !isRight)
		{
			return new Contact(entity, ContactType.Below);
		}
		if (moverBounds.max.x == entityBounds.min.x && !isAbove && !isBelow)
		{
			return new Contact(entity, ContactType.Left);
		}
		if (moverBounds.min.x == entityBounds.max.x && !isAbove && !isBelow)
		{
			return new Contact(entity, ContactType.Right);
		}

		return new Contact(null, ContactType.None);
	}
}
