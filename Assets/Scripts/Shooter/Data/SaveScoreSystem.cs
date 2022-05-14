using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveScoreSystem
{
    #region DEATH DATA
    // called when ever you die during a level
    public static void UpdateDeathDataWhenDead()
    {
        DeathData data;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/DeathData.ggData";

        DeathData previousData = GetDeathData();
        if (previousData == null)
        {
            // there is no previous death data. First time dying
            data = new DeathData(0, 1);
        }
        else {
            // there is previous data
            int previousDeaths = previousData.deaths;

            data = new DeathData(0, previousDeaths + 1);
        }

        FileStream stream = new FileStream(path, FileMode.Create);// creates a new file or overwrites the pre-existing file

        try
        {
            formatter.Serialize(stream, data);

            Debug.Log("Success at saving death data at. " + path);
        }
        catch {
            Debug.LogError("Failed to save the death data");
        }
        finally
        {
            stream.Close();
        }
    }

    // called when you have passed a level
    public static void UpdateDeathDateWhenWin() {
        DeathData data;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/DeathData.ggData";

        DeathData previousData = GetDeathData();
        
        if (previousData == null)
        {
            // there is no previous death data. First time winning
            data = new DeathData(1, 0);
        }
        else
        {
            Debug.Log("I have cleared " + previousData.missionClearedCount);
            // there is previous data
            int previousClearCount = previousData.missionClearedCount;
            int previousDeaths = previousData.deaths;

            data = new DeathData(previousClearCount+1, previousDeaths);
        }

        FileStream stream = new FileStream(path, FileMode.Create);// creates a new file or overwrites the pre-existing file

        try
        {
            formatter.Serialize(stream, data);

            Debug.Log("Success at saving death data at. " + path);
        }
        catch
        {
            Debug.LogError("Failed to save the death data");
        }
        finally
        {
            stream.Close();
        }
    }

    public static DeathData GetDeathData()
    {
        // path to the file I want to load
        string path = Application.persistentDataPath + "/DeathData.ggData";

        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        DeathData data = formatter.Deserialize(stream) as DeathData;
        stream.Close();

        return data;

    }

    #endregion

    #region LEVEL DATA
    // called when a lelvel has been beaten
    public static void CompletedAnotherLevel(int newLevel) {
        LevelData data;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/LevelData.ggData";

        LevelData previousData = GetLevelData();
        if (previousData == null)
        {
            // this is the first time completing a level
            data = new LevelData(newLevel);
        }
        else {
            // have a previous value repersenting the farthest the player has gotten
            if (newLevel > previousData.levelCompleted)
            {
                // we have completed a level that is greater than we have recently completed
                data = new LevelData(newLevel);
            }
            else {
                // the player has already beaten this level
                return;
            }
        }

        FileStream stream = new FileStream(path, FileMode.Create);// creates a new file or overwrites the pre-existing file

        try
        {
            formatter.Serialize(stream, data);

            Debug.Log("Success at saving death data at. " + path);
        }
        catch
        {
            Debug.LogError("Failed to save the death data");
        }
        finally
        {
            stream.Close();
        }
    }

    public static LevelData GetLevelData()
    {
        string path = Application.persistentDataPath + "/LevelData.ggData";
        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        LevelData data = formatter.Deserialize(stream) as LevelData;
        stream.Close();

        return data;
    }

    #endregion

    #region SCORE DATA
    // called when saving the score 
    public static void SaveScoreData(DataCollection dataCollect) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerScoreData/Level-" + dataCollect.levelReference +".ggData";

        if (Directory.Exists(Application.persistentDataPath + "/playerScoreData"))
        {
            Debug.Log("This directory exists.");

        }
        else {
            Directory.CreateDirectory(Application.persistentDataPath + "/playerScoreData");
        }

        FileStream stream = new FileStream(path, FileMode.Create);// creates a new file or overwrites the pre-existing file

        ScoreData data = new ScoreData(dataCollect.GetDamageTaken(), dataCollect.GetTimer(),
            dataCollect.GetNumberShots(), dataCollect.GetStarsAquired());

        try
        {
            formatter.Serialize(stream, data);

            Debug.Log("Success save at " + path);
        }
        catch
        {
            Debug.LogError("Failed to save the score data");

        }
        finally {
            stream.Close();
        }
        
    }

    // called when saving the score
    public static void SaveScoreData(int levelIndicator, float damageTaken, float timer, int shots, int stars)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/playerScoreData/Level-" + levelIndicator + ".ggData";

        if (Directory.Exists(Application.persistentDataPath + "/playerScoreData"))
        {
            Debug.Log("This directory exists.");

        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/playerScoreData");
        }

        FileStream stream = new FileStream(path, FileMode.Create);// creates a new file or overwrites the pre-existing file

        ScoreData data = new ScoreData(damageTaken, timer, shots, stars);

        try
        {
            formatter.Serialize(stream, data);

            Debug.Log("Success save. AT " + path);
        }
        catch
        {
            Debug.LogError("Failed to save the score data");

        }
        finally
        {
            stream.Close();
        }

    }

    // Gets the median value of all the score datas and creates a new score data with those vlaues. So that it can be shown in the leader baord
    public static ScoreData LoadAverageScoreDateValue()
    {
        string[] filePaths;

        if (Directory.Exists(Application.persistentDataPath + "/playerScoreData"))
        {
            // sweet keep goinf
            filePaths = Directory.GetFiles(Application.persistentDataPath + "/playerScoreData");
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/playerScoreData");
            Debug.Log("There have been no saved data for your game so far.");
            return null;
        }

        if (filePaths.Length > 0)
        {
            float[] allTheDamage = new float[filePaths.Length];
            float[] allTheTimer = new float[filePaths.Length];
            int[] allTheShots = new int[filePaths.Length];

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream;
            ScoreData testData;
            for (int i = 0; i < filePaths.Length; i++)
            {
                stream = new FileStream(filePaths[i], FileMode.Open);

                testData = formatter.Deserialize(stream) as ScoreData;
                stream.Close();

                allTheDamage[i] = testData.damageTaken;
                allTheTimer[i] = testData.timer;
                allTheShots[i] = testData.shots;

            }

            float medianDamage = CalculateMedian(allTheDamage);
            float medianTime = CalculateMedian(allTheTimer);
            int medianShots = CalculateMedian(allTheShots);

            ScoreData data = new ScoreData(medianDamage, medianTime, medianShots, 0);
            return data;
        }
        else
        {
            return null;
        }

    }

    // loads up a single file of Score Data. Could be used to show player what his best score for this level was.
    public static ScoreData LoadSpecifcLevelScoreData(DataCollection dataCollect)
    {
        string path = Application.persistentDataPath + "/playerScoreData/Level-" + dataCollect.levelReference + ".ggData";

        if (Directory.Exists(Application.persistentDataPath + "/playerScoreData"))
        {
            // sweet keep goinf
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/playerScoreData");
            Debug.Log("There have been no saved data for your game so far.");
            return null;
        }

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ScoreData data = formatter.Deserialize(stream) as ScoreData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Cannot find Score Data Save File. Loaded for 'Specific Level' ");
            return null;
        }
    }

    public static ScoreData LoadSpecifcLevelScoreData(int levelReference)
    {
        string path = Application.persistentDataPath + "/playerScoreData/Level-" + levelReference + ".ggData";

        if (Directory.Exists(Application.persistentDataPath + "/playerScoreData"))
        {
            // sweet keep goinf
        }
        else
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/playerScoreData");
            Debug.Log("There have been no saved data for your game so far.");
            return null;
        }

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ScoreData data = formatter.Deserialize(stream) as ScoreData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Cannot find Score Data Save File. Loaded for Level " + levelReference);
            return null;
        }
    }

    #endregion

    #region OPTION DATA
    public static void SetUXOptionData(OptionData newData, bool showPreGameInfo) {
        OptionData data;
        string path = Application.persistentDataPath + "/OptionData.ggData";

        if (newData != null)
        {
            data = newData;
            data.showPregameInfo = showPreGameInfo;
        }
        else
        {
            data = new OptionData();
            data.SetUXInfo(showPreGameInfo);
        }



        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        try
        {
            formatter.Serialize(stream, data);

            Debug.Log("Success save. AT " + path);
        }
        catch
        {
            Debug.LogError("Failed to save the option data");

        }
        finally
        {
            stream.Close();
        }

    }

    public static void SetMusicOptionData(OptionData newData, bool music)
    {
        OptionData data;
        string path = Application.persistentDataPath + "/OptionData.ggData";

        if (newData != null)
        {
            data = newData;
            data.playMusic = music;
        }
        else
        {
            data = new OptionData();
            data.SetPlayMusic(music);
        }



        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        try
        {
            formatter.Serialize(stream, data);

            Debug.Log("Success save. AT " + path);
        }
        catch
        {
            Debug.LogError("Failed to save the option data");

        }
        finally
        {
            stream.Close();
        }

    }

    public static void SetSoundOptionData(OptionData newData, bool sound)
    {
        OptionData data;
        string path = Application.persistentDataPath + "/OptionData.ggData";

        if (newData != null)
        {
            data = newData;
            data.playSound = sound;
        }
        else
        {
            data = new OptionData();
            data.SetPlaySound(sound);
        }



        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        try
        {
            formatter.Serialize(stream, data);

            Debug.Log("Success save. AT " + path);
        }
        catch
        {
            Debug.LogError("Failed to save the option data");

        }
        finally
        {
            stream.Close();
        }

    }

    public static OptionData GetOptionData()
    {
        string path = Application.persistentDataPath + "/OptionData.ggData";
        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        OptionData data = formatter.Deserialize(stream) as OptionData;
        stream.Close();

        return data;
    }

    #endregion

    #region Calculate Medians
    static float CalculateMedian(float[] array) {
        float newFloat =0f;
        if (array.Length % 2 == 0)
        {
            // even length
            newFloat = array[array.Length / 2] + array[(array.Length / 2) - 1];
            newFloat /= 2;
        }
        else if (array.Length % 2 == 1)
        {
            // odd length
            newFloat = array[array.Length / 2];

        }
        else {
            Debug.Log("Some thign went wrong moduling this array float");
        }
        return newFloat;
    }

    

    static int CalculateMedian(int[] array) {
        int newInt = 0;
        if (array.Length % 2 == 0)
        {
            // even length
            newInt = array[array.Length / 2] + array[(array.Length / 2) - 1];
            newInt /= 2;
        }
        else if (array.Length % 2 == 1)
        {
            // odd length
            newInt = array[array.Length / 2];

        }
        else
        {
            Debug.Log("Some thign went wrong moduling this array int");
        }
        return newInt;
    }
    #endregion

    #region Extra Methods that are not being used

    // writes out all the score data files 
    public static void GetAllTheLevelScoreFiles() {
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath + "/playerScoreData");

        if (filePaths.Length > 0) {
            for (int i = 0; i < filePaths.Length; i++) {
                Debug.Log(filePaths[i]);
            }
        }
    }

    // checks if there is a score data alredy exists at the path
    public static bool CheckScoreDataExists(string path) {
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath + "/playerScoreData");

        for (int i = 0; i < filePaths.Length; i++)
        {
            if (filePaths[i].Equals(path)) {
                // there is already a score data for this level
                return true;
            }
        }

        // there were no score datas for this level
        return false;
    }
    #endregion
}
