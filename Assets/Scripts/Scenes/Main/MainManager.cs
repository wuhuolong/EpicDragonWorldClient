﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Author: Pantelis Andrianakis
 * Date: December 22th 2018
 */
public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; }

    public static readonly string LOGIN_SCENE = "Login";
    public static readonly string CHARACTER_SELECTION_SCENE = "CharacterSelection";
    public static readonly string CHARACTER_CREATION_SCENE = "CharacterCreation";
    public static readonly string WORLD_SCENE = "World";

    public Canvas loadingCanvas;
    public Slider loadingBar;
    public Text loadingPercentage;
    public Canvas optionsCanvas;
    public MusicManager musicManager;

    [HideInInspector]
    public bool hasInitialized = false; // Set to true when login scene has initialized.
    [HideInInspector]
    public string accountName;
    [HideInInspector]
    public ArrayList characterList;
    [HideInInspector]
    public CharacterDataHolder selectedCharacterData;
    [HideInInspector]
    public bool isDraggingWindow = false;
    [HideInInspector]
    public bool isChatBoxActive = false;

    private string previousLoadedScene;
    private string previousScene;
    [HideInInspector]
    public int buildIndex;

    void OnEnable()
    {
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        buildIndex = scene.buildIndex;
        musicManager.PlayMusic(scene.buildIndex);
    }

    private void Start()
    {
        Instance = this;

        // Loading canvas should be enabled.
        loadingCanvas.enabled = true;
        // Close UIs.
        OptionsManager.Instance.HideOptionsMenu();
        // Initialize network manager.
        new NetworkManager();
        // Load first scene.
        LoadScene(LOGIN_SCENE);
    }

    private void Update()
    {
        if (InputManager.ESCAPE_DOWN && !ConfirmDialog.Instance.confirmDialogActive)
        {
            OptionsManager.Instance.ShowOptionsMenu();
        }
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadSceneCoroutine(scene));
    }

    private IEnumerator LoadSceneCoroutine(string scene)
    {
        loadingBar.value = 0;
        loadingPercentage.text = "0%";
        loadingCanvas.enabled = true;
        musicManager.PlayLoadingMusic(buildIndex, true);
        AsyncOperation operation;
        if (previousLoadedScene != null)
        {
            operation = SceneManager.UnloadSceneAsync(previousLoadedScene);
            yield return new WaitUntil(() => operation.isDone);
        }
        operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            loadingPercentage.text = (int)(progress * 100f) + "%";
            yield return null;
        }
        previousLoadedScene = scene;
        loadingCanvas.enabled = false;
        musicManager.PlayLoadingMusic(buildIndex, false);
    }
}