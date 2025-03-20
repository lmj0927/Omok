using UnityEngine;

public class Stone : MonoBehaviour
{
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] MeshCollider meshCollider;
    
    public Constants.CELL_TYPE stoneType;
    private bool isDisposable = true;
    
    void Update()
    {
        if (isDisposable)
        {
            if (transform.position.y < -10)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetKinematic(bool isKinematic)
    {
        meshCollider.enabled = !isKinematic;
        rigidbody.isKinematic = isKinematic;
        
        isDisposable = !isKinematic;
    }
}