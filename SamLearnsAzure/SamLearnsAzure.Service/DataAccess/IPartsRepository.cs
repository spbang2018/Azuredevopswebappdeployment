﻿using SamLearnsAzure.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface IPartsRepository
    {
        Task<IEnumerable<Parts>> GetParts();
        //Task<IEnumerable<PartsSummary>> GetPartsSummary(string setNum);
    }
}