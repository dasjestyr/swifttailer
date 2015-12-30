﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SwiftTailer.Wpf.Infrastructure
{
    public class FileDropContainer
    {
        public FileDropContainer(IEnumerable<string> files)
        {
            Files = files.Select(Path.GetFileName).ToArray();
        }

        public IEnumerable<string> Files { get; }
    }
}
