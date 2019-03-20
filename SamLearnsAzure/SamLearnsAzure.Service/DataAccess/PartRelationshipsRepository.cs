﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Models;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.EFCore;
using Newtonsoft.Json;

namespace SamLearnsAzure.Service.DataAccess
{
    public class PartRelationshipsRepository : IPartRelationshipsRepository
    {
        private readonly SamsAppDBContext _context;

        public PartRelationshipsRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PartRelationships>> GetPartRelationships(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "PartRelationships-all";
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);

            List<PartRelationships> result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = JsonConvert.DeserializeObject<List<PartRelationships>>(cachedJSON);
            }
            else
            {
                result = await _context.PartRelationships
                 .OrderBy(p => p.PartRelationshipId)
                 .ToListAsync();

                if (redisService != null)
                {
                    //set the cache with the updated record
                    await redisService.SetAsync(cacheKeyName, JsonConvert.SerializeObject(result), cacheExpirationTime);
                }
            }

            return result;
        }
    }
}
