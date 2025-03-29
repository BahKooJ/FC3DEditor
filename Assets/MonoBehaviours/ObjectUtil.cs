

using FCopParser;
using UnityEngine;

public abstract class ObjectUtil {

    public static float GroundCast(ActorGroundCast groundCast, Vector2 position) {

        Vector3 castDirection = Vector3.down;
        float startingHeight = 100f;

        switch (groundCast) {
            case ActorGroundCast.Highest:
                break;
            case ActorGroundCast.Lowest:
                castDirection = Vector3.up;
                startingHeight = -100f;
                break;
            case ActorGroundCast.Default:
                break;
        }

        var castPos = new Vector3(position.x, startingHeight, position.y);

        if (Physics.Raycast(castPos, castDirection, out RaycastHit hit, Mathf.Infinity, 1)) {

            return hit.point.y;

        }
        else {

            return 6f;

        }

    }

}