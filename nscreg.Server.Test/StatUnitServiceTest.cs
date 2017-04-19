using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using nscreg.Data.Constants;
using nscreg.Data.Entities;
using nscreg.Server.Core;
using nscreg.Server.Models;
using nscreg.Server.Models.Lookup;
using nscreg.Server.Models.StatUnits;
using nscreg.Server.Models.StatUnits.Create;
using nscreg.Server.Models.StatUnits.Edit;
using nscreg.Server.Services;
using nscreg.Server.Test.Extensions;
using Xunit;
using static nscreg.Server.Test.InMemoryDb;

namespace nscreg.Server.Test
{
    public class StatUnitServiceTest
    {
        #region SearchTests

        [Theory]
        [InlineData(StatUnitTypes.LegalUnit)]
        [InlineData(StatUnitTypes.LocalUnit)]
        [InlineData(StatUnitTypes.EnterpriseUnit)]
        [InlineData(StatUnitTypes.EnterpriseGroup)]
        public async void SearchByNameOrAddressTest(StatUnitTypes unitType)
        {
            var unitName = Guid.NewGuid().ToString();
            var addressPart = Guid.NewGuid().ToString();
            var address = new Address {AddressPart1 = addressPart};
            using (var context = CreateContext())
            {
                context.Initialize();
                IStatisticalUnit unit;
                switch (unitType)
                {
                    case StatUnitTypes.LocalUnit:
                        unit = new LocalUnit {Name = unitName, Address = address};
                        context.LocalUnits.Add((LocalUnit) unit);
                        break;
                    case StatUnitTypes.LegalUnit:
                        unit = new LegalUnit {Name = unitName, Address = address};
                        context.LegalUnits.Add((LegalUnit) unit);
                        break;
                    case StatUnitTypes.EnterpriseUnit:
                        unit = new EnterpriseUnit {Name = unitName, Address = address};
                        context.EnterpriseUnits.Add((EnterpriseUnit) unit);
                        break;
                    case StatUnitTypes.EnterpriseGroup:
                        unit = new EnterpriseGroup {Name = unitName, Address = address};
                        context.EnterpriseGroups.Add((EnterpriseGroup) unit);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
                }
                context.SaveChanges();

                #region ByName

                var query = new SearchQueryM {Wildcard = unitName.Remove(unitName.Length - 1)};
                var result = await new StatUnitService(context).Search(query, DbContextExtensions.UserId);
                Assert.Equal(1, result.TotalCount);

                #endregion

                #region ByAddress

                query = new SearchQueryM {Wildcard = addressPart.Remove(addressPart.Length - 1)};
                result = await new StatUnitService(context).Search(query, DbContextExtensions.UserId);
                Assert.Equal(1, result.TotalCount);

                #endregion
            }
        }

        [Fact]
        public async void SearchByNameMultiplyResultTest()
        {
            var commonName = Guid.NewGuid().ToString();
            var legal = new LegalUnit {Name = commonName + Guid.NewGuid()};
            var local = new LocalUnit {Name = Guid.NewGuid() + commonName + Guid.NewGuid()};
            var enterprise = new EnterpriseUnit {Name = Guid.NewGuid() + commonName};
            var group = new EnterpriseGroup {Name = Guid.NewGuid() + commonName};
            using (var context = CreateContext())
            {
                context.Initialize();
                context.LegalUnits.Add(legal);
                context.LocalUnits.Add(local);
                context.EnterpriseUnits.Add(enterprise);
                context.EnterpriseGroups.Add(group);
                context.SaveChanges();

                var query = new SearchQueryM {Wildcard = commonName};
                var result = await new StatUnitService(context).Search(query, DbContextExtensions.UserId);

                Assert.Equal(4, result.TotalCount);
            }
        }

