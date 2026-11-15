using Esper.ESave;
using UnityEngine;

public class SavesManager : MonoBehaviour
{
    public static SavesManager Instance;

    private SaveFileSetup saveFileSetup;
    private SaveFile saveFile;

    private void Awake() {
        Instance = this;
        saveFileSetup = GetComponent<SaveFileSetup>();
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
}
