//objectTracker system
//tracks class "trackedObject.cs" and serialized using "gameLoader.cs"
//written by renee chia-lei chen

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

public class ObjectTracker : MonoBehaviour {

    static public int sessionNumber;
    static public string userID = "";

    static public List<trackedObject> trackedObjects = new List<trackedObject>();
    static public List<trackedObject> allObjects = new List<trackedObject>();

    static public float moduleTime_ef;
    static public float moduleTime_stl;
    static public float moduleTime_env;
    static public float moduleTime_ass;
    static public float moduleTime_temp;
    static public float moduleHazardFound_ef;
    static public float moduleHazardFound_stl;
    static public float moduleHazardFound_env;
    static public float moduleHazardFound_ass;

    static public float moduleHazardTotal_ef;
    static public float moduleHazardTotal_stl;
    static public float moduleHazardTotal_env;
    static public float moduleHazardTotal_ass;

    static public int crashTrack;
    static public bool assessmentLock = false;

    static public Vector3 playerPosition = new Vector3();
    static public Quaternion playerDirection = new Quaternion();

    static public float WhyCorrect, WhyTotal, WTDCorrect, WTDTotal;
    static public bool scoreVisited;
    
    static public void trackModuleTime(string module, float moduleTime)
    {
        switch (module)
        {
            case "electricalFireHazards2":
                moduleTime_ef += moduleTime;
                break;
            case "slipTripLift":
                moduleTime_stl += moduleTime;
                break;
            case "environmental":
                moduleTime_env += moduleTime;
                break;
            case "accessment":
                moduleTime_ass += moduleTime;
                break;
            default:
                break;
        }
    }

    public static string formatTime(float time){
        System.TimeSpan t = System.TimeSpan.FromSeconds(time);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
    }

    static public void trackObject(string module, int room, string name, int answer, int answer_user)
    {
        trackedObject a = new trackedObject();
        a.module = module;
        a.room = room;
        a.answer = answer;
        a._name = name;
        a.answer_user = answer_user;
        trackedObjects.Add(a);
    }

    static public void trackAllObjects(string module, int room, string name, int answer, int answer_user)
    {
        trackedObject a = new trackedObject();
        a.module = module;
        a.room = room;
        a.answer = answer;
        a._name = name;
        a.answer_user = answer_user;
        allObjects.Add(a);
    }


    static public void trackAllObjects_multiple(string module, int room, string name, int answer)
    {
        trackedObject a = new trackedObject();
        a.module = module;
        a.room = room;
        a._name = name;
        a.answer_multiple = new List<int>(){999};
        a.answer_multiple_user = new List<int>() { 999 };
        a.answer_multiple[0]=answer;
        a.frameId = 0;
        allObjects.Add(a);
    }


    static public void trackObjectAbsoluteTime(int objectPosition, string absoluteTime) 
    {
        trackedObjects[objectPosition].absoluteTime = absoluteTime;
    }

    static public void trackObjectSession(int objectPosition)
    {
        trackedObjects[objectPosition].objSession = ObjectTracker.sessionNumber;
    }

    static public void trackObjectTimePages(string pageTitle, float time, int objectPosition)
    {
        trackedObjects[objectPosition].pagesTitles.Add(pageTitle);
        trackedObjects[objectPosition].timeForPages.Add(time);

    }

    static public void trackObject_multiple(string module, int room, string name, List<int> answer_multiple, List<int> answer_multiple_user, float time, int frameId)
    {

        trackedObject a = new trackedObject();
        a.module = module;
        a.room = room;
        a._name = name;
        a.answer_multiple = new List<int>(answer_multiple);
        a.answer_multiple_user = new List<int>(answer_multiple_user);
        a.time = time;
        a.frameId = frameId;
        trackedObjects.Add(a);
    }

