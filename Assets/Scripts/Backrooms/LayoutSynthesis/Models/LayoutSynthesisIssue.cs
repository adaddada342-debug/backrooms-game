using System;

namespace Backrooms.LayoutSynthesis.Models
{
    [Serializable]
    public class LayoutSynthesisIssue
    {
        public string code;
        public string message;
        public bool blocker;
    }
}
