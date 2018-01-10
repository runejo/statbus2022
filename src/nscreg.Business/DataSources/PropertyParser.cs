using nscreg.Data.Entities;
using System;
using System.Globalization;
using System.Reflection;
using nscreg.Data.Constants;
using static nscreg.Utilities.JsonPathHelper;

namespace nscreg.Business.DataSources
{
    public static class PropertyParser
    {
        public static object ConvertOrDefault(Type type, string raw)
        {
            try
            {
                return Convert.ChangeType(raw, type, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
            }
        }

        public static Activity ParseActivity(string propPath, string value, Activity prev)
        {
            var result = prev ?? new Activity();
            switch (PathHead(propPath))
            {
                case nameof(Activity.ActivityType):
                    if (Enum.TryParse(value, true, out ActivityTypes activityType))
                        result.ActivityType = activityType;
                    else throw BadValueFor<ActivityTypes>(propPath, value);
                    break;
                case nameof(Activity.ActivityCategory):
                    result.ActivityCategory = ParseActivityCategory(PathTail(propPath), value, result.ActivityCategory);
                    break;
                default: throw UnsupportedPropertyOf<Activity>(propPath);
            }
            return result;
        }

        public static Person ParsePerson(string propPath, string value, Person prev)
        {
            var result = prev ?? new Person();
            switch (PathHead(propPath))
            {
                case nameof(Person.GivenName):
                    result.GivenName = value;
                    break;
                case nameof(Person.MiddleName):
                    result.MiddleName = value;
                    break;
                case nameof(Person.Surname):
                    result.Surname = value;
                    break;
                case nameof(Person.PersonalId):
                    result.PersonalId = value;
                    break;
                case nameof(Person.BirthDate):
                    if (DateTime.TryParse(value, out var birthDate)) result.BirthDate = birthDate;
                    else throw BadValueFor<Person>(propPath, value);
                    break;
                case nameof(Person.NationalityCode):
                    result.NationalityCode = ParseCountry(PathTail(propPath), value, result.NationalityCode);
                    break;
                default: throw UnsupportedPropertyOf<Person>(propPath);
            }
            return result;
        }

        public static Address ParseAddress(string propPath, string value, Address prev)
        {
            var result = prev ?? new Address();
            switch (PathHead(propPath))
            {
                case nameof(Address.AddressPart1):
                    result.AddressPart1 = value;
                    break;
                case nameof(Address.AddressPart2):
                    result.AddressPart2 = value;
                    break;
                case nameof(Address.AddressPart3):
                    result.AddressPart3 = value;
                    break;
                case nameof(Address.Region):
                    result.Region = ParseRegion(PathTail(propPath), value, result.Region);
                    break;
                default: throw UnsupportedPropertyOf<Address>(propPath);
            }
            return result;
        }

        public static ActivityCategory ParseActivityCategory(string prop, string value, ActivityCategory prev)
        {
            var result = prev ?? new ActivityCategory();
            switch (prop)
            {
                case nameof(ActivityCategory.Code):
                    result.Code = value;
                    break;
                case nameof(ActivityCategory.Name):
                    result.Name = value;
                    break;
                case nameof(ActivityCategory.Section):
                    result.Section = value;
                    break;
                default: throw UnsupportedPropertyOf<ActivityCategory>(prop);
            }
            return result;
        }

        public static Region ParseRegion(string prop, string value, Region prev)
        {
            var result = prev ?? new Region();
            switch (prop)
            {
                case nameof(Region.Code):
                    result.Code = value;
                    break;
                case nameof(Region.Name):
                    result.Name = value;
                    break;
                case nameof(Region.AdminstrativeCenter):
                    result.AdminstrativeCenter = value;
                    break;
                default: throw UnsupportedPropertyOf<Region>(prop);
            }
            return result;
        }

        public static Country ParseCountry(string prop, string value, Country prev)
        {
            var result = prev ?? new Country();
            switch (prop)
            {
                case nameof(Country.Code):
                    result.Code = value;
                    break;
                case nameof(Country.Name):
                    result.Name = value;
                    break;
                default: throw UnsupportedPropertyOf<Country>(prop);
            }
            return result;
        }

        public static LegalForm ParseLegalForm(string prop, string value, LegalForm prev)
        {
            var result = prev ?? new LegalForm();
            switch (prop)
            {
                case nameof(LegalForm.Code):
                    result.Code = value;
                    break;
                case nameof(LegalForm.Name):
                    result.Name = value;
                    break;
                default: throw UnsupportedPropertyOf<LegalForm>(prop);
            }
            return result;
        }

        public static SectorCode ParseSectorCode(string prop, string value, SectorCode prev)
        {
            var result = prev ?? new SectorCode();
            switch (prop)
            {
                case nameof(LegalForm.Code):
                    result.Code = value;
                    break;
                case nameof(LegalForm.Name):
                    result.Name = value;
                    break;
                default: throw UnsupportedPropertyOf<SectorCode>(prop);
            }
            return result;
        }

        public static DataSourceClassification ParseDataSourceClassification(string prop, string value,
            DataSourceClassification prev)
        {
            var result = prev ?? new DataSourceClassification();
            switch (prop)
            {
                case nameof(DataSourceClassification.Name):
                    result.Name = value;
                    break;
                default: throw UnsupportedPropertyOf<DataSourceClassification>(prop);
            }
            return result;
        }

        private static Exception UnsupportedPropertyOf<T>(string propPath) =>
            new Exception($"Property path `{propPath}` in type `{nameof(T)}` is not supported");

        private static Exception BadValueFor<T>(string propPath, string rawValue) =>
            new Exception($"Value `{rawValue}` at property path `{propPath}` in type `{nameof(T)}` couldn't be parsed");
    }
}