        [Theory]
        [InlineData("2017", 3)]
        [InlineData("2016", 1)]
        public async void SearchUnitsByCode(string code, int rows)
        {
            AutoMapperConfiguration.Configure();

            using (var context = CreateContext())
            {
                context.Initialize();
                var service = new StatUnitService(context);

                var list = new StatisticalUnit[]
                {
                    new LegalUnit {StatId = "201701", Name = "Unit1"},
                    new LegalUnit {StatId = "201602", Name = "Unit2"},
                    new LocalUnit() {StatId = "201702", Name = "Unit3"},
                };
                context.StatisticalUnits.AddRange(list);

                var group = new EnterpriseGroup() {StatId = "201703", Name = "Unit4"};
                context.EnterpriseGroups.Add(group);

                await context.SaveChangesAsync();

                var result = await service.Search(code);

                Assert.Equal(rows, result.Count);
            }
        }

        [Theory]
        [InlineData(StatUnitTypes.LegalUnit)]
        [InlineData(StatUnitTypes.LocalUnit)]
        [InlineData(StatUnitTypes.EnterpriseUnit)]
        [InlineData(StatUnitTypes.EnterpriseGroup)]
        public async void SearchUsingUnitTypeTest(StatUnitTypes type)
        {
            using (var context = CreateContext())
            {
                context.Initialize();
                var unitName = Guid.NewGuid().ToString();
                var legal = new LegalUnit {Name = unitName};
                var local = new LocalUnit {Name = unitName};
                var enterprise = new EnterpriseUnit {Name = unitName};
                var group = new EnterpriseGroup {Name = unitName};
                context.LegalUnits.Add(legal);
                context.LocalUnits.Add(local);
                context.EnterpriseUnits.Add(enterprise);
                context.EnterpriseGroups.Add(group);
                context.SaveChanges();

                var query = new SearchQueryM
                {
                    Wildcard = unitName,
                    Type = type
                };

                var result = await new StatUnitService(context).Search(query, DbContextExtensions.UserId);

                Assert.Equal(1, result.TotalCount);
            }
        }

        #endregion

        #region CreateTest

