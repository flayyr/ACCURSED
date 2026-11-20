using Esper.ESave;
using UnityEngine;

public class SavesManager : MonoBehaviour
{
    public static SavesManager Instance;

    private SaveFileSetup saveFileSetup;
    private SaveFile saveFile;

    [SerializeField] string gameStartScene;

    private void Awake() {
        DontDestroyOnLoad(gameObject);

        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        saveFileSetup = GetComponent<SaveFileSetup>();
        //saveFileSetup.saveFileData.fileName = "";
        saveFileSetup.name = "";
        saveFile = saveFileSetup.GetSaveFile();
    }

#if UNITY_EDITOR
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Backslash)) {
            saveFile.DeleteFile();
        }
    }

#endif

    public void AddData<T>(string reference, T data) {
        saveFile.AddOrUpdateData(reference, data);
    }

    public bool TryLoadData<T>(string reference, out T output) {
        if (saveFile.HasData(reference)) {
            output = saveFile.GetData<T>(reference);
            return true;
        }
        output = default(T);
        return false;
    }

    public void Save() {
        saveFile.Save();
    }

    public void OpenSave(string saveName) {
        if (!SaveStorage.instance.ContainsKey(saveName)) {
            Debug.LogError("A save with the name " + saveName + " does not exist!!");
            return;
        }

        saveFile = SaveStorage.instance.GetSaveByFileName(saveName);
        saveFileSetup.SetSaveFile(saveFile);
        LoadSave();
    }

    public void CreateNewSave(string saveName) {
        saveFile = saveFileSetup.GetSaveFile(saveName);
        SaveStorage.instance.AddSave(saveFile);

        saveFile.AddOrUpdateData("_SceneName", gameStartScene);
        LoadSave();
    }

    public void DeleteSave(string saveName) {
        SaveStorage.instance.GetSaveByFileName(saveName).DeleteFile();
        saveFile = saveFileSetup.GetSaveFile();
    }

    private void LoadSave() {
        RoomTransitionManager.Instance.BeginTransition(saveFile.GetData<string>("_SceneName"));
    }
}
