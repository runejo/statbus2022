﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using nscreg.Resources.Languages;
using nscreg.Utilities.Enums;

namespace nscreg.Server.Models.StatUnits.Edit
{
    public class EnterpriseGroupEditM : IStatUnitM
    {
        [Required]
        public int? RegId { get; set; }
        public string StatId { get; set; }

        [DataType(DataType.Date)]
        public DateTime StatIdDate { get; set; }

        public int TaxRegId { get; set; }

        [DataType(DataType.Date)]
        public DateTime TaxRegDate { get; set; }

        public int ExternalId { get; set; }
        public int ExternalIdType { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExternalIdDate { get; set; }

        public string DataSource { get; set; }

        [Required]
        public string Name { get; set; }

        public string ShortName { get; set; }
        public AddressM Address { get; set; }
        public int PostalAddressId { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string TelephoneNo { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataType(DataType.Url)]
        public string WebAddress { get; set; }

        public string EntGroupType { get; set; }

        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }

        public string RegistrationReason { get; set; }

        [DataType(DataType.Date)]
        public DateTime LiqDateStart { get; set; }

        [DataType(DataType.Date)]
        public DateTime LiqDateEnd { get; set; }

        public string LiqReason { get; set; }
        public string SuspensionStart { get; set; }
        public string SuspensionEnd { get; set; }
        public string ReorgTypeCode { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReorgDate { get; set; }

        public string ReorgReferences { get; set; }
        public AddressM ActualAddress { get; set; }
        public string ContactPerson { get; set; }
        public int Employees { get; set; }
        public int EmployeesFte { get; set; }

        [DataType(DataType.Date)]
        public DateTime EmployeesYear { get; set; }

        [DataType(DataType.Date)]
        public DateTime EmployeesDate { get; set; }

        public decimal Turnover { get; set; }

        [DataType(DataType.Date)]
        public DateTime TurnoverYear { get; set; }

        [DataType(DataType.Date)]
        public DateTime TurnoveDate { get; set; }

        public string Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime StatusDate { get; set; }

        public string Notes { get; set; }
        public int[] EnterpriseUnits { get; set; }
        public int[] LegalUnits { get; set; }
        public ICollection<string> DataAccess { get; set; }

        public ChangeReasons ChangeReason { get; set; }
        public string EditComment { get; set; }
    }
    public class EnterpriseGroupEditMValidator : AbstractValidator<EnterpriseGroupEditM>
    {
        public EnterpriseGroupEditMValidator()
        {
            RuleFor(x => x.LegalUnits)
                .Must(x => x != null && x.Length != 0)
                .When(x => x.EnterpriseUnits?.Length == 0)
                .WithMessage(Resource.ChooseAtLeastOne);
            RuleFor(x => x.EnterpriseUnits)
                .Must(x => x != null && x.Length != 0)
                .When(x => x.LegalUnits?.Length == 0)
                .WithMessage(Resource.ChooseAtLeastOne);
            RuleFor(v => v.ChangeReason)
                .NotEmpty()
                .Must(v =>
                    v == ChangeReasons.Edit ||
                    v == ChangeReasons.Correction)
                .WithMessage(nameof(Resource.ChangeReasonMandatory));
            RuleFor(v => v.EditComment)
                .NotEmpty()
                .When(v => v.ChangeReason == ChangeReasons.Edit)
                .WithMessage(nameof(Resource.EditCommentMandatory));

        }
    }
}
