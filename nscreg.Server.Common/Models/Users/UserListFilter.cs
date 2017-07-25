﻿using nscreg.Data.Constants;

namespace nscreg.Server.Common.Models.Users
{
    public class UserListFilter : PaginationModel
    {
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public UserStatuses? Status { get; set; }
        public string SortColumn { get; set; }
        public bool SortAscending { get; set; }
    }
}