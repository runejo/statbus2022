using System;
using System.Linq;
using nscreg.Data.Constants;
using nscreg.Data.Entities;

namespace nscreg.Data
{
    internal static partial class SeedData
    {
        public static void AddAnalysisLogs(NSCRegDbContext context)
        {
            var queueEntry = new AnalysisQueue
            {
                UserStartPeriod = DateTime.Now,
                UserEndPeriod = DateTime.Now,
                ServerStartPeriod = DateTime.Now,
                User = context.Users.First(),
            };

            var log = new[]
            {
                new AnalysisLog { AnalyzedUnitId = 1, AnalyzedUnitType = StatUnitTypes.LocalUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:25.7736777"), ResolvedAt = null, ErrorValues = "{'LegalUnitId':['AnalysisRelatedLegalUnit'],'Activities':['AnalysisRelatedActivity'],'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive']}" },
                new AnalysisLog { AnalyzedUnitId = 2, AnalyzedUnitType = StatUnitTypes.LocalUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:28.4686079"), ResolvedAt = null, ErrorValues = "{'LegalUnitId':['AnalysisRelatedLegalUnit'],'Activities':['AnalysisRelatedActivity'],'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive']}" },
                new AnalysisLog { AnalyzedUnitId = 3, AnalyzedUnitType = StatUnitTypes.LegalUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:30.3878882"), ResolvedAt = null, ErrorValues = "{'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive'],'Persons':['AnalysisMandatoryPersonOwner']}" },
                new AnalysisLog { AnalyzedUnitId = 4, AnalyzedUnitType = StatUnitTypes.EnterpriseUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings;OrphanUnitsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:32.1186972"), ResolvedAt = null, ErrorValues = "{'LegalUnits':['AnalysisRelatedLegalUnit'],'Activities':['AnalysisRelatedActivity'],'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive'],'EntGroupId':['AnalysisOrphanEnterprise']}" },
                new AnalysisLog { AnalyzedUnitId = 5, AnalyzedUnitType = StatUnitTypes.EnterpriseUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings;OrphanUnitsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:33.8364446"), ResolvedAt = null, ErrorValues = "{'LegalUnits':['AnalysisRelatedLegalUnit'],'Activities':['AnalysisRelatedActivity'],'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive'],'EntGroupId':['AnalysisOrphanEnterprise']}" },
                new AnalysisLog { AnalyzedUnitId = 6, AnalyzedUnitType = StatUnitTypes.EnterpriseUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings;OrphanUnitsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:35.5120566"), ResolvedAt = null, ErrorValues = "{'LegalUnits':['AnalysisRelatedLegalUnit'],'Activities':['AnalysisRelatedActivity'],'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive'],'EntGroupId':['AnalysisOrphanEnterprise']}" },
                new AnalysisLog { AnalyzedUnitId = 7, AnalyzedUnitType = StatUnitTypes.EnterpriseUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings;OrphanUnitsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:37.2072083"), ResolvedAt = null, ErrorValues = "{'LegalUnits':['AnalysisRelatedLegalUnit'],'Activities':['AnalysisRelatedActivity'],'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive'],'EntGroupId':['AnalysisOrphanEnterprise']}" },
                new AnalysisLog { AnalyzedUnitId = 8, AnalyzedUnitType = StatUnitTypes.EnterpriseUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings;OrphanUnitsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:39.0724324"), ResolvedAt = null, ErrorValues = "{'LegalUnits':['AnalysisRelatedLegalUnit'],'Activities':['AnalysisRelatedActivity'],'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive'],'EntGroupId':['AnalysisOrphanEnterprise']}" },
                new AnalysisLog { AnalyzedUnitId = 9, AnalyzedUnitType = StatUnitTypes.EnterpriseUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:40.7310654"), ResolvedAt = null, ErrorValues = "{'Activities':['AnalysisRelatedActivity'],'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive']}" },
                new AnalysisLog { AnalyzedUnitId = 10, AnalyzedUnitType = StatUnitTypes.LegalUnit, SummaryMessages = "ConnectionRulesWarnings;MandatoryFieldsRulesWarnings", IssuedAt = DateTime.Parse("2017-12-22 19:09:42.3663417"), ResolvedAt = null, ErrorValues = "{'Address':['AnalysisRelatedAddress'],'DataSourceClassificationId':['AnalysisMandatoryDataSource'],'ShortName':['AnalysisMandatoryShortName'],'TelephoneNo':['AnalysisMandatoryTelephoneNo'],'RegistrationReason':['AnalysisMandatoryRegistrationReason'],'ContactPerson':['AnalysisMandatoryContactPerson'],'Status':['AnalysisMandatoryStatusActive'],'Persons':['AnalysisMandatoryPersonOwner']}" },
            };

            foreach (var entry in log) entry.AnalysisQueue = queueEntry;

            context.AnalysisLogs.AddRange(log);
            context.SaveChanges();
        }
    }
}
