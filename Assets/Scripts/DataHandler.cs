using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;

/// <summary>
/// Saves trial data in a list, then writes to a file when the game is closed. 
/// </summary>
public class DataHandler : MonoBehaviour {

    private List<Data> data = new List<Data>();
    private string fileName;
    private int finalScore;

	// Use this for initialization
	void Awake()
    {
        string identifier;

        switch (GameControl.Instance.label)
        {
            case MenuController.SessionLabels.BASELINE:
                identifier = "_baseline";
                break;
            case MenuController.SessionLabels.ACQUISITION1:
                identifier = "_aquisition1";
                break;
            case MenuController.SessionLabels.ACQUISITION2:
                identifier = "_aquisition2";
                break;
            case MenuController.SessionLabels.RETENTION:
                identifier = "_retention";
                break;
            case MenuController.SessionLabels.RETENTION_DISTRACTION:
                identifier = "_retention_distraction";
                break;
            default: // else transfer
                identifier = "_transfer";
                break;
        }

        UIController.RecordData += AddLine;
        System.DateTime today = System.DateTime.Today;
        fileName = GameControl.Instance.participantID + "_" + today.ToString("d").Replace('/','_') + identifier;
	}

    void OnDisable()
    {
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + fileName + ".csv"))
        {
            CsvRow header = new CsvRow();
            header.Add("Trial #");
            header.Add("Catch Time");
            header.Add("Throw Time");
            header.Add("Caught");
            header.Add("Thrown");
            header.Add("Hit Target");
            header.Add("Score");

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
                row.Add(d.score.ToString());

                writer.WriteRow(row);
            }

            writer.WriteRow(new CsvRow());
            CsvRow score = new CsvRow();
            score.Add("Score total: ");
            score.Add(finalScore.ToString());
        }

        UIController.RecordData -= AddLine;
    }

    private void AddLine(int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget, int score)
    {
        data.Add(new Data(trialNum, catchTime, throwTime, wasCaught, wasThrown, hitTarget, score));
    }

    class Data
    {
        public readonly int trialNum, score;
        public readonly float catchTime, throwTime;
        public readonly bool wasCaught, wasThrown, hitTarget;

        public Data(int trialNum, float catchTime, float throwTime, 
            bool wasCaught, bool wasThrown, bool hitTarget, int score)
        {
            this.trialNum = trialNum;
            this.catchTime = catchTime;
            this.throwTime = throwTime;
            this.wasCaught = wasCaught;
            this.wasThrown = wasThrown;
            this.hitTarget = hitTarget;
            this.score = score;
        }
    }
}
