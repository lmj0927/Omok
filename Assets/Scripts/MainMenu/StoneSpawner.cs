using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] stonePrefab;
    CancellationTokenSource _spawnCts;
    GameObject parent;

    void Start()
    {
        parent = new GameObject("Stones");
        parent.transform.SetParent(transform);   
    }

    public void StartSpawn()
    {
        gameObject.SetActive(true);

        _spawnCts = new CancellationTokenSource();
        SpawnStoneAsync().Forget();
    }

    public void StopSpawn()
    {
        if(_spawnCts != null){
            _spawnCts.Cancel();
            _spawnCts = null;
        }
    }

    public void DisableSpawner()
    {
        StopSpawn();

        if(parent != null){
            foreach(Transform child in parent.transform){
                Destroy(child.gameObject);
            }
        }
        
        gameObject.SetActive(false);
    }

    async UniTask SpawnStoneAsync()
    {
        while(_spawnCts != null && !_spawnCts.Token.IsCancellationRequested)
        {
            await UniTask.Delay(20);
            SpawnStone(Random.Range(0, stonePrefab.Length), new Vector3(Random.Range(-1f, 1f), transform.position.y, Random.Range(-1f, 1f)));
        }
    }

    public void SpawnStone(int index, Vector3 position)
    {
        Instantiate(stonePrefab[index], position, Random.rotation, parent.transform);
    }
}
