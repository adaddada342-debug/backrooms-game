using System;
using System.Collections.Generic;

namespace Backrooms.SceneAssembly
{
    [Serializable]
    public class SceneAssemblyResult
    {
        public bool succeeded;
        public string sceneName;
        public string scenePath;
        public string planId;
        public List<SceneAssemblyIssue> issues = new List<SceneAssemblyIssue>();
    }
}
