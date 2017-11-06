using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;
using System.IO;

/// <summary>
/// Saves trial data in a list, then writes to a file when the game is closed. 
/// </summary>
public class DataHandler : MonoBehaviour {

    private List<Data> data = new List<Data>();
    private string fileName;
    private int finalScore = 0;
    private int numSuccesses = 0;
    StreamWriter summaryOutput;

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
        UIController.OnTrialsComplete += WriteSummary;
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
                if (d.wasCaught) {
                    row.Add("yes");
                }
                else
                {
                    row.Add("no");
                }
                if (d.wasThrown)
                {
                    row.Add("yes");
                }
                else
                {
                    row.Add("no");
                }
                if (d.hitTarget)
                {
                    row.Add("yes");
                }
                else
                {
                    row.Add("no");
                }
                row.Add(d.score.ToString());

                writer.WriteRow(row);
            }

            writer.WriteRow(new CsvRow());
            
            CsvRow sumheader = new CsvRow();
            sumheader.Add("Level");
            sumheader.Add("Score");
            sumheader.Add("Successful Trials");
            sumheader.Add("Total Trials");
            sumheader.Add("Success Rate");

            writer.WriteRow(sumheader);

            string diff;

            switch (GameControl.Instance.label)
            {
                case MenuController.SessionLabels.BASELINE:
                    diff = "baseline";
                    break;
                case MenuController.SessionLabels.ACQUISITION1:
                    diff = "aquisition 1";
                    break;
                case MenuController.SessionLabels.ACQUISITION2:
                    diff = "aquisition 2";
                    break;
                case MenuController.SessionLabels.RETENTION:
                    diff = "retention";
                    break;
                case MenuController.SessionLabels.RETENTION_DISTRACTION:
                    diff = "retention distraction";
                    break;
                default:
                    diff = "transfer";
                    break;

            }

            CsvRow summary = new CsvRow();
            summary.Add(diff);
            summary.Add(finalScore.ToString());
            summary.Add(numSuccesses.ToString());
            summary.Add(GameControl.Instance.numTrials.ToString());
            summary.Add(((float)numSuccesses / (float)GameControl.Instance.numTrials).ToString());
            writer.WriteRow(summary);

        }

        UIController.RecordData -= AddLine;
        UIController.OnTrialsComplete -= WriteSummary;
    }

    private void AddLine(int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget, int score)
    {
        data.Add(new Data(trialNum, catchTime, throwTime, wasCaught, wasThrown, hitTarget, score));
    }

    private void WriteSummary(int score, int successes)
    {
        finalScore = score;
        numSuccesses = successes;
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
