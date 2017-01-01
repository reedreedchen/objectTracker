using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class trackedObject : IEquatable<trackedObject> {
    public string absoluteTime="999,999,";
    public string module;
    public int room;
    public string _name;
    public float time;

    public int objSession=999;
    public List<float> timeForPages = new List<float>();
    public List<string> pagesTitles = new List<string>();

    //single
    public int answer;
    public int answer_user;

    //multiple
    public int frameId;
    public List<int> answer_multiple;
    public List<int> answer_multiple_user;

    public int GetHashCode(object obj)
    {
        int hash = 17;
        hash = hash * 23 + module.GetHashCode();
        hash = hash * 23 + _name.GetHashCode();
        hash = hash * 23 + room.GetHashCode();
        return hash;
    }

    public bool Equals(trackedObject other)
    {
        if (other == null) return false;
        return (this.module == other.module && this._name == other._name && this.room == other.room);
    }
}


