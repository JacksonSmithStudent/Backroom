using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GenerationState
{
    Idle,
    GeneratingRooms,
    GeneratingLighting,

    GeneratingSpawn,
    GeneratingExit,

    GeneratingBarrier
}

public class GenerationManager : MonoBehaviour
{
    [Header("Refrenece")]
    [SerializeField] Transform WorldGrid;

    [SerializeField] List<GameObject> RoomTypes;

    [SerializeField] List<GameObject> LightTypes;

    [SerializeField] int mapSize = 16; // Size of map

    [SerializeField] Slider MapSizeSlider, EmptinessSlider, BrigtnessSlider;

    [SerializeField] Button GenerateButton;

    [SerializeField] GameObject E_Room;

    [SerializeField] GameObject B_Room;

    [SerializeField] GameObject SpawnRoom, ExitRoom;

    public List<GameObject> GeneratedRooms;

    [SerializeField] GameObject PlayerObject, MainCameraObject;


    [Header("Settings")]

    public int mapEmptiness;

    public int mapBrightness;

    private int mapSizeSqr;

    private float currentPosX, currentPosZ, currentPosTracker, currentRoom;

    private Vector3 currentPos;

    public float roomSize = 7;

    public GenerationState currentState;

    private void Update()
    {
        mapSize = (int)Mathf.Pow(MapSizeSlider.value, 4);

        mapSizeSqr = (int)Mathf.Sqrt(mapSize);

        mapEmptiness = (int)EmptinessSlider.value;

        mapBrightness = (int)BrigtnessSlider.value;
    }

    public void ReloadWorld() // Reload the world
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GenerateWorld() // Creates the world when clicked
    {

        for (int i = 0; i < mapEmptiness; i++)
        {
            RoomTypes.Add(E_Room);
        }

        GenerateButton.interactable = false;

        for (int state = 0; state < 6; state++)
        {
            for (int i = 0; i < mapSize; i++)
            {
                if (currentPosTracker == mapSizeSqr)
                {
                    if (currentState == GenerationState.GeneratingBarrier) GenerateBarrier();

                    currentPosX = 0;
                    currentPosTracker = 0;

                    currentPosZ += roomSize;

                    if (currentState == GenerationState.GeneratingBarrier) GenerateBarrier();
                }

                currentPos = new(currentPosX, 0, currentPosZ);

                switch (currentState) {
                    case GenerationState.GeneratingRooms:
                      GeneratedRooms.Add(Instantiate(RoomTypes[Random.Range(0, RoomTypes.Count)], currentPos, Quaternion.identity, WorldGrid));
                   break;
                    case GenerationState.GeneratingLighting:
                        int lightSpawn = Random.Range(-1, mapBrightness);

                        if (lightSpawn == 0)
                        Instantiate(LightTypes[Random.Range(0, LightTypes.Count)], currentPos, Quaternion.identity, WorldGrid);
                        break;

                    case GenerationState.GeneratingBarrier:

                        if (currentRoom <= mapSizeSqr && currentRoom >= 0)
                        {
                            GenerateBarrier();
                        }

                        if (currentRoom <= mapSize && currentRoom >= mapSize - mapSizeSqr)
                        {
                            GenerateBarrier();
                        }

                        break;

                }

                currentRoom++;
                currentPosTracker++;
                currentPosX += roomSize;
            }
            NextState();

            switch(currentState)
            {
                case GenerationState.GeneratingExit:

                    int roomToReplace = Random.Range(0, GeneratedRooms.Count);

                    GameObject exitRoom = Instantiate(ExitRoom, GeneratedRooms[roomToReplace].transform.position, Quaternion.identity, WorldGrid);

                    Destroy(GeneratedRooms[roomToReplace]);

                    GeneratedRooms[roomToReplace] = exitRoom;

                    break;

                case GenerationState.GeneratingSpawn:

                    int _roomToReplace = Random.Range(0, GeneratedRooms.Count);

                     spawnRoom = Instantiate(SpawnRoom, GeneratedRooms[_roomToReplace].transform.position, Quaternion.identity, WorldGrid);

                    Destroy(GeneratedRooms[_roomToReplace]);

                    GeneratedRooms[_roomToReplace] = spawnRoom;

                    break;
            }
        }
    }
    public GameObject spawnRoom;
    public void SpawnPlayer()
    {
        PlayerObject.SetActive(false);

        PlayerObject.transform.position = new Vector3(spawnRoom.transform.position.x,1.8f, spawnRoom.transform.position.z);

        PlayerObject.SetActive(true);
        MainCameraObject.SetActive(false);
    }
    public void NextState()
    {
        currentState++;

        currentRoom = 0;
        currentPosX = 0;
        currentPosZ = 0;
        currentPosTracker = 0;
        currentPos = Vector3.zero;
    }

    public void WinGame()
    {
        MainCameraObject.SetActive(true);
        PlayerObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Player Exit");
    }

    public void GenerateBarrier()
    {
        currentPos = new(currentPosX, 0, currentPosZ);

        Instantiate(B_Room, currentPos, Quaternion.identity, WorldGrid);
    }
}
