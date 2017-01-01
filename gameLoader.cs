using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class gameLoader : MonoBehaviour {

    public static void saveData()
    {
        if(!ObjectTracker.userID.Equals(""))
        {
            BinaryFormatter bf = new BinaryFormatter();

            string fileDirectory = Application.dataPath + "/userData/";

            if (!Directory.Exists(fileDirectory)) Directory.CreateDirectory(Application.dataPath + "/userData/");

            FileStream file = File.Open(fileDirectory + ObjectTracker.userID + ".dat", FileMode.Create);

            playerData data = new playerData();

            data.trackedObjects = new List<trackedObject>(ObjectTracker.trackedObjects);
            data.allObjects = new List<trackedObject>(ObjectTracker.allObjects);
            data.assessmentLock = ObjectTracker.assessmentLock;
            data.moduleTime_ef = ObjectTracker.moduleTime_ef;
            data.moduleTime_stl = ObjectTracker.moduleTime_stl;
            data.moduleTime_env = ObjectTracker.moduleTime_env;
            data.moduleTime_ass = ObjectTracker.moduleTime_ass;
            data.moduleTime_ass = ObjectTracker.moduleTime_temp;
            data.sessionNumber = ObjectTracker.sessionNumber;
            
            data.moduleHazardFound_ef = ObjectTracker.moduleHazardFound_ef;
            data.moduleHazardFound_stl = ObjectTracker.moduleHazardFound_stl;
            data.moduleHazardFound_env = ObjectTracker.moduleHazardFound_env;
            data.moduleHazardFound_ass = ObjectTracker.moduleHazardFound_ass;

            data.moduleHazardTotal_ef = ObjectTracker.moduleHazardTotal_ef;
            data.moduleHazardTotal_stl = ObjectTracker.moduleHazardTotal_stl;
            data.moduleHazardTotal_env = ObjectTracker.moduleHazardTotal_env;
            data.moduleHazardTotal_ass = ObjectTracker.moduleHazardTotal_ass;

            data.crashTrack = ObjectTracker.crashTrack;

            data.playerPosition_x = ObjectTracker.playerPosition.x;
            data.playerPosition_y = ObjectTracker.playerPosition.y;
            data.playerPosition_z = ObjectTracker.playerPosition.z;

            data.playerDirection_x = ObjectTracker.playerDirection.x;
            data.playerDirection_x = ObjectTracker.playerDirection.y;
            data.playerDirection_x = ObjectTracker.playerDirection.z;
            data.playerDirection_x = ObjectTracker.playerDirection.w;


            data.WhyCorrect= ObjectTracker.WhyCorrect;
            data.WhyTotal = ObjectTracker.WhyTotal;
            data.WTDCorrect = ObjectTracker.WTDCorrect;
            data.WTDTotal = ObjectTracker.WTDTotal;
            data.scoreVisited = ObjectTracker.scoreVisited;



            bf.Serialize(file, data);
            Debug.Log("Data Serialized");
            file.Close();
        }
    }  


    public static void loadData()
    {
        if(File.Exists(Application.dataPath + "/userData/" + ObjectTracker.userID + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/userData/" + ObjectTracker.userID + ".dat", FileMode.Open);
            playerData data = new playerData();
            data = (playerData)bf.Deserialize(file);

            ObjectTracker.trackedObjects= new List<trackedObject>(data.trackedObjects);
            ObjectTracker.allObjects = new List<trackedObject>(data.allObjects);
            ObjectTracker.assessmentLock = data.assessmentLock;
            ObjectTracker.moduleTime_ef = data.moduleTime_ef;
            ObjectTracker.moduleTime_stl = data.moduleTime_stl;
            ObjectTracker.moduleTime_env = data.moduleTime_env;
            ObjectTracker.moduleTime_ass = data.moduleTime_ass;
            ObjectTracker.moduleTime_temp = data.moduleTime_temp;
            ObjectTracker.sessionNumber = data.sessionNumber;

            ObjectTracker.moduleHazardFound_ef = data.moduleHazardFound_ef;
            ObjectTracker.moduleHazardFound_stl = data.moduleHazardFound_stl;
            ObjectTracker.moduleHazardFound_env = data.moduleHazardFound_env;
            ObjectTracker.moduleHazardFound_ass = data.moduleHazardFound_ass;

            ObjectTracker.moduleHazardTotal_ef = data.moduleHazardTotal_ef;
            ObjectTracker.moduleHazardTotal_stl = data.moduleHazardTotal_stl;
            ObjectTracker.moduleHazardTotal_env = data.moduleHazardTotal_env;
            ObjectTracker.moduleHazardTotal_ass = data.moduleHazardTotal_ass;

            ObjectTracker.WhyCorrect = data.WhyCorrect;
            ObjectTracker.WhyTotal = data.WhyTotal;
            ObjectTracker.WTDCorrect = data.WTDCorrect;
            ObjectTracker.WTDTotal= data.WTDTotal;
            ObjectTracker.scoreVisited = data.scoreVisited;

            ObjectTracker.playerPosition = new Vector3(data.playerPosition_x, data.playerPosition_y, data.playerPosition_z);
            ObjectTracker.playerDirection = new Quaternion(ObjectTracker.playerDirection.x, ObjectTracker.playerDirection.y, ObjectTracker.playerDirection.z, ObjectTracker.playerDirection.w);



            ObjectTracker.crashTrack = data.crashTrack;
            Debug.Log("Data Deserialized");
            file.Close();
        }
    }
}
[System.Serializable]
class playerData
{
    public List<trackedObject> trackedObjects;
    public List<trackedObject> allObjects;    
    public bool assessmentLock=false;

    public float moduleTime_ef=0;
    public float moduleTime_stl=0;
    public float moduleTime_env=0;
    public float moduleTime_ass=0;
    public float moduleTime_temp=0;
    public int sessionNumber=0;

    public float moduleHazardFound_ef=0;
    public float moduleHazardFound_stl=0;
    public float moduleHazardFound_env=0;
    public float moduleHazardFound_ass=0;

    public float moduleHazardTotal_ef=0;
    public float moduleHazardTotal_stl=0;
    public float moduleHazardTotal_env=0;
    public float moduleHazardTotal_ass=0;

    public int crashTrack;

    public float playerPosition_x=0;
    public float playerPosition_y=0;
    public float playerPosition_z=0;

    public float playerDirection_x=0;
    public float playerDirection_y=0;
    public float playerDirection_z=0;
    public float playerDirection_w=0;


    public float WhyCorrect=0;
    public float WhyTotal=0;
    public float WTDCorrect=0;
    public float WTDTotal=0;
    public bool scoreVisited=false;


}
