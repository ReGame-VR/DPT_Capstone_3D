using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;


public class DataHandler : MonoBehaviour {

    private List<Data> data = new List<Data>();

	// Use this for initialization
	void Awake()
    {
        UIController.RecordData += AddLine;
	}

    void OnDisable()
    {
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/test.csv"))
        {
            foreach (Data d in data)
            {
                // do the thing
            }
        }

        UIController.RecordData -= AddLine;
    }

    private void AddLine(int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget)
    {
        data.Add(new Data(trialNum, catchTime, throwTime, wasCaught, wasThrown, hitTarget));
    }

    private class Data
    {
        int trialNum;
        float catchTime, throwTime;
        bool wasCaught, wasThrown, hitTarget;

        public Data(int trialNum, float catchTime, float throwTime, 
            bool wasCaught, bool wasThrown, bool hitTarget)
        {
            this.trialNum = trialNum;
            this.catchTime = catchTime;
            this.throwTime = throwTime;
            this.wasCaught = wasCaught;
            this.wasThrown = wasThrown;
            this.hitTarget = hitTarget;
        }
    }
}
