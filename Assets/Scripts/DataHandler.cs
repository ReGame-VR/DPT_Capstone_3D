using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;

/// <summary>
/// Saves trial data in a list, then writes to a file when the game is closed. 
/// </summary>
public class DataHandler : MonoBehaviour {

    // a list of the data from each trial
    private List<Data> data = new List<Data>();

    // the file name of the .csv data file
    private string fileName;

    // the score after the last trial
    private int finalScore = 0;

    // the number of trials where the ball was successfully caught, thrown, and hit target
    private int numSuccesses = 0;

	/// <summary>
    /// Create the file name based on user ID, time, and session label and subscribe to relevant
    /// events.
    /// </summary>
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

    /// <summary>
    /// Write data to .csv file, close file stream, and unsubscribe from events.
    /// </summary>
    void OnDisable()
    {
        // all data writing takes place inside using statement
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

    /// <summary>
    /// Adds a new Data entry into the List of data, to be written to file in the disable function.
    /// </summary>
    /// <param name="trialNum"></param> which trial this is
    /// <param name="catchTime"></param> seconds it took to catch ball, infinity if not caught
    /// <param name="throwTime"></param> seconds it took to throw ball after catching, infinity if not thrown
    /// <param name="wasCaught"></param> was the ball caught?
    /// <param name="wasThrown"></param> was the ball thrown?
    /// <param name="hitTarget"></param> did the ball hit the target?
    /// <param name="score"></param>
    private void AddLine(int trialNum, float catchTime,
        float throwTime, bool wasCaught, bool wasThrown, bool hitTarget, int score)
    {
        data.Add(new Data(trialNum, catchTime, throwTime, wasCaught, wasThrown, hitTarget, score));
    }

    /// <summary>
    /// When all trials are done, record the final score and the number of successes.
    /// </summary>
    /// <param name="score"></param>
    /// <param name="successes"></param>
    private void WriteSummary(int score, int successes)
    {
        finalScore = score;
        numSuccesses = successes;
    }

    /// <summary>
    /// A class for storing data on each trial.
    /// </summary>
    class Data
    {
        public readonly int trialNum, score; // which trial it is, and the score for that trial only
        public readonly float catchTime, throwTime; // time taken to catch, time taken to throw
        public readonly bool wasCaught, wasThrown, hitTarget; 
                            // was the ball caught, thrown, thrown into target?

        /// <summary>
        /// Constructor for a Data object.
        /// </summary>
        /// <param name="trialNum"></param> the trial number
        /// <param name="catchTime"></param> the time it took to catch
        /// <param name="throwTime"></param> the time it took to throw
        /// <param name="wasCaught"></param> was the ball caught?
        /// <param name="wasThrown"></param> was the ball thrown?
        /// <param name="hitTarget"></param> did the ball hit the target?
        /// <param name="score"></param>
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