    static void printPracticeModuleScore(List<trackedObject> moduleList, StringBuilder sb, string moduleName)
    {
        foreach (trackedObject i in moduleList)
        {
            string collectstring = "";

            collectstring = userID + "," + i.objSession + moduleName + formatTime(moduleTime_env) + "," + i.absoluteTime + LayerMask.LayerToName(i.room) + "," + i._name + ",";
            float answerTime = 0, whyTime = 0, wtdTime = 0, infoTime = 0;

            int finalAnswer = 999;
            if (i.answer == 1)
            {
                if (i.answer_user == 1) finalAnswer = 1;//hit
                else if (i.answer_user == 0) finalAnswer = 3;//miss
                else if (i.answer_user == 999) finalAnswer = 5; //unfound miss
            }
            else if (i.answer == 0)
            {
                if (i.answer_user == 1) finalAnswer = 2;//false alarm
                else if (i.answer_user == 0) finalAnswer = 4;//correct rejection
                else if (i.answer_user == 999) finalAnswer = 6; //unfound correct rejection
            }

            for (int a = 0; a < i.pagesTitles.Count; a++)
            {
                if (i.pagesTitles[a].Contains("Is this a hazard?") || i.pagesTitles[a].Equals("Test")) answerTime += i.timeForPages[a]; // system still saves the old title so this is ok
                else if (i.pagesTitles[a].Contains("Why")) whyTime += i.timeForPages[a];
                else if (i.pagesTitles[a].Contains("What to do")) wtdTime += i.timeForPages[a];
                else if (i.pagesTitles[a].Contains("Additional")) infoTime += i.timeForPages[a];
            }

            if (answerTime.Equals(0)) answerTime = 999;
            if (whyTime.Equals(0)) whyTime = 999;
            if (wtdTime.Equals(0)) wtdTime = 999;
            if (infoTime.Equals(0)) infoTime = 999;

            collectstring += finalAnswer + "," + string.Format("{0:0.#}", answerTime) + "," + string.Format("{0:0.#}", whyTime) + "," + string.Format("{0:0.#}", wtdTime) + "," + string.Format("{0:0.#}", infoTime);
            sb.AppendLine(collectstring);
        }


    }
    static public void printScore() {

        string fileDirectory = Application.dataPath + "/userData/";

        if (!Directory.Exists(fileDirectory)) { Directory.CreateDirectory(Application.dataPath + "/userData/"); }

        string filePath = fileDirectory +  userID + "_" + sessionNumber + "_EF_STL_ENV.csv";
        string filePath_AS = fileDirectory + userID + "_" + sessionNumber + "_AS.csv";

        StringBuilder sb = new StringBuilder();
        StringBuilder sb_AS = new StringBuilder();


       /*separate modules and repace numbers in allobjects list with tractedobjects list*/
        List<trackedObject> singleAnswers_ef = new List<trackedObject>();
        List<trackedObject> singleAnswers_stl = new List<trackedObject>();
        List<trackedObject> singleAnswers_env = new List<trackedObject>();
        List<trackedObject> multipleAnswers = new List<trackedObject>();

        for (int a = 0; a < allObjects.Count; a++)
        {
            trackedObject tO = new trackedObject();
            tO.module = allObjects[a].module;
            tO._name = allObjects[a]._name;
            tO.room = allObjects[a].room;
            List<trackedObject> frames = trackedObjects.FindAll(x => x.Equals(tO));

            if (frames.Count == 0)
                frames.Add(allObjects[a]);
            for (int i = 0; i < frames.Count; i++)
            {
                if (allObjects[a].module.Equals("electricalFireHazards2"))
                    singleAnswers_ef.Add(frames[i]);
                else if (allObjects[a].module.Equals("slipTripLift"))
                    singleAnswers_stl.Add(frames[i]);
                else if (allObjects[a].module.Equals("environmental"))
                    singleAnswers_env.Add(frames[i]);
                else
                { 
                    multipleAnswers.Add(frames[i]);
                }
            }
        }

        singleAnswers_ef = singleAnswers_ef.OrderBy(x => x.room).ToList();
        singleAnswers_stl = singleAnswers_stl.OrderBy(x => x.room).ToList();
        singleAnswers_env = singleAnswers_env.OrderBy(x => x.room).ToList();
        multipleAnswers = multipleAnswers.OrderBy(x => x.room).ToList();
        /*separate modules ends*/

        /*start printing first 3 modules heading*/
        string collectstring_heading = "";

        if (singleAnswers_ef.Count > 0 || singleAnswers_stl.Count > 0 || singleAnswers_env.Count > 0)
        {
            collectstring_heading = "UserID,Session,Module,TotalTimeInModule,Date,Time,Room,Item,ID_Score,ID_time,Why_time,WTD_time,Add'l_info_time";
            sb.AppendLine(collectstring_heading);
        }

        /*print objects in in ef*/
        printPracticeModuleScore(singleAnswers_ef, sb, ",\"Electrical, Fire, & Burn\",");

        /*print objects in stl*/
        printPracticeModuleScore(singleAnswers_stl, sb, ",\"Slip, Trip, & Lift\",");

        /*print objects in env*/
        printPracticeModuleScore(singleAnswers_env, sb, ",Environmental,");

        /*start printing assessment*/
        if (multipleAnswers.Count > 0)
        {
            sb_AS.AppendLine("UserID,Session,Module,TotalTimeInModule,Date,Time,Room,Item,ID_Score,ID_time,why_a,why_b,why_c,why_d,why_time,WTD_a,WTD_b,WTD_c,WTD_d,WTD_time");
        }
        string collectstring_ass = "";
        
        foreach (trackedObject i in multipleAnswers)
        {
            if (i.frameId.Equals(0)) //1st page
            {
                collectstring_ass = userID + "," + i.objSession + ",Accessment," + formatTime(moduleTime_ass) + "," + i.absoluteTime;
                int finalAnswer = 999;
           
               if (i.answer_multiple[0] == 1)
                {
                    if (i.answer_multiple_user[0] == 1) finalAnswer = 1;//hit
                    else if (i.answer_multiple_user[0] == 0) finalAnswer = 3;//miss
                    else if (i.answer_multiple_user[0] == 999) finalAnswer = 5; //unfound miss
                }
                else if (i.answer_multiple[0] == 0)
                {
                    if (i.answer_multiple_user[0].Equals(1)) finalAnswer = 2;//false alarm
                    else if (i.answer_multiple_user[0].Equals(0)) finalAnswer = 4;//correct rejection
                    else if (i.answer_multiple_user[0] == 999) finalAnswer = 6; //unfound correct rejection
                }
                
                float time = i.time;
                if (i.time.Equals(0)) time = 999;
                collectstring_ass += LayerMask.LayerToName(i.room) + "," + i._name + "," + finalAnswer + "," + string.Format("{0:0.#}", time) + ",";//ID_time
                if (!finalAnswer.Equals(1)) {
                    collectstring_ass += "999,999,999,999,999,999,999,999,999,999";
                    sb_AS.AppendLine(collectstring_ass);
                }
            }
            else //2nd~3rd page
            {
                for (int a = 0; a < 4; a++)
                {
                    int finalAnswer = 999;
                    if (a >= i.answer_multiple.Count)
                    {
                        finalAnswer = 999;
                    }
                    else if (i.answer_multiple[a] == 1)
                    {
                        if (i.answer_multiple_user[a] == 1) finalAnswer = 1;//hit
                        else if (i.answer_multiple_user[a] == 0) finalAnswer = 3;//miss
                  //      else if (i.answer_multiple_user[a] == 999) finalAnswer = 5;//unfound miss
                    }
                    else if (i.answer_multiple[a] == 0)
                    {
                        if (i.answer_multiple_user[a] == 1) finalAnswer = 2;//false alarm
                        else if (i.answer_multiple_user[a] == 0) finalAnswer = 4;//correct rejection
                    //    else if (i.answer_multiple_user[a] == 999) finalAnswer = 6;//unfound correct rejection
                    }

                    collectstring_ass += finalAnswer + ",";
                }
                float time = i.time;
                if (i.time.Equals(0)) time = 999;
                collectstring_ass += string.Format("{0:0.#}", time) + ",";
                if (i.frameId.Equals(2)) sb_AS.AppendLine(collectstring_ass);
            }
        }
        /*finish printin assessment*/

        /*writing the .csv file*/
        if (!userID.Equals(""))
        {
            File.WriteAllText(filePath_AS, sb_AS.ToString());
            Debug.Log("AS Data Saved");
            File.WriteAllText(filePath, sb.ToString());
            Debug.Log("Data Saved");
            sessionNumber += 1;
        }
        /*finish writing the file*/
    }
}
