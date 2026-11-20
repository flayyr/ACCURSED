using Esper.ESave;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class SavesManager : MonoBehaviour
{
    public static SavesManager Instance;

    //private SaveFileSetup saveFileSetup;
    private SaveFile saveFile;

    [SerializeField] string gameStartScene;

    private void Awake() {
        DontDestroyOnLoad(gameObject);

        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //saveFileSetup = GetComponent<SaveFileSetup>();
        //saveFile = saveFileSetup.GetSaveFile();
        SaveFileSetupData saveFileSetupData = new SaveFileSetupData() {
            fileName = "Default Empty Save File",
            saveLocation = SaveFileSetupData.SaveLocation.PersistentDataPath,
            filePath = "Saves",
            encryptionMethod = SaveFileSetupData.EncryptionMethod.None,
            addToStorage = false
        };
        saveFile = new SaveFile(saveFileSetupData);

    }

#if UNITY_EDITOR
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Backslash)) {
            saveFile.DeleteFile();
            SaveStorage.instance.DeleteAllSaves();
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
        LoadSave();
    }

    public void CreateNewSave(string saveName) {
        SaveFileSetupData saveFileSetupData = new SaveFileSetupData() {
            fileName = saveName,
            saveLocation = SaveFileSetupData.SaveLocation.PersistentDataPath,
            filePath = "Saves",
            encryptionMethod = SaveFileSetupData.EncryptionMethod.None,
            addToStorage = true
        };
        saveFile = new SaveFile(saveFileSetupData);
        SaveStorage.instance.AddSave(saveFile);

        Debug.Log("ADDED NEW SAVE TO SAVE STORAGE "+saveName);

        saveFile.AddOrUpdateData("_SceneName", gameStartScene);
        LoadSave();
    }

    public void DeleteSave(string saveName) {
        SaveFile toDelete = SaveStorage.instance.GetSaveByFileName(saveName);
        SaveStorage.instance.RemoveSave(toDelete);
        toDelete.DeleteFile();
    }

    private void LoadSave() {
        RoomTransitionManager.Instance.BeginTransition(saveFile.GetData<string>("_SceneName"));
    }
}
