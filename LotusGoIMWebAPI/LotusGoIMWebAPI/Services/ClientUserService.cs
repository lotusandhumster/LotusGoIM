using LotusGoIMWebAPI.Common;
using LotusGoIMWebAPI.Common.Enums;
using LotusGoIMWebAPI.Common.ResultModel;
using LotusGoIMWebAPI.DbContexts;
using LotusGoIMWebAPI.Entities;
using LotusGoIMWebAPI.Models;
using LotusGoIMWebAPI.Models.SearchFilters;
using LotusGoIMWebAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LotusGoIMWebAPI.Services
{
    public class ClientUserService: IClientUserService
    {
        private readonly LotusGoIMContext _context;

        public ClientUserService(LotusGoIMContext lotusGoIMContext)
        {
            _context = lotusGoIMContext;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public async Task<ResultModel<bool>> AddAsync(ClientUser user)
        {
            var existUser = await _context.ClientUser.FirstOrDefaultAsync(u => u.Email == user.Email && u.IsDeleted == false);
            if (existUser != null)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("邮箱已存在");
            }
            if(user.Password.Length < 6 || user.Password.Length > 20)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("密码长度应在6-20位之间");
            }
            if(user.NickName.Length < 1 || user.NickName.Length > 20)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("昵称长度应在1-20位之间");
            }
            if(user.Description.Length > 50)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("个性签名长度应在50位之内");
            }
            if (!Enum.IsDefined(typeof(GenderType), user.Gender))
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("性别错误");
            }

            user.Password = EncryptHelper.EncryptPassword(user.Password);
            _ = await _context.ClientUser.AddAsync(user);
            return ResultModelFactory.ResultModelSusccess(await _context.SaveChangesAsync() > 0);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var clientUser = await _context.ClientUser.FirstOrDefaultAsync(u => u.UserId == id);
            if (clientUser is null)
            {
                return false;
            }
            clientUser.IsDeleted = true;
            _context.ClientUser.Update(clientUser);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ClientUser?> GetAsync(int id)
        {
            return await _context.ClientUser.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<ClientUser?> GetByEmailAsync(string email)
        {
            var user = await _context.ClientUser.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<int> GetCountAsync(ClientUserSearchFilter filter)
        {
            var query = _context.ClientUser.AsQueryable();
            if (!string.IsNullOrEmpty(filter.NickName))
            {
                query = query.Where(u => u.NickName.Contains(filter.NickName));
            }
            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(u => u.Email.Contains(filter.Email));
            }
            if (filter.Gender.HasValue)
            {
                query = query.Where(u => u.Gender == filter.Gender.Value);
            }
            query = query.Where(u => u.IsDeleted == filter.IsDeleted);

            return await query.CountAsync();
        }

        public Task<List<ClientUser>> GetListAsync(ClientUserSearchFilter filter)
        {
            var query = _context.ClientUser.AsQueryable();
            if (!string.IsNullOrEmpty(filter.NickName))
            {
                query = query.Where(u => u.NickName.Contains(filter.NickName));
            }
            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(u => u.Email.Contains(filter.Email));
            }
            if (filter.Gender.HasValue)
            {
                query = query.Where(u => u.Gender == filter.Gender.Value);
            }
            if (filter.State.HasValue)
            {
                query = query.Where(u => u.State == filter.State.Value);
            }
            query = query.Where(u => u.IsDeleted == filter.IsDeleted)
                .OrderBy(u => u.State);

            return query.ToListAsync();
        }

        public Task<List<ClientUser>> GetPageAsync(ClientUserSearchFilter filter)
        {
            var query = _context.ClientUser.AsQueryable();
            if (!string.IsNullOrEmpty(filter.NickName))
            {
                query = query.Where(u => u.NickName.Contains(filter.NickName));
            }
            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(u => u.Email.Contains(filter.Email));
            }
            if (filter.Gender.HasValue)
            {
                query = query.Where(u => u.Gender == filter.Gender.Value);
            }
            if(filter.State.HasValue)
            {
                query = query.Where(u => u.State == filter.State.Value);
            }
            query = query.Where(u => u.IsDeleted == filter.IsDeleted)
                .OrderBy(u => u.State);
            query = query.Skip(filter.PageSize * (filter.PageIndex - 1)).Take(filter.PageSize);
            return query.ToListAsync();
        }

        public async Task<ClientUser?> LoginAsync(LoginModel loginModel)
        {
            return await _context.ClientUser.FirstOrDefaultAsync(u => u.Email == loginModel.Email && u.Password == loginModel.Password);
        }

        public async Task<ResultModel<bool>> UpdateAsync(ClientUser user)
        {
            var clientUser = await _context.ClientUser.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            if (clientUser is null)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("用户不存在");
            }
            if(!string.IsNullOrEmpty(user.NickName))
            {
                if(user.NickName.Length < 1 || user.NickName.Length > 20)
                {
                    return ResultModelFactory.ResultModelInternalServerError<bool>("昵称长度应在1-20位之间");
                }
                clientUser.NickName = user.NickName;
            }
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                clientUser.AvatarUrl = user.AvatarUrl;
            }
            if (!string.IsNullOrEmpty(user.Description))
            {
                if (user.Description.Length > 50)
                {
                    return ResultModelFactory.ResultModelInternalServerError<bool>("个性签名长度应在50位之内");
                }
                clientUser.Description = user.Description;
            }
            if (Enum.IsDefined(typeof(GenderType), user.Gender))
            {
                clientUser.Gender = user.Gender;
            }
            if (Enum.IsDefined(typeof(ClientUserState), user.State))
            {
                clientUser.State = user.State;
            }
            else
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("性别错误");
            }

            _context.ClientUser.Update(clientUser);
            return ResultModelFactory.ResultModelSusccess(await _context.SaveChangesAsync() > 0);
        }

        public async Task<ResultModel<bool>> UpdatePasswordAsync(int id, string password)
        {
            var clientUser = await _context.ClientUser.FirstOrDefaultAsync(u => u.UserId == id);
            if (clientUser is null)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("用户不存在");
            }
            if (clientUser.Password == password)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("新密码不能与旧密码相同");
            }
            if (password.Length < 6 || password.Length > 20)
            {
                return ResultModelFactory.ResultModelInternalServerError<bool>("密码长度应在6-20位之间");
            }
            password = EncryptHelper.EncryptPassword(password);
            clientUser.Password = password;
            _context.ClientUser.Update(clientUser);
            return ResultModelFactory.ResultModelSusccess(await _context.SaveChangesAsync() > 0);
        }
    }
}
