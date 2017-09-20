﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using nscreg.Data.Constants;
using nscreg.Utilities.Attributes;
using nscreg.Utilities.Enums;

namespace nscreg.Data.Entities
{
    public class EnterpriseUnit : StatisticalUnit
    {
        public EnterpriseUnit()
        {
            LegalUnits = new HashSet<LegalUnit>();
        }

        public override StatUnitTypes UnitType => StatUnitTypes.EnterpriseUnit;
        [Display(Order = 800, GroupName = GroupNames.RegistrationInfo)]
        public DateTime EntGroupIdDate { get; set; }    //	Date of assosciation with enterprise group
        [Display(Order = 380, GroupName = GroupNames.RegistrationInfo)]
        public bool Commercial { get; set; }  //	Indicator for non-commercial activity (marked/non-marked?)
        [Display(GroupName = GroupNames.StatUnitInfo)]
        [Reference(LookupEnum.SectorCodeLookup)]
        public override int? InstSectorCodeId
        {
            get => base.InstSectorCodeId;
            set => base.InstSectorCodeId = value;
        }    //	Institutional sector code (see Annex 3)
        [NotMappedFor(ActionsEnum.Create | ActionsEnum.Edit | ActionsEnum.View)]
        public override int? LegalFormId
        {
            get => null;
            set { }
        }

        [Display (GroupName = GroupNames.CapitalInfo)]
        public string TotalCapital { get; set; }    //	total 5 fields (sums up the next ones) 
        [Display(GroupName = GroupNames.CapitalInfo)]
        public string MunCapitalShare { get; set; } //	
        [Display(GroupName = GroupNames.CapitalInfo)]
        public string StateCapitalShare { get; set; }   //	
        [Display(GroupName = GroupNames.CapitalInfo)]
        public string PrivCapitalShare { get; set; }    //	
        [Display(GroupName = GroupNames.CapitalInfo)]
        public string ForeignCapitalShare { get; set; } //
        [Display(GroupName = GroupNames.CapitalInfo)]
        public string ForeignCapitalCurrency { get; set; }  //	
        [Display(Order = 90, GroupName = GroupNames.StatUnitInfo)]
        public string EntGroupRole { get; set; }
        //	Role of enterprise within enterprise group (Management/control unit, global group head (controlling unit), Global decision centre (managing unit), highest level consolidation unit or “other”

        [Reference(LookupEnum.EnterpriseGroupLookup)]
        [Display(Order = 70, GroupName = GroupNames.LinkInfo)]
        public int? EntGroupId { get; set; } //	ID of enterprise group of which the unit belongs
        [NotMappedFor(ActionsEnum.Create | ActionsEnum.Edit | ActionsEnum.View)]
        public virtual EnterpriseGroup EnterpriseGroup { get; set; }

        [Reference(LookupEnum.LegalUnitLookup)]
        [Display(Order = 320, GroupName = GroupNames.LinkInfo)]
        public virtual ICollection<LegalUnit> LegalUnits { get; set; }
        [NotMappedFor(ActionsEnum.Create | ActionsEnum.Edit | ActionsEnum.View)]
        public string HistoryLegalUnitIds { get; set; }
    }
}
