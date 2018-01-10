using System;
using System.Collections.Generic;
using nscreg.Data.Constants;
using Newtonsoft.Json;

namespace nscreg.Data.Entities
{
    /// <summary>
    ///  Класс сущность журнал загрузки данных
    /// </summary>
    public class DataUploadingLog
    {
        public int Id { get; set; }
        public DateTime? StartImportDate { get; set; }
        public DateTime? EndImportDate { get; set; }
        public string TargetStatId { get; set; }
        public string StatUnitName { get; set; }
        public string SerializedUnit { get; set; }
        public string SerializedRawUnit { get; set; }
        public int DataSourceQueueId { get; set; }
        public DataUploadingLogStatuses Status { get; set; }
        public string Note { get; set; }
        public string Errors { get; set; }
        public string Summary { get; set; }

        public Dictionary<string, IEnumerable<string>> ErrorMessages => Errors != null
            ? JsonConvert.DeserializeObject<Dictionary<string, IEnumerable<string>>>(Errors)
            : new Dictionary<string, IEnumerable<string>>();

        public IEnumerable<string> SummaryMessages => Summary != null
            ? JsonConvert.DeserializeObject<IEnumerable<string>>(Summary)
            : Array.Empty<string>();

        public virtual DataSourceQueue DataSourceQueue { get; set; }
    }
}
