using UnityEngine;

public static class Extensions
{
    private static LayerMask layerMask = LayerMask.GetMask("Default");
    public static bool Raycast(this Rigidbody2D rigidbody, Vector2 direction)
    {
        if (rigidbody.isKinematic)
        {
            return false;
        }

        float radius = 0.25f;
        float distance = 1f;

        RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance, layerMask);
        
        return hit.collider != null && hit.rigidbody != rigidbody;
    }

    public static bool DotTest (this Transform transform, Transform other, Vector2 testDirection)
    {
        Vector2 direction = other.position - transform.position; //other.postion == block, transform.positon == mario
        return Vector2.Dot(direction.normalized, testDirection) > 0.25f; //1 == they're exactly the same, 0 == they're exactly perpendicular to another, -1 == they're exactly opposites of each other
    }
}
