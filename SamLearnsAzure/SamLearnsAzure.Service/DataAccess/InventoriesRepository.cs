﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SamLearnsAzure.Models;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Service.EFCore;

namespace SamLearnsAzure.Service.DataAccess
{
    public class InventoriesRepository : IInventoriesRepository
    {
        private readonly SamsAppDBContext _context;

        public InventoriesRepository(SamsAppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventories>> GetInventories(IRedisService redisService, bool useCache)
        {
            string cacheKeyName = "Inventories-all";
            TimeSpan cacheExpirationTime = new TimeSpan(24, 0, 0);

            List<Inventories> result = null;

            //Check the cache
            string cachedJSON = null;
            if (redisService != null && useCache == true)
            {
                cachedJSON = await redisService.GetAsync(cacheKeyName);
            }
            if (cachedJSON != null)
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Inventories>>(cachedJSON);
            }
            else
            {
                result = await _context.Inventories
                 .OrderBy(p => p.Id)
                 .ToListAsync();
            }
            return result;   
        }
    }
}
