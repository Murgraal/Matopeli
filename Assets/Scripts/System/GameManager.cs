using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static event Action OnSpeedUp; //WIP for showing message to player in ui when game is sped up
    public static event Action<int> OnScoreAdded;
    
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private GameObject wormPrefab;
    [SerializeField] private Transform interactablesParent;
    [SerializeField] private GameObject levelParent;
    
    //Input mappings should be extracted to own container for potential rebinding and add multi-button support per action
    [SerializeField] private KeyCode upKey = KeyCode.UpArrow;
    [SerializeField] private KeyCode rightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode downKey = KeyCode.DownArrow;
    [SerializeField] private KeyCode leftKey = KeyCode.LeftArrow;
    
    [SerializeField] private float intervalReductionPercentage = 0.9f;
    [SerializeField] private float startMoveInterval = 0.2f;
    [SerializeField] private int speedUpPerXPickups = 4;
    
    private float currentMoveInterval;
    private int playerScore;
    private int pickupCounter;

    private Pickup pickup;
    private Worm worm;
    private LevelBoundaries currentBoundaries;
    private InputHandler inputHandler;
    
    private void OnEnable() 
    {
        worm.OnInteraction += OnPlayerInteraction;
    }

    private void OnDisable() 
    {
        worm.OnInteraction -= OnPlayerInteraction;
    }

    private void Awake()
    {
        SetLevelBoundaries();
        InstantiateInteractablesIfDoesNotExist();
    }

    private void Start()
    {
        inputHandler = new InputHandler();
        
        inputHandler.RegisterInput(
            new ActionBinding(upKey,() => {worm.TurnWorm(Vector2.up);}),
            new ActionBinding(rightKey,() => {worm.TurnWorm(Vector2.right);}),
            new ActionBinding(downKey,() => {worm.TurnWorm(Vector2.down);}),
            new ActionBinding(leftKey,() => {worm.TurnWorm(Vector2.left);}),
            new ActionBinding(KeyCode.Escape,ReloadScene)
        );

        StartGame();
    }

    private void Update()
    {
        inputHandler.ProcessInputs();
    }

    private void InstantiateInteractablesIfDoesNotExist()
    {
        if(pickup == null)
        {
            pickup = Instantiate(pickupPrefab,
                Vector3.zero,
                Quaternion.identity,
                interactablesParent).GetComponent<Pickup>();
        }
        if (worm == null)
        {
            worm = Instantiate(wormPrefab
                , Vector3.zero,
                Quaternion.identity,
                interactablesParent).GetComponent<Worm>();
        }
    }
    
    private void StartGame()
    {
        pickup.transform.position = GetRandomLocationNotUnderWormInPlayArea();

        currentMoveInterval = startMoveInterval;
        
        worm.InitWorm(currentMoveInterval);
        worm.transform.position = Vector2.zero;
    }

    void SetLevelBoundaries()
    {
        var wallPositions = new List<Vector2>();

        foreach(Transform child in levelParent.transform)
        {
            wallPositions.Add((Vector2)child.localPosition);
        }

        currentBoundaries = new LevelBoundaries(wallPositions.ToArray());
    }

    void OnPickup()
    {
        playerScore += 10;
        pickupCounter++;

        worm.GrowWorm();

        OnScoreAdded?.Invoke(playerScore);
        
        if(pickupCounter % 4 == 0)
        {
            currentMoveInterval *= intervalReductionPercentage;
            worm.SetMoveInterval(currentMoveInterval);
            OnSpeedUp?.Invoke();
        }

        pickup.transform.position = GetRandomLocationNotUnderWormInPlayArea();
    }

    private Vector2 GetRandomLocationNotUnderWormInPlayArea()
    {
        var validPositions = currentBoundaries.gridPositions.Except(worm.GetAllNodePositions()).ToList();
        var randomPos = validPositions[UnityEngine.Random.Range(0,validPositions.Count - 1)];
        return randomPos;
    }

    void OnPlayerInteraction(PlayerInteractionType type)
    {
        if(type == PlayerInteractionType.HitWall)
            GameOver();
        else
            OnPickup();
    }

    void GameOver()
    {
        if(playerScore > PlayerPrefs.GetInt(Constants.PlayerPrefsKeys.HighScore,0))
        {
            PlayerPrefs.SetInt(Constants.PlayerPrefsKeys.HighScore, playerScore);
        }

        ReloadScene();
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}