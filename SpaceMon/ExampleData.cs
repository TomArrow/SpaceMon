using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceMon
{
    public class ExampleData
    {
        public DriveFreeSpace[] exampleFreeSpaceData { get; set; } = new DriveFreeSpace[]
        {
            new DriveFreeSpace{ driveLetter="C", freeBytes = 10000000 },
            new DriveFreeSpace{ driveLetter="D", freeBytes = 10000 },
            new DriveFreeSpace{ driveLetter="E", freeBytes = 300400 },
            new DriveFreeSpace{ driveLetter="F", freeBytes = 100000000 },
            new DriveFreeSpace{ driveLetter="G", freeBytes = 1000000000 },
            new DriveFreeSpace{ driveLetter="H", freeBytes = 10000000000 },
            new DriveFreeSpace{ driveLetter="I", freeBytes = 100000000000 },
            new DriveFreeSpace{ driveLetter="J", freeBytes = 10000000 },
            new DriveFreeSpace{ driveLetter="K", freeBytes = 30000000 },
            new DriveFreeSpace{ driveLetter="L", freeBytes = 2000000000 },
            new DriveFreeSpace{ driveLetter="M", freeBytes = 3000000000 },
            new DriveFreeSpace{ driveLetter="N", freeBytes = 4000000000 },
        };
    }
}
