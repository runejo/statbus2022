﻿namespace nscreg.Server.Models.Dynamic.Property
{
    public class StringPropertyMetadata : PropertyMetadataBase
    {
        public StringPropertyMetadata(string name, bool isRequired, string value) : base(name, isRequired)
        {
            Value = value;
        }

        public string Value { get; set; }
        public override PropertyType Selector => PropertyType.String;

    }
}