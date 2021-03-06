using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface ISetsRepository
    {
        Task<IEnumerable<Sets>> GetSets();

        Task<Sets> GetSet(IRedisService redisService, bool useCache, string setNum);

    }
}