        [Theory]
        [InlineData(StatUnitTypes.LegalUnit)]
        [InlineData(StatUnitTypes.LocalUnit)]
        [InlineData(StatUnitTypes.EnterpriseUnit)]
        [InlineData(StatUnitTypes.EnterpriseGroup)]
        public async Task CreateTest(StatUnitTypes type)
        {
            AutoMapperConfiguration.Configure();
            var unitName = Guid.NewGuid().ToString();
            var address = new AddressM {AddressPart1 = Guid.NewGuid().ToString()};
            var expected = typeof(BadRequestException);
            Type actual = null;
            using (var context = CreateContext())
            {
                context.Initialize();
                switch (type)
                {
                    case StatUnitTypes.LegalUnit:
                        await new StatUnitService(context).CreateLegalUnit(new LegalUnitCreateM
                        {
                            Name = unitName,
                            Address = address,
                            Activities = new List<ActivityM>()
                        }, DbContextExtensions.UserId);

                        Assert.IsType<LegalUnit>(
                            context.LegalUnits.Single(
                                x =>
                                    x.Name == unitName && x.Address.AddressPart1 == address.AddressPart1 && !x.IsDeleted));
                        try
                        {
                            await new StatUnitService(context).CreateLegalUnit(new LegalUnitCreateM
                            {
                                Name = unitName,
                                Address = address,
                                Activities = new List<ActivityM>()
                            }, DbContextExtensions.UserId);
                        }
                        catch (Exception e)
                        {
                            actual = e.GetType();
                        }
                        Assert.Equal(expected, actual);
                        break;
                    case StatUnitTypes.LocalUnit:
                        await new StatUnitService(context).CreateLocalUnit(new LocalUnitCreateM
                        {
                            Name = unitName,
                            Address = address,
                            Activities = new List<ActivityM>()
                        }, DbContextExtensions.UserId);

                        Assert.IsType<LocalUnit>(
                            context.LocalUnits.Single(
                                x =>
                                    x.Name == unitName && x.Address.AddressPart1 == address.AddressPart1 && !x.IsDeleted));

                        var category = new ActivityCategory
                        {
                            Code = "01.13.1",
                            Name = "����������� �������� ������ � �� �����",
                            Section = "A"
                        };
                        context.ActivityCategories.Add(category);

                        await context.SaveChangesAsync();

                        try
                        {
                            await new StatUnitService(context).CreateLocalUnit(new LocalUnitCreateM
                            {
                                Name = unitName,
                                Address = address,
                                Activities = new List<ActivityM>()
                                {
                                    new ActivityM()
                                    {
                                        ActivityYear = 2017,
                                        Employees = 666,
                                        Turnover = 1000000,
                                        ActivityRevxCategory = new CodeLookupVm()
                                        {
                                            Code = category.Code,
                                            Id = category.Id
                                        },
                                        ActivityRevy = 2,
                                        ActivityType = ActivityTypes.Primary,
                                    },
                                    new ActivityM()
                                    {
                                        ActivityYear = 2017,
                                        Employees = 888,
                                        Turnover = 2000000,
                                        ActivityRevxCategory = new CodeLookupVm()
                                        {
                                            Code = category.Code,
                                            Id = category.Id
                                        },
                                        ActivityRevy = 3,
                                        ActivityType = ActivityTypes.Secondary,
                                    }
                                },
                            }, DbContextExtensions.UserId);

                            var activities = context.Activities.ToList();
                            Assert.Equal(2, activities.Count);
                        }
                        catch (Exception e)
                        {
                            actual = e.GetType();
                        }
                        Assert.Equal(expected, actual);
                        break;
                    case StatUnitTypes.EnterpriseUnit:
                        await new StatUnitService(context).CreateEnterpriseUnit(new EnterpriseUnitCreateM
                        {
                            Name = unitName,
                            Address = address,
                            Activities = new List<ActivityM>()
                        }, DbContextExtensions.UserId);

                        Assert.IsType<EnterpriseUnit>(
                            context.EnterpriseUnits.Single(
                                x =>
                                    x.Name == unitName && x.Address.AddressPart1 == address.AddressPart1 && !x.IsDeleted));
                        try
                        {
                            await new StatUnitService(context).CreateEnterpriseUnit(new EnterpriseUnitCreateM
                            {
                                Name = unitName,
                                Address = address,
                                Activities = new List<ActivityM>()
                            }, DbContextExtensions.UserId);
                        }
                        catch (Exception e)
                        {
                            actual = e.GetType();
                        }
                        Assert.Equal(expected, actual);
                        break;
                    case StatUnitTypes.EnterpriseGroup:
                        await new StatUnitService(context).CreateEnterpriseGroupUnit(new EnterpriseGroupCreateM
                        {
                            Name = unitName,
                            Address = address
                        }, DbContextExtensions.UserId);
                        Assert.IsType<EnterpriseGroup>(
                            context.EnterpriseGroups.Single(
                                x =>
                                    x.Name == unitName && x.Address.AddressPart1 == address.AddressPart1 && !x.IsDeleted));
                        try
                        {
                            await new StatUnitService(context).CreateEnterpriseGroupUnit(new EnterpriseGroupCreateM
                            {
                                Name = unitName,
                                Address = address
                            }, DbContextExtensions.UserId);
                        }
                        catch (Exception e)
                        {
                            actual = e.GetType();
                        }
                        Assert.Equal(expected, actual);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        #endregion

        #region EditTest

        [Fact]
        public async Task EditDataAccessAttributes()
        {
            AutoMapperConfiguration.Configure();
            using (var context = CreateContext())
            {
                context.Initialize();

                var user = context.Users.Single(v => v.Id == DbContextExtensions.UserId);
                user.DataAccessArray = user.DataAccessArray
                    .Where(v => !v.EndsWith(nameof(LegalUnit.ShortName))).ToArray();

                const string unitName = "Legal with Data Access Limits";
                const string unitShortName = "Default Value";

                var unit = new LegalUnit
                {
                    Name = unitName,
                    UserId = DbContextExtensions.UserId,
                    ShortName = unitShortName,
                };
                context.LegalUnits.Add(unit);
                await context.SaveChangesAsync();

                await new StatUnitService(context).EditLegalUnit(new LegalUnitEditM
                {
                    RegId = unit.RegId,
                    ShortName = "qwerty 666 / 228 / 322"
                }, DbContextExtensions.UserId);

                await context.SaveChangesAsync();

                var name = context.LegalUnits.Where(v => v.Name == unitName && v.ParrentId == null).Select(v => v.ShortName).Single();
                Assert.Equal(unitShortName, name);
            }


        }

        [Fact]
        public async Task EditActivities()
        {
            AutoMapperConfiguration.Configure();

            const string unitName = "Legal with activities";
            var activity1 = new Activity
            {
                ActivityYear = 2017,
                Employees = 666,
                Turnover = 1000000,
                ActivityRevxCategory = new ActivityCategory { Code = "01.12.0", Name = "����������� ����", Section = "A" },
                ActivityRevy = 2,
                ActivityType = ActivityTypes.Primary,
            };

            var activity2 = new Activity
            {
                ActivityYear = 2017,
                Employees = 888,
                Turnover = 2000000,
                ActivityRevxCategory = new ActivityCategory { Code = "01.13", Name = "����������� ������, ����, �����- � ������������", Section = "A" },
                ActivityRevy = 3,
                ActivityType = ActivityTypes.Secondary,
            };

            var activity3 = new Activity
            {
                ActivityYear = 2017,
                Employees = 999,
                Turnover = 3000000,
                ActivityRevxCategory = new ActivityCategory { Code = "01.13.1", Name = "����������� �������� ������ � �� �����", Section = "A" },
                ActivityRevy = 4,
                ActivityType = ActivityTypes.Ancilliary,
            };


            var activityCategory = new ActivityCategory
            {
                Code = "02.3",
                Name = "���� ������������ ����������� �������������",
                Section = "A"
            };

            using (var context = CreateContext())
            {
                context.Initialize();

                context.ActivityCategories.Add(activityCategory);
                context.LegalUnits.AddRange(new List<LegalUnit>
                {
                    new LegalUnit
                    {
                        Name = unitName,
                        UserId = DbContextExtensions.UserId,
                        ActivitiesUnits = new List<ActivityStatisticalUnit>
                        {
                            new ActivityStatisticalUnit
                            {
                                Activity = activity1
                            },
                            new ActivityStatisticalUnit
                            {
                                Activity = activity2
                            },
                            new ActivityStatisticalUnit
                            {
                                Activity = activity3
                            }
                        }
                    },
                });
                context.SaveChanges();

                var unitId = context.LegalUnits.Single(x => x.Name == unitName).RegId;
                const int changedEmployees = 9999;
                await new StatUnitService(context).EditLegalUnit(new LegalUnitEditM
                {
                    RegId = unitId,
                    Name = "new name test",
                    DataAccess = DbContextExtensions.DataAccessLegalUnit,
                    Activities = new List<ActivityM>()
                    {
                        new ActivityM //New
                        {
                            ActivityRevxCategory = new CodeLookupVm()
                            {
                                Id = activityCategory.Id,
                                Code = activityCategory.Code,
                            },
                            ActivityRevy = 1,
                            ActivityType = ActivityTypes.Primary,
                            Employees = 2,
                            Turnover = 10,
                            ActivityYear = 2016,
                            IdDate = new DateTime(2017, 03, 28),
                        },
                        new ActivityM //Not Changed
                        {
                            Id = activity1.Id,
                            ActivityRevxCategory = new CodeLookupVm()
                            {
                                Id = activity1.ActivityRevxCategory.Id,
                                Code = activity1.ActivityRevxCategory.Code
                            },
                            ActivityRevy = activity1.ActivityRevy,
                            ActivityType = activity1.ActivityType,
                            IdDate = activity1.IdDate,
                            Employees = activity1.Employees,
                            Turnover = activity1.Turnover,
                            ActivityYear = activity1.ActivityYear,
                        },
                        new ActivityM //Changed
                        {
                            Id = activity2.Id,
                             ActivityRevxCategory = new CodeLookupVm()
                            {
                                Id = activity2.ActivityRevxCategory.Id,
                                Code = activity2.ActivityRevxCategory.Code
                            },
                            ActivityRevy = activity2.ActivityRevy,
                            ActivityType = activity2.ActivityType,
                            IdDate = activity2.IdDate,
                            Employees = changedEmployees,
                            Turnover = activity2.Turnover,
                            ActivityYear = activity2.ActivityYear,
                        }
                    }
                }, DbContextExtensions.UserId);

                var unitResult = context.LegalUnits
                    .Include(v => v.ActivitiesUnits)
                    .ThenInclude(v => v.Activity)
                    .Single(v => v.RegId == unitId).Activities;

                var activities = unitResult as Activity[] ?? unitResult.ToArray();
                Assert.Equal(3, activities.Length);
                Assert.DoesNotContain(activities, v => v.Id == activity3.Id);
                Assert.Contains(activities, v => v.Id == activity1.Id);
                Assert.Contains(activities, v => v.Employees == changedEmployees);
                Assert.NotEqual(activity2.Id, activities.First(v => v.Employees == changedEmployees).Id);
            }
        }

        [Theory]
        [InlineData(StatUnitTypes.LegalUnit)]
        [InlineData(StatUnitTypes.LocalUnit)]
        [InlineData(StatUnitTypes.EnterpriseUnit)]
        [InlineData(StatUnitTypes.EnterpriseGroup)]
        public async Task EditTest(StatUnitTypes type)
        {
            AutoMapperConfiguration.Configure();
            var unitName = Guid.NewGuid().ToString();
            var unitNameEdit = Guid.NewGuid().ToString();
            var dublicateName = Guid.NewGuid().ToString();
            var addressPartOne = Guid.NewGuid().ToString();

            int unitId;
            var expected = typeof(BadRequestException);
            Type actual = null;

            using (var context = CreateContext())
            {
                context.Initialize();
                switch (type)
                {
                    case StatUnitTypes.LegalUnit:
                        context.LegalUnits.AddRange(new List<LegalUnit>
                        {
                            new LegalUnit
                            {
                                Name = unitName,
                                UserId = DbContextExtensions.UserId
                            },
                            new LegalUnit
                            {
                                Name = dublicateName,
                                UserId = DbContextExtensions.UserId,
                                Address = new Address {AddressPart1 = addressPartOne},
                            },
                        });
                        context.SaveChanges();

                        unitId = context.LegalUnits.Single(x => x.Name == unitName).RegId;

                        await new StatUnitService(context).EditLegalUnit(new LegalUnitEditM
                        {
                            RegId = unitId,
                            Name = unitNameEdit,
                            DataAccess = DbContextExtensions.DataAccessLegalUnit,
                        }, DbContextExtensions.UserId);
                        Assert.IsType<LegalUnit>(
                            context.LegalUnits.Single(x => x.RegId == unitId && x.Name == unitNameEdit && !x.IsDeleted));
                        Assert.IsType<LegalUnit>(
                            context.LegalUnits.Single(
                                x => x.RegId != unitId && x.ParrentId == unitId && x.Name == unitName));

                        try
                        {
                            await new StatUnitService(context).EditLegalUnit(new LegalUnitEditM
                            {
                                RegId = unitId,
                                Name = dublicateName,
                                Address = new AddressM {AddressPart1 = addressPartOne},
                                DataAccess = DbContextExtensions.DataAccessLegalUnit
                            }, DbContextExtensions.UserId);
                        }
                        catch (Exception e)
                        {
                            actual = e.GetType();
                        }
                        Assert.Equal(expected, actual);
                        break;
                    case StatUnitTypes.LocalUnit:
                        context.LocalUnits.AddRange(new List<LocalUnit>
                        {
                            new LocalUnit {Name = unitName, UserId = DbContextExtensions.UserId},
                            new LocalUnit
                            {
                                Name = dublicateName,
                                Address = new Address {AddressPart1 = addressPartOne},
                                UserId = DbContextExtensions.UserId
                            }
                        });
                        context.SaveChanges();

                        unitId = context.LocalUnits.Single(x => x.Name == unitName).RegId;
                        await new StatUnitService(context).EditLocalUnit(new LocalUnitEditM
                        {
                            RegId = unitId,
                            Name = unitNameEdit,
                            Activities = new List<ActivityM>(),
                            DataAccess = DbContextExtensions.DataAccessLocalUnit,
                        }, DbContextExtensions.UserId);
                        Assert.IsType<LocalUnit>(
                            context.LocalUnits.Single(x => x.RegId == unitId && x.Name == unitNameEdit && !x.IsDeleted));
                        Assert.IsType<LocalUnit>(
                            context.LocalUnits.Single(
                                x => x.RegId != unitId && x.ParrentId == unitId && x.Name == unitName));
                        try
                        {
                            await new StatUnitService(context).EditLocalUnit(new LocalUnitEditM
                            {
                                RegId = unitId,
                                Name = dublicateName,
                                Address = new AddressM {AddressPart1 = addressPartOne},
                                DataAccess = DbContextExtensions.DataAccessLocalUnit
                            }, DbContextExtensions.UserId);
                        }
                        catch (Exception e)
                        {
                            actual = e.GetType();
                        }
                        Assert.Equal(expected, actual);
                        break;
                    case StatUnitTypes.EnterpriseUnit:
                        context.EnterpriseUnits.AddRange(new List<EnterpriseUnit>
                        {
                            new EnterpriseUnit {Name = unitName, UserId = DbContextExtensions.UserId},
                            new EnterpriseUnit
                            {
                                Name = dublicateName,
                                Address = new Address {AddressPart1 = addressPartOne},
                                UserId = DbContextExtensions.UserId
                            }
                        });
                        context.SaveChanges();

                        unitId = context.EnterpriseUnits.Single(x => x.Name == unitName).RegId;
                        await new StatUnitService(context).EditEnterpiseUnit(new EnterpriseUnitEditM
                        {
                            RegId = unitId,
                            Name = unitNameEdit,
                            Activities = new List<ActivityM>(),
                            DataAccess = DbContextExtensions.DataAccessEnterpriseUnit
                        }, DbContextExtensions.UserId);
                        Assert.IsType<EnterpriseUnit>(
                            context.EnterpriseUnits.Single(
                                x => x.RegId == unitId && x.Name == unitNameEdit && !x.IsDeleted));
                        Assert.IsType<EnterpriseUnit>(
                            context.EnterpriseUnits.Single(
                                x => x.RegId != unitId && x.ParrentId == unitId && x.Name == unitName));
                        try
                        {
                            await new StatUnitService(context).EditEnterpiseUnit(new EnterpriseUnitEditM
                            {
                                RegId = unitId,
                                Name = dublicateName,
                                Address = new AddressM {AddressPart1 = addressPartOne},
                                Activities = new List<ActivityM>(),
                                DataAccess = DbContextExtensions.DataAccessEnterpriseUnit,
                            }, DbContextExtensions.UserId);
                        }
                        catch (Exception e)
                        {
                            actual = e.GetType();
                        }
                        Assert.Equal(expected, actual);
                        break;
                    case StatUnitTypes.EnterpriseGroup:
                        context.EnterpriseGroups.AddRange(new List<EnterpriseGroup>
                        {
                            new EnterpriseGroup {Name = unitName, UserId = DbContextExtensions.UserId},
                            new EnterpriseGroup
                            {
                                Name = dublicateName,
                                UserId = DbContextExtensions.UserId,
                                Address = new Address {AddressPart1 = addressPartOne},
                                EnterpriseUnits = new List<EnterpriseUnit>
                                {
                                    new EnterpriseUnit {Name = unitName},
                                },
                            }
                        });
                        context.SaveChanges();

                        unitId = context.EnterpriseGroups.Single(x => x.Name == unitName).RegId;
                        await new StatUnitService(context).EditEnterpiseGroup(new EnterpriseGroupEditM
                        {
                            RegId = unitId,
                            Name = unitNameEdit,
                            EnterpriseUnits = new[]
                            {
                                context.EnterpriseGroups
                                    .Where(x => x.Name == dublicateName)
                                    .Select(x => x.EnterpriseUnits).FirstOrDefault()
                                    .Where(x => x.Name == unitName)
                                    .Select(x => x.RegId).FirstOrDefault()
                            },
                            DataAccess = DbContextExtensions.DataAccessEnterpriseGroup,
                        }, DbContextExtensions.UserId);
                        Assert.IsType<EnterpriseGroup>(
                            context.EnterpriseGroups.Single(
                                x => x.RegId == unitId && x.Name == unitNameEdit && !x.IsDeleted));
                        Assert.IsType<EnterpriseGroup>(
                            context.EnterpriseGroups.Single(
                                x => x.RegId != unitId && x.ParrentId == unitId && x.Name == unitName));
                        try
                        {
                            await new StatUnitService(context).EditEnterpiseGroup(new EnterpriseGroupEditM
                            {
                                RegId = unitId,
                                Name = dublicateName,
                                Address = new AddressM {AddressPart1 = addressPartOne},
                                DataAccess = DbContextExtensions.DataAccessEnterpriseGroup,
                            }, DbContextExtensions.UserId);
                        }
                        catch (Exception e)
                        {
                            actual = e.GetType();
                        }
                        Assert.Equal(expected, actual);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        #endregion

        #region DeleteTest

        [Theory]
        [InlineData(StatUnitTypes.LegalUnit)]
        [InlineData(StatUnitTypes.LocalUnit)]
        [InlineData(StatUnitTypes.EnterpriseUnit)]
        [InlineData(StatUnitTypes.EnterpriseGroup)]
        public void DeleteTest(StatUnitTypes type)
        {
            AutoMapperConfiguration.Configure();
            var unitName = Guid.NewGuid().ToString();
            using (var context = CreateContext())
            {
                int unitId;
                switch (type)
                {
                    case StatUnitTypes.LegalUnit:
                        context.LegalUnits.Add(new LegalUnit {Name = unitName, IsDeleted = false, UserId = DbContextExtensions.UserId});
                        context.SaveChanges();
                        unitId = context.LegalUnits.Single(x => x.Name == unitName && !x.IsDeleted).RegId;
                        new StatUnitService(context).DeleteUndelete(type, unitId, true, DbContextExtensions.UserId);
                        Assert.IsType<LegalUnit>(context.LegalUnits.Single(x => x.Name == unitName && x.IsDeleted));
                        Assert.IsType<LegalUnit>(
                            context.LegalUnits.Single(x => x.Name == unitName && !x.IsDeleted && x.ParrentId == unitId));
                        break;
                    case StatUnitTypes.LocalUnit:
                        context.LocalUnits.Add(new LocalUnit {Name = unitName, IsDeleted = false, UserId = DbContextExtensions.UserId });
                        context.SaveChanges();
                        unitId = context.LocalUnits.Single(x => x.Name == unitName && !x.IsDeleted).RegId;
                        new StatUnitService(context).DeleteUndelete(type, unitId, true, DbContextExtensions.UserId);
                        Assert.IsType<LocalUnit>(context.LocalUnits.Single(x => x.Name == unitName && x.IsDeleted));
                        Assert.IsType<LocalUnit>(
                            context.LocalUnits.Single(x => x.Name == unitName && !x.IsDeleted && x.ParrentId == unitId));
                        break;
                    case StatUnitTypes.EnterpriseUnit:
                        context.EnterpriseUnits.Add(new EnterpriseUnit {Name = unitName, IsDeleted = false, UserId = DbContextExtensions.UserId });
                        context.SaveChanges();
                        unitId = context.EnterpriseUnits.Single(x => x.Name == unitName && !x.IsDeleted).RegId;
                        new StatUnitService(context).DeleteUndelete(type, unitId, true, DbContextExtensions.UserId);
                        Assert.IsType<EnterpriseUnit>(
                            context.EnterpriseUnits.Single(x => x.Name == unitName && x.IsDeleted));
                        Assert.IsType<EnterpriseUnit>(
                            context.EnterpriseUnits.Single(
                                x => x.Name == unitName && !x.IsDeleted && x.ParrentId == unitId));
                        break;
                    case StatUnitTypes.EnterpriseGroup:
                        context.EnterpriseGroups.Add(new EnterpriseGroup {Name = unitName, IsDeleted = false, UserId = DbContextExtensions.UserId });
                        context.SaveChanges();
                        unitId = context.EnterpriseGroups.Single(x => x.Name == unitName && !x.IsDeleted).RegId;
                        new StatUnitService(context).DeleteUndelete(type, unitId, true, DbContextExtensions.UserId);
                        Assert.IsType<EnterpriseGroup>(
                            context.EnterpriseGroups.Single(x => x.Name == unitName && x.IsDeleted));
                        Assert.IsType<EnterpriseGroup>(
                            context.EnterpriseGroups.Single(
                                x => x.Name == unitName && !x.IsDeleted && x.ParrentId == unitId));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        #endregion

        #region UndeleteTest

        [Theory]
        [InlineData(StatUnitTypes.LegalUnit)]
        [InlineData(StatUnitTypes.LocalUnit)]
        [InlineData(StatUnitTypes.EnterpriseUnit)]
        [InlineData(StatUnitTypes.EnterpriseGroup)]
        public void UndeleteTest(StatUnitTypes type)
        {
            AutoMapperConfiguration.Configure();
            var unitName = Guid.NewGuid().ToString();
            using (var context = CreateContext())
            {
                int unitId;
                switch (type)
                {
                    case StatUnitTypes.LegalUnit:
                        context.LegalUnits.Add(new LegalUnit {Name = unitName, IsDeleted = true, UserId = DbContextExtensions.UserId });
                        context.SaveChanges();
                        unitId = context.LegalUnits.Single(x => x.Name == unitName && x.IsDeleted).RegId;
                        new StatUnitService(context).DeleteUndelete(type, unitId, false, DbContextExtensions.UserId);
                        Assert.IsType<LegalUnit>(context.LegalUnits.Single(x => x.Name == unitName && !x.IsDeleted));
                        Assert.IsType<LegalUnit>(
                            context.LegalUnits.Single(x => x.Name == unitName && x.IsDeleted && x.ParrentId == unitId));
                        break;
                    case StatUnitTypes.LocalUnit:
                        context.LocalUnits.Add(new LocalUnit {Name = unitName, IsDeleted = true, UserId = DbContextExtensions.UserId });
                        context.SaveChanges();
                        unitId = context.LocalUnits.Single(x => x.Name == unitName && x.IsDeleted).RegId;
                        new StatUnitService(context).DeleteUndelete(type, unitId, false, DbContextExtensions.UserId);
                        Assert.IsType<LocalUnit>(context.LocalUnits.Single(x => x.Name == unitName && !x.IsDeleted));
                        Assert.IsType<LocalUnit>(
                            context.LocalUnits.Single(x => x.Name == unitName && x.IsDeleted && x.ParrentId == unitId));
                        break;
                    case StatUnitTypes.EnterpriseUnit:
                        context.EnterpriseUnits.Add(new EnterpriseUnit {Name = unitName, IsDeleted = true, UserId = DbContextExtensions.UserId });
                        context.SaveChanges();
                        unitId = context.EnterpriseUnits.Single(x => x.Name == unitName && x.IsDeleted).RegId;
                        new StatUnitService(context).DeleteUndelete(type, unitId, false, DbContextExtensions.UserId);
                        Assert.IsType<EnterpriseUnit>(
                            context.EnterpriseUnits.Single(x => x.Name == unitName && !x.IsDeleted));
                        Assert.IsType<EnterpriseUnit>(
                            context.EnterpriseUnits.Single(
                                x => x.Name == unitName && x.IsDeleted && x.ParrentId == unitId));
                        break;
                    case StatUnitTypes.EnterpriseGroup:
                        context.EnterpriseGroups.Add(new EnterpriseGroup {Name = unitName, IsDeleted = true, UserId = DbContextExtensions.UserId });
                        context.SaveChanges();
                        unitId = context.EnterpriseGroups.Single(x => x.Name == unitName && x.IsDeleted).RegId;
                        new StatUnitService(context).DeleteUndelete(type, unitId, false, DbContextExtensions.UserId);
                        Assert.IsType<EnterpriseGroup>(
                            context.EnterpriseGroups.Single(x => x.Name == unitName && !x.IsDeleted));
                        Assert.IsType<EnterpriseGroup>(
                            context.EnterpriseGroups.Single(
                                x => x.Name == unitName && x.IsDeleted && x.ParrentId == unitId));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }

        #endregion
    }
}
