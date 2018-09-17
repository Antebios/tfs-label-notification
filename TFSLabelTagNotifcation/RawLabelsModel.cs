using System;
using System.Collections.Generic;
using System.Text;

namespace TFSLabelTagNotifcation
{
    public class RawLabelsModel
    {
        public int PartitionId { get; set; }
        public int LabelId { get; set; }
        public string LabelName { get; set; }
        public bool IsNotified { get; set; }
    }

    public class FinalLabelsModel
    {
        public int LabelId { get; set; }
        public string LabelName { get; set; }
        public string Comment { get; set; }
        public int Changeset { get; set; }
        public string ProjectName { get; set; }
        public string Path { get; set; }
        public DateTime LastModified { get; set; }
        public string User { get; set; }
    }
}
