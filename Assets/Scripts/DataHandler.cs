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
            CsvRow header = new CsvRow();
            header.Add("Trial #");
            header.Add("Catch Time");
            header.Add("Throw Time");
            header.Add("Caught");
            header.Add("Thrown");
            header.Add("Hit Target");

            writer.WriteRow(header);

            foreach (Data d in data)
            {
                CsvRow row = new CsvRow();

                row.Add(d.trialNum.ToString());
                row.Add(d.catchTime.ToString());
                row.Add(d.throwTime.ToString());
                row.Add(d.wasCaught.ToString());
                row.Add(d.wasThrown.ToString());
                row.Add(d.hitTarget.ToString());

                writer.WriteRow(row);
            }
        }

        UIController.RecordData -= AddLine;
    }

    private void AddLine(int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget)
    {
        data.Add(new Data(trialNum, catchTime, throwTime, wasCaught, wasThrown, hitTarget));
    }

    class Data
    {
        public readonly int trialNum;
        public readonly float catchTime, throwTime;
        public readonly bool wasCaught, wasThrown, hitTarget;

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
