﻿using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Dto.Users;
using TaskManager.Core.Dto.UserTask;
using TaskManager.Core.Interfaces;
using TaskManager.DataProvider.Context;
using TaskManager.DataProvider.Entities;
using TaskManager.DataProvider.Enums;
using TeleSales.Core.Responses;

namespace TaskManager.Core.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;
    public UserService(ApplicationDbContext db)
    {
    _db = db; 
    }

    public async Task<BaseResponse<GetUserDto>> Create(CreateUserDto user)
    {
        var data = new Users
        {
            FullName = user.FirstName + " " + user.LastName,
            Email = user.Email,
            Password = user.Password,
            CreateAt = DateTime.Now,
        };

        await _db.Users.AddAsync(data); 
        await _db.SaveChangesAsync();

        var dto = new GetUserDto
        {
            Id = data.Id,
            CreateAt = data.CreateAt,
            Email = data.Email,
            FullName = data.FullName,
            isDeleted = data.IsDeleted,
        };

        return new BaseResponse<GetUserDto>(dto);
    }

    public async Task<BaseResponse<ICollection<GetUserDto>>> GetAdmin()
    {
        var data = await _db.Users.Where(x => !x.IsDeleted && x.Role == Role.Admin).ToListAsync();

        var dto = data.Select(user => new GetUserDto
        {
            Id = user.Id,
            CreateAt = user.CreateAt,
            Email = user.Email,
            FullName = user.FullName,
            isDeleted = user.IsDeleted,
        }).ToList();

        return new BaseResponse<ICollection<GetUserDto>> (dto);
    }

    public async Task<BaseResponse<GetUserDto>> GetById(long id)
    {
        if (id <= 0)
            return new BaseResponse<GetUserDto>(null, false);

        var data = await _db.Users.SingleOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
        
        if (data == null)
            return new BaseResponse<GetUserDto>(null, false);

        var dto = new GetUserDto
        {
            Id = data.Id,
            CreateAt = data.CreateAt,
            Email = data.Email,
            FullName = data.FullName,
            isDeleted = data.IsDeleted,
        };

        return new BaseResponse<GetUserDto>(dto);
    }

    public async Task<BaseResponse<ICollection<GetUserDto>>> GetUser()
    {
        var data = await _db.Users.Where(x => !x.IsDeleted && x.Role == Role.User).ToListAsync();

        var dto = data.Select(user => new GetUserDto
        {
            Id = user.Id,
            CreateAt = user.CreateAt,
            Email = user.Email,
            FullName = user.FullName,
            isDeleted = user.IsDeleted,
        }).ToList();

        return new BaseResponse<ICollection<GetUserDto>>(dto);
    }

    public async Task<BaseResponse<GetUserDto>> Remove(long id)
    {
        if (id <= 0)
            return new BaseResponse<GetUserDto>(null);

        var data = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
        if (data == null)
            return new BaseResponse<GetUserDto>(null);

        data.IsDeleted = true;

        _db.Users.Remove(data);
        await _db.SaveChangesAsync();

        var dto = new GetUserDto
        {
            Id = data.Id,
            CreateAt = data.CreateAt,
            Email = data.Email,
            FullName = data.FullName,
            isDeleted = data.IsDeleted,
        };

        return new BaseResponse<GetUserDto>(dto);
    }

    public async Task<BaseResponse<GetUserDto>> ChangeRole(long id, Role role)
    {
        if (id <= 0)
            return new BaseResponse<GetUserDto>(null);

        var data = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
        if (data == null)
            return new BaseResponse<GetUserDto>(null);

        data.Role = role;

        _db.Users.Update(data);
        await _db.SaveChangesAsync();

        var dto = new GetUserDto
        {
            Id = data.Id,
            CreateAt = data.CreateAt,
            Email = data.Email,
            FullName = data.FullName,
            isDeleted = data.IsDeleted,
            
        };

        return new BaseResponse<GetUserDto>(dto);
    }

    public async Task<BaseResponse<GetUserDto>> Update(long id, UpdateUserDto user)
    {
        if (id <= 0)
            return new BaseResponse<GetUserDto>(null);

        var data = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
        if (data == null)
            return new BaseResponse<GetUserDto>(null);

        data.FullName = user.FirstName + " " + user.LastName;
        data.Email = user.Email;
        
        _db.Update(data);
        await _db.SaveChangesAsync();

        var dto = new GetUserDto
        {
            Id = data.Id,
            CreateAt = data.CreateAt,
            Email = data.Email,
            FullName = data.FullName,
            isDeleted = data.IsDeleted,
        };

        return new BaseResponse<GetUserDto>(dto);
    }


}
