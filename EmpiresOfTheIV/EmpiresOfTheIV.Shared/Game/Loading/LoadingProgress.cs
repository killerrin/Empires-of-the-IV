using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Loading
{
    public struct LoadingProgress
    {
        public int Progress;
        public string Status;

        public LoadingProgress(int progress)
        {
            Progress = progress;
            Status = "";
        }
        public LoadingProgress(int progress, string status)
        {
            Progress = progress;
            Status = status;
        }

        public override string ToString()
        {
            return Status + ": " + Progress;
        }
    }
}
