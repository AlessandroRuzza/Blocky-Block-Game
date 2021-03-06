using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CubeSpawner : MonoBehaviour
{
    [SerializeField] GameObject cubeRoot;
    [SerializeField] Player playerRef;
    float maxOffset;
    [SerializeField] float minScale=0.75f, maxScale=3f;
    [SerializeField] float minSpeed=0.75f, maxSpeed=2; 
    [SerializeField] float spawnPeriod=0.35f;
    public bool isSpawning=false;
    void Start()
    {
        playerRef = FindObjectOfType<Player>();
        maxOffset = CameraUtils.halfWidth - minScale/2f; 
        if(SceneManager.GetActiveScene().name == "GamePlay")
            StartSpawner();
        else
            Debug.Log("Loaded");  
    }
    public void StartSpawner(){
        StopSpawner();
        GameObject[] allCubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach(GameObject cube in allCubes)
            Destroy(cube);
        StartCoroutine("SpawnCubes");
        isSpawning=true;
    }
    public void StopSpawner(){
        StopAllCoroutines();
        isSpawning=false;
    }
    IEnumerator SpawnCubes(){
        for(int cubeSpawned=0; playerRef.isAlive; cubeSpawned++){
            SpawnCube();
            yield return new WaitForSeconds(spawnPeriod);
        }
    }
    public GameObject SpawnCube(bool randomCube=true){
        GameObject cubeHandle;
        Quaternion cubeRotation = Quaternion.Euler(RandomRotation());
        if(randomCube){
            Vector3 cubeScale = RandomScale(minScale, maxScale);
            Vector3 spawnPoint = transform.position + RandomSpawnOffset(maxOffset-cubeScale.x/2f);
            float randomSpeed = Random.Range(minSpeed, maxSpeed);

            cubeHandle = Instantiate<GameObject>(cubeRoot, spawnPoint, cubeRotation);
            cubeHandle.transform.localScale = cubeScale;
            cubeHandle.GetComponent<Rigidbody2D>().gravityScale = randomSpeed;
            if(randomSpeed > maxSpeed*3/4){
                cubeHandle.GetComponent<Rigidbody2D>().velocity = Vector2.down*randomSpeed/2;
            }
        }
        else{
            cubeHandle = Instantiate<GameObject>(cubeRoot, transform.position, cubeRotation);
            cubeHandle.transform.localScale = Vector3.one*maxScale; 
        }

        return cubeHandle;
    }
    Vector3 RandomSpawnOffset(float maxOffset){
        // 50% direction is left(-offset) or right(+offset)
        Vector3 direction = new Vector3(Random.Range(-maxOffset, maxOffset), 0);
        return direction;
    }
    Vector3 RandomScale(float min, float max){
        float scaleMultiplier = Random.Range(min, max);
        Vector3 scale = new Vector3(1,1);
        scale *= scaleMultiplier;
        return scale;
    }
    Vector3 RandomRotation(){
        float rotationMultiplier = Random.Range(0f, 360f);
        Vector3 rotation = new Vector3(0,0,1);
        rotation *= rotationMultiplier;
        return rotation;
    }
}
