﻿namespace Redis.Common.Dto
{
    public class EntryDto
    {
        public string Key { get; set; }
        public int HashKey { get; set; }
        public string Value { get; set; }
    }
}
