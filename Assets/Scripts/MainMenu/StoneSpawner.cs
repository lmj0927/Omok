using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoneSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] stonePrefab;
    CancellationTokenSource _spawnCts;

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
        
        gameObject.SetActive(false);
    }

    async UniTask SpawnStoneAsync()
    {
        try 
        {
            while(!_spawnCts.Token.IsCancellationRequested)
            {
                await UniTask.Delay(20, cancellationToken: _spawnCts.Token);
                SpawnStone(Random.Range(0, stonePrefab.Length), new Vector3(Random.Range(-1f, 1f), transform.position.y, Random.Range(-1f, 1f)));
            }
        }
        catch (OperationCanceledException) 
        {
            Debug.Log("SpawnStoneAsync Cancelled");
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    public void SpawnStone(int index, Vector3 position)
    {
        var stoneObject = Instantiate(stonePrefab[index], position, Random.rotation, transform);
        if (stoneObject.TryGetComponent<Stone>(out var stone))
        {
            stone.SetKinematic(false);
        }
    }
}
