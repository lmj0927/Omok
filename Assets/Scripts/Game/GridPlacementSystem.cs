using UnityEngine;
using System.Collections.Generic;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField] GameObject[] objectPrefab;
    [SerializeField] float gridSize = 0.2f;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Material previewMaterial;
    [SerializeField] Transform gridParent;
    [SerializeField] LayerMask layerMask;
    
    Dictionary<Vector3Int, GameObject> placedObjects = new Dictionary<Vector3Int, GameObject>();
    Camera mainCamera;
    
    // 프리뷰 관련 변수
    GameObject previewObject;
    Vector3Int currentGridPosition;
    bool canPlace = true;
    bool isReplacing = false;
    
    int selectedPrefabType = 0;
    
    void Start()
    {
        mainCamera = Camera.main;
        CreatePreviewObject();
    }
    
    void CreatePreviewObject()
    {
        if (objectPrefab.Length == 0)
        {
            Debug.LogError("프리팹이 없습니다.");
            return;
        }

        if(previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = Instantiate(objectPrefab[selectedPrefabType]);
        

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material previewMat = new Material(previewMaterial);
            previewMat.color = new Color(1f, 1f, 1f, 0.5f);
            renderer.material = previewMat;
        }
        

        Collider[] colliders = previewObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        
        previewObject.SetActive(false);
    }
    
    void Update()
    {
        UpdatePreview();
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            isReplacing = !isReplacing;

            if (previewObject.activeSelf)
            {
                UpdatePreviewState();
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            PlaceStone();
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedPrefabType = Mathf.Clamp(selectedPrefabType - 1, 0, objectPrefab.Length - 1);
            CreatePreviewObject();
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedPrefabType = Mathf.Clamp(selectedPrefabType + 1, 0, objectPrefab.Length - 1);
            CreatePreviewObject();
        }
    }
    
    void UpdatePreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        int layerMask = ~(1 << previewObject.layer);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Vector3 worldPosition = hit.point;
            Vector3Int newGridPosition = WorldToGrid(worldPosition);
            

            if (newGridPosition != currentGridPosition || !previewObject.activeSelf)
            {
                currentGridPosition = newGridPosition;
                Vector3 stonePosition = GridToWorld(currentGridPosition);
                
                previewObject.transform.position = stonePosition;
             
                UpdatePreviewState();

                previewObject.SetActive(true);
            }
        }
        else
        {
            previewObject.SetActive(false);
        }
    }
    
    void UpdatePreviewState()
    {
        bool stoneExists = placedObjects.ContainsKey(currentGridPosition);
        
        if (isReplacing)
        {
            canPlace = stoneExists;
        }
        else
        {
            canPlace = !stoneExists;
        }
        
        UpdatePreviewColor();
    }
    
    void UpdatePreviewColor()
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        Color color;
        
        if (canPlace)
        {
            color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
        }
        else
        {
            color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        }

        if (isReplacing && canPlace)
        {
            color = new Color(0.2f, 0.7f, 0.7f, 0.5f);
        }
        
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }
    
    void PlaceStone()
    {
        if (!previewObject.activeSelf || !canPlace)
        {
            return;
        }
        
        if (isReplacing && placedObjects.TryGetValue(currentGridPosition, out GameObject existingStone))
        {
            Destroy(existingStone);
            placedObjects.Remove(currentGridPosition);
        }

        else if (!isReplacing && placedObjects.ContainsKey(currentGridPosition))
        {
            return;
        }
        
        Vector3 position = GridToWorld(currentGridPosition);
        GameObject stone = Instantiate(objectPrefab[selectedPrefabType]);
        stone.transform.position = position;        
     
        
        placedObjects.Add(currentGridPosition, stone);
        
        UpdatePreview();
    }
    
    void RemovePrefab()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldPosition = hit.point;
            Vector3Int gridPosition = WorldToGrid(worldPosition);
            
            if (placedObjects.TryGetValue(gridPosition, out GameObject stone))
            {
                Destroy(stone);
                placedObjects.Remove(gridPosition);
                
                UpdatePreview();
            }
        }
    }
        
    public int gridCountX = 14;  // X축 셀 개수
    public int gridCountY = 14;  // Y축 셀 개수
    public Color gridColor = Color.green; // 그리드 색상
    public Vector3 offset = new Vector3(0, 0, 0); // 그리드 위치 오프셋

    private void OnDrawGizmos()
    {
        if (gridParent == null) return;

        Gizmos.color = gridColor;
        Vector3 startPosition = gridParent.GetComponent<Renderer>().bounds.min + new Vector3(0, 0.01f, 0) + offset;

        // 그리드의 시작 위치를 계산 (y = 1 고정)
        for (int x = 0; x <= gridCountX; x++)
        {
            Vector3 start = startPosition +  new Vector3(x * gridSize, 0, 0);
            Vector3 end = start +  new Vector3(0, 0, gridCountY * gridSize);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= gridCountY; y++)
        {
            Vector3 start = startPosition +  new Vector3(0, 0, y * gridSize);
            Vector3 end = start +  new Vector3(gridCountX * gridSize, 0, 0);
            Gizmos.DrawLine(start, end);
        }
    }

    Vector3Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 startPosition = gridParent.GetComponent<Renderer>().bounds.min;
        Vector3 relativePosition = worldPosition - startPosition - offset;

        int x = Mathf.RoundToInt(relativePosition.x / gridSize);
        int z = Mathf.RoundToInt(relativePosition.z / gridSize);
        
        x = Mathf.Clamp(x, 0, gridCountX);
        z = Mathf.Clamp(z, 0, gridCountY);

        return new Vector3Int(x, 0, z);
    }

    Vector3 GridToWorld(Vector3Int gridPosition)
    {
        Vector3 startPosition = gridParent.GetComponent<Renderer>().bounds.min;
        Vector3 position = startPosition + new Vector3(gridPosition.x * gridSize, 0, gridPosition.z * gridSize) + offset;

        return position;
    }

    // 유니티 에디터에서 종료 시 프리뷰 오브젝트 정리
    void OnDestroy()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }
}