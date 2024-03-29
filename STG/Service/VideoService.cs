﻿using Microsoft.EntityFrameworkCore;
using STG.Component;
using STG.Data;
using STG.DTO;
using STG.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STG.Service
{
    public class VideoService
    {
        private const int lengthOfRandomPathOfFile = 32;

        private ApplicationDbContext _dbc;
        public VideoService(ApplicationDbContext dbc)
        {
            this._dbc = dbc;
        }

        public Video findById(int id)
        {
            return this._dbc.Videos.SingleOrDefault(p => p.id == id);
        }

        public List<Video> listAll()
        {
            return this._dbc.Videos.OrderByDescending(p => p.id).ToList();
        }

        public IEnumerable<Video> listAllEnum()
        {
            return this._dbc.Videos.OrderByDescending(p => p.id).ToList();
        }

        public Video add(VideoNewDTO videoNewDTO)
        {
            Video video = new Video();
            video.name = videoNewDTO.name;
            video.dateOfAdd = DateTime.Now;
            video.hashPath = RandomComponent.RandomString(lengthOfRandomPathOfFile);

            this._dbc.Videos.Add(video);

            this._dbc.SaveChanges();

            return video;
        }

        public bool update(VideoDTO videoDTO)
        {
            Video video = findById(videoDTO.id);

            if (video == null) return false;

            video.name = video.name;

            this._dbc.SaveChanges();

            return true;
        }

        public bool delete(int id)
        {
            Video video = findById(id);
            this._dbc.Videos.Remove(video);
            this._dbc.SaveChanges();
            return true;
        }

        public Video save(VideoDTO videoDTO)
        {
            Video video = findById(videoDTO.id);
            if (video == null) return null;

            video.name = videoDTO.name;
            if(video.hashPath == null)
            {
                video.hashPath = RandomComponent.RandomString(lengthOfRandomPathOfFile);
            }
            video.duration = videoDTO.durationSeconds + videoDTO.durationMinutes * 60 + videoDTO.durationHours * 60 * 60;


            this._dbc.SaveChanges();

            return video;
        }








        private void updateRandomPathOfFiles(Video video)
        {

        }

        private string generateRandomPathForFilePath(int id)
        {
            return String.Empty;
        }

    }
}
