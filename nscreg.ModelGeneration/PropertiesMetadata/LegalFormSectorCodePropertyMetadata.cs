﻿namespace nscreg.ModelGeneration.PropertiesMetadata
{
    public class LegalFormSectorCodePropertyMetadata : PropertyMetadataBase
    {
        public LegalFormSectorCodePropertyMetadata(string name, bool isRequired, int? value, string groupName = null, string localizeKey = null)
            : base(name, isRequired, localizeKey, groupName)
        {
            Value = value;
        }

        public int? Value { get; set; }

        public override PropertyType Selector => PropertyType.SearchComponent;
    }
}