using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject environment;
    public GameObject buildingEnvironment;
    public GameObject player;

    public Vector2 randomBuildingSizeZ = new Vector2(.85f, 1.15f);
    public Vector2 randomBuildingSizeY = new Vector2(.8f, 1.5f);
    public Vector2 randomBuildingGapSize = new Vector2(.2f, 1.5f);

    public int maxTerrain = 10;

    private float _currentBuildingPositionLeft = 0;
    private float _currentBuildingPositionRight = 0;
    private Controller _playerController;
    private GameObject[] _terrain;
    void Start()
    {
        instance = this;
        _playerController = player.GetComponent<Controller>();
        _playerController.onPlayerChangedFloor += PlayerChangedFloor;

        _terrain = new GameObject[maxTerrain];
        for(int i = 0; i < maxTerrain; ++i) {
            _terrain[i] = GenerateFloor(i);
        }
        GenerateBuildings();

        _terrain[0].tag = "Untagged";
    }

    private void PlayerChangedFloor() {
        StartCoroutine(UpdateFloor());
        StartCoroutine(UpdateBuildings());
    }

    GameObject GenerateFloor(int position) {
        GameObject floor = ObjectPool.GetObjectPool("Floor").PopRandom();
        floor.transform.position = new Vector3(0, 0, 20 * position);
        floor.transform.parent = environment.transform;

        int obstacles = Random.Range(0, 5);
        float gap = 19.0f / obstacles;
        for(int i = 0; i < obstacles; ++i) {
            GameObject obstacle = ObjectPool.GetObjectPool("Obstacle").PopRandom();

            obstacle.transform.position = new Vector3((Random.Range(0, 3) * 2) + 1f, 0.25f, (20 * position) + Random.Range(gap * i, gap * i + 1));
            obstacle.transform.parent = floor.transform;

            if(Physics.Raycast(obstacle.transform.position, Vector3.back, 1.5f, ~0, QueryTriggerInteraction.Collide) || Physics.Raycast(obstacle.transform.position, Vector3.forward, 1.5f, ~0, QueryTriggerInteraction.Collide)) {
                ObjectPool.GetObjectPool("Obstacle").Push(obstacle);
                i--;
                continue;
            }
        }
        
        int maxTries = 10;
        int tries = 0;
        int collectibles = Random.Range(0, 3);
        for(int i = 0; i < collectibles && tries++ < maxTries; ++i) {
            GameObject collectible = ObjectPool.GetObjectPool("Collectible").PopRandom();
            
            collectible.transform.position = new Vector3((Random.Range(0, 3) * 2) + 1, 0.5f, (20 * position) + Random.Range(1, 19));
            collectible.transform.parent = floor.transform;
            
            if(Physics.CheckSphere(collectible.transform.position, 0.5f, LayerMask.GetMask("HoleCollider"), QueryTriggerInteraction.Collide)) {
                ObjectPool.GetObjectPool("Collectible").Push(collectible);
                i--;
            }
        }

        return floor;
    }

    void GenerateBuildings() {
        float distance = 20 * maxTerrain;

        while(_currentBuildingPositionLeft < distance) {
            GameObject building = ObjectPool.GetObjectPool("Building").PopRandom();
            building.transform.position = new Vector3(-5.5f, 0f, _currentBuildingPositionLeft);
            float yScale = Random.Range(randomBuildingSizeY.x, randomBuildingSizeY.y);
            float zScale = Random.Range(randomBuildingSizeZ.x, randomBuildingSizeZ.y);
            building.transform.localScale = new Vector3(1, yScale, zScale);
            building.transform.parent = buildingEnvironment.transform;
            _currentBuildingPositionLeft += (zScale * 5) + Random.Range(randomBuildingGapSize.x, randomBuildingGapSize.y);
        }
        while(_currentBuildingPositionRight < distance) {
            GameObject building = ObjectPool.GetObjectPool("Building").PopRandom();
            building.transform.position = new Vector3(6.5f, 0f, _currentBuildingPositionRight);
            float yScale = Random.Range(randomBuildingSizeY.x, randomBuildingSizeY.y);
            float zScale = Random.Range(randomBuildingSizeZ.x, randomBuildingSizeZ.y);
            building.transform.localScale = new Vector3(1, yScale, zScale);
            building.transform.parent = buildingEnvironment.transform;
            _currentBuildingPositionRight += (zScale * 5) + Random.Range(randomBuildingGapSize.x, randomBuildingGapSize.y);
        }
    }

    IEnumerator UpdateBuildings() {
        yield return new WaitForSeconds(0.25f);

        for(int i = 0; i < buildingEnvironment.transform.childCount; i++) {
            GameObject go = buildingEnvironment.transform.GetChild(i).gameObject;

            go.transform.position = go.transform.position - new Vector3(0, 0, 20);
            if(go.transform.position.z + (go.transform.localScale.z * 5) < 0) {
                ObjectPool.GetObjectPool("Building").Push(go);
                i--;
            }
        }
        
        _currentBuildingPositionLeft -= 20;
        _currentBuildingPositionRight -= 20;
        GenerateBuildings();
    }

    IEnumerator UpdateFloor() {
        yield return new WaitForSeconds(0.25f);
        _terrain[0].tag = "Floor";

        for(int i = 0; i < _terrain[0].transform.childCount;) {
            GameObject go = _terrain[0].transform.GetChild(i).gameObject;
            if(go.tag.Equals("Collectible")) {
                ObjectPool.GetObjectPool("Collectible").Push(go);
            } else 
            if(go.tag.Equals("Obstacle")) {
                ObjectPool.GetObjectPool("Obstacle").Push(go);
            } else {
                i++;
            }
        }
        ObjectPool.GetObjectPool("Floor").Push(_terrain[0]);

        for(int i = 1; i < maxTerrain; ++i) {
            _terrain[i].transform.position = _terrain[i].transform.position - new Vector3(0, 0, 20);
            _terrain[i - 1] = _terrain[i];
        }
        _terrain[maxTerrain - 1] = GenerateFloor((maxTerrain - 1));

        player.transform.position = player.transform.position - new Vector3(0, 0, 20);
    }
}
