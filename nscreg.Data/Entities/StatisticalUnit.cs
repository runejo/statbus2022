﻿using System;
using System.ComponentModel.DataAnnotations;

namespace nscreg.Data.Entities
{
    public class StatisticalUnit : IStatisticalUnit
    {
        [Key]
        public int RegId { get; set; }  //	Automatically generated id unit
        public DateTime RegIdDate { get; set; } //	Date of id (i.e. Date of unit entered into the register)
        public int StatId { get; set; } //	The Identifier given the Statistical unit by NSO
        public DateTime StatIdDate { get; set; }    //	Date of unit registered within the NSO (Might be before it was entered into this register)
        public int TaxRegId { get; set; }   //	unique fiscal code from tax authorities
        public DateTime TaxRegDate { get; set; }    //	Date of registration at tax authorities
        public int ExternalId { get; set; } //	ID of another external data source
        public int ExternalIdType { get; set; } //	Type of external  id (linked to table containing possible types)
        public DateTime ExternalIdDate { get; set; }    //	Date of registration in external source
        public string DataSource { get; set; }  //	code of data source (linked to source table(s)
        public int RefNo { get; set; }  //	Reference number to paper questionnaire
        public string Name { get; set; }    //	Full name of Unit
        public string ShortName { get; set; }   //	Short name of legal unit/soundex name (to make it more searchable)
        public int AddressId { get; set; }  //	ID of visiting address (as given by the sources)
        public int PostalAddressId { get; set; }    //	Id of postal address (post box or similar, if relevant)
        public string TelephoneNo { get; set; } //	
        public string EmailAddress { get; set; }    //	
        public string WebAddress { get; set; }  //	
        public string RegMainActivity { get; set; } //	Code of main activity as originally registered  (Nace or ISIC)
        public DateTime RegistrationDate { get; set; }  //	Date of registration
        public string RegistrationReason { get; set; }  //	Reason for registration
        public string LiqDate { get; set; } //	Liquidation details, if relevant
        public string LiqReason { get; set; }   //	
        public string SuspensionStart { get; set; } //	suspension details, if relevant
        public string SuspensionEnd { get; set; }   //	
        public string ReorgTypeCode { get; set; }   //	Code of reorganization type (merger, split etc)
        public DateTime ReorgDate { get; set; } //	
        public string ReorgReferences { get; set; } //	Ids of other units affected by the reorganisation
        public string ActualAddressId { get; set; } //	Address after it has been corrected by NSO
        public string ContactPerson { get; set; }   //	
        public int Employees { get; set; }  //	Number of employees (excluding owner)
        public int NumOfPeople { get; set; }    //	Number of people employed (including owner)
        public DateTime EmployeesYear { get; set; } //	Year of which the employee information is/was valid
        public DateTime EmployeesDate { get; set; } //	Date of registration of employees data
        public string Turnover { get; set; }    //	
        public DateTime TurnoverYear { get; set; }  //	Year of which the turnover is/was valid
        public DateTime TurnoveDate { get; set; }   //	Date of registration of the current turnover
        public string Status { get; set; }  //	Active/inactive/dormant (or national classification)
        public DateTime StatusDate { get; set; }    //	
        public string Notes { get; set; }   //	
        public bool FreeEconZone { get; set; }  //	Yes/no (whether the unit operates in a Free economic zone with different tax rules)
        public string ForeignParticipation { get; set; }    //	Dependent on the country, this might be a variable that is irrelevant, is a yes/no question, or has a longer code list. (In Kyrgyzstan it has 9 elements)
        public string Classified { get; set; }	//	Whether the information about the unit is classified or not
        public bool IsDeleted { get; set; }
    }
}