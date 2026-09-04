using System;
using System.Collections.Generic;
using UnityEngine;

public static class ChestStateSave
{
    [Serializable]
    private class ChestSaveData
    {
        public List<string> openedChestIDs = new List<string>();
    }

    private const string SaveKeyPrefix = "OpenedChests_SaveSlot_";

    // Public
    public static bool IsChestOpen(string chestID)
    {
        if (string.IsNullOrWhiteSpace(chestID))
            return false;

        ChestSaveData data = LoadCurrentSlot();

        return data.openedChestIDs.Contains(chestID);
    }

    public static void MarkChestOpen(string chestID)
    {
        if (string.IsNullOrWhiteSpace(chestID))
        {
            Debug.LogWarning("Tried to save a chest with an empty chestID.");

            return;
        }

        ChestSaveData data = LoadCurrentSlot();

        if (data.openedChestIDs.Contains(chestID))
            return;

        data.openedChestIDs.Add(chestID);

        SaveCurrentSlot(data);
    }

    public static void ClearCurrentSlot()
    {
        int slot = GetCurrentSaveSlot();

        ClearSlot(slot);
    }

    public static void ClearSlot(int slot)
    {
        PlayerPrefs.DeleteKey(GetSaveKey(slot));
        PlayerPrefs.Save();
    }

    // LOAD / SAVE
    private static ChestSaveData LoadCurrentSlot()
    {
        int slot = GetCurrentSaveSlot();
        string key = GetSaveKey(slot);

        if (!PlayerPrefs.HasKey(key))
            return new ChestSaveData();

        string json = PlayerPrefs.GetString(key);

        if (string.IsNullOrWhiteSpace(json))
            return new ChestSaveData();

        ChestSaveData data =
            JsonUtility.FromJson<ChestSaveData>(json);

        if (data == null)
            data = new ChestSaveData();

        if (data.openedChestIDs == null)
            data.openedChestIDs = new List<string>();

        return data;
    }

    private static void SaveCurrentSlot(ChestSaveData data)
    {
        int slot = GetCurrentSaveSlot();
        string key = GetSaveKey(slot);

        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    private static int GetCurrentSaveSlot()
    {
        // -1 is for when directly testing a gameplay scene without entering through the save menu
        return PlayerPrefs.GetInt("ActiveSaveSlot", -1);
    }

    private static string GetSaveKey(int slot)
    {
        return SaveKeyPrefix + slot;
    }
}