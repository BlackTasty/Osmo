using System;
using System.IO;
using Vibrance.Core.Classes;
using UIParser = Vibrance.Core.Classes.Parser;

namespace Osmo.Core.Patcher
{
    public class PatchData : ITimelineEntry, IComparable
    {
        private DateTime patchDate = new DateTime(1989, 1, 1);
        private string patchNumber = "0.0.0.0";
        private string[] patchContent = new string[] { "EMPTY" };
        private bool isSketch, isHotfix;

        private double timelineIndex;
        
        public DateTime PatchDate { get { return patchDate; } }

        public string PatchNumber { get { return patchNumber; } }

        public string[] PatchContent { get { return patchContent; } }

        public bool IsSketch { get { return isSketch; } }

        public bool IsHotfix { get { return isHotfix; } }

        public DateTime GetTimelinePosition()
        {
            return patchDate;
        }

        public PatchData(string path)
        {
            string[] content = File.ReadAllLines(path);
            string[] initialData = content[0].Split(' ');

            SetDate(initialData[0]);

            patchContent = new string[content.Length - 1];
            for (int i = 1; i < content.Length; i++)
                patchContent[i - 1] = content[i];

            patchNumber = new FileInfo(path).Name.Replace(".txt", "");
            if (initialData.Length > 1)
            {
                isSketch = initialData[1].Contains("s") || initialData[1].Contains("n");
                isHotfix = initialData[1].Contains("h");
            }
        }

        /// <summary>
        /// Purely used to create a dummy object
        /// </summary>
        /// <param name="version">The version of this dummy object</param>
        /// <param name="dummyData">If set to true, this dummy object will contain test data</param>
        public PatchData(string version, bool dummyData)
        {
            patchNumber = version;
            if (dummyData)
            {
                patchContent = new string[]
                {
                    "Added: Dummy",
                    "Changed: Dummy dummy",
                    "Fixed: Dummy",
                    "Re-Enabled: No"
                };
                SetDate("17.02.1998");
            }
        }

        /// <summary>
        /// Generates the patch data from a string array
        /// </summary>
        /// <param name="dbData">0 = date, 1 = number, 2 = content</param>
        public PatchData(string[] dbData)
        {
            SetDate(dbData[0]);
            patchNumber = dbData[1];
            patchContent = dbData[2].Split(';');
        }

        private void SetDate(string dateRaw)
        {
            string[] raw = dateRaw.Replace(":", "").Split('.');
            patchDate = new DateTime(UIParser.TryParse(raw[2], 1989), UIParser.TryParse(raw[1], 1), UIParser.TryParse(raw[0], 1));
        }

        public int CompareTo(object obj)
        {
            PatchData pd = (PatchData)obj;
            return patchDate.CompareTo(pd.patchDate);
        }

        public override string ToString()
        {
            if (patchNumber != null)
            {
                if (patchDate != null) //1.0.0.0 [Sketch] [(Hotfix)] - 01.01.2018
                    return string.Format("{0} {1} {2} - {3}",
                        patchNumber, IsSketch ? "Sketch" : "", IsHotfix ? "(Hotfix)" : "", patchDate.ToShortDateString());
                else //1.0.0.0 [Sketch] [(Hotfix)]
                    return string.Format("{0} {1} {2}", 
                        patchNumber, IsSketch ? "Sketch" : "", IsHotfix ? "(Hotfix)": "");
            }
            else
                return "EMPTY";
        }

        public string GetTooltip()
        {
            return patchNumber;
        }

        public void SetTimelineIndex(double value)
        {
            timelineIndex = value;
        }

        public double GetTimelineIndex()
        {
            return timelineIndex;
        }
    }
}
