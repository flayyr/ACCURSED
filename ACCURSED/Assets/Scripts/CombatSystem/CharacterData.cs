using UnityEngine;

public class CharacterData : MonoBehaviour
{
    //current direction, cannot be zero
    private Vector2 currDirection = Vector2.down;
    //currently inputted direction, can be zero
    public Vector2 inputDirection {  get; private set; }
    //current direction translated into the othogonal directions
    public Vector2 facingDirection { 
        get {
            Vector2[] orthogonalDirections = {Vector2.up, Vector2.right, Vector2.down, Vector2.left};

            Vector2 output = orthogonalDirections[0];
            float largestDot = Vector2.Dot(output, currDirection);

            for (int i = 1; i < orthogonalDirections.Length; i++) {
                if (Vector2.Dot(orthogonalDirections[i], currDirection) > largestDot) {
                    output = orthogonalDirections[i];
                }
            }

            return output;
        }
    }

    private void Start() {
        
    }
}
