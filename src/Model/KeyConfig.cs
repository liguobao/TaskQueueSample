using System;

namespace MTQueue.Model
{
    public class KeyConfig
    {
        public string Key { get; set; }

        public TimeSpan ExpireTime { get; set; }

        public int DBName { get; set; }
    }
}