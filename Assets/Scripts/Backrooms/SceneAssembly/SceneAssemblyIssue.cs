using System;

namespace Backrooms.SceneAssembly
{
    [Serializable]
    public class SceneAssemblyIssue
    {
        public string code;
        public string message;
        public bool blocker;
    }
}
