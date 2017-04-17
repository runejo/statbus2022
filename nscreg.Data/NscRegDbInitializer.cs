using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using nscreg.Data.Constants;
using nscreg.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using nscreg.Data.Configuration;
using nscreg.Utilities.Attributes;

namespace nscreg.Data
{
    public static class NscRegDbInitializer
    {
        public static void RecreateDb(NSCRegDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        public static void Seed(NSCRegDbContext context)
        {
            var sysAdminRole = context.Roles.FirstOrDefault(r => r.Name == DefaultRoleNames.SystemAdministrator);
            var daa = typeof(EnterpriseGroup).GetProperties()
                .Where(v => v.GetCustomAttribute<NotMappedForAttribute>() == null)
                .Select(x => $"{nameof(EnterpriseGroup)}.{x.Name}")
                .Union(typeof(EnterpriseUnit).GetProperties()
                    .Where(v => v.GetCustomAttribute<NotMappedForAttribute>() == null)
                    .Select(x => $"{nameof(EnterpriseUnit)}.{x.Name}"))
                .Union(typeof(LegalUnit).GetProperties()
                    .Where(v => v.GetCustomAttribute<NotMappedForAttribute>() == null)
                    .Select(x => $"{nameof(LegalUnit)}.{x.Name}"))
                .Union(typeof(LocalUnit).GetProperties()
                    .Where(v => v.GetCustomAttribute<NotMappedForAttribute>() == null)
                    .Select(x => $"{nameof(LocalUnit)}.{x.Name}"))
                .ToArray();
            if (sysAdminRole == null)
            {
                sysAdminRole = new Role
                {
                    Name = DefaultRoleNames.SystemAdministrator,
                    Status = RoleStatuses.Active,
                    Description = "System administrator role",
                    NormalizedName = DefaultRoleNames.SystemAdministrator.ToUpper(),
                    AccessToSystemFunctionsArray =
                        ((SystemFunctions[]) Enum.GetValues(typeof(SystemFunctions))).Select(x => (int) x),
                    StandardDataAccessArray = daa,
                };
                context.Roles.Add(sysAdminRole);
            }
            var anyAdminHere = context.UserRoles.Any(ur => ur.RoleId == sysAdminRole.Id);
            if (anyAdminHere) return;

            if (!context.ActivityCategories.Any())
            {
                context.ActivityCategories.AddRange(
                    new ActivityCategory
                    {
                        Code = "A",
                        Name = "Сельское хозяйство, лесное хозяйство и рыболовство",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01",
                        Name = "Сельское хозяйство, охота и предоставление услуг в этих областях",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.1", Name = "Выращивание немноголетних культур", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.11",
                        Name = "Выращивание зерновых (кроме риса), бобовых и масличных культур",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.11.1",
                        Name = "Выращивание зерновых культур (кроме риса и гречихи)",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.11.2", Name = "Выращивание бобовых культур", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.11.9",
                        Name = "Выращивание масличных культур и их семян",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.12", Name = "Выращивание риса", Section = "A"},
                    new ActivityCategory {Code = "01.12.0", Name = "Выращивание риса", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.13",
                        Name = "Выращивание овощей, дынь, корне- и клубнеплодов",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.13.1",
                        Name = "Выращивание сахарной свеклы и ее семян",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.13.2",
                        Name = "Выращивание корне- и клубнеплодов с высоким содержанием крахмала и инулина и их семян",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.13.3", Name = "Выращивание грибов и трюфелей", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.13.9",
                        Name = "Выращивание прочих овощных, бахчевых, корне- и клубнеплодных культур и их семян",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.14", Name = "Выращивание сахарного тростника", Section = "A"},
                    new ActivityCategory {Code = "01.14.0", Name = "Выращивание сахарного тростника", Section = "A"},
                    new ActivityCategory {Code = "01.15", Name = "Выращивание табака", Section = "A"},
                    new ActivityCategory {Code = "01.15.0", Name = "Выращивание табака", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.16",
                        Name = "Выращивание прядильных (лубяных) культур",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.16.1", Name = "Выращивание хлопчатника", Section = "A"},
                    new ActivityCategory {Code = "01.16.2", Name = "Выращивание льна", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.16.9",
                        Name = "Выращивание прочих прядильных (лубяных) культур",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.19",
                        Name = "Выращивание прочих немноголетних культур",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.19.1",
                        Name = "Выращивание кормовых культур и их семян",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.19.2", Name = "Выращивание цветов и их семян", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.19.9",
                        Name = "Выращивание прочих немноголетних культур, не включенных в другие группировки",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.2", Name = "Выращивание многолетних культур", Section = "A"},
                    new ActivityCategory {Code = "01.21", Name = "Выращивание винограда", Section = "A"},
                    new ActivityCategory {Code = "01.21.0", Name = "Выращивание винограда", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.22",
                        Name = "Выращивание тропических и субтропических плодов",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.22.0",
                        Name = "Выращивание тропических и субтропических плодов",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.23", Name = "Выращивание цитрусовых плодов", Section = "A"},
                    new ActivityCategory {Code = "01.23.0", Name = "Выращивание цитрусовых плодов", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.24",
                        Name = "Выращивание косточковых и семечковых плодов",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.24.1", Name = "Выращивание яблок", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.24.9",
                        Name = "Выращивание прочих косточковых и семечковых плодов",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.25",
                        Name = "Выращивание прочих плодов, ягод и орехов",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.25.1",
                        Name = "Выращивание ягод и плодов прочих фруктовых деревьев",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.25.9",
                        Name =
                            "Выращивание орехов (кроме диких, или несъедобных, орехов, земляного ореха и кокосовых орехов)",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.26", Name = "Выращивание маслосодержащих плодов", Section = "A"},
                    new ActivityCategory {Code = "01.26.0", Name = "Выращивание маслосодержащих плодов", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.27",
                        Name = "Выращивание культур для производства напитков",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.27.0",
                        Name = "Выращивание культур для производства напитков",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.28",
                        Name = "Выращивание специй (пряностей), лекарственных и используемых в парфюмерии растений",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.28.1",
                        Name = "Выращивание пряных культур (специй)",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.28.2", Name = "Выращивание шишек хмеля", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.28.9",
                        Name =
                            "Выращивание прочих, растений используемых в парфюмерии, фармацевтике или в качестве инсектицидов, фунгицидов или для аналогичных целей",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.29",
                        Name = "Выращивание прочих многолетних растений",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.29.1", Name = "Выращивание новогодних елок", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.29.9",
                        Name = "Выращивание прочих многолетних культур, не включенных в другие группировки",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.3", Name = "Воспроизводство (посадка) растений", Section = "A"},
                    new ActivityCategory {Code = "01.30", Name = "Воспроизводство (посадка) растений", Section = "A"},
                    new ActivityCategory {Code = "01.30.0", Name = "Воспроизводство (посадка) растений", Section = "A"},
                    new ActivityCategory {Code = "01.4", Name = "Животноводство", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.41",
                        Name = "Разведение крупного рогатого скота молочного направления",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.41.0",
                        Name = "Разведение крупного рогатого скота молочного направления",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.42",
                        Name = "Разведение прочего крупного рогатого скота",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.42.0",
                        Name = "Разведение прочего крупного рогатого скота",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.43",
                        Name = "Разведение лошадей, ослов, мулов и лошаков",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.43.0",
                        Name = "Разведение лошадей, ослов, мулов и лошаков",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.44", Name = "Разведение верблюдов и верблюдиц", Section = "A"},
                    new ActivityCategory {Code = "01.44.0", Name = "Разведение верблюдов и верблюдиц", Section = "A"},
                    new ActivityCategory {Code = "01.45", Name = "Разведение овец и коз", Section = "A"},
                    new ActivityCategory {Code = "01.45.0", Name = "Разведение овец и коз", Section = "A"},
                    new ActivityCategory {Code = "01.46", Name = "Разведение свиней", Section = "A"},
                    new ActivityCategory {Code = "01.46.0", Name = "Разведение свиней", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.47",
                        Name = "Разведение сельскохозяйственной птицы",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.47.0",
                        Name = "Разведение сельскохозяйственной птицы",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.49", Name = "Разведение прочих животных", Section = "A"},
                    new ActivityCategory {Code = "01.49.0", Name = "Разведение прочих животных", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.5",
                        Name = "Растениеводство в сочетании с животноводством",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.50",
                        Name = "Растениеводство в сочетании с животноводством",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.50.0",
                        Name = "Растениеводство в сочетании с животноводством",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.6",
                        Name = "Предоставление услуг в области сельского хозяйства",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.61",
                        Name = "Предоставление услуг в области растениеводства",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "01.61.1", Name = "Эксплуатация  ирригационных систем", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "01.61.9",
                        Name = "Предоставление прочих  услуг в области растениеводства",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.62",
                        Name = "Предоставление услуг в области животноводства",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.62.0",
                        Name = "Предоставление услуг в области животноводства",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.63",
                        Name = "Предоставление услуг по обработке урожая сельскохозяйственных культур",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.63.0",
                        Name = "Предоставление услуг по обработке урожая сельскохозяйственных культур",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.64",
                        Name = "Предоставление услуг по подготовке семян к посадке (формирование семенного фонда)",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.64.0",
                        Name = "Предоставление услуг по подготовке семян к посадке (формирование семенного фонда)",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.7",
                        Name = "Охота, разведение диких животных и предоставление услуг в этих областях",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.70",
                        Name = "Охота, разведение диких животных и предоставление услуг в этих областях",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "01.70.0",
                        Name = "Охота, разведение диких животных и предоставление услуг в этих областях",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02",
                        Name = "Лесное хозяйство и предоставление услуг в этой области",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02.1",
                        Name = "Лесоводство и прочая деятельность в области лесоразведения и лесовосстановления",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02.10",
                        Name = "Лесоводство и прочая деятельность в области лесоразведения и лесовосстановления",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02.10.0",
                        Name = "Лесоводство и прочая деятельность в области лесоразведения и лесовосстановления",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "02.2", Name = "Лесозаготовки", Section = "A"},
                    new ActivityCategory {Code = "02.20", Name = "Лесозаготовки", Section = "A"},
                    new ActivityCategory {Code = "02.20.0", Name = "Лесозаготовки", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "02.3",
                        Name = "Сбор дикорастущих недревесных лесопродуктов",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02.30",
                        Name = "Сбор дикорастущих недревесных лесопродуктов",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02.30.1",
                        Name =
                            "Сбор дикорастущих лесных грибов и трюфелей, плодов, ягод, орехов и прочих съедобных лесопродуктов",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02.30.9",
                        Name = "Сбор прочих дикорастущих не древесных лесопродуктов (кроме съедобных)",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02.4",
                        Name = "Предоставление услуг в области лесного хозяйства",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02.40",
                        Name = "Предоставление услуг в области лесного хозяйства (лесоразведения и лесозаготовок)",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "02.40.0",
                        Name = "Предоставление услуг в области лесного хозяйства (лесоразведения и лесозаготовок)",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "03", Name = "Рыболовство и рыбоводство", Section = "A"},
                    new ActivityCategory {Code = "03.1", Name = "Рыболовство", Section = "A"},
                    new ActivityCategory
                    {
                        Code = "03.12",
                        Name = "Рыболовство в реках, озерах, водохранилищах (Рыболовство пресноводное)",
                        Section = "A"
                    },
                    new ActivityCategory
                    {
                        Code = "03.12.0",
                        Name = "Рыболовство в реках, озерах, водохранилищах (Рыболовство пресноводное)",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "03.2", Name = "Рыбоводство", Section = "A"},
                    new ActivityCategory {Code = "03.22", Name = "Рыбоводство пресноводное", Section = "A"},
                    new ActivityCategory {Code = "03.22.0", Name = "Рыбоводство пресноводное", Section = "A"},
                    new ActivityCategory {Code = "B", Name = "Добыча полезных ископаемых", Section = "B"},
                    new ActivityCategory
                    {
                        Code = "05",
                        Name = "Добыча каменного угля   и бурого угля (лигнита )",
                        Section = "B"
                    },
                    new ActivityCategory {Code = "05.1", Name = "Добыча каменного угля", Section = "B"},
                    new ActivityCategory {Code = "05.10", Name = "Добыча каменного угля", Section = "B"},
                    new ActivityCategory {Code = "05.10.0", Name = "Добыча каменного угля", Section = "B"},
                    new ActivityCategory {Code = "05.2", Name = "Добыча бурого угля (лигнита)", Section = "B"},
                    new ActivityCategory {Code = "05.20", Name = "Добыча бурого угля (лигнита)", Section = "B"},
                    new ActivityCategory {Code = "05.20.0", Name = "Добыча бурого угля (лигнита)", Section = "B"},
                    new ActivityCategory {Code = "06", Name = "Добыча сырой нефти и природного газа", Section = "B"},
                    new ActivityCategory {Code = "06.1", Name = "Добыча сырой нефти", Section = "B"},
                    new ActivityCategory {Code = "06.10", Name = "Добыча сырой нефти", Section = "B"},
                    new ActivityCategory {Code = "06.10.0", Name = "Добыча сырой нефти", Section = "B"},
                    new ActivityCategory {Code = "06.2", Name = "Добыча природного (горючего) газа", Section = "B"},
                    new ActivityCategory {Code = "06.20", Name = "Добыча природного (горючего) газа", Section = "B"},
                    new ActivityCategory {Code = "06.20.0", Name = "Добыча природного (горючего) газа", Section = "B"},
                    new ActivityCategory {Code = "07", Name = "Добыча металлических руд", Section = "B"},
                    new ActivityCategory {Code = "07.1", Name = "Добыча железных руд", Section = "B"},
                    new ActivityCategory {Code = "07.10", Name = "Добыча железных руд", Section = "B"},
                    new ActivityCategory {Code = "07.10.0", Name = "Добыча железных руд", Section = "B"},
                    new ActivityCategory {Code = "07.2", Name = "Добыча руд цветных металлов", Section = "B"},
                    new ActivityCategory {Code = "07.21", Name = "Добыча урановой и ториевой руд", Section = "B"},
                    new ActivityCategory {Code = "07.21.0", Name = "Добыча урановой и ториевой руд", Section = "B"},
                    new ActivityCategory {Code = "07.29", Name = "Добыча руд прочих цветных металлов", Section = "B"},
                    new ActivityCategory {Code = "07.29.1", Name = "Добыча и обогащение медной руды", Section = "B"},
                    new ActivityCategory {Code = "07.29.2", Name = "Добыча и обогащение никелевых руд", Section = "B"},
                    new ActivityCategory
                    {
                        Code = "07.29.3",
                        Name = "Добыча и обогащение алюминий - содержащего сырья",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "07.29.4",
                        Name = "Добыча и обогащение руд драгоценных (благородных) металлов",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "07.29.5",
                        Name = "Добыча и обогащение свинцовых, цинковых и оловянных руд",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "07.29.9",
                        Name = "Добыча и обогащение прочих руд цветных металлов, не включенных в другие группировки",
                        Section = "B"
                    },
                    new ActivityCategory {Code = "08", Name = "Добыча прочих полезных ископаемых", Section = "B"},
                    new ActivityCategory
                    {
                        Code = "08.1",
                        Name = "Добыча камня для строительства, песков, глины",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "08.11",
                        Name = "Добыча отделочного и строительного камня, известняка, гипса, мела и сланцев",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "08.11.1",
                        Name = "Добыча отделочного и строительного камня",
                        Section = "B"
                    },
                    new ActivityCategory {Code = "08.11.2", Name = "Добыча известняка и гипса", Section = "B"},
                    new ActivityCategory
                    {
                        Code = "08.11.3",
                        Name = "Добыча мела и некальционированного доломита",
                        Section = "B"
                    },
                    new ActivityCategory {Code = "08.11.9", Name = "Добыча сланцев", Section = "B"},
                    new ActivityCategory
                    {
                        Code = "08.12",
                        Name = "Разработка гравийных и песчаных карьеров, добыча глины и каолина",
                        Section = "B"
                    },
                    new ActivityCategory {Code = "08.12.1", Name = "Добыча гравия и песка", Section = "B"},
                    new ActivityCategory {Code = "08.12.9", Name = "Добыча глины и каолина", Section = "B"},
                    new ActivityCategory
                    {
                        Code = "08.9",
                        Name = "Добыча полезных ископаемых, не включенных в другие группировки",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "08.91",
                        Name = "Добыча минерального сырья для химической промышленности  и производства удобрений",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "08.91.1",
                        Name = "Добыча природных фосфатов калийных солей",
                        Section = "B"
                    },
                    new ActivityCategory {Code = "08.91.2", Name = "Добыча природной серы", Section = "B"},
                    new ActivityCategory
                    {
                        Code = "08.91.9",
                        Name =
                            "Добыча прочего минерального сырья для химической промышленности  и производства удобрений",
                        Section = "B"
                    },
                    new ActivityCategory {Code = "08.92", Name = "Добыча торфа", Section = "B"},
                    new ActivityCategory {Code = "08.92.0", Name = "Добыча торфа", Section = "B"},
                    new ActivityCategory {Code = "08.93", Name = "Добыча соли", Section = "B"},
                    new ActivityCategory {Code = "08.93.0", Name = "Добыча соли", Section = "B"},
                    new ActivityCategory
                    {
                        Code = "08.99",
                        Name = "Добыча прочих полезных ископаемых, не включенных в другие группировки",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "08.99.1",
                        Name = "Добыча природного асфальта и битума",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "08.99.2",
                        Name = "Добыча природных драгоценных и полудрагоценных камней (кроме алмазов)",
                        Section = "B"
                    },
                    new ActivityCategory {Code = "08.99.3", Name = "Добыча природных алмазов", Section = "B"},
                    new ActivityCategory
                    {
                        Code = "08.99.4",
                        Name = "Добыча пемзы и природных абразивных материалов",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "08.99.9",
                        Name = "Добыча прочих природных минералов и горных пород",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09",
                        Name = "Предоставление услуг по добыче полезных ископаемых",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09.1",
                        Name = "Предоставление услуг по добыче нефти и природного газа",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09.10",
                        Name = "Предоставление услуг по добыче нефти и природного газа",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09.10.1",
                        Name = "Бурение скважин для добычи нефти и природного газа",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09.10.2",
                        Name = "Сжижение и повторная газификация для транспортирования  природного газа",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09.10.9",
                        Name = "Предоставление прочих услуг по добыче нефти и природного газа",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09.9",
                        Name = "Предоставление услуг по добыче прочих полезных ископаемых",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09.90",
                        Name = "Предоставление услуг по добыче прочих полезных ископаемых",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09.90.1",
                        Name = "Предоставление услуг по добыче и обогащению каменного угля",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "09.90.9",
                        Name = "Предоставление услуг по добыче прочего минерального сырья и строительного камня",
                        Section = "B"
                    },
                    new ActivityCategory
                    {
                        Code = "C",
                        Name = "Обрабатывающие производства (обрабатывающая промышленность)",
                        Section = "C"
                    },
                    new ActivityCategory
                    {
                        Code = "CA",
                        Name = "Производство пищевых продуктов (включая напитки) и табачных изделий",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "10", Name = "Производство пищевых продуктов", Section = "CA"},
                    new ActivityCategory {Code = "10.1", Name = "Производство мясных продуктов", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "10.11",
                        Name = "Производство (переработка и сохранение) мяса, кроме мяса птицы",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.11.1",
                        Name = "Производство свежего, охлажденного и замороженного мяса и пищевых субпродуктов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.11.9",
                        Name = "Производство прочих продуктов убоя животных, включая диких",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.12",
                        Name = "Производство (переработка и сохранение) мяса сельскохозяйственной птицы",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.12.1",
                        Name = "Производство свежего, охлажденного и замороженного мяса птицы  и пищевых субпродуктов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.12.9",
                        Name = "Производство прочих продуктов убоя птицы",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.13",
                        Name = "Производство продуктов (изделий) из мяса и мяса птицы",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.13.1",
                        Name =
                            "Производство соленых, в рассоле, сушеных или копченых мяса, мяса птицы и пищевых субпродуктов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.13.9",
                        Name =
                            "Производство колбасных и прочих изделий из мяса, мясных субпродуктов или крови животных",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.2",
                        Name = "Переработка и консервирование рыбы, ракообразных и моллюсков",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.20",
                        Name = "Переработка и консервирование рыбы, ракообразных и моллюсков",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.20.1",
                        Name = "Охлаждение, замораживание рыбы, ракообразных моллюсков или сохранение их в свежем виде",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.20.9",
                        Name =
                            "Переработка рыбы, ракообразных и моллюсков прочими способами и производство из них пищевых и непищевых продуктов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.3",
                        Name = "Переработка и консервирование фруктов и овощей",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.31",
                        Name = "Переработка и консервирование картофеля",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.31.0",
                        Name = "Переработка и консервирование картофеля",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.32",
                        Name = "Производство фруктовых и овощных соков",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.32.0",
                        Name = "Производство фруктовых и овощных соков",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.39",
                        Name = "Прочие способы переработки и консервирования фруктов и овощей",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.39.0",
                        Name = "Прочие способы переработки и консервирования фруктов и овощей",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.4",
                        Name = "Производство растительных и животных масел и жиров",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.41",
                        Name = "Производство растительных и животных масел и жиров",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.41.1",
                        Name = "Производство нерафинированных (неочищенных) растительных   и животных масел и жиров",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.41.9",
                        Name = "Производство рафинированных (чищенных) растительных и животных масел и жиров",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.42",
                        Name = "Производство маргарина и смешанных пищевых жиров",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.42.0",
                        Name = "Производство маргарина и смешанных пищевых жиров",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "10.5", Name = "Производство молочных продуктов", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "10.51",
                        Name = "Переработка молока и производство сыров (сыроварение)",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.51.1",
                        Name = "Производство жидкого молока и сливок",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.51.9",
                        Name = "Производство сыров и прочих молочных и кисломолочных продуктов",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "10.52", Name = "Производство мороженого", Section = "CA"},
                    new ActivityCategory {Code = "10.52.0", Name = "Производство мороженого", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "10.6",
                        Name = "Производство муки и круп, крахмалов и крахмалопродуктов",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "10.61", Name = "Производство муки и круп", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "10.61.1",
                        Name = "Обработка риса и производство рисовой муки",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.61.2",
                        Name = "Производство муки из зерновых (кроме риса), овощных культур и орехов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.61.9",
                        Name = "Производство круп, гранул и хлопьев для завтрака и прочих аналогичных продуктов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.62",
                        Name = "Производство крахмалов и крахмалопродуктов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.62.0",
                        Name = "Производство крахмалов и крахмалопродуктов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.7",
                        Name = "Производство хлебобулочных изделий и выпечки",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.71",
                        Name = "Производство хлеба и мучных кондитерских изделий недлительного хранения",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.71.0",
                        Name = "Производство хлеба и мучных кондитерских изделий недлительного хранения",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.72",
                        Name = "Производство сухарей и печенья, мучных кондитерских изделий    длительного хранения",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.72.0",
                        Name = "Производство сухарей и печенья, мучных кондитерских изделий    длительного хранения",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.73",
                        Name =
                            "Производство макаронных изделий (макарон, лапши, кускуса и аналогичных мучных продуктов)",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.73.0",
                        Name =
                            "Производство макаронных изделий (макарон, лапши, кускуса и аналогичных мучных продуктов)",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.8",
                        Name = "Производство прочих пищевых продуктов",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "10.81", Name = "Производство сахара", Section = "CA"},
                    new ActivityCategory {Code = "10.81.0", Name = "Производство сахара", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "10.82",
                        Name = "Производство какао, шоколада и кондитерских изделий из сахара",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.82.0",
                        Name = "Производство какао, шоколада и кондитерских изделий из сахара",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.83",
                        Name = "Производство (переработка) чая и кофе",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.83.0",
                        Name = "Производство (переработка) чая и кофе",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "10.84", Name = "Производство пряностей и приправ", Section = "CA"},
                    new ActivityCategory {Code = "10.84.0", Name = "Производство пряностей и приправ", Section = "CA"},
                    new ActivityCategory {Code = "10.85", Name = "Производство готовых блюд", Section = "CA"},
                    new ActivityCategory {Code = "10.85.0", Name = "Производство готовых блюд", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "10.86",
                        Name = "Производство гомогенизированных (детского питания) и диетических продуктов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.86.0",
                        Name = "Производство гомогенизированных (детского питания) и диетических продуктов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.89",
                        Name = "Производство прочих пищевых продуктов, не включенных в другие    группировки",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.89.1",
                        Name = "Производство супов, бульонов, яйцепродуктов",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "10.89.2", Name = "Производство дрожжей", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "10.89.9",
                        Name = "Производство прочих пищевых продуктов, не включенных в другие группировки",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.9",
                        Name = "Производство готовых кормов для животных",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.91",
                        Name = "Производство готовых кормов для животных, содержащихся на фермах",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.91.0",
                        Name = "Производство готовых кормов для животных, содержащихся на фермах",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.92",
                        Name = "Производство готовых кормов для домашних животных (питомцев)",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "10.92.0",
                        Name = "Производство готовых кормов для домашних животных (питомцев)",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "11", Name = "Производство напитков", Section = "CA"},
                    new ActivityCategory {Code = "11.0", Name = "Производство напитков", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "11.01",
                        Name = "Очистка ректификация и купажирование спиртов",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "11.01.0",
                        Name = "Очистка ректификация и купажирование спиртов",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "11.02", Name = "Производство вина из винограда", Section = "CA"},
                    new ActivityCategory {Code = "11.02.0", Name = "Производство вина из винограда", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "11.03",
                        Name = "Производство сидра и прочих плодово- ягодных вин",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "11.03.0",
                        Name = "Производство сидра и прочих плодово- ягодных вин",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "11.04",
                        Name = "Производство прочих недистиллированных напитков из сброженного    материала",
                        Section = "CA"
                    },
                    new ActivityCategory
                    {
                        Code = "11.04.0",
                        Name = "Производство прочих недистиллированных напитков из сброженного    материала",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "11.05", Name = "Производство пива", Section = "CA"},
                    new ActivityCategory {Code = "11.05.0", Name = "Производство пива", Section = "CA"},
                    new ActivityCategory {Code = "11.06", Name = "Производство солода", Section = "CA"},
                    new ActivityCategory {Code = "11.06.0", Name = "Производство солода", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "11.07",
                        Name =
                            "Производство безалкогольных напитков; производство минеральных    вод и других вод в бутылках",
                        Section = "CA"
                    },
                    new ActivityCategory {Code = "12", Name = "Производство табачных изделий", Section = "CA"},
                    new ActivityCategory {Code = "12.0", Name = "Производство табачных изделий", Section = "CA"},
                    new ActivityCategory {Code = "12.00", Name = "Производство табачных изделий", Section = "CA"},
                    new ActivityCategory {Code = "12.00.0", Name = "Производство табачных изделий", Section = "CA"},
                    new ActivityCategory
                    {
                        Code = "CB",
                        Name = "Текстильное производство; производство одежды и обуви, кожи и прочих кожаных изделий",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "13", Name = "Текстильное производство", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "13.1",
                        Name = "Подготовка текстильных волокон и пряжи",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.10",
                        Name = "Подготовка и прядение текстильных волокон и пряжи",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.10.1",
                        Name = "Подготовка и прядение  хлопковых волокон",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.10.2",
                        Name = "Подготовка и прядение шерстяных волокон",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.10.3",
                        Name = "Подготовка и прядение шелковых волокон",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.10.4",
                        Name = "Подготовка и прядение льняных волокон",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "13.10.5", Name = "Производство швейных ниток", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "13.10.9",
                        Name = "Подготовка и прядение прочих волокон",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "13.2", Name = "Ткацкое производство", Section = "CB"},
                    new ActivityCategory {Code = "13.20", Name = "Ткацкое производство", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "13.20.1",
                        Name = "Производство хлопчатобумажных тканей",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "13.20.2", Name = "Производство шерстяных тканей", Section = "CB"},
                    new ActivityCategory {Code = "13.20.3", Name = "Производство шелковых тканей", Section = "CB"},
                    new ActivityCategory {Code = "13.20.4", Name = "Производство льняных тканей", Section = "CB"},
                    new ActivityCategory {Code = "13.20.9", Name = "Производство прочих тканей", Section = "CB"},
                    new ActivityCategory {Code = "13.3", Name = "Отделка тканей и текстильных изделий", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "13.30",
                        Name = "Отделка тканей и текстильных изделий",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.30.0",
                        Name = "Отделка тканей и текстильных изделий",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.9",
                        Name = "Производство прочих текстильных изделий",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.91",
                        Name = "Производство трикотажного полотна машинного или ручного вязания",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.91.0",
                        Name = "Производство трикотажного полотна машинного или ручного вязания",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.92",
                        Name = "Производство готовых текстильных изделий, кроме одежды",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.92.0",
                        Name = "Производство готовых текстильных изделий, кроме одежды",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.93",
                        Name = "Производство ковров и ковровых изделий",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.93.0",
                        Name = "Производство ковров и ковровых изделий",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.94",
                        Name = "Производство канатов, веревок, шпагата и сетей",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.94.0",
                        Name = "Производство канатов, веревок, шпагата и сетей",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.95",
                        Name = "Производство нетканых текстильных материалов и изделий из них",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.95.0",
                        Name = "Производство нетканых текстильных материалов и изделий из них",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.96",
                        Name = "Производство прочих текстильных изделий технического и производственного назначения",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.96.0",
                        Name = "Производство прочих текстильных изделий технического и производственного назначения",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.99",
                        Name = "Производство прочих текстильных изделий, не включенных в другие группировки",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "13.99.0",
                        Name = "Производство прочих текстильных изделий, не включенных в другие группировки",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "14", Name = "Производство одежды", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "14.1",
                        Name = "Производство одежды, кроме одежды из меха",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "14.11", Name = "Производство одежды из кожи", Section = "CB"},
                    new ActivityCategory {Code = "14.11.0", Name = "Производство одежды из кожи", Section = "CB"},
                    new ActivityCategory {Code = "14.12", Name = "Производство рабочей одежды", Section = "CB"},
                    new ActivityCategory {Code = "14.12.0", Name = "Производство рабочей одежды", Section = "CB"},
                    new ActivityCategory {Code = "14.13", Name = "Производство прочей верхней одежды", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "14.13.0",
                        Name = "Производство прочей верхней одежды",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "14.14", Name = "Производство нижнего белья", Section = "CB"},
                    new ActivityCategory {Code = "14.14.0", Name = "Производство нижнего белья", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "14.19",
                        Name = "Производство прочей одежды и аксессуаров",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "14.19.1", Name = "Производство головных уборов", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "14.19.9",
                        Name = "Производство прочей одежды и аксессуаров, не включенных в другие группировки",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "14.2", Name = "Производство меховых изделий", Section = "CB"},
                    new ActivityCategory {Code = "14.20", Name = "Производство меховых изделий", Section = "CB"},
                    new ActivityCategory {Code = "14.20.0", Name = "Производство меховых изделий", Section = "CB"},
                    new ActivityCategory {Code = "14.3", Name = "Производство трикотажных изделий", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "14.31",
                        Name = "Производство трикотажных чулочно- носочных изделий",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "14.31.0",
                        Name = "Производство трикотажных чулочно- носочных изделий",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "14.39",
                        Name = "Производство прочих трикотажных изделий",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "14.39.0",
                        Name = "Производство прочих трикотажных изделий",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "15",
                        Name = "Производство кожи, изделий из кожи, производство обуви",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "15.1",
                        Name =
                            "Дубление и выделка кож; производство чемоданов и аналогичных изделий, шорно- седельных изделий и упряжи; выделка и окраска меха",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "15.11",
                        Name = "Дубление и выделка кожи; выделка и окраска меха",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "15.11.1",
                        Name = "Выделка и окраска меховых шкур (меха)",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "15.11.9",
                        Name = "Дубление и выделка прочих шкур и кож; производство натуральной  и композиционной кожи",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "15.12",
                        Name = "Производство чемоданов, сумок и других изделий из кожи",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "15.12.1",
                        Name = "Производство чемоданов, сумок, аналогичных изделий и мелкой кожгалантереи",
                        Section = "CB"
                    },
                    new ActivityCategory
                    {
                        Code = "15.12.9",
                        Name = "Производство  изделий шорно-седельных",
                        Section = "CB"
                    },
                    new ActivityCategory {Code = "15.2", Name = "Производство обуви", Section = "CB"},
                    new ActivityCategory {Code = "15.20", Name = "Производство обуви", Section = "CB"},
                    new ActivityCategory {Code = "15.20.0", Name = "Производство обуви", Section = "CB"},
                    new ActivityCategory
                    {
                        Code = "CC",
                        Name = "Производство деревянных и бумажных изделий; полиграфическая деятельность",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16",
                        Name =
                            "Обработка древесины и производство изделий из дерева и пробки (кроме мебели), плетенных изделий",
                        Section = "CC"
                    },
                    new ActivityCategory {Code = "16.1", Name = "Распиловка и строгание древесины", Section = "CC"},
                    new ActivityCategory {Code = "16.10", Name = "Распиловка и строгание древесины", Section = "CC"},
                    new ActivityCategory {Code = "16.10.0", Name = "Распиловка и строгание древесины", Section = "CC"},
                    new ActivityCategory
                    {
                        Code = "16.2",
                        Name = "Производство изделий из дерева и пробки, плетенных изделий",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.21",
                        Name = "Производство шпона, фанеры,  плит  и панелей из древесины",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.21.0",
                        Name = "Производство шпона, фанеры,  плит  и панелей из древесины",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.22",
                        Name = "Производство щитового паркета в сборе",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.22.0",
                        Name = "Производство щитового паркета в сборе",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.23",
                        Name = "Производство деревянных строительных конструкций и столярных    изделий",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.23.1",
                        Name = "Производство деревянных сборно- разборных домов",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.23.9",
                        Name = "Производство прочих деревянных строительных конструкций и столярных изделий",
                        Section = "CC"
                    },
                    new ActivityCategory {Code = "16.24", Name = "Производство деревянной тары", Section = "CC"},
                    new ActivityCategory
                    {
                        Code = "16.24.0",
                        Name = "Производство деревянной тары и прочих деревянных изделий",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.29",
                        Name =
                            "Производство прочих деревянных изделий, изделий из пробки, соломки  и растительных материалов для плетения",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.29.1",
                        Name = "Производство изделий из пробки, соломки и растительных материалов для плетения",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "16.29.9",
                        Name = "Производство прочих деревянных изделий, не включенных в другие группировки",
                        Section = "CC"
                    },
                    new ActivityCategory {Code = "17", Name = "Производство бумаги и картона", Section = "CC"},
                    new ActivityCategory
                    {
                        Code = "17.1",
                        Name = "Производство бумажной массы, бумаги и картона",
                        Section = "CC"
                    },
                    new ActivityCategory {Code = "17.11", Name = "Производство бумажной массы", Section = "CC"},
                    new ActivityCategory {Code = "17.11.0", Name = "Производство бумажной массы", Section = "CC"},
                    new ActivityCategory {Code = "17.12", Name = "Производство бумаги и картона", Section = "CC"},
                    new ActivityCategory {Code = "17.12.0", Name = "Производство бумаги и картона", Section = "CC"},
                    new ActivityCategory
                    {
                        Code = "17.2",
                        Name = "Производство изделий из бумаги или картона",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "17.21",
                        Name = "Производство гофрированных бумаги и картона, бумажной и картонной тары",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "17.21.1",
                        Name = "Производство гофрированных бумаги и картона",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "17.21.9",
                        Name = "Производство бумажной и картонной тары",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "17.22",
                        Name =
                            "Производство бумажных изделий хозяйственно-бытового и санитарно- гигиенического назначения",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "17.22.0",
                        Name =
                            "Производство бумажных изделий хозяйственно-бытового и санитарно- гигиенического назначения",
                        Section = "CC"
                    },
                    new ActivityCategory {Code = "17.23", Name = "Производство писчебумажных изделий", Section = "CC"},
                    new ActivityCategory
                    {
                        Code = "17.23.0",
                        Name = "Производство писчебумажных изделий",
                        Section = "CC"
                    },
                    new ActivityCategory {Code = "17.24", Name = "Производство обоев", Section = "CC"},
                    new ActivityCategory {Code = "17.24.0", Name = "Производство обоев", Section = "CC"},
                    new ActivityCategory
                    {
                        Code = "17.29",
                        Name = "Производство прочих изделий из бумаги и картона",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "17.29.0",
                        Name = "Производство прочих изделий из бумаги и картона",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18",
                        Name = "Полиграфическая деятельность и тиражирование записанных носителей информации",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18.1",
                        Name = "Полиграфическая деятельность и предоставление услуг в этой области",
                        Section = "CC"
                    },
                    new ActivityCategory {Code = "18.11", Name = "Печатание газет", Section = "CC"},
                    new ActivityCategory {Code = "18.11.0", Name = "Печатание газет", Section = "CC"},
                    new ActivityCategory
                    {
                        Code = "18.12",
                        Name = "Печатание прочей полиграфической продукции (кроме газет)",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18.12.0",
                        Name = "Печатание прочей полиграфической продукции (кроме газет)",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18.13",
                        Name = "Предоставление услуг по подготовке к печати и тиражированию",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18.13.0",
                        Name = "Предоставление услуг по подготовке к печати и тиражированию",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18.14",
                        Name = "Переплетная и отделочная деятельность",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18.14.0",
                        Name = "Переплетная и отделочная деятельность",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18.2",
                        Name = "Воспроизведение (копирование), тиражирование записанных носителей информации",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18.20",
                        Name = "Воспроизведение (копирование), тиражирование записанных носителей информации",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "18.20.0",
                        Name = "Воспроизведение (копирование), тиражирование записанных носителей информации",
                        Section = "CC"
                    },
                    new ActivityCategory
                    {
                        Code = "CD",
                        Name = "Производство кокса и очищенных нефтепродуктов",
                        Section = "CD"
                    },
                    new ActivityCategory
                    {
                        Code = "19",
                        Name = "Производство кокса и очищенных нефтепродуктов",
                        Section = "CD"
                    },
                    new ActivityCategory {Code = "19.1", Name = "Производство кокса", Section = "CD"},
                    new ActivityCategory {Code = "19.10", Name = "Производство кокса", Section = "CD"},
                    new ActivityCategory {Code = "19.10.0", Name = "Производство кокса", Section = "CD"},
                    new ActivityCategory
                    {
                        Code = "19.2",
                        Name = "Производство очищенных нефтепродуктов",
                        Section = "CD"
                    },
                    new ActivityCategory
                    {
                        Code = "19.20",
                        Name = "Производство очищенных нефтепродуктов",
                        Section = "CD"
                    },
                    new ActivityCategory
                    {
                        Code = "19.20.1",
                        Name = "Производство очищенных нефтепродуктов, в том числе брикетов из нефтепродуктов",
                        Section = "CD"
                    },
                    new ActivityCategory
                    {
                        Code = "19.20.9",
                        Name = "Производство топливных брикетов из угля и торфа",
                        Section = "CD"
                    },
                    new ActivityCategory {Code = "CE", Name = "Производство химической продукции", Section = "CE"},
                    new ActivityCategory {Code = "20", Name = "Производство химической продукции", Section = "CE"},
                    new ActivityCategory
                    {
                        Code = "20.1",
                        Name =
                            "Производство основных химических веществ, удобрений и азотных соединений, пластмасс и синтетического каучука (резины) в первичных    формах",
                        Section = "CE"
                    },
                    new ActivityCategory {Code = "20.11", Name = "Производство промышленных газов", Section = "CE"},
                    new ActivityCategory {Code = "20.11.0", Name = "Производство промышленных газов", Section = "CE"},
                    new ActivityCategory {Code = "20.12", Name = "Производство красителей и пигментов", Section = "CE"},
                    new ActivityCategory
                    {
                        Code = "20.12.0",
                        Name = "Производство красителей и пигментов",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.13",
                        Name = "Производство прочих основных неорганических химических веществ",
                        Section = "CE"
                    },
                    new ActivityCategory {Code = "20.13.1", Name = "Производство обогащенного урана", Section = "CE"},
                    new ActivityCategory
                    {
                        Code = "20.13.9",
                        Name =
                            "Производство прочих основных неорганических химических веществ, не включенных в другие группировки",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.14",
                        Name = "Производство прочих основных органических химических веществ",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.14.1",
                        Name = "Производство углеводородов и их производных",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.14.2",
                        Name = "Производство нециклических и циклических спиртов",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.14.9",
                        Name =
                            "Производство прочих основных органических химических веществ, не включенных в другие группировки",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.15",
                        Name = "Производство удобрений и азотных соединений",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.15.0",
                        Name = "Производство удобрений и азотных соединений",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.16",
                        Name = "Производство пластмасс в первичных формах",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.16.0",
                        Name = "Производство пластмасс в первичных формах",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.17",
                        Name = "Производство синтетического каучука в первичных формах",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.17.0",
                        Name = "Производство синтетического каучука в первичных формах",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.2",
                        Name = "Производство пестицидов и прочих агрохимических продуктов",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.20",
                        Name = "Производство пестицидов и прочих агрохимических продуктов",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.20.0",
                        Name = "Производство пестицидов и прочих агрохимических продуктов",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.3",
                        Name = "Производство красок, лаков и аналогичных покрытий, типографских красок и мастик",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.30",
                        Name = "Производство красок, лаков и аналогичных покрытий, типографских красок и мастик",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.30.1",
                        Name = "Производство красок, эмалей и лаков на основе полимеров",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.30.9",
                        Name =
                            "Производство прочих красок (в т. ч. типографских), эмалей, мастик и связанных с ними продуктов (разбавителей, растворителей и т. п.)",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.4",
                        Name =
                            "Производство мыла и моющих, чистящих и полирующих средств,    парфюмерных и косметических средств",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.41",
                        Name = "Производство мыла и моющих, чистящих и полирующих средств",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.41.0",
                        Name = "Производство мыла и моющих, чистящих и полирующих средств",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.42",
                        Name = "Производство парфюмерных и косметических средств",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.42.0",
                        Name = "Производство парфюмерных и косметических средств",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.5",
                        Name = "Производство прочих химических продуктов",
                        Section = "CE"
                    },
                    new ActivityCategory {Code = "20.51", Name = "Производство взрывчатых веществ", Section = "CE"},
                    new ActivityCategory {Code = "20.51.0", Name = "Производство взрывчатых веществ", Section = "CE"},
                    new ActivityCategory {Code = "20.52", Name = "Производство клеев", Section = "CE"},
                    new ActivityCategory {Code = "20.52.0", Name = "Производство клеев", Section = "CE"},
                    new ActivityCategory {Code = "20.53", Name = "Производство эфирных масел", Section = "CE"},
                    new ActivityCategory {Code = "20.53.0", Name = "Производство эфирных масел", Section = "CE"},
                    new ActivityCategory
                    {
                        Code = "20.59",
                        Name = "Производство прочих химических продуктов, не включенных в другие группировки",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.59.1",
                        Name = "Производство фотохимических материалов",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.59.2",
                        Name = "Производство желатина и его производных",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.59.9",
                        Name = "Производство различных химических продуктов, не включенных в другие группировки",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.6",
                        Name = "Производство химических (искусственных и синтетических) волокон",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.60",
                        Name = "Производство химических (искусственных и синтетических) волокон",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "20.60.0",
                        Name = "Производство химических (искусственных и синтетических) волокон",
                        Section = "CE"
                    },
                    new ActivityCategory
                    {
                        Code = "CF",
                        Name = "Производство фармацевтической продукции",
                        Section = "CF"
                    },
                    new ActivityCategory
                    {
                        Code = "21",
                        Name = "Производство фармацевтической продукции",
                        Section = "CF"
                    },
                    new ActivityCategory
                    {
                        Code = "21.1",
                        Name = "Производство основных фармацевтических продуктов",
                        Section = "CF"
                    },
                    new ActivityCategory
                    {
                        Code = "21.10",
                        Name = "Производство основных фармацевтических продуктов",
                        Section = "CF"
                    },
                    new ActivityCategory
                    {
                        Code = "21.10.0",
                        Name = "Производство основных фармацевтических продуктов",
                        Section = "CF"
                    },
                    new ActivityCategory
                    {
                        Code = "21.2",
                        Name = "Производство фармацевтических препаратов и материалов",
                        Section = "CF"
                    },
                    new ActivityCategory
                    {
                        Code = "21.20",
                        Name = "Производство фармацевтических препаратов и материалов",
                        Section = "CF"
                    },
                    new ActivityCategory
                    {
                        Code = "21.20.1",
                        Name = "Производство радиоактивных веществ для диагностики",
                        Section = "CF"
                    },
                    new ActivityCategory
                    {
                        Code = "21.20.9",
                        Name = "Производство прочих медикаментов, фармацевтических препаратов и материалов",
                        Section = "CF"
                    },
                    new ActivityCategory
                    {
                        Code = "CG",
                        Name =
                            "Производство резиновых и пластмассовых изделий, прочих неметаллических минеральных продуктов",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22",
                        Name = "Производство резиновых и пластмассовых изделий",
                        Section = "CG"
                    },
                    new ActivityCategory {Code = "22.1", Name = "Производство резиновых изделий", Section = "CG"},
                    new ActivityCategory
                    {
                        Code = "22.11",
                        Name = "Производство резиновых шин, покрышек и камер",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.11.1",
                        Name = "Производство резиновых шин, покрышек и камер",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.11.9",
                        Name = "Восстановление резиновых шин и покрышек",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.19",
                        Name = "Производство прочих резиновых изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.19.0",
                        Name = "Производство прочих резиновых изделий",
                        Section = "CG"
                    },
                    new ActivityCategory {Code = "22.2", Name = "Производство пластмассовых изделий", Section = "CG"},
                    new ActivityCategory
                    {
                        Code = "22.21",
                        Name = "Производство пластмассовых плит, полос, труб и профилей",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.21.0",
                        Name = "Производство пластмассовых плит, полос, труб и профилей",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.22",
                        Name = "Производство пластмассовых изделий для упаковывания товаров",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.22.0",
                        Name = "Производство пластмассовых изделий для упаковывания товаров",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.23",
                        Name = "Производство пластмассовых изделий, используемых в строительстве",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.23.0",
                        Name = "Производство пластмассовых изделий, используемых в строительстве",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.29",
                        Name = "Производство прочих пластмассовых изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "22.29.0",
                        Name = "Производство прочих пластмассовых изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23",
                        Name = "Производство прочих неметаллических минеральных продуктов",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.1",
                        Name = "Производство стекла и изделий из стекла",
                        Section = "CG"
                    },
                    new ActivityCategory {Code = "23.11", Name = "Производство листового стекла", Section = "CG"},
                    new ActivityCategory {Code = "23.11.0", Name = "Производство листового стекла", Section = "CG"},
                    new ActivityCategory
                    {
                        Code = "23.12",
                        Name = "Формование и обработка листового стекла",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.12.0",
                        Name = "Формование и обработка листового стекла",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.13",
                        Name = "Производство полых стеклянных изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.13.0",
                        Name = "Производство полых стеклянных изделий",
                        Section = "CG"
                    },
                    new ActivityCategory {Code = "23.14", Name = "Производство стекловолокна", Section = "CG"},
                    new ActivityCategory {Code = "23.14.0", Name = "Производство стекловолокна", Section = "CG"},
                    new ActivityCategory
                    {
                        Code = "23.19",
                        Name = "Производство и обработка прочих стеклянных изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.19.1",
                        Name = "Производство лабораторных, гигиенических и фармацевтических стеклянных изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.19.9",
                        Name =
                            "Производство технического и прочего стекла и стеклянных изделий, не включенных в другие группировки",
                        Section = "CG"
                    },
                    new ActivityCategory {Code = "23.2", Name = "Производство огнеупоров", Section = "CG"},
                    new ActivityCategory {Code = "23.20", Name = "Производство огнеупоров", Section = "CG"},
                    new ActivityCategory {Code = "23.20.0", Name = "Производство огнеупоров", Section = "CG"},
                    new ActivityCategory
                    {
                        Code = "23.3",
                        Name = "Производство прочих строительных изделий из глины",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.31",
                        Name = "Производство керамических плиток и плит",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.31.0",
                        Name = "Производство керамических плиток и плит",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.32",
                        Name = "Производство кирпича, черепицы и прочих строительных изделий из обожженной глины",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.32.0",
                        Name = "Производство кирпича, черепицы и прочих строительных изделий из обожженной глины",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.4",
                        Name = "Производство прочих изделий из керамики и фарфора",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.41",
                        Name = "Производство керамических хозяйственных и декоративных изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.41.0",
                        Name = "Производство керамических хозяйственных и декоративных изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.42",
                        Name = "Производство керамических санитарно- технических изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.42.0",
                        Name = "Производство керамических санитарно- технических изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.43",
                        Name = "Производство керамических электроизоляторов и изолирующей арматуры",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.43.0",
                        Name = "Производство керамических электроизоляторов и изолирующей арматуры",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.44",
                        Name = "Производство прочих керамических технических изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.44.0",
                        Name = "Производство прочих керамических технических изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.49",
                        Name = "Производство прочих керамических изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.49.0",
                        Name = "Производство прочих керамических изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.5",
                        Name = "Производство цемента, извести и гипса",
                        Section = "CG"
                    },
                    new ActivityCategory {Code = "23.51", Name = "Производство цемента", Section = "CG"},
                    new ActivityCategory {Code = "23.51.0", Name = "Производство цемента", Section = "CG"},
                    new ActivityCategory {Code = "23.52", Name = "Производство извести и гипса", Section = "CG"},
                    new ActivityCategory {Code = "23.52.0", Name = "Производство извести и гипса", Section = "CG"},
                    new ActivityCategory
                    {
                        Code = "23.6",
                        Name = "Производство изделий из бетона, гипса и цемента",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.61",
                        Name = "Производство изделий из бетона для использования в строительстве",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.61.0",
                        Name = "Производство изделий из бетона для использования в строительстве",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.62",
                        Name = "Производство изделий из гипса для использования в строительстве",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.62.0",
                        Name = "Производство изделий из гипса для использования в строительстве",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.63",
                        Name = "Производство товарного бетона (готовой бетонной смеси)",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.63.0",
                        Name = "Производство товарного бетона (готовой бетонной смеси)",
                        Section = "CG"
                    },
                    new ActivityCategory {Code = "23.64", Name = "Производство сухих бетонных смесей", Section = "CG"},
                    new ActivityCategory
                    {
                        Code = "23.64.0",
                        Name = "Производство сухих бетонных смесей",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.65",
                        Name = "Производство изделий из асбестоцемента (волокнистого цемент)",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.65.0",
                        Name = "Производство изделий из асбестоцемента (волокнистого цемента)",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.69",
                        Name = "Производство прочих изделий из бетона, гипса и цемента",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.69.0",
                        Name = "Производство прочих изделий из бетона, гипса и цемента",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.7",
                        Name = "Резка, обработка и отделка декоративного и строительного камня",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.70",
                        Name = "Резка, обработка и отделка декоративного и строительного камня",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.70.0",
                        Name = "Резка, обработка и отделка декоративного и строительного камня",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.9",
                        Name =
                            "Производство абразивных изделий и прочей неметаллической    минеральной продукции, не включенной в другие группировки",
                        Section = "CG"
                    },
                    new ActivityCategory {Code = "23.91", Name = "Производство абразивных изделий", Section = "CG"},
                    new ActivityCategory {Code = "23.91.0", Name = "Производство абразивных изделий", Section = "CG"},
                    new ActivityCategory
                    {
                        Code = "23.99",
                        Name =
                            "Производство прочей неметаллической минеральной продукции, не включенной в другие группировки.",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.99.1",
                        Name = "Производство асбестовых технических изделий",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "23.99.9",
                        Name =
                            "Производство прочей неметаллической минеральной продукции, в другом месте не поименованной",
                        Section = "CG"
                    },
                    new ActivityCategory
                    {
                        Code = "CH",
                        Name =
                            "Производство основных металлов и готовых металлических изделий, кроме машин и оборудования",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "24", Name = "Производство основных металлов", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "24.1",
                        Name = "Производство чугуна, стали и ферросплавов",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.10",
                        Name = "Производство чугуна, стали и ферросплавов",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.10.0",
                        Name = "Производство чугуна, стали и ферросплавов",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.2",
                        Name = "Производство стальных труб, полых профилей и фитингов",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.20",
                        Name = "Производство стальных труб, полых профилей и фитингов",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.20.0",
                        Name = "Производство стальных труб, полых профилей и фитингов",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "24.3", Name = "Прочая первичная обработка стали", Section = "CH"},
                    new ActivityCategory {Code = "24.31", Name = "Холодное волочение", Section = "CH"},
                    new ActivityCategory {Code = "24.31.0", Name = "Холодное волочение", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "24.32",
                        Name = "Холодная прокатка лент и узких полос",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.32.0",
                        Name = "Холодная прокатка лент и узких полос",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "24.33", Name = "Холодная штамповка и гибка", Section = "CH"},
                    new ActivityCategory {Code = "24.33.0", Name = "Холодная штамповка и гибка", Section = "CH"},
                    new ActivityCategory {Code = "24.34", Name = "Производство проволоки", Section = "CH"},
                    new ActivityCategory {Code = "24.34.0", Name = "Производство проволоки", Section = "CH"},
                    new ActivityCategory {Code = "24.4", Name = "Производство цветных металлов", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "24.41",
                        Name = "Производство благородных (драгоценных) металлов",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.41.0",
                        Name = "Производство благородных (драгоценных) металлов",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "24.42", Name = "Производство алюминия", Section = "CH"},
                    new ActivityCategory {Code = "24.42.0", Name = "Производство алюминия", Section = "CH"},
                    new ActivityCategory {Code = "24.43", Name = "Производство свинца, цинка и олова", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "24.43.0",
                        Name = "Производство свинца, цинка и олова",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "24.44", Name = "Производство меди", Section = "CH"},
                    new ActivityCategory {Code = "24.44.0", Name = "Производство меди", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "24.45",
                        Name = "Производство прочих цветных  металлов",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.45.1",
                        Name = "Производство никеля и изделий из него",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.45.9",
                        Name =
                            "Производство прочих цветных металлов, не включенных в другие группировки, и изделия из них",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.46",
                        Name = "Производство ядерных материалов (ядерного топлива)",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "24.46.0",
                        Name = "Производство ядерных материалов (ядерного топлива)",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "24.5", Name = "Литье металлов", Section = "CH"},
                    new ActivityCategory {Code = "24.51", Name = "Литье чугуна", Section = "CH"},
                    new ActivityCategory {Code = "24.51.0", Name = "Литье чугуна", Section = "CH"},
                    new ActivityCategory {Code = "24.52", Name = "Литье стали", Section = "CH"},
                    new ActivityCategory {Code = "24.52.0", Name = "Литье стали", Section = "CH"},
                    new ActivityCategory {Code = "24.53", Name = "Литье легких металлов", Section = "CH"},
                    new ActivityCategory {Code = "24.53.0", Name = "Литье легких металлов", Section = "CH"},
                    new ActivityCategory {Code = "24.54", Name = "Литье прочих цветных металлов", Section = "CH"},
                    new ActivityCategory {Code = "24.54.0", Name = "Литье прочих цветных металлов", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "25",
                        Name = "Производство готовых металлических изделий, кроме машин и оборудования",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.1",
                        Name = "Производство строительных металлических конструкций и изделий",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.11",
                        Name = "Производство строительных металлических конструкций",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.11.0",
                        Name = "Производство строительных металлических конструкций",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.12",
                        Name = "Производство металлических дверей и оконных рам",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.12.0",
                        Name = "Производство металлических дверей и оконных рам",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.2",
                        Name = "Производство металлических резервуаров, радиаторов и котлов центрального отопления",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.21",
                        Name = "Производство радиаторов и котлов центрального отопления",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.21.0",
                        Name = "Производство радиаторов и котлов центрального отопления",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.29",
                        Name = "Производство прочих металлических цистерн, резервуаров и контейнеров",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.29.0",
                        Name = "Производство прочих металлических цистерн, резервуаров и контейнеров",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.3",
                        Name = "Производство паровых котлов, кроме котлов центрального отопления",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.30",
                        Name = "Производство паровых котлов, кроме котлов центрального отопления",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.30.0",
                        Name = "Производство паровых котлов, кроме котлов центрального отопления",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "25.4", Name = "Производство оружия и боеприпасов", Section = "CH"},
                    new ActivityCategory {Code = "25.40", Name = "Производство оружия и боеприпасов", Section = "CH"},
                    new ActivityCategory {Code = "25.40.0", Name = "Производство оружия и боеприпасов", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "25.5",
                        Name = "Ковка, прессование, штамповка, прокатка; порошковая металлургия",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.50",
                        Name = "Ковка, прессование, штамповка, прокатка; порошковая металлургия",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.50.1",
                        Name = "Ковка, прессование, штамповка, прокатка",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.50.9",
                        Name = "Изготовление металлических изделий методом порошковой металлургии",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.6",
                        Name =
                            "Обработка металлов и нанесение покрытий на металлы; основные технологические процессы машиностроения",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.61",
                        Name = "Обработка металлов и нанесение покрытий на металлы",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.61.0",
                        Name = "Обработка металлов и нанесение покрытий на металлы",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.62",
                        Name = "Основные технологические процессы машиностроения",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.62.0",
                        Name = "Основные технологические процессы машиностроения",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.7",
                        Name = "Производство ножевых изделий, инструментов и скобяных изделий",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "25.71", Name = "Производство ножевых изделий", Section = "CH"},
                    new ActivityCategory {Code = "25.71.0", Name = "Производство ножевых изделий", Section = "CH"},
                    new ActivityCategory {Code = "25.72", Name = "Производство замков и петель", Section = "CH"},
                    new ActivityCategory {Code = "25.72.0", Name = "Производство замков и петель", Section = "CH"},
                    new ActivityCategory {Code = "25.73", Name = "Производство инструментов", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "25.73.1",
                        Name =
                            "Производство ручных инструментов для использования в сельском хозяйстве, садоводстве или лесном хозяйстве",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "25.73.9", Name = "Производство прочих инструментов", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "25.9",
                        Name = "Производство прочих готовых металлических изделий",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.91",
                        Name = "Производство металлических бочек и аналогичных емкостей",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.91.0",
                        Name = "Производство металлических бочек и аналогичных емкостей",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.92",
                        Name = "Производство упаковки из легких металлов",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.92.0",
                        Name = "Производство упаковки из легких металлов",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.93",
                        Name = "Производство изделий из проволоки, цепей и пружин",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.93.1",
                        Name = "Производство  изделий  из проволоки",
                        Section = "CH"
                    },
                    new ActivityCategory {Code = "25.93.9", Name = "Производство цепей и пружин", Section = "CH"},
                    new ActivityCategory
                    {
                        Code = "25.94",
                        Name = "Производство крепежных изделий, резьбовых изделий",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.94.0",
                        Name = "Производство крепежных изделий, резьбовых изделий",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.99",
                        Name = "Производство прочих готовых металлических изделий, не включенных в другие группировки",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.99.1",
                        Name =
                            "Производство металлических изделий санитарно-технического и хозяйственно- бытового назначения",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "25.99.9",
                        Name = "Производство прочих готовых металлических изделий, в другом месте непоименованных",
                        Section = "CH"
                    },
                    new ActivityCategory
                    {
                        Code = "CI",
                        Name = "Производство компьютеров, электронного и оптического оборудования",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26",
                        Name = "Производство компьютеров, электронного и оптического оборудования",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.1",
                        Name = "Производство электронных компонентов и плат (схем)",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.11",
                        Name = "Производство электронных компонентов",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.11.0",
                        Name = "Производство электронных компонентов",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.12",
                        Name = "Производство электронных плат (схем)",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.12.0",
                        Name = "Производство электронных плат (схем)",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.2",
                        Name = "Производство компьютеров и периферийных устройств",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.20",
                        Name = "Производство компьютеров и периферийных устройств",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.20.0",
                        Name = "Производство компьютеров и периферийных устройств",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.3",
                        Name = "Производство телекоммуникационного оборудования (оборудования связи)",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.30",
                        Name = "Производство телекоммуникационного оборудования (оборудования вязи)",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.30.1",
                        Name = "Производство теле- и радиоаппаратуры производственного назначения",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.30.2",
                        Name = "Производство аппаратуры для проводной телефонной или телеграфной связи",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.30.9",
                        Name = "Производство электросигнального и прочего оборудования связи",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.4",
                        Name = "Производство бытовой электронной аппаратуры",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.40",
                        Name = "Производство бытовой электронной аппаратуры",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.40.0",
                        Name = "Производство бытовой электронной аппаратуры",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.5",
                        Name =
                            "Производство приборов и инструментов для измерения, контроля, испытаний и навигации; производство часов всех типов",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.51",
                        Name = "Производство приборов и инструментов для измерения, контроля, испытаний, навигации",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.51.1",
                        Name =
                            "Производство навигационных, метеорологических, геофизических и аналогических приборов и инструментов",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.51.9",
                        Name = "Производство прочих контрольно- измерительных приборов",
                        Section = "CI"
                    },
                    new ActivityCategory {Code = "26.52", Name = "Производство часов всех типов", Section = "CI"},
                    new ActivityCategory
                    {
                        Code = "26.52.1",
                        Name = "Производство наручных и прочих часов",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.52.9",
                        Name =
                            "Производство деталей и принадлежностей для часов; производство прочих приборов для регистрации времени",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.6",
                        Name =
                            "Производство рентгеновского, электромедицинского и электротерапевтического оборудования",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.60",
                        Name =
                            "Производство рентгеновского, электромедицинского и электротерапевтического оборудования",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.60.0",
                        Name =
                            "Производство рентгеновского, электромедицинского и электротерапевтического оборудования",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.7",
                        Name = "Производство оптических приборов и фотооборудования",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.70",
                        Name = "Производство оптических приборов и фотооборудования",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.70.1",
                        Name = "Производство фото -  и кинооборудования и их частей",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.70.9",
                        Name =
                            "Производство очков, линз, оптических микроскопов, биноклей и прочих оптических приборов и их частей",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.8",
                        Name = "Производство магнитных и оптических носителей информации",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.80",
                        Name = "Производство магнитных и оптических носителей информации",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "26.80.0",
                        Name = "Производство магнитных и оптических носителей информации",
                        Section = "CI"
                    },
                    new ActivityCategory
                    {
                        Code = "CJ",
                        Name = "Производство электрического оборудования",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27",
                        Name = "Производство электрического оборудования",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.1",
                        Name =
                            "Производство электродвигателей, генераторов и трансформаторов, электрораспределительной и регулирующей аппаратуры",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.11",
                        Name = "Производство электродвигателей, генераторов и трансформаторов",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.11.0",
                        Name = "Производство электродвигателей, генераторов и трансформаторов",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.12",
                        Name = "Производство электрораспределительной и регулирующей аппаратуры",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.12.0",
                        Name = "Производство электрораспределительной и регулирующей аппаратуры",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.2",
                        Name = "Производство первичных элементов, батарей первичных элементов и их частей",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.20",
                        Name = "Производство первичных элементов, батарей первичных элементов и их частей",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.20.0",
                        Name = "Производство первичных элементов, батарей первичных элементов и их частей",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.3",
                        Name = "Производство проводов и кабелей и приспособлений для электропроводки",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.31",
                        Name = "Производство волоконно-оптических кабелей",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.31.0",
                        Name = "Производство волоконно-оптических кабелей",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.32",
                        Name = "Производство прочих изолированных проводов и кабелей",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.32.0",
                        Name = "Производство прочих изолированных проводов и кабелей",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.33",
                        Name = "Производство приспособлений для электропроводки",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.33.0",
                        Name = "Производство приспособлений для электропроводки",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.4",
                        Name = "Производство электрического осветительного оборудования",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.40",
                        Name = "Производство электрического осветительного оборудования",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.40.0",
                        Name = "Производство электрического осветительного оборудования",
                        Section = "CJ"
                    },
                    new ActivityCategory {Code = "27.5", Name = "Производство бытовых приборов", Section = "CJ"},
                    new ActivityCategory
                    {
                        Code = "27.51",
                        Name = "Производство бытовых электрических приборов",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.51.1",
                        Name = "Производство бытовых холодильников и морозильников",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.51.9",
                        Name = "Производство прочих бытовых электроприборов",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.52",
                        Name = "Производство бытовых неэлектрических приборов",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.52.0",
                        Name = "Производство бытовых неэлектрических приборов",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.9",
                        Name = "Производство прочего электрического оборудования",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.90",
                        Name = "Производство прочего электрического оборудования",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.90.1",
                        Name = "Производство электросварочного оборудования",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.90.2",
                        Name = "Производство электродной продукции",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "27.90.9",
                        Name = "Производство прочего электрического оборудования, не включенного в другие группировки",
                        Section = "CJ"
                    },
                    new ActivityCategory
                    {
                        Code = "CK",
                        Name = "Производство машин и оборудования, не включенных в другие группировки",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28",
                        Name = "Производство машин и оборудования, не включенных в другие группировки",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.1",
                        Name = "Производство машин и оборудования общего назначения",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.11",
                        Name =
                            "Производство двигателей и турбин, кроме авиационных, автомобильных и мотоциклетных двигателей",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.11.1",
                        Name =
                            "Производство двигателей и их частей кроме авиационных, автомобильных, и мотоциклетных двигателей",
                        Section = "CK"
                    },
                    new ActivityCategory {Code = "28.11.9", Name = "Производство турбин и их частей", Section = "CK"},
                    new ActivityCategory
                    {
                        Code = "28.12",
                        Name = "Производство гидравлического и пневматического силового оборудования",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.12.0",
                        Name = "Производство гидравлического и пневматического силового оборудования",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.13",
                        Name = "Производство прочих насосов и компрессоров",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.13.1",
                        Name = "Производство насосов для перекачки жидкостей и подъемников жидкостей",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.13.9",
                        Name =
                            "Производство воздушных или вакуумных насосов, воздушных или прочих газовых компрессоров",
                        Section = "CK"
                    },
                    new ActivityCategory {Code = "28.14", Name = "Производство кранов и клапанов", Section = "CK"},
                    new ActivityCategory {Code = "28.14.0", Name = "Производство кранов и клапанов", Section = "CK"},
                    new ActivityCategory
                    {
                        Code = "28.15",
                        Name = "Производство подшипников, зубчатых передач, элементов механических передач и приводов",
                        Section = "CK"
                    },
                    new ActivityCategory {Code = "28.15.1", Name = "Производство подшипников", Section = "CK"},
                    new ActivityCategory
                    {
                        Code = "28.15.9",
                        Name = "Производство прочих общемашиностроительных узлов и деталей",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.2",
                        Name = "Производство прочих машин и оборудования общего назначения",
                        Section = "CK"
                    },
                    new ActivityCategory {Code = "28.21", Name = "Производство печей и печных горелок", Section = "CK"},
                    new ActivityCategory
                    {
                        Code = "28.21.1",
                        Name = "Производство электрических печей и печных грелок",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.21.9",
                        Name = "Производство прочих (неэлектрических) печей и печных грелок",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.22",
                        Name = "Производство подъемно-транспортного оборудования",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.22.1",
                        Name = "Производство подъемных кранов и их частей",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.22.2",
                        Name = "Производство оборудования непрерывного транспорта и его части",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.22.9",
                        Name =
                            "Производство прочего подъемно- транспортного, погрузочно- разгрузочного оборудования и его частей",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.23",
                        Name = "Производство офисного оборудования ( кроме компьютеров и периферийного оборудования)",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.23.0",
                        Name = "Производство офисного оборудования (кроме компьютеров и периферийного оборудования)",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.24",
                        Name = "Производство ручных механизированных инструментов",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.24.0",
                        Name = "Производство ручных механизированных инструментов",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.25",
                        Name = "Производство промышленного холодильного и вентиляционного оборудования",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.25.0",
                        Name = "Производство промышленного холодильного и вентиляционного оборудования",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.29",
                        Name =
                            "Производство прочих машин и оборудования общего назначения, не включенных в другие группировки",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.29.1",
                        Name = "Производство фильтрующего и очистительного оборудования",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.29.2",
                        Name =
                            "Производство машин и оборудования для распыления и разбрызгивания жидкостей или порошков, упаковочных и оберточных машин",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.29.3",
                        Name = "Производство весоизмерительного оборудования (кроме прецизионных лабораторных весов)",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.29.9",
                        Name =
                            "Производство прочих машин и оборудования общего назначения деталей и узлов к ним, в другом месте не поименованных",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.3",
                        Name = "Производство машин и оборудования для сельского и  лесного хозяйства",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.30",
                        Name = "Производство машин и оборудования для сельского и лесного хозяйства",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.30.1",
                        Name = "Производство тракторов для сельского и лесного хозяйства и частей к ним",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.30.2",
                        Name = "Производство прочих машин и оборудования для растениеводства и частей к ним",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.30.3",
                        Name =
                            "Производство прочих машин и оборудования для  животноводства и приготовления кормов и частей к ним",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.30.9",
                        Name = "Производство прочих машин и оборудования для лесозаготовок и мелиорации",
                        Section = "CK"
                    },
                    new ActivityCategory {Code = "28.4", Name = "Производство станков", Section = "CK"},
                    new ActivityCategory {Code = "28.41", Name = "Производство металлорежущих станков", Section = "CK"},
                    new ActivityCategory
                    {
                        Code = "28.41.0",
                        Name = "Производство металлорежущих станков",
                        Section = "CK"
                    },
                    new ActivityCategory {Code = "28.49", Name = "Производство прочих станков", Section = "CK"},
                    new ActivityCategory
                    {
                        Code = "28.49.1",
                        Name =
                            "Производство станков для обработки камня, дерева, керамики и аналогичных твердых материалов",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.49.9",
                        Name = "Производство прочих станков и оборудования, не включенных в другие группировки",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.9",
                        Name = "Производство прочих машин и оборудования специального назначения",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.91",
                        Name = "Производство машин и оборудования для металлургии",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.91.0",
                        Name = "Производство машин и оборудования для металлургии",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.92",
                        Name = "Производство машин и оборудования для добычи полезных ископаемых и строительства",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.92.0",
                        Name = "Производство машин и оборудования для добычи полезных ископаемых и строительства",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.93",
                        Name =
                            "Производство машин и оборудования для изготовления пищевых продуктов, включая напитки, и табачных изделий",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.93.0",
                        Name =
                            "Производство машин и оборудования для изготовления пищевых продуктов, включая напитки, и табачных изделий",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.94",
                        Name =
                            "Производство машин и оборудования для изготовления текстильных, швейных, меховых и кожаных изделий",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.94.1",
                        Name = "Производство машин и оборудования для изготовления текстильных изделий",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.94.2",
                        Name = "Производство машин и оборудования для изготовления швейных и трикотажных изделий",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.94.3",
                        Name = "Производство бытовых швейных машин",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.94.4",
                        Name =
                            "Производство машин и оборудования для обработки кожи и меха и производства обуви и прочих кожаных и меховых изделий",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.94.9",
                        Name =
                            "Производство машин и оборудования для прачечных и прочих предприятий бытового обслуживания",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.95",
                        Name = "Производство машин и оборудования для изготовления бумаги и картона",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.95.0",
                        Name = "Производство машин и оборудования для изготовления бумаги и картона",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.96",
                        Name = "Производство машин и оборудования для обработки мягкой резины или пластмасс",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.96.0",
                        Name = "Производство машин и оборудования для обработки мягкой резины или пластмасс",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.99",
                        Name =
                            "Производство прочих машин и оборудования специального назначения, не включенных в другие группировки",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.99.1",
                        Name = "Производство печатных и брошюровочно- переплетных машин и оборудования",
                        Section = "CK"
                    },
                    new ActivityCategory
                    {
                        Code = "28.99.9",
                        Name =
                            "Производство прочих машин и оборудования специального назначения в другом месте не поименованных",
                        Section = "CK"
                    },
                    new ActivityCategory {Code = "CL", Name = "Производство транспортных средств", Section = "CL"},
                    new ActivityCategory {Code = "29", Name = "Производство автомобилей", Section = "CL"},
                    new ActivityCategory {Code = "29.1", Name = "Производство автомобилей", Section = "CL"},
                    new ActivityCategory {Code = "29.10", Name = "Производство автомобилей", Section = "CL"},
                    new ActivityCategory {Code = "29.10.1", Name = "Производство автомобилей", Section = "CL"},
                    new ActivityCategory
                    {
                        Code = "29.10.9",
                        Name = "Производство автомобильных двигателей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "29.2",
                        Name = "Производство автомобильных кузовов; производство прицепов",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "29.20",
                        Name = "Производство автомобильных кузовов; производство прицепов",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "29.20.1",
                        Name = "Производство автомобильных кузовов",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "29.20.2",
                        Name = "Производство прицепов, полуприцепов, контейнеров и их частей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "29.3",
                        Name = "Производство частей и принадлежностей автомобилей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "29.31",
                        Name = "Производство электрооборудования для автомобилей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "29.31.0",
                        Name = "Производство электрооборудования для автомобилей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "29.32",
                        Name = "Производство прочих частей и принадлежностей автомобилей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "29.32.0",
                        Name = "Производство прочих частей и принадлежностей автомобилей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30",
                        Name = "Производство прочих транспортных средств",
                        Section = "CL"
                    },
                    new ActivityCategory {Code = "30.1", Name = "Строительство судов", Section = "CL"},
                    new ActivityCategory
                    {
                        Code = "30.11",
                        Name = "Строительство судов и плавучих средств",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.11.0",
                        Name = "Строительство судов и плавучих средств",
                        Section = "CL"
                    },
                    new ActivityCategory {Code = "30.12", Name = "Строительство спортивных судов", Section = "CL"},
                    new ActivityCategory {Code = "30.12.0", Name = "Строительство спортивных судов", Section = "CL"},
                    new ActivityCategory
                    {
                        Code = "30.2",
                        Name = "Производство железнодорожного подвижного состава",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.20",
                        Name = "Производство железнодорожного подвижного состава",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.20.0",
                        Name = "Производство железнодорожного подвижного состава",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.3",
                        Name = "Производство летательных аппаратов, включая космические",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.30",
                        Name = "Производство летательных аппаратов, включая космические",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.30.1",
                        Name = "Производство межконтинентальных баллистических ракет",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.30.2",
                        Name = "Производство космических летательных аппаратов и их частей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.30.9",
                        Name = "Производство прочих летательных аппаратов и их частей",
                        Section = "CL"
                    },
                    new ActivityCategory {Code = "30.4", Name = "Производство военных боевых машин", Section = "CL"},
                    new ActivityCategory {Code = "30.40", Name = "Производство военных боевых машин", Section = "CL"},
                    new ActivityCategory {Code = "30.40.0", Name = "Производство военных боевых машин", Section = "CL"},
                    new ActivityCategory
                    {
                        Code = "30.9",
                        Name = "Производство прочих транспортных средств, не включенных другие группировки",
                        Section = "CL"
                    },
                    new ActivityCategory {Code = "30.91", Name = "Производство мотоциклов", Section = "CL"},
                    new ActivityCategory {Code = "30.91.0", Name = "Производство мотоциклов", Section = "CL"},
                    new ActivityCategory
                    {
                        Code = "30.92",
                        Name = "Производство велосипедов и инвалидных колясок",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.92.1",
                        Name = "Производство велосипедов и их частей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.92.2",
                        Name = "Производство инвалидных колясок и их частей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.92.9",
                        Name = "Производство детских колясок и их частей",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.99",
                        Name =
                            "Производство прочих транспортных средств и оборудования, не включенных в другие группировки",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "30.99.0",
                        Name =
                            "Производство прочих транспортных средств и оборудования, не включенных в другие группировки",
                        Section = "CL"
                    },
                    new ActivityCategory
                    {
                        Code = "CM",
                        Name = "Прочие производства, ремонт и установка машин и оборудования",
                        Section = "CM"
                    },
                    new ActivityCategory {Code = "31", Name = "Производство мебели", Section = "CM"},
                    new ActivityCategory {Code = "31.0", Name = "Производство мебели", Section = "CM"},
                    new ActivityCategory
                    {
                        Code = "31.01",
                        Name = "Производство мебели для офисов и предприятий торговли",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "31.01.0",
                        Name = "Производство мебели для офисов и предприятий торговли",
                        Section = "CM"
                    },
                    new ActivityCategory {Code = "31.02", Name = "Производство кухонной мебели", Section = "CM"},
                    new ActivityCategory {Code = "31.02.0", Name = "Производство кухонной мебели", Section = "CM"},
                    new ActivityCategory {Code = "31.03", Name = "Производство матрасов", Section = "CM"},
                    new ActivityCategory {Code = "31.03.0", Name = "Производство матрасов", Section = "CM"},
                    new ActivityCategory {Code = "31.09", Name = "Производство прочей мебели", Section = "CM"},
                    new ActivityCategory {Code = "31.09.0", Name = "Производство прочей мебели", Section = "CM"},
                    new ActivityCategory {Code = "32", Name = "Производство прочей продукции", Section = "CM"},
                    new ActivityCategory
                    {
                        Code = "32.1",
                        Name = "Производство ювелирных изделий, монет и медалей",
                        Section = "CM"
                    },
                    new ActivityCategory {Code = "32.11", Name = "Чеканка монет и медалей", Section = "CM"},
                    new ActivityCategory {Code = "32.11.0", Name = "Чеканка монет и медалей", Section = "CM"},
                    new ActivityCategory {Code = "32.12", Name = "Производство ювелирных изделий", Section = "CM"},
                    new ActivityCategory {Code = "32.12.0", Name = "Производство ювелирных изделий", Section = "CM"},
                    new ActivityCategory
                    {
                        Code = "32.13",
                        Name = "Производство бижутерии и аналогичных изделий",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.13.0",
                        Name = "Производство бижутерии и аналогичных изделий",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.2",
                        Name = "Производство музыкальных инструментов",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.20",
                        Name = "Производство музыкальных инструментов",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.20.0",
                        Name = "Производство музыкальных инструментов",
                        Section = "CM"
                    },
                    new ActivityCategory {Code = "32.3", Name = "Производство спортивных товаров", Section = "CM"},
                    new ActivityCategory {Code = "32.30", Name = "Производство спортивных товаров", Section = "CM"},
                    new ActivityCategory {Code = "32.30.0", Name = "Производство спортивных товаров", Section = "CM"},
                    new ActivityCategory {Code = "32.4", Name = "Производство игр и игрушек", Section = "CM"},
                    new ActivityCategory {Code = "32.40", Name = "Производство игр и игрушек", Section = "CM"},
                    new ActivityCategory {Code = "32.40.0", Name = "Производство игр и игрушек", Section = "CM"},
                    new ActivityCategory
                    {
                        Code = "32.5",
                        Name = "Производство инструментов и приспособлений, используемых в медицине и стоматологии",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.50",
                        Name = "Производство инструментов и приспособлений, используемых в медицине и стоматологии",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.50.0",
                        Name = "Производство инструментов и приспособлений, используемых в медицине и стоматологии",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.9",
                        Name = "Производство прочей продукции, не включенной в другие группировки",
                        Section = "CM"
                    },
                    new ActivityCategory {Code = "32.91", Name = "Производство метел и щеток", Section = "CM"},
                    new ActivityCategory {Code = "32.91.0", Name = "Производство метел и щеток", Section = "CM"},
                    new ActivityCategory
                    {
                        Code = "32.99",
                        Name = "Производство различной продукции, не включенной в другие группировки",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.99.1",
                        Name = "Производство защитных головных уборов, одежды и прочих защитных средств",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.99.2",
                        Name = "Производство наборов пишущих принадлежностей и прочих канцелярских изделий",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "32.99.9",
                        Name = "Производство прочей продукции, в другом месте не поименованной (включая набивку чучел)",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33",
                        Name = "Ремонт и установка машин и оборудования",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.1",
                        Name = "Ремонт готовых металлических изделий, машин и оборудования",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.11",
                        Name = "Ремонт готовых металлических изделий",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.11.1",
                        Name = "Ремонт и техническое обслуживание металлоконструкций",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.11.2",
                        Name = "Ремонт и техническое обслуживание металлических цистерн, баков, резервуаров и емкостей",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.11.3",
                        Name = "Ремонт и техническое обслуживание радиаторов и котлов центрального отопления",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.11.4",
                        Name = "Ремонт и техническое обслуживание парогенераторов и прочих водяных котлов",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.11.5",
                        Name = "Ремонт и техническое обслуживание оружия и систем вооружения",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.11.9",
                        Name = "Ремонт прочих готовых металлических изделий",
                        Section = "CM"
                    },
                    new ActivityCategory {Code = "33.12", Name = "Ремонт продукции машиностроения", Section = "CM"},
                    new ActivityCategory
                    {
                        Code = "33.12.1",
                        Name = "Ремонт и техническое обслуживание оборудования общего назначения",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.12.9",
                        Name = "Ремонт и техническое обслуживание оборудования специального назначения",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.13",
                        Name = "Ремонт электронного и оптического оборудования",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.13.1",
                        Name =
                            "Ремонт и техническое обслуживание приборов и инструментов для измерения контроля испытаний и навигации группы 26.5",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.13.2",
                        Name =
                            "Ремонт и техническое обслуживание медицинских инструментов, рентгеновского, электромедицинского и электротерапевтического оборудования группы 26.6",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.13.9",
                        Name =
                            "Ремонт и техническое обслуживание профессиональных оптических приборов и фотооборудования группы 26.7",
                        Section = "CM"
                    },
                    new ActivityCategory {Code = "33.14", Name = "Ремонт электрического оборудования", Section = "CM"},
                    new ActivityCategory
                    {
                        Code = "33.14.1",
                        Name =
                            "Ремонт и техническое обслуживание электродвигателей, генераторов и трансформаторов, электрораспределительной и регулирующей аппаратуры группы 27.1",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.14.9",
                        Name =
                            "Ремонт и техническое обслуживание прочего электрического оборудования раздела 27, кроме группы 27.1",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.15",
                        Name = "Ремонт и техническое обслуживание водного транспорта",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.15.0",
                        Name = "Ремонт и техническое обслуживание водного транспорта",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.16",
                        Name = "Ремонт и техническое обслуживание летательных аппаратов и космических аппаратов",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.16.0",
                        Name = "Ремонт и техническое обслуживание летательных аппаратов и космических аппаратов",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.17",
                        Name = "Ремонт и техническое обслуживание прочего транспортного оборудования",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.17.1",
                        Name =
                            "Ремонт и техническое обслуживание железнодорожных локомотивов и подвижного состава, в т.ч. трамваев, вагонов метро и троллейбусов",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.17.9",
                        Name =
                            "Ремонт и техническое обслуживание прочего транспортного оборудования, не включенного в другие группировки",
                        Section = "CM"
                    },
                    new ActivityCategory {Code = "33.19", Name = "Ремонт прочего оборудования", Section = "CM"},
                    new ActivityCategory {Code = "33.19.0", Name = "Ремонт прочего оборудования", Section = "CM"},
                    new ActivityCategory
                    {
                        Code = "33.2",
                        Name = "Установка промышленных машин и оборудования",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.20",
                        Name = "Установка промышленных машин и оборудования",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.20.1",
                        Name = "Установка готовых металлических изделий (кроме машин и оборудования)",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.20.2",
                        Name = "Установка машин и оборудования общего назначения",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.20.3",
                        Name = "Установка машин и оборудования специального назначения",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.20.4",
                        Name = "Установка электронного и оптического оборудования",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.20.5",
                        Name = "Установка электрического оборудования",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.20.6",
                        Name = "Установка оборудования контроля за технологическими процессами",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "33.20.9",
                        Name = "Установка прочего оборудования, не включенного в другие группировки",
                        Section = "CM"
                    },
                    new ActivityCategory
                    {
                        Code = "D",
                        Name = "Обеспечение (снабжение) электроэнергией, газом, паром и кондиционированным воздухом",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35",
                        Name = "Обеспечение (снабжение) электроэнергией, газом, паром и кондиционированным воздухом",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.1",
                        Name = "Производство (выработка) электроэнергии, ее передача и распределение",
                        Section = "D"
                    },
                    new ActivityCategory {Code = "35.11", Name = "Производство электроэнергии", Section = "D"},
                    new ActivityCategory
                    {
                        Code = "35.11.1",
                        Name = "Производство электроэнергии тепловыми электростанциями",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.11.2",
                        Name = "Производство электроэнергии гидроэлектростанциями",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.11.3",
                        Name = "Производство электроэнергии ядерными (атомными) электростанциями",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.11.9",
                        Name = "Производство электроэнергии прочими электростанциями",
                        Section = "D"
                    },
                    new ActivityCategory {Code = "35.12", Name = "Передача электроэнергии", Section = "D"},
                    new ActivityCategory {Code = "35.12.0", Name = "Передача электроэнергии", Section = "D"},
                    new ActivityCategory {Code = "35.13", Name = "Распределение электроэнергии", Section = "D"},
                    new ActivityCategory {Code = "35.13.0", Name = "Распределение электроэнергии", Section = "D"},
                    new ActivityCategory {Code = "35.14", Name = "Продажа электроэнергии", Section = "D"},
                    new ActivityCategory {Code = "35.14.0", Name = "Продажа электроэнергии", Section = "D"},
                    new ActivityCategory
                    {
                        Code = "35.2",
                        Name = "Производство газа; распределение газообразного топлива через системы газоснабжения",
                        Section = "D"
                    },
                    new ActivityCategory {Code = "35.21", Name = "Производство газообразного топлива", Section = "D"},
                    new ActivityCategory {Code = "35.21.0", Name = "Производство газообразного топлива", Section = "D"},
                    new ActivityCategory
                    {
                        Code = "35.22",
                        Name = "Распределение газообразного топлива через системы газоснабжения",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.22.0",
                        Name = "Распределение газообразного топлива через системы газоснабжения",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.23",
                        Name = "Продажа газообразного топлива, поступающего через системы газоснабжения",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.23.0",
                        Name = "Продажа газообразного топлива, поступающего через системы газоснабжения",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.3",
                        Name = "Обеспечение (снабжение) паром и кондиционированным воздухом",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.30",
                        Name = "Обеспечение (снабжение) паром и кондиционированным воздухом",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "35.30.0",
                        Name = "Обеспечение (снабжение) паром и кондиционированным воздухом",
                        Section = "D"
                    },
                    new ActivityCategory
                    {
                        Code = "E",
                        Name = "Водоснабжение, очистка, обработка отходов и получение вторичного сырья",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "36",
                        Name = "Сбор, обработка и распределение воды (водоснабжение)",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "36.0",
                        Name = "Сбор, обработка и распределение воды (водоснабжение)",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "36.00",
                        Name = "Сбор, обработка и распределение воды (водоснабжение)",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "36.00.0",
                        Name = "Сбор, обработка и распределение воды (водоснабжение)",
                        Section = "E"
                    },
                    new ActivityCategory {Code = "37", Name = "Сбор и обработка сточных вод", Section = "E"},
                    new ActivityCategory {Code = "37.0", Name = "Сбор и обработка сточных вод", Section = "E"},
                    new ActivityCategory {Code = "37.00", Name = "Сбор и обработка сточных вод", Section = "E"},
                    new ActivityCategory {Code = "37.00.0", Name = "Сбор и обработка сточных вод", Section = "E"},
                    new ActivityCategory
                    {
                        Code = "38",
                        Name = "Сбор, обработка и уничтожение отходов, получение вторичного сырья",
                        Section = "E"
                    },
                    new ActivityCategory {Code = "38.1", Name = "Сбор отходов", Section = "E"},
                    new ActivityCategory {Code = "38.11", Name = "Сбор безопасных отходов", Section = "E"},
                    new ActivityCategory {Code = "38.11.0", Name = "Сбор безопасных отходов", Section = "E"},
                    new ActivityCategory {Code = "38.12", Name = "Сбор опасных отходов", Section = "E"},
                    new ActivityCategory {Code = "38.12.0", Name = "Сбор опасных отходов", Section = "E"},
                    new ActivityCategory {Code = "38.2", Name = "Обработка и уничтожение отходов", Section = "E"},
                    new ActivityCategory
                    {
                        Code = "38.21",
                        Name = "Обработка и уничтожение безопасных отходов",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "38.21.0",
                        Name = "Обработка и уничтожение безопасных отходов",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "38.22",
                        Name = "Обработка и уничтожение опасных отходов",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "38.22.0",
                        Name = "Обработка и уничтожение опасных отходов",
                        Section = "E"
                    },
                    new ActivityCategory {Code = "38.3", Name = "Получение вторичного сырья", Section = "E"},
                    new ActivityCategory
                    {
                        Code = "38.31",
                        Name = "Разрезка и разбор остатков судов, машин и прочего оборудование",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "38.31.0",
                        Name = "Разрезка и разбор остатков судов, машин и прочего оборудование",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "38.32",
                        Name = "Сортировка и переработка отходов для получения вторичного сырья",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "38.32.0",
                        Name = "Сортировка и переработка отходов для получения вторичного сырья",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "39",
                        Name = "Обеззараживание и прочая обработка отходов",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "39.0",
                        Name = "Обеззараживание и прочая обработка отходов",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "39.00",
                        Name = "Обеззараживание и прочая обработка отходов",
                        Section = "E"
                    },
                    new ActivityCategory
                    {
                        Code = "39.00.0",
                        Name = "Обеззараживание и прочая обработка отходов",
                        Section = "E"
                    },
                    new ActivityCategory {Code = "F", Name = "Строительство", Section = "F"},
                    new ActivityCategory {Code = "41", Name = "Строительство зданий", Section = "F"},
                    new ActivityCategory {Code = "41.1", Name = "Разработка строительных проектов", Section = "F"},
                    new ActivityCategory {Code = "41.10", Name = "Разработка строительных проектов", Section = "F"},
                    new ActivityCategory {Code = "41.10.0", Name = "Разработка строительных проектов", Section = "F"},
                    new ActivityCategory {Code = "41.2", Name = "Строительство жилых и нежилых зданий", Section = "F"},
                    new ActivityCategory {Code = "41.20", Name = "Строительство жилых и нежилых здании", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "41.20.0",
                        Name = "Строительство жилых и нежилых здании",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "42",
                        Name = "Строительство объектов гражданского назначения",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "42.1",
                        Name = "Строительство автомобильных и железных дорог",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "42.11",
                        Name = "Строительство шоссе и автомагистралей",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "42.11.0",
                        Name = "Строительство шоссе и автомагистралей",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "42.12", Name = "Строительство железных дорог", Section = "F"},
                    new ActivityCategory {Code = "42.12.0", Name = "Строительство железных дорог", Section = "F"},
                    new ActivityCategory {Code = "42.13", Name = "Строительство мостов и туннелей", Section = "F"},
                    new ActivityCategory {Code = "42.13.0", Name = "Строительство мостов и туннелей", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "42.2",
                        Name = "Строительство прочих объектов гражданского назначения",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "42.21", Name = "Строительство трубопроводов", Section = "F"},
                    new ActivityCategory {Code = "42.21.0", Name = "Строительство трубопроводов", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "42.22",
                        Name = "Строительство линий электропередач и телекоммуникаций",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "42.22.0",
                        Name = "Строительство линий электропередач и телекоммуникаций",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "42.9",
                        Name =
                            "Строительство прочих объектов гражданского назначения, не включенных в другие группировки",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "42.91", Name = "Строительство водных сооружений", Section = "F"},
                    new ActivityCategory {Code = "42.91.0", Name = "Строительство водных сооружений", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "42.99",
                        Name = "Строительство прочих объектов гражданского назначения, в другом месте не поименованных",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "42.99.0",
                        Name = "Строительство прочих объектов гражданского назначения, в другом месте не поименованных",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "43", Name = "Специальные строительные работы", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "43.1",
                        Name = "Снос зданий и подготовка строительного участка",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "43.11", Name = "Снос зданий", Section = "F"},
                    new ActivityCategory {Code = "43.11.0", Name = "Снос зданий", Section = "F"},
                    new ActivityCategory {Code = "43.12", Name = "Подготовка строительного участка", Section = "F"},
                    new ActivityCategory {Code = "43.12.0", Name = "Подготовка строительного участка", Section = "F"},
                    new ActivityCategory {Code = "43.13", Name = "Разведочное бурение", Section = "F"},
                    new ActivityCategory {Code = "43.13.0", Name = "Разведочное бурение", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "43.2",
                        Name =
                            "Монтаж (установка) электрических и водопроводных систем и прочие строительно-монтажные работы",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "43.21", Name = "Электромонтажные работы", Section = "F"},
                    new ActivityCategory {Code = "43.21.0", Name = "Электромонтажные работы", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "43.22",
                        Name =
                            "Монтаж (установка) водопроводных систем, систем отопления, вентиляции, кондиционирования воздуха",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "43.22.0",
                        Name =
                            "Монтаж (установка) водопроводных систем, систем отопления, вентиляции, кондиционирования воздуха",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "43.29", Name = "Прочие строительно-монтажные работы", Section = "F"},
                    new ActivityCategory {Code = "43.29.1", Name = "Изоляционные работы", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "43.29.9",
                        Name = "Прочие строительно-монтажные работы, не включенные в другие группировки",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "43.3",
                        Name = "Работы по завершению строительства и отделочные работы",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "43.31", Name = "Штукатурные работы", Section = "F"},
                    new ActivityCategory {Code = "43.31.0", Name = "Штукатурные работы", Section = "F"},
                    new ActivityCategory {Code = "43.32", Name = "Столярные и плотницкие работы", Section = "F"},
                    new ActivityCategory {Code = "43.32.0", Name = "Столярные и плотницкие работы", Section = "F"},
                    new ActivityCategory {Code = "43.33", Name = "Покрытие полов и облицовка стен", Section = "F"},
                    new ActivityCategory {Code = "43.33.0", Name = "Покрытие полов и облицовка стен", Section = "F"},
                    new ActivityCategory {Code = "43.34", Name = "Малярные и стекольные работы", Section = "F"},
                    new ActivityCategory {Code = "43.34.0", Name = "Малярные и стекольные работы", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "43.39",
                        Name = "Прочие отделочные и завершающие работы",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "43.39.0",
                        Name = "Прочие отделочные и завершающие работы",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "43.9",
                        Name = "Прочие специальные строительные работы",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "43.91", Name = "Кровельные работы", Section = "F"},
                    new ActivityCategory {Code = "43.91.0", Name = "Кровельные работы", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "43.99",
                        Name = "Прочие специальные строительные работы, не включенные в другие группировки",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "43.99.1",
                        Name = "Закладка фундамента, включая забивку свай",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "43.99.2",
                        Name = "Бурение и постройка водяных колодцев, артезианских скважин",
                        Section = "F"
                    },
                    new ActivityCategory {Code = "43.99.3", Name = "Кладка кирпичей и камней", Section = "F"},
                    new ActivityCategory
                    {
                        Code = "43.99.9",
                        Name = "Прочие специальные строительные работы, в другом месте не поименованные",
                        Section = "F"
                    },
                    new ActivityCategory
                    {
                        Code = "G",
                        Name = "Оптовая и розничная торговля; ремонт автомобилей и мотоциклов",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45",
                        Name =
                            "Оптовая и розничная торговля автомобилями и мотоциклами; ремонт автомобилей и мотоциклов",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "45.1", Name = "Торговля автомобилями", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "45.11",
                        Name = "Торговля автомобилями и легкими автотранспортными средствами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.11.1",
                        Name = "Оптовая торговля автомобилями и легкими автотранспортными средствами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.11.9",
                        Name = "Розничная торговля автомобилями и легкими автотранспортными средствами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.19",
                        Name = "Торговля прочими автотранспортными средствами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.19.1",
                        Name = "Оптовая торговля прочими автотранспортными средствами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.19.9",
                        Name = "Розничная торговля прочими автотранспортными средствами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.2",
                        Name = "Техническое обслуживание и ремонт автомобилей",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.20",
                        Name = "Техническое обслуживание и ремонт автомобилей",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.20.1",
                        Name = "Техническое  обслуживание и ремонт легковых автомобилей",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.20.9",
                        Name = "Техническое  обслуживание и ремонт прочих автомобилей",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.3",
                        Name = "Торговля автомобильными деталями, узлами и принадлежностями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.31",
                        Name = "Оптовая торговля автомобильными деталями, узлами и принадлежностями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.31.0",
                        Name = "Оптовая торговля автомобильными деталями, узлами и принадлежностями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.32",
                        Name = "Розничная торговля автомобильными деталями, узлами и  принадлежностями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.32.0",
                        Name = "Розничная торговля автомобильными деталями, узлами  и принадлежностями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.4",
                        Name =
                            "Торговля мотоциклами, их деталями, узлами и принадлежностями; техническое обслуживание и ремонт мотоциклов",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.40",
                        Name =
                            "Торговля мотоциклами, их деталями, узлами и принадлежностями; техническое обслуживание и ремонт мотоциклов",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.40.1",
                        Name = "Оптовая торговля мотоциклами, их деталями, узлами и принадлежностями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.40.2",
                        Name = "Розничная торговля мотоциклами, их деталями, узлами и принадлежностями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "45.40.9",
                        Name = "Техническое обслуживание и ремонт мотоциклов",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46",
                        Name = "Оптовая торговля, кроме торговли автомобилями и мотоциклами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.1",
                        Name = "Оптовая торговля через агентов (за вознаграждение или на договорной основе)",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.11",
                        Name =
                            "Деятельность агентов по оптовой торговле сельскохозяйственным сырьем, живыми    животными, текстильным сырьем и полуфабрикатами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.11.0",
                        Name =
                            "Деятельность агентов по оптовой торговле сельскохозяйственным сырьем, живыми    животными, текстильным сырьем и полуфабрикатами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.12",
                        Name =
                            "Деятельность агентов по оптовой торговле топливом, рудами, металлами и химическими веществами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.12.0",
                        Name =
                            "Деятельность агентов по оптовой торговле топливом, рудами, металлами и химическими веществами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.13",
                        Name = "Деятельность агентов по оптовой торговле древесиной и строительными материалами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.13.0",
                        Name = "Деятельность агентов по оптовой торговле древесиной и строительными материалами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.14",
                        Name =
                            "Деятельность агентов по оптовой торговле машинами, оборудованием, судами и летательными аппаратами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.14.0",
                        Name =
                            "Деятельность агентов по оптовой торговле машинами, оборудованием, судами и летательными аппаратами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.15",
                        Name =
                            "Деятельность агентов по оптовой торговле мебелью, бытовыми товарами, скобяными и прочими металлическими изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.15.0",
                        Name =
                            "Деятельность агентов по оптовой торговле мебелью, бытовыми товарами, скобяными и прочими металлическими изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.16",
                        Name =
                            "Деятельность агентов по оптовой торговле текстильными изделиями, одеждой, обувью, изделиями  из  кожи и меха",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.16.0",
                        Name =
                            "Деятельность агентов по оптовой торговле текстильными изделиями, одеждой, обувью, изделиями из кожи и меха",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.17",
                        Name =
                            "Деятельность агентов по оптовой торговле пищевыми  продуктами, включая напитки, и табачными  изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.17.0",
                        Name =
                            "Деятельность агентов по оптовой торговле пищевыми  продуктами, включая напитки, и табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.18",
                        Name =
                            "Деятельность агентов, специализирующихся на оптовой торговле отдельными видами товаров или группами товаров, не включенными в другие группировки",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.18.0",
                        Name =
                            "Деятельность агентов, специализирующихся на оптовой торговле отдельными видами товаров или группами товаров, не включенными в другие группировки",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.19",
                        Name = "Деятельность агентов по оптовой торговле товарами широкого ассортимента",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.19.0",
                        Name = "Деятельность агентов по оптовой торговле товарами широкого ассортимента",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.2",
                        Name = "Оптовая торговля сельскохозяйственным сырьем и живыми животными",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.21",
                        Name = "Оптовая торговля зерном, необработанным табаком, семенами и кормами для животных",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.21.1",
                        Name = "Оптовая торговля зерном, семенами и кормами для животных",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.21.9",
                        Name = "Оптовая торговля необработанным табаком",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.22",
                        Name = "Оптовая торговля цветами и другими растениями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.22.0",
                        Name = "Оптовая торговля цветами и другими растениями",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.23", Name = "Оптовая торговля живыми животными", Section = "G"},
                    new ActivityCategory {Code = "46.23.0", Name = "Оптовая торговля живыми животными", Section = "G"},
                    new ActivityCategory {Code = "46.24", Name = "Оптовая торговля шкурами и кожей", Section = "G"},
                    new ActivityCategory {Code = "46.24.0", Name = "Оптовая торговля шкурами и кожей", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.3",
                        Name = "Оптовая торговля пищевыми продуктами, включая  напитки, и табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.31", Name = "Оптовая торговля фруктами и овощами", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.31.0",
                        Name = "Оптовая торговля фруктами и овощами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.32",
                        Name = "Оптовая торговля мясом и мясными продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.32.0",
                        Name = "Оптовая торговля мясом и мясными продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.33",
                        Name = "Оптовая торговля молочными продуктами, яйцами, пищевыми маслами и жирами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.33.0",
                        Name = "Оптовая торговля молочными продуктами, яйцами, пищевыми маслами и жирами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.34",
                        Name = "Оптовая торговля алкогольными и другими напитками",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.34.0",
                        Name = "Оптовая торговля алкогольными и другими напитками",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.35", Name = "Оптовая торговля табачными изделиями", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.35.0",
                        Name = "Оптовая торговля табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.36",
                        Name = "Оптовая торговля сахаром, шоколадом и кондитерскими изделиями из сахара",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.36.0",
                        Name = "Оптовая торговля сахаром, шоколадом и кондитерскими изделиями из сахара",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.37",
                        Name = "Оптовая торговля кофе, чаем, какао и пряностями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.37.0",
                        Name = "Оптовая торговля кофе, чаем, какао и пряностями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.38",
                        Name =
                            "Оптовая торговля прочими пищевыми продуктами, в том числе рыбой, ракообразными и моллюсками",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.39",
                        Name =
                            "Неспециализированная оптовая торговля пищевыми продуктами, включая  напитки, и табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.39.0",
                        Name =
                            "Неспециализированная оптовая торговля пищевыми продуктами, включая  напитки, и табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.4",
                        Name = "Оптовая торговля непродовольственными товарами потребительского назначения",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.41",
                        Name = "Оптовая торговля текстильными товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.41.0",
                        Name = "Оптовая торговля текстильными товарами",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.42", Name = "Оптовая торговля одеждой и обувью", Section = "G"},
                    new ActivityCategory {Code = "46.42.1", Name = "Оптовая торговля одеждой", Section = "G"},
                    new ActivityCategory {Code = "46.42.9", Name = "Оптовая торговля обувью", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.43",
                        Name = "Оптовая торговля бытовыми электротоварами, радио- и телеаппаратурой",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.43.1",
                        Name = "Оптовая торговля бытовой радио- и телеаппаратурой",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.43.2",
                        Name =
                            "Оптовая торговля грампластинками, магнитными лентами, компакт-дисками, видеокассетами, DVD-дисками записанными",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.43.9",
                        Name = "Оптовая торговля прочими бытовыми электротоварами, кроме осветительного оборудования",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.44",
                        Name = "Оптовая торговля изделиями из фарфора и стекла, обоями и чистящими средствами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.44.0",
                        Name = "Оптовая торговля изделиями из фарфора и стекла, обоями и чистящими средствами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.45",
                        Name = "Оптовая торговля парфюмерными и косметическими товарами (включая мыло)",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.45.0",
                        Name = "Оптовая торговля парфюмерными и косметическими товарами (включая мыло)",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.46",
                        Name = "Оптовая торговля фармацевтическими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.46.0",
                        Name = "Оптовая торговля фармацевтическими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.47",
                        Name = "Оптовая торговля бытовой мебелью, коврами и осветительными приборами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.47.1",
                        Name = "Оптовая торговля бытовой мебелью и коврами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.47.9",
                        Name = "Оптовая торговля осветительными приборами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.48",
                        Name = "Оптовая торговля часами и ювелирными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.48.0",
                        Name = "Оптовая торговля часами и ювелирными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.49",
                        Name = "Оптовая торговля прочими непродовольственными товарами потребительского назначения",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.49.0",
                        Name = "Оптовая торговля прочими непродовольственными товарами потребительского назначения",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.5",
                        Name = "Оптовая торговля компьютерами и телекоммуникационным оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.51",
                        Name =
                            "Оптовая торговля компьютерами (ЭВМ), периферийными устройствами и программным обеспечением",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.51.0",
                        Name =
                            "Оптовая торговля компьютерами (ЭВМ), периферийными устройствами и программным обеспечением",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.52",
                        Name = "Оптовая торговля компонентами электронного и телекоммуникационного оборудования",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.52.1",
                        Name = "Оптовая торговля компонентами электронного и телекоммуникационного оборудования",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.52.9",
                        Name =
                            "Оптовая торговля грампластинками, магнитными лентами, компакт-дисками, видеокассетами, DVD-дисками чистыми",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.6",
                        Name = "Оптовая торговля машинами и оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.61",
                        Name = "Оптовая торговля сельскохозяйственными машинами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.61.0",
                        Name = "Оптовая торговля сельскохозяйственными машинами",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.62", Name = "Оптовая торговля станками", Section = "G"},
                    new ActivityCategory {Code = "46.62.0", Name = "Оптовая торговля станками", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.63",
                        Name =
                            "Оптовая торговля машинами и оборудованием для горнодобывающей промышленности, строительства, в том числе гражданского строительства",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.63.0",
                        Name =
                            "Оптовая торговля машинами и оборудованием для горнодобывающей промышленности, строительства, в том числе гражданского строительства",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.64",
                        Name =
                            "Оптовая торговля машинами для текстильной промышленности, швейными и вязальными машинами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.64.0",
                        Name =
                            "Оптовая торговля машинами для текстильной промышленности, швейными и вязальными машинами",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.65", Name = "Оптовая торговля офисной мебелью", Section = "G"},
                    new ActivityCategory {Code = "46.65.0", Name = "Оптовая торговля офисной мебелью", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.66",
                        Name = "Оптовая торговля офисными машинами и оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.66.0",
                        Name = "Оптовая торговля офисными машинами и оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.69",
                        Name = "Оптовая торговля прочими машинами и оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.69.0",
                        Name = "Оптовая торговля прочими машинами и оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.7", Name = "Оптовая торговля прочими товарами", Section = "G"},
                    new ActivityCategory {Code = "46.71", Name = "Оптовая торговля топливом", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.71.1",
                        Name = "Оптовая торговля сырой нефтью и попутным газом",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.71.2",
                        Name = "Оптовая торговля природным (горючим) газом",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.71.3",
                        Name = "Оптовая торговля каменным углем и лигнитом (бурым углем)",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.71.4",
                        Name = "Оптовая торговля авиационным и автомобильным топливом",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.71.9",
                        Name = "Оптовая торговля прочим топливом, смазочными материалами, техническими маслами и т.п.",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.72",
                        Name = "Оптовая торговля металлами и металлическими  рудами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.72.1",
                        Name = "Оптовая торговля рудами черных и цветных металлов",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.72.2",
                        Name = "Оптовая торговля чугуном, сталью и отливками из них",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.72.3",
                        Name = "Оптовая торговля цветными металлами и отливками из них",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.72.4",
                        Name = "Оптовая торговля драгоценными металлами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.72.9",
                        Name = "Оптовая торговля прочими металлами и отливками из них",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.73",
                        Name =
                            "Оптовая торговля  лесоматериалами, строительными  материалами и санитарно- техническим оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.73.1", Name = "Оптовая торговля лесоматериалами", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.73.2",
                        Name = "Оптовая торговля обоями и напольными покрытиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.73.9",
                        Name =
                            "Оптовая торговля прочими строительными материалами и санитарно-техническим оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.74",
                        Name = "Оптовая торговля скобяными изделиями, водопроводным и отопительным оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.74.0",
                        Name = "Оптовая торговля скобяными изделиями, водопроводным и отопительным оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.75",
                        Name = "Оптовая торговля химическими продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.75.0",
                        Name = "Оптовая торговля химическими продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.76",
                        Name = "Оптовая торговля прочими промежуточными продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.76.0",
                        Name = "Оптовая торговля прочими промежуточными продуктами",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.77", Name = "Оптовая торговля отходами и ломом", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.77.1",
                        Name = "Оптовая торговля отходами и ломом черных и цветных металлов",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.77.2",
                        Name = "Оптовая торговля отходами и ломом драгоценных металлов и драгоценных камней",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.77.9",
                        Name = "Оптовая торговля прочими неметаллическими отходами  и неметаллическим ломом",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "46.9", Name = "Оптовая неспециализированная торговля", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.90",
                        Name = "Оптовая неспециализированная торговля",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.90.0",
                        Name = "Оптовая неспециализированная торговля",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47",
                        Name = "Розничная торговля, кроме торговли автомобилями и мотоциклами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.1",
                        Name = "Розничная торговля в неспециализированных магазинах",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.11",
                        Name =
                            "Розничная торговля в неспециализированных магазинах преимущественно  пищевыми продуктами, включая  напитки, и табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.11.0",
                        Name =
                            "Розничная торговля в неспециализированных магазинах преимущественно пищевыми продуктами, включая напитки, и табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.19",
                        Name = "Розничная торговля в неспециализированных магазинах прочими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.19.0",
                        Name = "Розничная торговля в неспециализированных магазинах прочими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.2",
                        Name =
                            "Розничная торговля в специализированных магазинах пищевыми продуктами, включая напитки, и табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.21",
                        Name = "Розничная торговля в специализированных магазинах фруктами и овощами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.21.0",
                        Name = "Розничная торговля в специализированных магазинах фруктами и овощами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.22",
                        Name = "Розничная торговля в специализированных магазинах мясом и мясными продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.22.1",
                        Name =
                            "Розничная торговля в специализированных магазинах домашней птицей, дичью и изделиями из них",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.22.9",
                        Name = "Розничная торговля в специализированных магазинах мясом и мясными продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.23",
                        Name = "Розничная торговля в специализированных магазинах рыбой, ракообразными и моллюсками",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.23.0",
                        Name = "Розничная торговля в специализированных магазинах рыбой, ракообразными и моллюсками",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.24",
                        Name =
                            "Розничная торговля  в специализированных магазинах хлебобулочными изделиями, мучными и кондитерскими изделиями из сахара",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.24.0",
                        Name =
                            "Розничная торговля  в специализированных магазинах хлебобулочными изделиями, мучными и кондитерскими изделиями из сахара",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.25",
                        Name = "Розничная торговля в специализированных магазинах алкогольными и другими напитками",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.25.0",
                        Name = "Розничная торговля в специализированных магазинах алкогольными и другими напитками",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.26",
                        Name = "Розничная торговля в специализированных магазинах табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.26.0",
                        Name = "Розничная торговля в специализированных магазинах табачными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.29",
                        Name = "Розничная торговля в специализированных магазинах прочими пищевыми  продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.29.0",
                        Name = "Розничная торговля в специализированных магазинах прочими пищевыми  продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.3",
                        Name = "Розничная торговля в специализированных магазинах моторным топливом",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.30",
                        Name = "Розничная торговля в специализированных магазинах моторным топливом",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.30.0",
                        Name = "Розничная торговля в специализированных магазинах моторным топливом",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.4",
                        Name =
                            "Розничная торговля в специализированных магазинах информационным и телекоммуникационным оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.41",
                        Name =
                            "Розничная торговля в специализированных магазинах компьютерами и программным обеспечением",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.41.0",
                        Name =
                            "Розничная торговля в специализированных магазинах компьютерами и программным обеспечением",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.42",
                        Name = "Розничная торговля в специализированных магазинах телекоммуникационным оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.42.0",
                        Name = "Розничная торговля в специализированных магазинах телекоммуникационным оборудованием",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.43",
                        Name = "Розничная торговля в специализированных магазинах аудио- и видеоаппаратурой",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.43.0",
                        Name = "Розничная торговля в специализированных магазинах аудио- и видеоаппаратурой",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.5",
                        Name = "Розничная торговля в специализированных магазинах прочим оборудованием для дома",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.51",
                        Name = "Розничная торговля  в специализированных магазинах текстильными  изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.51.0",
                        Name = "Розничная торговля  в специализированных магазинах текстильными  изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.52",
                        Name =
                            "Розничная торговля в специализированных магазинах скобяными изделиями, лакокрасочными материалами и стеклом",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.52.0",
                        Name =
                            "Розничная торговля в специализированных магазинах скобяными изделиями, лакокрасочными материалами и стеклом",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.53",
                        Name =
                            "Розничная торговля в специализированных магазинах коврами, настенными напольными покрытиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.53.1",
                        Name = "Розничная торговля в специализированных магазинах коврами и ковровыми изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.53.9",
                        Name =
                            "Розничная торговля в специализированных магазинах прочими настенными и напольными покрытиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.54",
                        Name = "Розничная торговля в специализированных магазинах бытовыми электротоварами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.54.0",
                        Name = "Розничная торговля в специализированных магазинах бытовыми электротоварами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.59",
                        Name =
                            "Розничная торговля в специализированных магазинах мебелью, осветительными приборами и прочими бытовыми товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.59.1",
                        Name = "Розничная торговля в специализированных магазинах мебелью",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.59.9",
                        Name =
                            "Розничная торговля в специализированных магазинах осветительными приборами и прочими бытовыми товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.6",
                        Name =
                            "Розничная торговля в специализированных магазинах товарами для культурных развлечений, спорта и отдыха",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.61",
                        Name = "Розничная торговля в специализированных магазинах книгами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.61.0",
                        Name = "Розничная торговля в специализированных магазинах книгами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.62",
                        Name = "Розничная торговля в специализированных магазинах журналами и канцелярскими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.62.0",
                        Name = "Розничная торговля в специализированных магазинах журналами и канцелярскими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.63",
                        Name = "Розничная торговля в специализированных магазинах видео и музыкальными записями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.63.0",
                        Name = "Розничная торговля в специализированных магазинах видео и музыкальными записями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.64",
                        Name = "Розничная торговля в специализированных магазинах спортивными товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.64.0",
                        Name = "Розничная торговля в специализированных магазинах спортивными товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.65",
                        Name = "Розничная торговля в специализированных магазинах играми и игрушками",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.65.0",
                        Name = "Розничная торговля в специализированных магазинах играми и игрушками",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.7",
                        Name = "Розничная торговля в специализированных магазинах прочими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.71",
                        Name = "Розничная торговля в специализированных магазинах одеждой",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.71.1",
                        Name =
                            "Розничная торговля в специализированных магазинах трикотажными и чулочно-носочными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.71.9",
                        Name =
                            "Розничная торговля в специализированных магазинах одеждой (кроме трикотажных и чулочно-носочных изделий)",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.72",
                        Name = "Розничная торговля в специализированных магазинах обувью и кожаными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.72.1",
                        Name = "Розничная торговля в специализированных магазинах обувью",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.72.9",
                        Name = "Розничная торговля в специализированных магазинах кожаными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.73",
                        Name = "Розничная торговля в специализированных магазинах фармацевтическими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.73.0",
                        Name = "Розничная торговля в специализированных магазинах фармацевтическими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.74",
                        Name =
                            "Розничная торговля в специализированных магазинах медицинскими и ортопедическими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.74.0",
                        Name =
                            "Розничная торговля в специализированных магазинах медицинскими и ортопедическими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.75",
                        Name =
                            "Розничная торговля в специализированных магазинах косметическими  и парфюмерными товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.75.0",
                        Name =
                            "Розничная торговля в специализированных магазинах косметическими  и парфюмерными товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.76",
                        Name =
                            "Розничная торговля в специализированных магазинах цветами и другими растениями, семенами, удобрениями, домашними животными (питомцами) и кормом для них",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.76.0",
                        Name =
                            "Розничная торговля в специализированных магазинах цветами и другими растениями, семенами, удобрениями, домашними животными (питомцами) и кормом для них",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.77",
                        Name = "Розничная торговля в специализированных магазинах часами и ювелирными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.77.0",
                        Name = "Розничная торговля в специализированных магазинах часами и ювелирными изделиями",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.78",
                        Name = "Розничная торговля в специализированных магазинах прочими новыми товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.78.0",
                        Name = "Розничная торговля в специализированных магазинах прочими новыми товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.79",
                        Name = "Розничная торговля в магазинах товарами, бывшими в употреблении",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.79.0",
                        Name = "Розничная торговля в магазинах товарами, бывшими в употреблении",
                        Section = "G"
                    },
                    new ActivityCategory {Code = "47.8", Name = "Розничная торговля вне магазинов", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "47.81",
                        Name = "Розничная торговля в палатках и на рынках пищевыми продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.81.0",
                        Name = "Розничная торговля в палатках и на рынках пищевыми продуктами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.82",
                        Name = "Розничная торговля в палатках и на рынках текстилем, одеждой, обувью",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.82.0",
                        Name = "Розничная торговля в палатках и на рынках текстилем, одеждой, обувью",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.89",
                        Name = "Розничная торговля в палатках и на рынках прочими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.89.0",
                        Name = "Розничная торговля в палатках и на рынках прочими товарами",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.9",
                        Name = "Розничная торговля вне магазинов, палаток и рынков",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.91",
                        Name = "Розничная дистанционная торговля (по почте и через Интернет)",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.91.0",
                        Name = "Розничная дистанционная торговля (по почте и через Интернет)",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.99",
                        Name = "Прочая розничная торговля вне магазинов, палаток и рынков",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "47.99.0",
                        Name = "Прочая розничная торговля вне магазинов, палаток и рынков",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "H",
                        Name = "Транспортная деятельность и хранение грузов",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49",
                        Name = "Деятельность наземного и трубопроводного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.1",
                        Name = "Деятельность пассажирского железнодорожного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.10",
                        Name = "Деятельность пассажирского железнодорожного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.10.0",
                        Name = "Деятельность пассажирского железнодорожного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.2",
                        Name = "Деятельность грузового железнодорожного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.20",
                        Name = "Деятельность грузового железнодорожного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.20.0",
                        Name = "Деятельность грузового железнодорожного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.3",
                        Name = "Деятельность прочего наземного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.31",
                        Name = "Деятельность наземного транспорта по городским и пригородным пассажирским перевозкам",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.31.0",
                        Name = "Деятельность наземного транспорта по городским и пригородным пассажирским перевозкам",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.32",
                        Name = "Деятельность такси по перевозке пассажиров",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.32.0",
                        Name = "Деятельность такси по перевозке пассажиров",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.39",
                        Name =
                            "Деятельность прочего пассажирского наземного транспорта, не включенного в другие группировки",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.39.0",
                        Name =
                            "Деятельность прочего пассажирского наземного транспорта, не включенного в другие группировки",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.4",
                        Name = "Деятельность грузового автомобильного транспорта и предоставление услуг по переезду",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.41",
                        Name = "Деятельность грузового автомобильного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.41.0",
                        Name = "Деятельность грузового автомобильного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.42",
                        Name = "Предоставление услуг по переезду (перемещению)",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.42.0",
                        Name = "Предоставление услуг по переезду (перемещению)",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.5",
                        Name = "Деятельность трубопроводного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.50",
                        Name = "Деятельность трубопроводного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "49.50.0",
                        Name = "Деятельность трубопроводного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory {Code = "50", Name = "Деятельность водного транспорта", Section = "H"},
                    new ActivityCategory
                    {
                        Code = "50.1",
                        Name = "Деятельность пассажирского морского транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.10",
                        Name = "Деятельность пассажирского морского транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.10.0",
                        Name = "Деятельность пассажирского морского транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.2",
                        Name = "Деятельность грузового морского транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.20",
                        Name = "Деятельность грузового морского транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.20.0",
                        Name = "Деятельность грузового морского транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.3",
                        Name = "Деятельность пассажирского речного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.30",
                        Name = "Деятельность пассажирского речного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.30.0",
                        Name = "Деятельность пассажирского речного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.4",
                        Name = "Деятельность грузового речного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.40",
                        Name = "Деятельность грузового речного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "50.40.0",
                        Name = "Деятельность грузового речного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory {Code = "51", Name = "Деятельность воздушного транспорта", Section = "H"},
                    new ActivityCategory
                    {
                        Code = "51.1",
                        Name = "Деятельность пассажирского воздушного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "51.10",
                        Name = "Деятельность пассажирского воздушного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "51.10.0",
                        Name = "Деятельность пассажирского воздушного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "51.2",
                        Name = "Деятельность грузового воздушного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "51.21",
                        Name = "Деятельность грузового воздушного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "51.21.0",
                        Name = "Деятельность грузового воздушного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory {Code = "51.22", Name = "Деятельность космического транспорта", Section = "H"},
                    new ActivityCategory
                    {
                        Code = "51.22.0",
                        Name = "Деятельность космического транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "52",
                        Name = "Складирование грузов и вспомогательная транспортная деятельность",
                        Section = "H"
                    },
                    new ActivityCategory {Code = "52.1", Name = "Складирование и хранение грузов", Section = "H"},
                    new ActivityCategory {Code = "52.10", Name = "Складирование и хранение грузов", Section = "H"},
                    new ActivityCategory {Code = "52.10.0", Name = "Складирование и хранение грузов", Section = "H"},
                    new ActivityCategory
                    {
                        Code = "52.2",
                        Name = "Прочая вспомогательная транспортная деятельность",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "52.21",
                        Name = "Прочая вспомогательная деятельность наземного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "52.22",
                        Name = "Прочая вспомогательная деятельность водного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "52.22.0",
                        Name = "Прочая вспомогательная деятельность водного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "52.23",
                        Name = "Прочая вспомогательная деятельность воздушного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "52.23.0",
                        Name = "Прочая вспомогательная деятельность воздушного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory {Code = "52.24", Name = "Транспортная обработка грузов", Section = "H"},
                    new ActivityCategory {Code = "52.24.0", Name = "Транспортная обработка грузов", Section = "H"},
                    new ActivityCategory
                    {
                        Code = "52.29",
                        Name = "Прочая вспомогательная транспортная деятельность",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "52.29.0",
                        Name = "Прочая вспомогательная транспортная деятельность",
                        Section = "H"
                    },
                    new ActivityCategory {Code = "53", Name = "Почтовая и курьерская деятельность", Section = "H"},
                    new ActivityCategory
                    {
                        Code = "53.1",
                        Name = "Почтовая деятельность, осуществляемая под руководством национальных операторов",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "53.10",
                        Name = "Почтовая деятельность, осуществляемая под руководством национальных операторов",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "53.10.1",
                        Name = "Предоставление универсальных почтовых услуг",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "53.10.2",
                        Name = "Предоставление прочих почтовых услуг",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "53.2",
                        Name = "Прочая почтовая и курьерская деятельность",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "53.20",
                        Name = "Прочая почтовая и курьерская деятельность",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "53.20.0",
                        Name = "Прочая почтовая и курьерская деятельность",
                        Section = "H"
                    },
                    new ActivityCategory {Code = "I", Name = "Деятельность  гостиниц и ресторанов", Section = "I"},
                    new ActivityCategory {Code = "55", Name = "Деятельность гостиниц", Section = "I"},
                    new ActivityCategory {Code = "55.1", Name = "Предоставление услуг гостиницами", Section = "I"},
                    new ActivityCategory {Code = "55.10", Name = "Предоставление услуг гостиницами", Section = "I"},
                    new ActivityCategory {Code = "55.10.0", Name = "Предоставление услуг гостиницами", Section = "I"},
                    new ActivityCategory
                    {
                        Code = "55.2",
                        Name = "Предоставление услуг для туристического проживания",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "55.20",
                        Name = "Предоставление услуг для туристического проживания",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "55.20.0",
                        Name = "Предоставление услуг для туристического проживания",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "55.3",
                        Name = "Предоставление услуг кемпингами, в том числе стоянками для автофургонов и автоприцепов",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "55.30",
                        Name = "Предоставление услуг кемпингами, в том числе стоянками для автофургонов и автоприцепов",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "55.30.0",
                        Name = "Предоставление услуг кемпингами, в том числе стоянками для автофургонов и автоприцепов",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "55.9",
                        Name = "Предоставление услуг прочими местами для проживания",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "55.90",
                        Name = "Предоставление услуг прочими местами для проживания",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "55.90.0",
                        Name = "Предоставление услуг прочими местами для проживания",
                        Section = "I"
                    },
                    new ActivityCategory {Code = "56", Name = "Деятельность ресторанов", Section = "I"},
                    new ActivityCategory
                    {
                        Code = "56.1",
                        Name = "Деятельность ресторанов и предоставление мобильных услуг по обеспечению пищей",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "56.10",
                        Name = "Деятельность ресторанов и предоставление мобильных услуг по обеспечению пищей",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "56.10.0",
                        Name = "Деятельность ресторанов и предоставление мобильных услуг по обеспечению пищей",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "56.2",
                        Name = "Предоставление разовых и прочих услуг по обеспечению пищей",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "56.21",
                        Name = "Предоставление разовых и прочих услуг по обеспечению пищей",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "56.21.0",
                        Name = "Предоставление разовых и прочих услуг по обеспечению пищей",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "56.29",
                        Name = "Предоставление прочих услуг ресторанами",
                        Section = "I"
                    },
                    new ActivityCategory
                    {
                        Code = "56.29.0",
                        Name = "Предоставление прочих услуг ресторанами",
                        Section = "I"
                    },
                    new ActivityCategory {Code = "56.3", Name = "Предоставление услуг барами", Section = "I"},
                    new ActivityCategory {Code = "56.30", Name = "Предоставление услуг барами", Section = "I"},
                    new ActivityCategory {Code = "56.30.0", Name = "Предоставление услуг барами", Section = "I"},
                    new ActivityCategory {Code = "J", Name = "Информация и связь", Section = "J"},
                    new ActivityCategory
                    {
                        Code = "JA",
                        Name = "Издательская деятельность; видео- и звукозапись; теле- и радиовещание",
                        Section = "JA"
                    },
                    new ActivityCategory {Code = "58", Name = "Издательская деятельность", Section = "JA"},
                    new ActivityCategory
                    {
                        Code = "58.1",
                        Name = "Издание книг, периодических публикаций и прочая издательская деятельность",
                        Section = "JA"
                    },
                    new ActivityCategory {Code = "58.11", Name = "Издание книг", Section = "JA"},
                    new ActivityCategory {Code = "58.11.0", Name = "Издание книг", Section = "JA"},
                    new ActivityCategory
                    {
                        Code = "58.12",
                        Name = "Издание справочников и списков адресов",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "58.12.0",
                        Name = "Издание справочников и списков адресов",
                        Section = "JA"
                    },
                    new ActivityCategory {Code = "58.13", Name = "Издание газет", Section = "JA"},
                    new ActivityCategory {Code = "58.13.0", Name = "Издание газет", Section = "JA"},
                    new ActivityCategory
                    {
                        Code = "58.14",
                        Name = "Издание журналов и периодических публикаций",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "58.14.0",
                        Name = "Издание журналов и периодических публикаций",
                        Section = "JA"
                    },
                    new ActivityCategory {Code = "58.19", Name = "Прочая издательская деятельность", Section = "JA"},
                    new ActivityCategory {Code = "58.19.0", Name = "Прочая издательская деятельность", Section = "JA"},
                    new ActivityCategory
                    {
                        Code = "58.2",
                        Name = "Издание программного обеспечения (софта)",
                        Section = "JA"
                    },
                    new ActivityCategory {Code = "58.21", Name = "Издание компьютерных игр", Section = "JA"},
                    new ActivityCategory {Code = "58.21.0", Name = "Издание компьютерных игр", Section = "JA"},
                    new ActivityCategory
                    {
                        Code = "58.29",
                        Name = "Издание прочего программного обеспечения",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "58.29.0",
                        Name = "Издание прочего программного обеспечения",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "59",
                        Name =
                            "Производство кинофильмов, видео и телевизионных программ,  звукозапись и издание музыки",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "59.1",
                        Name = "Производство кинофильмов, видео и телевизионных программ",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "59.11",
                        Name = "Производство кинофильмов, видео и телевизионных программ",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "59.11.0",
                        Name = "Производство кинофильмов, видео и телевизионных программ",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "59.12",
                        Name = "Компоновка кинофильмов, видео и телевизионных программ",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "59.12.0",
                        Name = "Компоновка кинофильмов, видео и телевизионных программ",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "59.13",
                        Name = "Распространение кинофильмов, видео и телевизионных программ",
                        Section = "JA"
                    },
                    new ActivityCategory
                    {
                        Code = "59.13.0",
                        Name = "Распространение кинофильмов, видео и телевизионных программ",
                        Section = "JA"
                    },
                    new ActivityCategory {Code = "59.14", Name = "Показ кинофильмов", Section = "JA"},
                    new ActivityCategory {Code = "59.14.0", Name = "Показ кинофильмов", Section = "JA"},
                    new ActivityCategory {Code = "59.2", Name = "Звукозапись и издание музыки", Section = "JA"},
                    new ActivityCategory {Code = "59.20", Name = "Звукозапись и издание музыки", Section = "JA"},
                    new ActivityCategory {Code = "59.20.0", Name = "Звукозапись и издание музыки", Section = "JA"},
                    new ActivityCategory {Code = "60", Name = "Радиовещание и телевидение", Section = "JA"},
                    new ActivityCategory {Code = "60.1", Name = "Радиовещание", Section = "JA"},
                    new ActivityCategory {Code = "60.10", Name = "Радиовещание", Section = "JA"},
                    new ActivityCategory {Code = "60.10.0", Name = "Радиовещание", Section = "JA"},
                    new ActivityCategory {Code = "60.2", Name = "Телевидение", Section = "JA"},
                    new ActivityCategory {Code = "60.20", Name = "Телевидение", Section = "JA"},
                    new ActivityCategory {Code = "60.20.0", Name = "Телевидение", Section = "JA"},
                    new ActivityCategory {Code = "JB", Name = "Связь", Section = "JB"},
                    new ActivityCategory {Code = "61", Name = "Связь", Section = "JB"},
                    new ActivityCategory {Code = "61.1", Name = "Предоставление услуг проводной связи", Section = "JB"},
                    new ActivityCategory
                    {
                        Code = "61.10",
                        Name = "Предоставление услуг проводной связи",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.10.0",
                        Name = "Предоставление услуг проводной связи",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.2",
                        Name = "Предоставление услуг беспроводной связи",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.20",
                        Name = "Предоставление услуг беспроводной связи",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.20.0",
                        Name = "Предоставление услуг беспроводной связи",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.3",
                        Name = "Предоставление услуг спутниковой связи",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.30",
                        Name = "Предоставление услуг спутниковой связи",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.30.0",
                        Name = "Предоставление услуг спутниковой связи",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.9",
                        Name = "Предоставление прочих телекоммуникационных услуг",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.90",
                        Name = "Предоставление прочих телекоммуникационных услуг",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "61.90.0",
                        Name = "Предоставление прочих телекоммуникационных услуг",
                        Section = "JB"
                    },
                    new ActivityCategory
                    {
                        Code = "JC",
                        Name = "Деятельность в области вычислительной техники и информационного обслуживания",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "62",
                        Name =
                            "Разработка программного обеспечения, консультирование и прочая деятельность в области вычислительной техники",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "62.0",
                        Name =
                            "Разработка программного обеспечения, консультирование и прочая деятельность в области вычислительной техники",
                        Section = "JC"
                    },
                    new ActivityCategory {Code = "62.01", Name = "Разработка программного обеспечения", Section = "JC"},
                    new ActivityCategory
                    {
                        Code = "62.01.0",
                        Name = "Разработка программного обеспечения",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "62.02",
                        Name = "Консультирование в области вычислительной техники",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "62.02.0",
                        Name = "Консультирование в области вычислительной техники",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "62.03",
                        Name = "Управление вычислительными системами",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "62.03.0",
                        Name = "Управление вычислительными системами",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "62.09",
                        Name = "Прочая деятельность в области информационных технологий и вычислительной техники",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "62.09.0",
                        Name = "Прочая деятельность в области информационных технологий и вычислительной техники",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63",
                        Name = "Деятельность в области информационного обслуживания",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.1",
                        Name =
                            "Обработка данных, размещение прикладных программ и связанная с этим деятельность, использование Web- порталов",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.11",
                        Name = "Обработка данных, размещение прикладных программ и связанная с этим деятельность",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.11.0",
                        Name = "Обработка данных, размещение прикладных программ и связанная с этим деятельность",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.12",
                        Name = "Использование Web-порталов (Интернета)",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.12.0",
                        Name = "Использование Web-порталов (Интернета)",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.9",
                        Name = "Прочая деятельность по информационному обслуживанию",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.91",
                        Name = "Деятельность информационных агентств",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.91.0",
                        Name = "Деятельность информационных агентств",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.99",
                        Name =
                            "Прочая деятельность по информационному обслуживанию, не включенная в другие группировки",
                        Section = "JC"
                    },
                    new ActivityCategory
                    {
                        Code = "63.99.0",
                        Name =
                            "Прочая деятельность по информационному обслуживанию, не включенная в другие группировки",
                        Section = "JC"
                    },
                    new ActivityCategory {Code = "K", Name = "Финансовое посредничество и страхование", Section = "K"},
                    new ActivityCategory
                    {
                        Code = "64",
                        Name = "Финансовое посредничество, кроме услуг по страхованию и пенсионному обеспечению",
                        Section = "K"
                    },
                    new ActivityCategory {Code = "64.1", Name = "Денежное посредничество", Section = "K"},
                    new ActivityCategory {Code = "64.11", Name = "Деятельность центральных банков", Section = "K"},
                    new ActivityCategory {Code = "64.11.0", Name = "Деятельность центральных банков", Section = "K"},
                    new ActivityCategory {Code = "64.19", Name = "Прочее денежное посредничество", Section = "K"},
                    new ActivityCategory {Code = "64.2", Name = "Деятельность холдинг-компаний", Section = "K"},
                    new ActivityCategory {Code = "64.20", Name = "Деятельность холдинг-компаний", Section = "K"},
                    new ActivityCategory {Code = "64.20.0", Name = "Деятельность холдинг-компаний", Section = "K"},
                    new ActivityCategory
                    {
                        Code = "64.3",
                        Name =
                            "Деятельность траст-компаний, инвестиционных фондов и аналогичных финансовых организаций",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "64.30",
                        Name =
                            "Деятельность траст-компаний, инвестиционных фондов и аналогичных финансовых организаций",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "64.30.0",
                        Name =
                            "Деятельность траст-компаний, инвестиционных фондов и аналогичных финансовых организаций",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "64.9",
                        Name = "Прочее финансовое посредничество, кроме страхования и пенсионного обеспечения",
                        Section = "K"
                    },
                    new ActivityCategory {Code = "64.91", Name = "Финансовый лизинг", Section = "K"},
                    new ActivityCategory {Code = "64.91.0", Name = "Финансовый лизинг", Section = "K"},
                    new ActivityCategory {Code = "64.92", Name = "Предоставление кредита", Section = "K"},
                    new ActivityCategory {Code = "64.92.0", Name = "Предоставление кредита", Section = "K"},
                    new ActivityCategory
                    {
                        Code = "64.99",
                        Name = "Прочее финансовое посредничество, не включенное в другие группировки",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "64.99.0",
                        Name = "Прочее финансовое посредничество, не включенное в другие группировки",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "65",
                        Name =
                            "Страхование, перестрахование и пенсионное обеспечение, кроме обязательного социального обеспечения",
                        Section = "K"
                    },
                    new ActivityCategory {Code = "65.1", Name = "Страхование", Section = "K"},
                    new ActivityCategory {Code = "65.11", Name = "Страхование жизни", Section = "K"},
                    new ActivityCategory {Code = "65.11.0", Name = "Страхование жизни", Section = "K"},
                    new ActivityCategory
                    {
                        Code = "65.12",
                        Name = "Прочие виды страхования (кроме страхования жизни)",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "65.12.0",
                        Name = "Прочие виды страхования (кроме страхования жизни)",
                        Section = "K"
                    },
                    new ActivityCategory {Code = "65.2", Name = "Перестрахование", Section = "K"},
                    new ActivityCategory {Code = "65.20", Name = "Перестрахование", Section = "K"},
                    new ActivityCategory {Code = "65.20.0", Name = "Перестрахование", Section = "K"},
                    new ActivityCategory {Code = "65.3", Name = "Пенсионное обеспечение", Section = "K"},
                    new ActivityCategory {Code = "65.30", Name = "Пенсионное обеспечение", Section = "K"},
                    new ActivityCategory {Code = "65.30.0", Name = "Пенсионное обеспечение", Section = "K"},
                    new ActivityCategory
                    {
                        Code = "66",
                        Name = "Вспомогательная деятельность в сфере финансового посредничеств и страхования",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.1",
                        Name = "Вспомогательная деятельность в сфере финансового посредничеств и страхования",
                        Section = "K"
                    },
                    new ActivityCategory {Code = "66.11", Name = "Управление финансовыми рынками", Section = "K"},
                    new ActivityCategory {Code = "66.11.0", Name = "Управление финансовыми рынками", Section = "K"},
                    new ActivityCategory
                    {
                        Code = "66.12",
                        Name = "Биржевые операции с фондовыми ценностями",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.19",
                        Name = "Прочая вспомогательная деятельность в сфере финансового посредничества",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.19.0",
                        Name = "Прочая вспомогательная деятельность в сфере финансового посредничества",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.2",
                        Name = "Вспомогательная деятельность в сфере страхования и пенсионного обеспечения",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.21",
                        Name = "Деятельность по оценке страхового риска и ущерба",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.21.0",
                        Name = "Деятельность по оценке страхового риска и ущерба",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.22",
                        Name = "Деятельность страховых агентов и брокеров",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.22.0",
                        Name = "Деятельность страховых агентов и брокеров",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.29",
                        Name = "Прочие услуги, вспомогательные по отношению к страхованию и пенсионному обеспечению",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.29.0",
                        Name = "Прочие услуги, вспомогательные по отношению к страхованию и пенсионному обеспечению",
                        Section = "K"
                    },
                    new ActivityCategory {Code = "66.3", Name = "Управление фондами", Section = "K"},
                    new ActivityCategory {Code = "66.30", Name = "Управление фондами", Section = "K"},
                    new ActivityCategory {Code = "66.30.0", Name = "Управление фондами", Section = "K"},
                    new ActivityCategory {Code = "L", Name = "Операции с недвижимым имуществом", Section = "L"},
                    new ActivityCategory {Code = "68", Name = "Операции с недвижимым имуществом", Section = "L"},
                    new ActivityCategory
                    {
                        Code = "68.1",
                        Name = "Покупка и продажа собственного недвижимого имущества",
                        Section = "L"
                    },
                    new ActivityCategory
                    {
                        Code = "68.10",
                        Name = "Покупка и продажа собственного недвижимого имущества",
                        Section = "L"
                    },
                    new ActivityCategory
                    {
                        Code = "68.10.0",
                        Name = "Покупка и продажа собственного недвижимого имущества",
                        Section = "L"
                    },
                    new ActivityCategory
                    {
                        Code = "68.2",
                        Name = "Сдача внаем собственного недвижимого имущества",
                        Section = "L"
                    },
                    new ActivityCategory
                    {
                        Code = "68.20",
                        Name = "Сдача внаем собственного недвижимого имущества",
                        Section = "L"
                    },
                    new ActivityCategory
                    {
                        Code = "68.20.0",
                        Name = "Сдача внаем собственного недвижимого имущества",
                        Section = "L"
                    },
                    new ActivityCategory
                    {
                        Code = "68.3",
                        Name = "Операции с недвижимым имуществом за вознаграждение или на договорной основе",
                        Section = "L"
                    },
                    new ActivityCategory
                    {
                        Code = "68.31",
                        Name = "Деятельность агентств по операциям с недвижимым имуществом",
                        Section = "L"
                    },
                    new ActivityCategory
                    {
                        Code = "68.31.0",
                        Name = "Деятельность агентств по операциям с недвижимым имуществом",
                        Section = "L"
                    },
                    new ActivityCategory {Code = "68.32", Name = "Управление недвижимым имуществом", Section = "L"},
                    new ActivityCategory {Code = "68.32.0", Name = "Управление недвижимым имуществом", Section = "L"},
                    new ActivityCategory
                    {
                        Code = "M",
                        Name = "Профессиональная, научная и техническая деятельность",
                        Section = "M"
                    },
                    new ActivityCategory
                    {
                        Code = "MA",
                        Name =
                            "Деятельность в области права, бухгалтерского учета, управления, архитектуры, инженерных изысканий, технических испытаний и контроля",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "69",
                        Name = "Деятельность в области права и бухгалтерского учета",
                        Section = "MA"
                    },
                    new ActivityCategory {Code = "69.1", Name = "Деятельность в области права", Section = "MA"},
                    new ActivityCategory {Code = "69.10", Name = "Деятельность в области права", Section = "MA"},
                    new ActivityCategory {Code = "69.10.0", Name = "Деятельность в области права", Section = "MA"},
                    new ActivityCategory
                    {
                        Code = "69.2",
                        Name = "Деятельность в области бухгалтерского учета и аудита",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "69.20",
                        Name = "Деятельность в области бухгалтерского учета и аудита",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "69.20.0",
                        Name = "Деятельность в области бухгалтерского учета и аудита",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "70",
                        Name = "Деятельность центральных офисов, деятельность в области управления",
                        Section = "MA"
                    },
                    new ActivityCategory {Code = "70.1", Name = "Деятельность центральных офисов", Section = "MA"},
                    new ActivityCategory {Code = "70.10", Name = "Деятельность центральных офисов", Section = "MA"},
                    new ActivityCategory {Code = "70.10.0", Name = "Деятельность центральных офисов", Section = "MA"},
                    new ActivityCategory
                    {
                        Code = "70.2",
                        Name = "Консультирование по вопросам управления",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "70.21",
                        Name = "Консультирование по вопросам связи с общественностью",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "70.21.0",
                        Name = "Консультирование по вопросам связи с общественностью",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "70.22",
                        Name = "Консультирование по вопросам коммерческой деятельности и управления",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "70.22.0",
                        Name = "Консультирование по вопросам коммерческой деятельности и управления",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "71",
                        Name =
                            "Деятельность в области архитектуры и инженерных изысканий;  технические испытания и контроль",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "71.1",
                        Name =
                            "Деятельность в области архитектуры, инженерных изысканий и предоставление технических консультаций в этих областях",
                        Section = "MA"
                    },
                    new ActivityCategory {Code = "71.11", Name = "Деятельность в области архитектуры", Section = "MA"},
                    new ActivityCategory
                    {
                        Code = "71.11.0",
                        Name = "Деятельность в области архитектуры",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "71.12",
                        Name =
                            "Деятельность в области инженерных изысканий и предоставление технических консультаций в этих областях",
                        Section = "MA"
                    },
                    new ActivityCategory
                    {
                        Code = "71.12.0",
                        Name =
                            "Деятельность в области инженерных изысканий и предоставление технических консультаций в этих областях",
                        Section = "MA"
                    },
                    new ActivityCategory {Code = "71.2", Name = "Технические испытания и контроль", Section = "MA"},
                    new ActivityCategory {Code = "71.20", Name = "Технические испытания и контроль", Section = "MA"},
                    new ActivityCategory {Code = "71.20.0", Name = "Технические испытания и контроль", Section = "MA"},
                    new ActivityCategory {Code = "MB", Name = "Научные исследования и разработки", Section = "MB"},
                    new ActivityCategory {Code = "72", Name = "Научные исследования и разработки", Section = "MB"},
                    new ActivityCategory
                    {
                        Code = "72.1",
                        Name = "Исследования и разработки в области естественных и технических наук",
                        Section = "MB"
                    },
                    new ActivityCategory
                    {
                        Code = "72.11",
                        Name = "Исследования и разработки в области биотехнологии",
                        Section = "MB"
                    },
                    new ActivityCategory
                    {
                        Code = "72.11.0",
                        Name = "Исследования и разработки в области биотехнологии",
                        Section = "MB"
                    },
                    new ActivityCategory
                    {
                        Code = "72.19",
                        Name =
                            "Исследования и разработки в области естественных и технических наук, кроме биотехнологии",
                        Section = "MB"
                    },
                    new ActivityCategory
                    {
                        Code = "72.19.0",
                        Name =
                            "Исследования и разработки в области естественных и технических наук, кроме биотехнологии",
                        Section = "MB"
                    },
                    new ActivityCategory
                    {
                        Code = "72.2",
                        Name = "Исследования и разработки в области общественных и гуманитарных наук",
                        Section = "MB"
                    },
                    new ActivityCategory
                    {
                        Code = "72.20",
                        Name = "Исследования и разработки в области общественных и гуманитарных наук",
                        Section = "MB"
                    },
                    new ActivityCategory
                    {
                        Code = "72.20.0",
                        Name = "Исследования и разработки в области общественных и гуманитарных наук",
                        Section = "MB"
                    },
                    new ActivityCategory
                    {
                        Code = "MC",
                        Name = "Прочая профессиональная, научная и техническая деятельность",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "73",
                        Name = "Рекламная деятельность и изучение рынка",
                        Section = "MC"
                    },
                    new ActivityCategory {Code = "73.1", Name = "Рекламная деятельность", Section = "MC"},
                    new ActivityCategory {Code = "73.11", Name = "Деятельность рекламных агентств", Section = "MC"},
                    new ActivityCategory {Code = "73.11.0", Name = "Деятельность рекламных агентств", Section = "MC"},
                    new ActivityCategory
                    {
                        Code = "73.12",
                        Name = "Деятельность СМИ по предоставлению рекламных услуг",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "73.12.0",
                        Name = "Деятельность СМИ по предоставлению рекламных услуг",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "73.2",
                        Name = "Исследование конъюнктуры рынка и изучение общественного мнения",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "73.20",
                        Name = "Исследование конъюнктуры рынка и изучение общественного мнения",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "73.20.0",
                        Name = "Исследование конъюнктуры рынка и изучение общественного мнения",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "74",
                        Name = "Прочая профессиональная, научная и техническая деятельность",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "74.1",
                        Name = "Специализированная дизайнерская деятельность",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "74.10",
                        Name = "Специализированная дизайнерская деятельность",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "74.10.0",
                        Name = "Специализированная дизайнерская деятельность",
                        Section = "MC"
                    },
                    new ActivityCategory {Code = "74.2", Name = "Деятельность в области фотографии", Section = "MC"},
                    new ActivityCategory {Code = "74.20", Name = "Деятельность в области фотографии", Section = "MC"},
                    new ActivityCategory {Code = "74.20.0", Name = "Деятельность в области фотографии", Section = "MC"},
                    new ActivityCategory {Code = "74.3", Name = "Письменный и устный перевод", Section = "MC"},
                    new ActivityCategory {Code = "74.30", Name = "Письменный и устный перевод", Section = "MC"},
                    new ActivityCategory {Code = "74.30.0", Name = "Письменный и устный перевод", Section = "MC"},
                    new ActivityCategory
                    {
                        Code = "74.9",
                        Name =
                            "Прочая профессиональная, научная и техническая деятельность, не включенная в другие группировки",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "74.90",
                        Name =
                            "Прочая профессиональная, научная и техническая деятельность, не включенная в другие группировки",
                        Section = "MC"
                    },
                    new ActivityCategory
                    {
                        Code = "74.90.0",
                        Name =
                            "Прочая профессиональная, научная и техническая деятельность, не включенная в другие группировки",
                        Section = "MC"
                    },
                    new ActivityCategory {Code = "75", Name = "Ветеринарная деятельность", Section = "MC"},
                    new ActivityCategory {Code = "75.0", Name = "Ветеринарная деятельность", Section = "MC"},
                    new ActivityCategory {Code = "75.00", Name = "Ветеринарная деятельность", Section = "MC"},
                    new ActivityCategory {Code = "75.00.0", Name = "Ветеринарная деятельность", Section = "MC"},
                    new ActivityCategory
                    {
                        Code = "N",
                        Name = "Административная и вспомогательная деятельность",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "77", Name = "Аренда и лизинг", Section = "N"},
                    new ActivityCategory
                    {
                        Code = "77.1",
                        Name = "Аренда и лизинг пассажирских автомобилей и легких автофургонов",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.11",
                        Name = "Аренда и лизинг пассажирских автомобилей и легких автофургонов",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "77.11.0", Name = "Аренда и лизинг автомобилей", Section = "N"},
                    new ActivityCategory {Code = "77.12", Name = "Аренда и лизинг прочих автомобилей", Section = "N"},
                    new ActivityCategory {Code = "77.12.0", Name = "Аренда и лизинг прочих автомобилей", Section = "N"},
                    new ActivityCategory
                    {
                        Code = "77.2",
                        Name = "Прокат бытовых изделий и предметов личного пользования",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "77.21", Name = "Прокат товаров для отдыха и спорта", Section = "N"},
                    new ActivityCategory {Code = "77.21.0", Name = "Прокат товаров для отдыха и спорта", Section = "N"},
                    new ActivityCategory {Code = "77.22", Name = "Прокат видеокассет и дисков", Section = "N"},
                    new ActivityCategory {Code = "77.22.0", Name = "Прокат видеокассет и дисков", Section = "N"},
                    new ActivityCategory
                    {
                        Code = "77.29",
                        Name = "Прокат прочих бытовых изделий и предметов личного пользования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.29.0",
                        Name = "Прокат прочих бытовых изделий и предметов личного пользования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.3",
                        Name = "Аренда прочих машин и оборудования и прочих предметов",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.31",
                        Name = "Аренда сельскохозяйственных машин и оборудования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.31.0",
                        Name = "Аренда сельскохозяйственных машин и оборудования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.32",
                        Name = "Аренда строительных машин и оборудования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.32.0",
                        Name = "Аренда строительных машин и оборудования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.33",
                        Name = "Аренда офисных машин и оборудования, включая вычислительную технику",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.33.0",
                        Name = "Аренда офисных машин и оборудования, включая вычислительную технику",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.34",
                        Name = "Аренда водных транспортных средств и оборудования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.34.0",
                        Name = "Аренда водных транспортных средств и оборудования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.35",
                        Name = "Аренда воздушных транспортных средств и оборудования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.35.0",
                        Name = "Аренда воздушных транспортных средств и оборудования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.39",
                        Name =
                            "Аренда прочих машин и оборудования и прочих предметов, не включенных в другие группировки",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.39.0",
                        Name =
                            "Аренда прочих машин и оборудования и прочих предметов, не включенных в другие группировки",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.4",
                        Name =
                            "Лизинг интеллектуальной собственности и аналогичных продуктов, кроме работ (произведений), защищенных авторским правом",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.40",
                        Name =
                            "Лизинг интеллектуальной собственности и аналогичных продуктов, кроме работ (произведений), защищенных авторским правом",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "77.40.0",
                        Name =
                            "Лизинг интеллектуальной собственности и аналогичных продуктов, кроме работ (произведений), защищенных авторским правом",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "78", Name = "Деятельность в области занятости", Section = "N"},
                    new ActivityCategory
                    {
                        Code = "78.1",
                        Name = "Деятельность агентств по трудоустройству",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "78.10",
                        Name = "Деятельность агентств по трудоустройству",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "78.10.0",
                        Name = "Деятельность агентств по трудоустройству",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "78.2",
                        Name = "Деятельность по обеспечению временной рабочей силой",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "78.20",
                        Name = "Деятельность по обеспечению временной рабочей силой",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "78.20.0",
                        Name = "Деятельность по обеспечению временной рабочей силой",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "78.3",
                        Name = "Деятельность по обеспечению прочими трудовыми ресурсами (персоналом)",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "78.30",
                        Name = "Деятельность по обеспечению прочими трудовыми ресурсами (персоналом)",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "78.30.0",
                        Name = "Деятельность по обеспечению прочими трудовыми ресурсами (персоналом)",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "79",
                        Name =
                            "Деятельность туристических агентств и туроператоров, бронирование и прочая деятельность в области туризма",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "79.1",
                        Name = "Деятельность туристических агентств и туроператоров",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "79.11", Name = "Деятельность туристических агентств", Section = "N"},
                    new ActivityCategory {Code = "79.12", Name = "Деятельность туроператоров", Section = "N"},
                    new ActivityCategory {Code = "79.12.0", Name = "Деятельность туроператоров", Section = "N"},
                    new ActivityCategory
                    {
                        Code = "79.9",
                        Name = "Бронирование и прочая деятельность в области туризма",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "79.90",
                        Name = "Бронирование и прочая деятельность в области туризма",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "79.90.0",
                        Name = "Бронирование и прочая деятельность в области туризма",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "80",
                        Name = "Проведение расследований и обеспечение безопасности",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "80.1",
                        Name = "Деятельность частных охранников и частных охранных бюро",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "80.10",
                        Name = "Деятельность частных охранников и частных охранных бюро",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "80.10.0",
                        Name = "Деятельность частных охранников и частных охранных бюро",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "80.2",
                        Name = "Обеспечение функционирования систем безопасности",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "80.20",
                        Name = "Обеспечение функционирования систем безопасности",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "80.20.0",
                        Name = "Обеспечение функционирования систем безопасности",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "80.3", Name = "Проведение расследований", Section = "N"},
                    new ActivityCategory {Code = "80.30", Name = "Проведение расследований", Section = "N"},
                    new ActivityCategory {Code = "80.30.0", Name = "Проведение расследований", Section = "N"},
                    new ActivityCategory
                    {
                        Code = "81",
                        Name = "Деятельность по обслуживанию зданий и изменению ландшафта",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "81.1",
                        Name = "Комплексная деятельность вспомогательно-технических служб",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "81.10",
                        Name = "Комплексная деятельность вспомогательно-технических служб",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "81.10.0",
                        Name = "Комплексная деятельность вспомогательно-технических служб",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "81.2", Name = "Деятельность по уборке", Section = "N"},
                    new ActivityCategory
                    {
                        Code = "81.21",
                        Name = "Общая уборка помещений в зданиях всех типов",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "81.21.0",
                        Name = "Общая уборка помещений в зданиях всех типов",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "81.22",
                        Name = "Прочая (специализированная) уборка зданий и чистка промышленного оборудования",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "81.22.0",
                        Name = "Прочая (специализированная) уборка зданий и чистка промышленного оборудования",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "81.29", Name = "Прочая деятельность по уборке", Section = "N"},
                    new ActivityCategory {Code = "81.29.0", Name = "Прочая деятельность по уборке", Section = "N"},
                    new ActivityCategory {Code = "81.3", Name = "Изменение ландшафта", Section = "N"},
                    new ActivityCategory {Code = "81.30", Name = "Изменение ландшафта", Section = "N"},
                    new ActivityCategory {Code = "81.30.0", Name = "Изменение ландшафта", Section = "N"},
                    new ActivityCategory
                    {
                        Code = "82",
                        Name =
                            "Административная и прочая дополнительная деятельность, направленная на поддержание бизнеса",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.1",
                        Name =
                            "Административная и прочая дополнительная деятельность, направленная на поддержание бизнеса",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.11",
                        Name = "Комплексная деятельность административных служб предприятий, организаций",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.11.0",
                        Name = "Комплексная деятельность административных служб предприятий, организаций",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.19",
                        Name =
                            "Фотокопирование, подготовка документов и прочая специальная дополнительная деятельность",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.19.0",
                        Name =
                            "Фотокопирование, подготовка документов и прочая специальная дополнительная деятельность",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.2",
                        Name = "Деятельность телефонных справочных центров",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.20",
                        Name = "Деятельность телефонных справочных центров",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.20.0",
                        Name = "Деятельность телефонных справочных центров",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.3",
                        Name = "Организация профессиональных салонов и конгрессов",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.30",
                        Name = "Организация профессиональных салонов и конгрессов",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.30.0",
                        Name = "Организация профессиональных салонов и конгрессов",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.9",
                        Name =
                            "Дополнительная деятельность, направленная на поддержание бизнеса, не включенная в другие группировки",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.91",
                        Name = "Деятельность агентств по сбору платежей и бюро по отчету о кредитных операциях",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.91.0",
                        Name = "Деятельность агентств по сбору платежей и бюро по отчету о кредитных операциях",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "82.92", Name = "Упаковывание", Section = "N"},
                    new ActivityCategory {Code = "82.92.0", Name = "Упаковывание", Section = "N"},
                    new ActivityCategory
                    {
                        Code = "82.99",
                        Name =
                            "Прочая дополнительная деятельность, направленная на поддержание бизнеса, не включенная в другие группировки",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "82.99.0",
                        Name =
                            "Прочая дополнительная деятельность, направленная на поддержание бизнеса, не включенная в другие группировки",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "O",
                        Name = "Государственное управление и оборона; обязательное социальное обеспечение",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84",
                        Name = "Государственное управление и оборона; обязательное социальное обеспечение",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.1",
                        Name = "Государственное управление общего характера; социально-экономическое управление",
                        Section = "O"
                    },
                    new ActivityCategory {Code = "84.12", Name = "Управление социальными программами", Section = "O"},
                    new ActivityCategory {Code = "84.12.0", Name = "Управление социальными программами", Section = "O"},
                    new ActivityCategory
                    {
                        Code = "84.13",
                        Name = "Регулирование и содействие эффективному ведению экономической деятельности",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.13.0",
                        Name = "Регулирование и содействие эффективному ведению экономической деятельности",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.2",
                        Name = "Предоставление государством услуг обществу в целом",
                        Section = "O"
                    },
                    new ActivityCategory {Code = "84.21", Name = "Международная деятельность", Section = "O"},
                    new ActivityCategory {Code = "84.21.0", Name = "Международная деятельность", Section = "O"},
                    new ActivityCategory {Code = "84.22", Name = "Оборонная деятельность", Section = "O"},
                    new ActivityCategory {Code = "84.22.0", Name = "Оборонная деятельность", Section = "O"},
                    new ActivityCategory
                    {
                        Code = "84.23",
                        Name = "Деятельность в области юстиции и правосудия",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.23.0",
                        Name = "Деятельность в области юстиции и правосудия",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.24",
                        Name = "Обеспечение общественного порядка и безопасности",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.24.0",
                        Name = "Обеспечение общественного порядка и безопасности",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.25",
                        Name = "Обеспечение безопасности в чрезвычайных ситуациях",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.25.0",
                        Name = "Обеспечение безопасности в чрезвычайных ситуациях",
                        Section = "O"
                    },
                    new ActivityCategory {Code = "84.3", Name = "Обязательное социальное страхование", Section = "O"},
                    new ActivityCategory {Code = "84.30", Name = "Обязательное социальное страхование", Section = "O"},
                    new ActivityCategory
                    {
                        Code = "84.30.0",
                        Name = "Обязательное социальное страхование",
                        Section = "O"
                    },
                    new ActivityCategory {Code = "P", Name = "Образование", Section = "P"},
                    new ActivityCategory {Code = "85", Name = "Образование", Section = "P"},
                    new ActivityCategory {Code = "85.1", Name = "Дошкольное образование", Section = "P"},
                    new ActivityCategory {Code = "85.10", Name = "Дошкольное образование", Section = "P"},
                    new ActivityCategory {Code = "85.10.0", Name = "Дошкольное образование", Section = "P"},
                    new ActivityCategory {Code = "85.2", Name = "Начальное образование", Section = "P"},
                    new ActivityCategory {Code = "85.20", Name = "Начальное образование", Section = "P"},
                    new ActivityCategory {Code = "85.20.0", Name = "Начальное образование", Section = "P"},
                    new ActivityCategory {Code = "85.3", Name = "Среднее образование", Section = "P"},
                    new ActivityCategory
                    {
                        Code = "85.31",
                        Name = "Общее среднее образование (вторая ступень)",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.31.0",
                        Name = "Общее среднее образование (вторая ступень)",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.32",
                        Name = "Техническое и профессиональное среднее образование",
                        Section = "P"
                    },
                    new ActivityCategory {Code = "85.4", Name = "Высшее образование", Section = "P"},
                    new ActivityCategory {Code = "85.41", Name = "Высшее образование (неполное)", Section = "P"},
                    new ActivityCategory {Code = "85.41.0", Name = "Высшее образование (неполное)", Section = "P"},
                    new ActivityCategory {Code = "85.42", Name = "Высшее образование", Section = "P"},
                    new ActivityCategory {Code = "85.42.0", Name = "Высшее образование", Section = "P"},
                    new ActivityCategory {Code = "85.5", Name = "Прочая образовательная деятельность", Section = "P"},
                    new ActivityCategory
                    {
                        Code = "85.51",
                        Name = "Образовательная деятельность в области физической культуры и спорта",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.51.0",
                        Name = "Образовательная деятельность в области физической культуры и спорта",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.52",
                        Name = "Образовательная деятельность в области культуры",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.52.0",
                        Name = "Образовательная деятельность в области культуры",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.53",
                        Name = "Деятельность школ подготовки водителей",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.53.0",
                        Name = "Деятельность школ подготовки водителей",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.59",
                        Name = "Прочая деятельность в области образования, не включенная в другие группировки",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.59.0",
                        Name = "Прочая деятельность в области образования, не включенная в другие группировки",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.6",
                        Name = "Вспомогательная деятельность в области образования",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.60",
                        Name = "Вспомогательная деятельность в области образования",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.60.0",
                        Name = "Вспомогательная деятельность в области образования",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "Q",
                        Name = "Здравоохранение и социальное обслуживание населения",
                        Section = "Q"
                    },
                    new ActivityCategory {Code = "QA", Name = "Здравоохранение", Section = "QA"},
                    new ActivityCategory {Code = "86", Name = "Здравоохранение", Section = "QA"},
                    new ActivityCategory {Code = "86.1", Name = "Деятельность больниц", Section = "QA"},
                    new ActivityCategory {Code = "86.10", Name = "Деятельность больниц", Section = "QA"},
                    new ActivityCategory
                    {
                        Code = "86.2",
                        Name = "Деятельность в области врачебной практики  и стоматологии",
                        Section = "QA"
                    },
                    new ActivityCategory
                    {
                        Code = "86.21",
                        Name = "Деятельность в области общей врачебной практики",
                        Section = "QA"
                    },
                    new ActivityCategory
                    {
                        Code = "86.21.0",
                        Name = "Деятельность в области общей врачебной практики",
                        Section = "QA"
                    },
                    new ActivityCategory
                    {
                        Code = "86.22",
                        Name = "Деятельность в области специальной врачебной практики",
                        Section = "QA"
                    },
                    new ActivityCategory
                    {
                        Code = "86.22.0",
                        Name = "Деятельность в области специальной врачебной практики",
                        Section = "QA"
                    },
                    new ActivityCategory {Code = "86.23", Name = "Деятельность в области стоматологии", Section = "QA"},
                    new ActivityCategory
                    {
                        Code = "86.23.0",
                        Name = "Деятельность в области стоматологии",
                        Section = "QA"
                    },
                    new ActivityCategory
                    {
                        Code = "86.9",
                        Name = "Прочая деятельность в области здравоохранения",
                        Section = "QA"
                    },
                    new ActivityCategory
                    {
                        Code = "86.90",
                        Name = "Прочая деятельность в области здравоохранения",
                        Section = "QA"
                    },
                    new ActivityCategory
                    {
                        Code = "86.90.0",
                        Name = "Прочая деятельность в области здравоохранения",
                        Section = "QA"
                    },
                    new ActivityCategory {Code = "QB", Name = "Социальное обслуживание населения", Section = "QB"},
                    new ActivityCategory
                    {
                        Code = "87",
                        Name = "Социальное обслуживание населения с обеспечением проживания",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.1",
                        Name =
                            "Социальное обслуживание с обеспечением проживания и ухода за пациентами средним медицинским персоналом",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.10",
                        Name =
                            "Социальное обслуживание с обеспечением проживания и ухода за пациентами средним медицинским персоналом",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.10.0",
                        Name =
                            "Социальное обслуживание с обеспечением проживания и ухода за пациентами средним медицинским персоналом",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.2",
                        Name =
                            "Социальное обслуживание с обеспечением проживания лиц с задержкой умственного развития,  лиц злоупотребляющих алкоголем или наркотиками, с психическими отклонениями или ограниченными возможностями здоровья (физическими недостатками)",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.20",
                        Name =
                            "Социальное обслуживание с обеспечением проживания лиц с задержкой умственного развития,  лиц злоупотребляющих алкоголем или наркоти-ками, с психическими отклонениями или ограниченными возможностями здоровья (физическими недостатками)",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.20.0",
                        Name =
                            "Социальное обслуживание с обеспечением проживания лиц с задержкой умственного развития,  лиц злоупотребляющих алкоголем или наркоти-ками, с психическими отклонениями или ограниченными возможностями здоровья (физическими недостатками)",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.3",
                        Name = "Социальное обслуживание с обеспечением проживания пожилых и недееспособных",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.30",
                        Name = "Социальное обслуживание с обеспечением проживания пожилых и недееспособных",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.30.0",
                        Name = "Социальное обслуживание с обеспечением проживания пожилых и недееспособных",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.9",
                        Name = "Прочее социальное обслуживание с обеспечением проживания",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.90",
                        Name = "Прочее социальное обслуживание с обеспечением проживания",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "87.90.0",
                        Name = "Прочее социальное обслуживание с обеспечением проживания",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "88",
                        Name = "Социальное обслуживание без обеспечением проживания",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "88.1",
                        Name = "Социальное обслуживание без обеспечением проживания пожилых и недееспособных",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "88.10",
                        Name = "Социальное обслуживание без обеспечения проживания пожилых и недееспособных",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "88.10.0",
                        Name = "Социальное обслуживание без обеспечения проживания пожилых и недееспособных",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "88.9",
                        Name = "Прочее социальное обслуживание без обеспечения проживания",
                        Section = "QB"
                    },
                    new ActivityCategory {Code = "88.91", Name = "Дневной уход за детьми", Section = "QB"},
                    new ActivityCategory {Code = "88.91.0", Name = "Дневной уход за детьми", Section = "QB"},
                    new ActivityCategory
                    {
                        Code = "88.99",
                        Name =
                            "Прочее социальное обслуживание без обеспечения проживания, не включенное в другие группировки",
                        Section = "QB"
                    },
                    new ActivityCategory
                    {
                        Code = "88.99.0",
                        Name =
                            "Прочее социальное обслуживание без обеспечения проживания, не включенное в другие группировки",
                        Section = "QB"
                    },
                    new ActivityCategory {Code = "R", Name = "Искусство, развлечения и отдых", Section = "R"},
                    new ActivityCategory
                    {
                        Code = "90",
                        Name =
                            "Артистическая и прочая деятельность в области искусства и проведения культурно-массовых развлекательных мероприятий",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "90.0",
                        Name =
                            "Артистическая и прочая деятельность в области искусства и проведения культурно-массовых развлекательных мероприятий",
                        Section = "R"
                    },
                    new ActivityCategory {Code = "90.01", Name = "Артистическая деятельность", Section = "R"},
                    new ActivityCategory {Code = "90.01.0", Name = "Артистическая деятельность", Section = "R"},
                    new ActivityCategory
                    {
                        Code = "90.02",
                        Name = "Вспомогательная деятельность в области артистического искусства",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "90.02.0",
                        Name = "Вспомогательная деятельность в области артистического искусства",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "90.03",
                        Name = "Деятельность в области прочих видов искусства",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "90.03.0",
                        Name = "Деятельность в области прочих видов искусства",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "90.04",
                        Name = "Деятельность театральных и концертных залов",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "90.04.0",
                        Name = "Деятельность театральных и концертных залов",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "91",
                        Name = "Деятельность библиотек, архивов, музеев и прочих учреждений культуры",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "91.0",
                        Name = "Деятельность библиотек, архивов, музеев и прочих учреждений культуры",
                        Section = "R"
                    },
                    new ActivityCategory {Code = "91.01", Name = "Деятельность библиотек и архивов", Section = "R"},
                    new ActivityCategory {Code = "91.02", Name = "Деятельность музеев", Section = "R"},
                    new ActivityCategory {Code = "91.02.0", Name = "Деятельность музеев", Section = "R"},
                    new ActivityCategory
                    {
                        Code = "91.03",
                        Name = "Деятельность по сохранению и посещению исторических мест и зданий",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "91.03.0",
                        Name = "Деятельность по сохранению и посещению исторических мест и зданий",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "91.04",
                        Name = "Деятельность ботанических садов, зоопарков и заповедников",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "91.04.0",
                        Name = "Деятельность ботанических садов, зоопарков и заповедников",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "92.0",
                        Name = "Организация и проведение лотереи, а также продажу лотерейных билетов",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "92.00",
                        Name = "Организация и проведение лотереи, а также продажу лотерейных билетов",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "93",
                        Name = "Спортивная и прочая деятельность по организации отдыха и развлечений",
                        Section = "R"
                    },
                    new ActivityCategory {Code = "93.1", Name = "Спортивная деятельность", Section = "R"},
                    new ActivityCategory
                    {
                        Code = "93.11",
                        Name = "Эксплуатация и управление спортивными сооружениями",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "93.11.0",
                        Name = "Эксплуатация и управление спортивными сооружениями",
                        Section = "R"
                    },
                    new ActivityCategory {Code = "93.12", Name = "Деятельность спортивных клубов", Section = "R"},
                    new ActivityCategory {Code = "93.12.0", Name = "Деятельность спортивных клубов", Section = "R"},
                    new ActivityCategory {Code = "93.13", Name = "Деятельность фитнесс-клубов", Section = "R"},
                    new ActivityCategory {Code = "93.13.0", Name = "Деятельность фитнесс-клубов", Section = "R"},
                    new ActivityCategory {Code = "93.19", Name = "Прочая спортивная деятельность", Section = "R"},
                    new ActivityCategory {Code = "93.19.0", Name = "Прочая спортивная деятельность", Section = "R"},
                    new ActivityCategory
                    {
                        Code = "93.2",
                        Name = "Деятельность по организации отдыха и развлечений",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "93.21",
                        Name = "Деятельность  парков отдыха и развлечений",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "93.21.0",
                        Name = "Деятельность парков отдыха и развлечений",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "93.29",
                        Name = "Прочая деятельность по организации отдыха и развлечений",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "93.29.0",
                        Name = "Прочая деятельность по организации отдыха и развлечений",
                        Section = "R"
                    },
                    new ActivityCategory {Code = "S", Name = "Прочая обслуживающая деятельность", Section = "S"},
                    new ActivityCategory
                    {
                        Code = "94",
                        Name = "Деятельность общественных объединений (организаций)",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.1",
                        Name =
                            "Деятельность коммерческих, предпринимательских и профессиональных общественных организаций (ассоциаций)",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.11",
                        Name = "Деятельность коммерческих, предпринимательских общественных организаций (ассоциаций)",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.11.0",
                        Name = "Деятельность коммерческих, предпринимательских общественных организаций (ассоциаций)",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.12",
                        Name = "Деятельность профессиональных общественных организаций (ассоциаций)",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.12.0",
                        Name = "Деятельность профессиональных общественных организаций (ассоциаций)",
                        Section = "S"
                    },
                    new ActivityCategory {Code = "94.2", Name = "Деятельность профессиональных союзов", Section = "S"},
                    new ActivityCategory {Code = "94.20", Name = "Деятельность профессиональных союзов", Section = "S"},
                    new ActivityCategory
                    {
                        Code = "94.20.0",
                        Name = "Деятельность профессиональных союзов",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.9",
                        Name = "Деятельность прочих общественных объединений",
                        Section = "S"
                    },
                    new ActivityCategory {Code = "94.91", Name = "Деятельность религиозных организаций", Section = "S"},
                    new ActivityCategory
                    {
                        Code = "94.91.0",
                        Name = "Деятельность религиозных организаций",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.92",
                        Name = "Деятельность политических организаций",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.92.0",
                        Name = "Деятельность политических организаций",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.99",
                        Name = "Деятельность прочих общественных организаций, не включенных в    другие группировки",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95",
                        Name = "Ремонт компьютеров, предметов личного пользования и бытовых  товаров",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95.1",
                        Name = "Ремонт компьютеров  и оборудования связи",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95.11",
                        Name = "Ремонт компьютеров и периферийного оборудования",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95.11.0",
                        Name = "Ремонт компьютеров и периферийного оборудования",
                        Section = "S"
                    },
                    new ActivityCategory {Code = "95.12", Name = "Ремонт оборудования связи", Section = "S"},
                    new ActivityCategory {Code = "95.12.0", Name = "Ремонт оборудования связи", Section = "S"},
                    new ActivityCategory
                    {
                        Code = "95.2",
                        Name = "Ремонт предметов личного пользования и бытовых товаров",
                        Section = "S"
                    },
                    new ActivityCategory {Code = "95.21", Name = "Ремонт электробытовых товаров", Section = "S"},
                    new ActivityCategory {Code = "95.21.0", Name = "Ремонт электробытовых товаров", Section = "S"},
                    new ActivityCategory
                    {
                        Code = "95.22",
                        Name = "Ремонт бытовой техники и садового оборудования",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95.22.0",
                        Name = "Ремонт бытовой техники и садового оборудования",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95.23",
                        Name = "Ремонт обуви и прочих изделий из кожи",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95.23.0",
                        Name = "Ремонт обуви и прочих изделий из кожи",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95.24",
                        Name = "Ремонт мебели и аналогичных домашних изделий",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95.24.0",
                        Name = "Ремонт мебели и аналогичных домашних изделий",
                        Section = "S"
                    },
                    new ActivityCategory {Code = "95.25", Name = "Ремонт часов и ювелирных изделий", Section = "S"},
                    new ActivityCategory {Code = "95.25.0", Name = "Ремонт часов и ювелирных изделий", Section = "S"},
                    new ActivityCategory
                    {
                        Code = "95.29",
                        Name = "Ремонт прочих предметов личного пользования и бытовых товаров",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "95.29.0",
                        Name = "Ремонт прочих предметов личного пользования и бытовых товаров",
                        Section = "S"
                    },
                    new ActivityCategory {Code = "96", Name = "Прочее индивидуальное обслуживание", Section = "S"},
                    new ActivityCategory {Code = "96.0", Name = "Прочее индивидуальное обслуживание", Section = "S"},
                    new ActivityCategory
                    {
                        Code = "96.01",
                        Name = "Стирка, химическая чистка и окрашивание текстильных и меховых изделий",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "96.01.0",
                        Name = "Стирка, химическая чистка и окрашивание текстильных и меховых изделий",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "96.02",
                        Name = "Деятельность парикмахерских и салонов красоты",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "96.02.0",
                        Name = "Деятельность парикмахерских и салонов красоты",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "96.03",
                        Name = "Организация похорон и связанная с этим деятельность",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "96.03.0",
                        Name = "Организация похорон и связанная с этим деятельность",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "96.04",
                        Name = "Деятельность по обеспечению физического комфорта",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "96.04.0",
                        Name = "Деятельность по обеспечению физического комфорта",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "96.09",
                        Name = "Прочее индивидуальное обслуживание, не включенное в другие группировки",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "96.09.0",
                        Name = "Прочее индивидуальное обслуживание, не включенное в другие группировки",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "T",
                        Name =
                            "Деятельность   частных   домашних хозяйств   с наемными работниками; производство  частными домашними хозяйствами разнообразных товаров и услуг для собственного потребления",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "97",
                        Name = "Деятельность частных домашних хозяйств с наемными работниками",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "97.0",
                        Name = "Деятельность частных домашних хозяйств с наемными работниками",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "97.00",
                        Name = "Деятельность частных домашних хозяйств с наемными работниками",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "97.00.0",
                        Name = "Деятельность частных домашних хозяйств с наемными работниками",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "98",
                        Name =
                            "Производство частными домашними хозяйствами разнообразных товаров и услуг для собственного потребления",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "98.1",
                        Name =
                            "Производство частными домашними хозяйствами разнообразных товаров и услуг для собственного потребления",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "98.10",
                        Name =
                            "Производство частными домашними хозяйствами разнообразных товаров для собственного потребления",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "98.10.0",
                        Name =
                            "Производство частными домашними хозяйствами разнообразных товаров для собственного потребления",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "98.2",
                        Name =
                            "Предоставление частными домашними хозяйствами разнообразных услуг для собственного потребления",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "98.20",
                        Name =
                            "Предоставление частными домашними хозяйствами разнообразных услуг для собственного потребления",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "98.20.0",
                        Name =
                            "Предоставление частными домашними хозяйствами разнообразных услуг для собственного потребления",
                        Section = "T"
                    },
                    new ActivityCategory
                    {
                        Code = "U",
                        Name = "Деятельность экстерриториальных организаций",
                        Section = "U"
                    },
                    new ActivityCategory
                    {
                        Code = "99",
                        Name = "Деятельность экстерриториальных организаций",
                        Section = "U"
                    },
                    new ActivityCategory
                    {
                        Code = "99.0",
                        Name = "Деятельность экстерриториальных организаций",
                        Section = "U"
                    },
                    new ActivityCategory
                    {
                        Code = "99.00",
                        Name = "Деятельность экстерриториальных организаций",
                        Section = "U"
                    },
                    new ActivityCategory
                    {
                        Code = "99.00.0",
                        Name = "Деятельность экстерриториальных организаций",
                        Section = "U"
                    },
                    new ActivityCategory
                    {
                        Code = "79.11.1",
                        Name =
                            "Деятельность туристических агентств по оптовой и розничной продаже экскурсий, путешествий, организованных туров",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "79.11.2",
                        Name = "Деятельность туристических агентств по бронированию мест на транспортных средствах",
                        Section = "N"
                    },
                    new ActivityCategory
                    {
                        Code = "79.11.9",
                        Name = "Прочая деятельность туристических агентств",
                        Section = "N"
                    },
                    new ActivityCategory {Code = "64.19.1", Name = "Деятельность банков", Section = "K"},
                    new ActivityCategory
                    {
                        Code = "64.19.2",
                        Name = "Денежное посредничество прочих финансовых учреждений",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.12.1",
                        Name = "Операции на финансовых рынков по поручению других лиц",
                        Section = "K"
                    },
                    new ActivityCategory {Code = "64.19.9", Name = "Прочее денежное посредничество", Section = "K"},
                    new ActivityCategory
                    {
                        Code = "66.12.2",
                        Name = "Деятельность по управлению ценными бумагами",
                        Section = "K"
                    },
                    new ActivityCategory
                    {
                        Code = "66.12.9",
                        Name = "Деятельность меняльных контор, пунктов обмена валюты",
                        Section = "K"
                    },
                    new ActivityCategory {Code = "86.10.1", Name = "Деятельность больниц", Section = "QA"},
                    new ActivityCategory
                    {
                        Code = "86.10.9",
                        Name = "Деятельность санаторно-курортных учреждений",
                        Section = "QA"
                    },
                    new ActivityCategory {Code = "91.01.1", Name = "Деятельность библиотек и архивов", Section = "R"},
                    new ActivityCategory
                    {
                        Code = "91.01.9",
                        Name = "Деятельность учреждений культуры клубного типа",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "94.99.1",
                        Name = "Деятельность общественных организаций (объединений) в области культуры и рекреации",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.99.2",
                        Name = "Деятельность правозащитных общественных организаций (объединений)",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.99.3",
                        Name = "Деятельность общественных организаций (объединений) в области охраны окружающей среды",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.99.4",
                        Name =
                            "Деятельность общественных организаций (объединений) в области филантропии и поощрения добровольной деятельности",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.99.5",
                        Name = "Деятельность общественных организаций (объединений) имеющих патриотические цели",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.99.6",
                        Name =
                            "Деятельность общественных организаций (объединений) в области экономического, социального и общинного развития",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "94.99.9",
                        Name = "Деятельность прочих общественных организаций, не включенных в другие группировки",
                        Section = "S"
                    },
                    new ActivityCategory
                    {
                        Code = "85.32.1",
                        Name = "Начальное профессиональное (техническое) образование",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "85.32.9",
                        Name = "Среднее профессиональное образование",
                        Section = "P"
                    },
                    new ActivityCategory
                    {
                        Code = "92.00.0",
                        Name = "Организация и проведение лотереи, а также продажу лотерейных билетов",
                        Section = "R"
                    },
                    new ActivityCategory
                    {
                        Code = "11.07.1",
                        Name = "Розлив минеральных вод по бутылкам, включая производство натуральных минеральных вод",
                        Section = "NULL"
                    },
                    new ActivityCategory
                    {
                        Code = "11.07.2",
                        Name = "Производство национальных напитков (максым, чалап, жарма,бозо)",
                        Section = "NULL"
                    },
                    new ActivityCategory
                    {
                        Code = "11.07.3",
                        Name =
                            "Производство безалкогольных напитков, ароматизированных и/или с добавками сахара (лимонад, оранжад, кола, фруктовые напитки, тоник и т.п)",
                        Section = "NULL"
                    },
                    new ActivityCategory
                    {
                        Code = "11.07.9",
                        Name = "Производство  прочих безалкогольных напитков, не включенных в другие группировки",
                        Section = "NULL"
                    },
                    new ActivityCategory
                    {
                        Code = "01.61.2",
                        Name = "Услуги по выращиванию сельскохозяйственных культур в теплицах,  парниках",
                        Section = "A"
                    },
                    new ActivityCategory {Code = "46.38.1", Name = "Оптовая торговля мукой", Section = "G"},
                    new ActivityCategory
                    {
                        Code = "46.38.2",
                        Name =
                            "Оптовая торговля кормами для собак, кошек и других домашних животных (домашних питомцев)",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "46.38.9",
                        Name =
                            "Оптовая торговля прочими пищевыми продуктами, в том числе рыбой, ракообразными и моллюсками",
                        Section = "G"
                    },
                    new ActivityCategory
                    {
                        Code = "52.21.1",
                        Name = "Продажа билетов, предварительный заказ билетов, камеры хранения багажа",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "52.21.9",
                        Name = "Прочая вспомогательная деятельность наземного транспорта",
                        Section = "H"
                    },
                    new ActivityCategory
                    {
                        Code = "84.11.1",
                        Name = "Исполнительная и законодательная деятельность центральных органов управления",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.11.2",
                        Name = "Исполнительная деятельность региональных органов управления",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.11",
                        Name = "Государственное управление общего характера",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.11.3",
                        Name = "Деятельность представительных органов местного самоуправления - местные кенеши",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.11.4",
                        Name =
                            "Деятельность представительных органов местного самоуправления - айыл окмоту, мэрии городов",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "84.11.9",
                        Name = "Государственное управление общего характера",
                        Section = "O"
                    },
                    new ActivityCategory
                    {
                        Code = "92",
                        Name = "Организация и проведение лотереи, продажа лотерейных билетов",
                        Section = "R"
                    }
                );
                context.SaveChanges();
            }

            var sysAdminUser = context.Users.FirstOrDefault(u => u.Login == "admin");
            if (sysAdminUser == null)
            {
                sysAdminUser = new User
                {
                    Login = "admin",
                    PasswordHash =
                        "AQAAAAEAACcQAAAAEF+cTdTv1Vbr9+QFQGMo6E6S5aGfoFkBnsrGZ4kK6HIhI+A9bYDLh24nKY8UL3XEmQ==",
                    SecurityStamp = "9479325a-6e63-494a-ae24-b27be29be015",
                    Name = "Admin user",
                    PhoneNumber = "555123456",
                    Email = "admin@email.xyz",
                    NormalizedEmail = "admin@email.xyz".ToUpper(),
                    Status = UserStatuses.Active,
                    Description = "System administrator account",
                    NormalizedUserName = "admin".ToUpper(),
                    DataAccessArray = daa,
                };
                context.Users.Add(sysAdminUser);
            }
            var adminUserRoleBinding = new IdentityUserRole<string>
            {
                RoleId = sysAdminRole.Id,
                UserId = sysAdminUser.Id,
            };
            context.UserRoles.Add(adminUserRoleBinding);
            var soateTmp = 741000000000000;
            if (!context.StatisticalUnits.Any())
            {
                context.StatisticalUnits.AddRange(new LocalUnit
                {
                    Name = "local unit 1",
                    UserId = sysAdminUser.Id,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address = new Address {AddressPart1 = "local address 1", GeographicalCodes = soateTmp++.ToString()}
                }, new LocalUnit
                {
                    Name = "local unit 2",
                    StatId = "OKPO2LU",
                    UserId = sysAdminUser.Id,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address = new Address {AddressPart1 = "local address 2", GeographicalCodes = soateTmp++.ToString()},
                });

                var le1 = new LegalUnit
                {
                    Name = "legal unit 1",
                    UserId = sysAdminUser.Id,
                    RegIdDate = DateTime.Now,
                    StatId = "OKPO2LEGALU",
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address = new Address
                    {
                        AddressDetails = "legal address 1",
                        GeographicalCodes = soateTmp++.ToString()
                    },
                    ActivitiesUnits = new List<ActivityStatisticalUnit>()
                    {
                        new ActivityStatisticalUnit()
                        {
                            Activity = new Activity()
                            {
                                IdDate = new DateTime(2017, 03, 17),
                                Turnover = 2000,
                                ActivityType = ActivityTypes.Primary,
                                UpdatedByUser = sysAdminUser,
                                ActivityYear = DateTime.Today.Year,
                                ActivityRevxCategory = context.ActivityCategories.Single(v => v.Code == "11.07.9")
                            },
                        },
                        new ActivityStatisticalUnit()
                        {
                            Activity =
                                new Activity()
                                {
                                    IdDate = new DateTime(2017, 03, 28),
                                    Turnover = 4000,
                                    ActivityType = ActivityTypes.Secondary,
                                    UpdatedByUser = sysAdminUser,
                                    ActivityYear = 2006,
                                    ActivityRevxCategory = context.ActivityCategories.Single(v => v.Code == "91.01.9")
                                }
                        }
                    }
                };

                context.StatisticalUnits.AddRange(le1, new LegalUnit
                {
                    Name = "legal unit 2",
                    UserId = sysAdminUser.Id,
                    IsDeleted = true,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address = new Address
                    {
                        AddressDetails = "legal address 2",
                        GeographicalCodes = soateTmp++.ToString()
                    }
                });

                var eu1 = new EnterpriseUnit
                {
                    Name = "enterprise unit 1",
                    StatId = "OKPO1EU",
                    UserId = sysAdminUser.Id,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                };

                var eu2 = new EnterpriseUnit
                {
                    Name = "enterprise unit 2",
                    StatId = "OKPO2EU",
                    UserId = sysAdminUser.Id,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address = new Address
                    {
                        AddressDetails = "enterprise address 2",
                        GeographicalCodes = soateTmp++.ToString()
                    }
                };
                
                
                context.EnterpriseUnits.AddRange(eu1, eu2, new EnterpriseUnit
                {
                    Name = "enterprise unit 3",
                    StatId = "OKPO3EU",
                    UserId = sysAdminUser.Id,
                    IsDeleted = true,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address = new Address
                    {
                        AddressDetails = "enterprise address 2",
                        GeographicalCodes = soateTmp++.ToString()
                    }
                }, new EnterpriseUnit
                {
                    StatId = "OKPO4EU",
                    Name = "enterprise unit 4",
                    UserId = sysAdminUser.Id,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address = new Address
                    {
                        AddressDetails = "enterprise address 2",
                        GeographicalCodes = soateTmp++.ToString()
                    }
                }, new EnterpriseUnit
                {
                    Name = "enterprise unit 5",
                    UserId = sysAdminUser.Id,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address = new Address
                    {
                        AddressDetails = "enterprise address 2",
                        GeographicalCodes = soateTmp++.ToString()
                    }
                }, new EnterpriseUnit
                {
                    Name = "enterprise unit 6",
                    UserId = sysAdminUser.Id,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address = new Address
                    {
                        AddressDetails = "enterprise address 2",
                        GeographicalCodes = soateTmp++.ToString()
                    }
                });

                var eg1 = new EnterpriseGroup
                {
                    Name = "enterprise group 1",
                    UserId = sysAdminUser.Id,
                    StatId = "EG1",
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address =
                        new Address {AddressDetails = "ent. group address 1", GeographicalCodes = soateTmp++.ToString()}
                };

                var eg2 = new EnterpriseGroup
                {
                    Name = "enterprise group 2",
                    StatId = "EG2",
                    UserId = sysAdminUser.Id,
                    RegIdDate = DateTime.Now,
                    StartPeriod = DateTime.Now,
                    EndPeriod = DateTime.MaxValue,
                    Address =
                        new Address {AddressDetails = "ent. group address 2", GeographicalCodes = soateTmp++.ToString()}
                };

                context.EnterpriseGroups.AddRange(eg1, eg2);

                //Links:
                eu1.EnterpriseGroup = eg1;
                le1.EnterpriseGroup = eg2;
                le1.EnterpriseUnit = eu1;

            }

            if (!context.Regions.Any())
            {
                context.Regions.AddRange(
                    new Region {Name = "НСК/ГВЦ"},
                    new Region {Name = "Иссык-Кульский облстат"},
                    new Region {Name = "Джалал-Абадский облстат"},
                    new Region {Name = "Ала-Букинский райстат"},
                    new Region {Name = "Базар-Коргонский райстат"},
                    new Region {Name = "Баткенсктй облстат"},
                    new Region {Name = "Кадамжайский райстат"},
                    new Region {Name = "Нарынский облстат"},
                    new Region {Name = "Нарынский горстат"},
                    new Region {Name = "Жумгальский райстат"},
                    new Region {Name = "Ошский горстат"},
                    new Region {Name = "Бишкекский горстат"},
                    new Region {Name = "Аксуйский райстат"},
                    new Region {Name = "Жети-Огузский райстат"},
                    new Region {Name = "Иссык-Кульский райстат"},
                    new Region {Name = "Тонский райстат"},
                    new Region {Name = "Тюпский райстат"},
                    new Region {Name = "Балыкчинский горстат"},
                    new Region {Name = "Аксыйский райстат"},
                    new Region {Name = "Ноокенский райстат"},
                    new Region {Name = "Сузакский райстат"},
                    new Region {Name = "Тогуз-Тороуский райстат"},
                    new Region {Name = "Токтогульский райстат"},
                    new Region {Name = "Чаткальский райстат"},
                    new Region {Name = "Джалал-Абадский горстат"},
                    new Region {Name = "Таш-Кумырский горстат"},
                    new Region {Name = "Майлуу-Сууский горстат"},
                    new Region {Name = "Кара-Кульский горстат"},
                    new Region {Name = "Ак-Талинский райстат"},
                    new Region {Name = "Ат-Башынский райстат"},
                    new Region {Name = "Кочкорский райстат"},
                    new Region {Name = "Нарынский райстат"},
                    new Region {Name = "Баткенский райстат"},
                    new Region {Name = "Лейлекский райстат"},
                    new Region {Name = "Сулюктинский горстат"},
                    new Region {Name = "Ошский облстат"},
                    new Region {Name = "Алайский райстат"},
                    new Region {Name = "Араванский райстат"},
                    new Region {Name = "Кара-Сууский райстат"},
                    new Region {Name = "Ноокатский райстат"},
                    new Region {Name = "Кара-Кулжинский райстат"},
                    new Region {Name = "Узгенский райстат"},
                    new Region {Name = "Чон-Алайский райстат "},
                    new Region {Name = "Таласский облстат"},
                    new Region {Name = "Кара-Бууринский райстат"},
                    new Region {Name = "Бакай-Атинский райстат"},
                    new Region {Name = "Манасский райстат"},
                    new Region {Name = "Таласский райстат"},
                    new Region {Name = "Чуйский облстат"},
                    new Region {Name = "Аламудунский райстат"},
                    new Region {Name = "Ысык-Атинский райстат"},
                    new Region {Name = "Жайылский райстат"},
                    new Region {Name = "Кеминский райстат"},
                    new Region {Name = "Московский райстат"},
                    new Region {Name = "Панфиловский райстат"},
                    new Region {Name = "Сокулукский райстат"},
                    new Region {Name = "Чуйский райстат"},
                    new Region {Name = "Каракольский горстат"},
                    new Region {Name = "город Баткен"},
                    new Region {Name = "Кызыл-Киинский горстат"},
                    new Region {Name = "город Талас"},
                    new Region {Name = "город Токмок"}
                );
            }

            context.SaveChanges();
        }
    }
}
