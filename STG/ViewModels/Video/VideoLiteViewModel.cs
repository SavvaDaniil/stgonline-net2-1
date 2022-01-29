﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STG.ViewModels.Video
{
    public class VideoLiteViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string durationStr { get;set; }

        public VideoLiteViewModel(int id, string name, string durationStr)
        {
            this.id = id;
            this.name = name;
            this.durationStr = durationStr;
        }
    }
}
