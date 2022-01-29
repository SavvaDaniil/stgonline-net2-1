﻿using Microsoft.EntityFrameworkCore;
using STG.Component;
using STG.Data;
using STG.DTO;
using STG.DTO.Admin;
using STG.Entities;
using STG.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STG.Service
{
    public class AdminService
    {
        public ApplicationDbContext _dbc;
        public AdminService(ApplicationDbContext dbc)
        {
            this._dbc = dbc;
        }

        public async Task<Admin> add(string username)
        {
            Admin admin = new Admin();
            admin.Username = username;
            admin.dateOfAdd = DateTime.Now;
            admin.Password = BCrypt.Net.BCrypt.HashPassword(RandomComponent.RandomString(6));
            admin.AuthKey = RandomComponent.RandomString(32);

            await _dbc.Admins.AddAsync(admin);
            await _dbc.SaveChangesAsync();

            return admin;
        }

        public async Task<bool> delete(int id)
        {
            Admin admin = await findById(id);
            if (admin == null) return false;
            _dbc.Admins.Remove(admin);
            await _dbc.SaveChangesAsync();
            return true;
        }

        public bool isAlreadyExistByUsername(string username)
        {
            if (_dbc.Admins.Any(p => p.Username == username))
            {
                return true;
            }
            return false;
        }

        public bool isAlreadyExistByUsernameExceptId(int id, string username)
        {
            if (_dbc.Admins.Where(p => p.Username == username).Where(p => p.Id != id).Any())
            {
                return true;
            }
            return false;
        }

        public async Task<Admin> findByUsername(string username)
        {
            return await _dbc.Admins.FirstOrDefaultAsync(p => p.Username == username);
        }
        public async Task<Admin> findById(int id)
        {
            return await _dbc.Admins.FirstOrDefaultAsync(p => p.Id == id);
        }
        
        public async Task<JsonAnswerViewModel> update(Admin admin, AdminDTO adminDTO)
        {
            bool isNeedRelogin = false;
            admin.position = adminDTO.position;
            if (adminDTO.username != admin.Username)
            {
                if (this.isAlreadyExistByUsernameExceptId(admin.Id, adminDTO.username))
                {
                    return new JsonAnswerViewModel("error", "username_already_exist");
                }
                admin.Username = adminDTO.username;
                isNeedRelogin = true;
            }

            if (adminDTO.passwordNew != null)
            {
                if(adminDTO.passwordNew != adminDTO.passwordNewAgain) return new JsonAnswerViewModel("error", "wrong_password_new");
                if(!BCrypt.Net.BCrypt.Verify(adminDTO.passwordCurrent, admin.Password)) return new JsonAnswerViewModel("error", "wrong_password_current");

                admin.Password = BCrypt.Net.BCrypt.HashPassword(adminDTO.passwordNew);
                admin.AuthKey = RandomComponent.RandomString(32);
                isNeedRelogin = true;
            }
            await _dbc.SaveChangesAsync();

            return new JsonAnswerViewModel("success", null, admin, isNeedRelogin);
        }


        public async Task<JsonAnswerStatus> update(AdminEditDTO adminEditDTO)
        {
            Admin admin = await findById(adminEditDTO.id);
            if (admin == null) return new JsonAnswerStatus("error", "not_found");

            admin.position = adminEditDTO.position;
            if (admin.Username != adminEditDTO.username)
            {
                if (isAlreadyExistByUsernameExceptId(admin.Id, adminEditDTO.username))
                {
                    return new JsonAnswerStatus("error", "username_already_exist");
                }
                admin.Username = adminEditDTO.username;
            }

            admin.active = adminEditDTO.active;
            if (adminEditDTO.new_password != null)
            {
                admin.Password = BCrypt.Net.BCrypt.HashPassword(adminEditDTO.new_password);
                admin.AuthKey = RandomComponent.RandomString(32);
            }

            admin.panel_admins = adminEditDTO.panel_admins;
            admin.panel_homeworks = adminEditDTO.panel_homeworks;
            admin.panel_lessons = adminEditDTO.panel_lessons;
            admin.panel_lessontypes = adminEditDTO.panel_lessontypes;
            admin.panel_packages = adminEditDTO.panel_packages;
            admin.panel_mentoring = adminEditDTO.panel_mentoring;
            admin.panel_statements = adminEditDTO.panel_statements;
            admin.panel_styles = adminEditDTO.panel_styles;
            admin.panel_teachers = adminEditDTO.panel_teachers;
            admin.panel_users = adminEditDTO.panel_users;
            admin.panel_videos = adminEditDTO.panel_videos;

            await _dbc.SaveChangesAsync();

            return new JsonAnswerStatus("success", null);
        }


        public async Task checkBasicExist()
        {
            if (await this.findByUsername("admin2") == null)
            {
                Admin admin = new Admin();
                admin.Username = "admin2";
                admin.Password = BCrypt.Net.BCrypt.HashPassword("123");
                admin.dateOfAdd = DateTime.Now;
                admin.AuthKey = RandomComponent.RandomString(32);

                _dbc.Admins.Add(admin);
                await _dbc.SaveChangesAsync();
            }
        }


        public async Task<List<Admin>> searchAdmins(AdminSearchDTO adminSearchDTO)
        {
            adminSearchDTO.page--;
            IQueryable<Admin> q = _dbc.Admins.OrderByDescending(p => p.Id);

            q = q.Take(30).Skip(adminSearchDTO.page * 30);
            q = q.Where(p => p.level == 0);

            if (!string.IsNullOrEmpty(adminSearchDTO.queryString))
            {
                q = q.Where(p =>
                    EF.Functions.Like(p.Username, adminSearchDTO.queryString)
                    || EF.Functions.Like(p.position, adminSearchDTO.queryString)
                    || p.Username.Contains(adminSearchDTO.queryString)
                    || p.position.Contains(adminSearchDTO.queryString)
                );
            }
            return await q.ToListAsync();
        }

        public int searchCount(AdminSearchDTO adminSearchDTO)
        {
            IQueryable<Admin> q = _dbc.Admins.OrderByDescending(p => p.Id);

            q = q.Where(p => p.level == 0);

            if (!string.IsNullOrEmpty(adminSearchDTO.queryString))
            {
                q = q.Where(p =>
                    EF.Functions.Like(p.Username, adminSearchDTO.queryString)
                    || EF.Functions.Like(p.position, adminSearchDTO.queryString)
                    || p.Username.Contains(adminSearchDTO.queryString)
                    || p.position.Contains(adminSearchDTO.queryString)
                );
            }
            return q.Count();
        }
    }

}
