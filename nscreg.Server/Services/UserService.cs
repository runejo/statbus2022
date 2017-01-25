﻿using nscreg.CommandStack;
using nscreg.Data;
using nscreg.Data.Constants;
using nscreg.ReadStack;
using nscreg.Server.Models.Users;
using System;
using System.Linq;
using nscreg.Resources.Languages;
using Microsoft.EntityFrameworkCore;

namespace nscreg.Server.Services
{
    public class UserService
    {
        private readonly CommandContext _commandCtx;
        private readonly ReadContext _readCtx;

        public UserService(NSCRegDbContext db)
        {
            _commandCtx = new CommandContext(db);
            _readCtx = new ReadContext(db);
        }

        public UserListVm GetAllPaged(int page, int pageSize)
        {
            var activeUsers = _readCtx.Users.Where(u => u.Status == UserStatuses.Active);
            var resultGroup = activeUsers
                .Skip(pageSize * page)
                .Take(pageSize)
                .GroupBy(p => new { Total = activeUsers.Count() })
                .FirstOrDefault();

            return UserListVm.Create(
                resultGroup?.Select(UserListItemVm.Create) ?? Array.Empty<UserListItemVm>(),
                resultGroup?.Key.Total ?? 0,
                (int)Math.Ceiling((double)(resultGroup?.Key.Total ?? 0) / pageSize));
        }

        public UserVm GetById(string id)
        {
            var user = _readCtx.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Id == id && u.Status == UserStatuses.Active);
            if (user == null)
                throw new Exception(nameof(Resource.UserNotFoundError));

            var roleNames = _readCtx.Roles
                .Where(r => user.Roles.Any(ur => ur.RoleId == r.Id))
                .Select(r => r.Name);
            return UserVm.Create(user, roleNames);
        }

        public void Suspend(string id)
        {
            var user = _readCtx.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                throw new Exception(nameof(Resource.UserNotFoundError));

            var adminRole = _readCtx.Roles.FirstOrDefault(
                r => r.Name == DefaultRoleNames.SystemAdministrator);
            if (adminRole == null)
                throw new Exception(nameof(Resource.SysAdminRoleMissingError));

            if (adminRole.Users.Any(ur => ur.UserId == user.Id)
                && adminRole.Users.Count() == 1)
                throw new Exception(nameof(Resource.DeleteLastSysAdminError));

            _commandCtx.SuspendUser(id);
        }
    }
}
