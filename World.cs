using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    private HashSet<WorldObject> objects;

	void Start () {
        objects = new HashSet<WorldObject>();
	}

    public void Register(WorldObject worldObject)
    {
        objects.Add(worldObject);
    }

    public void Deregesister(WorldObject worldObject)
    {
        objects.Remove(worldObject);
    }

    public Vector3 Move(Collider2D mover, Vector3 destination)
    {
        var enumerator = objects.GetEnumerator();

        var resolvedDestination = destination;

        while (enumerator.MoveNext())
        {
            WorldObject worldObject = enumerator.Current;
            resolvedDestination = ResolveIntersectionIfAny(mover, resolvedDestination, worldObject);
        }
        return resolvedDestination;
    }

    public Vector3 ResolveIntersectionIfAny(Collider2D mover, Vector3 resolvedDestination, WorldObject worldObject)
    {
        var moverBounds = mover.bounds;
        var worldObjectBounds = worldObject.hitbox.bounds;

        bool isLeft = moverBounds.max.x <= worldObjectBounds.min.x;
        bool isRight = moverBounds.min.x >= worldObjectBounds.max.x;
        bool isAbove = moverBounds.min.y >= worldObjectBounds.max.y;
        bool isBelow = moverBounds.max.y <= worldObjectBounds.min.y;

        bool isColliding = !isLeft && !isRight && !isAbove && !isBelow;
        bool isHorizontallyAligned = !isAbove && !isBelow;
        bool isVerticallyAligned = !isLeft && !isRight;

        var destinationBounds = new Bounds(resolvedDestination + new Vector3(mover.offset.x, mover.offset.y), moverBounds.size);

        bool isDestLeft = destinationBounds.max.x <= worldObjectBounds.min.x;
        bool isDestRight = destinationBounds.min.x >= worldObjectBounds.max.x;
        bool isDestAbove = destinationBounds.min.y >= worldObjectBounds.max.y;
        bool isDestBelow = destinationBounds.max.y <= worldObjectBounds.min.y;

        bool isDestColliding = !isDestLeft && !isDestRight && !isDestAbove && !isDestBelow;

        if (isDestColliding)
        {
            Vector3 newResolvedDestination = new Vector3(resolvedDestination.x, resolvedDestination.y);
            if (isHorizontallyAligned)
            {
                if (isLeft)
                {
                    newResolvedDestination.x = worldObjectBounds.min.x - moverBounds.extents.x - mover.offset.x;
                    return newResolvedDestination;
                } else if (isRight)
                {
                    newResolvedDestination.x = worldObjectBounds.max.x + moverBounds.extents.x - mover.offset.x;
                    return newResolvedDestination;
                }
            } else if (true || isVerticallyAligned)
            {
                if (isBelow)
                {
                    newResolvedDestination.y = worldObjectBounds.min.y - moverBounds.extents.y - mover.offset.y;
                    return newResolvedDestination;
                }
                else if (isAbove)
                {
                    newResolvedDestination.y = worldObjectBounds.max.y + moverBounds.extents.y - mover.offset.y;
                    return newResolvedDestination;
                }
            }
        }
        return resolvedDestination;
    }
}
