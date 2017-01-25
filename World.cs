using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    private HashSet<WorldEntity> objects;

	void Start () {
        objects = new HashSet<WorldEntity>();
	}

    public void Register(WorldEntity entity)
    {
        objects.Add(entity);
    }

    public void Deregesister(WorldEntity entity)
    {
        objects.Remove(entity);
    }

    public Vector3 Move(Collider2D mover, Vector3 destination)
    {
        var enumerator = objects.GetEnumerator();

        var resolvedDestination = destination;

        while (enumerator.MoveNext())
        {
            WorldEntity entity = enumerator.Current;
            resolvedDestination = ResolveIntersectionIfAny(mover, resolvedDestination, entity);
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
}
