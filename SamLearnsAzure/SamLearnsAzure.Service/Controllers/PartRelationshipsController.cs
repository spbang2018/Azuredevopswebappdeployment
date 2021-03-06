using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SamLearnsAzure.Models;
using SamLearnsAzure.Service.DataAccess;

namespace SamLearnsAzure.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartRelationshipsController : ControllerBase
    {
        private readonly IPartRelationshipsRepository _repo;
      
        public PartRelationshipsController(IPartRelationshipsRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Return a list of all part relationships
        /// </summary>
        /// <returns>an IEnumerable list of part relationship objects</returns>
        [HttpGet("GetPartRelationships")]
        public async Task<IEnumerable<PartRelationships>> GetPartRelationships()
        {
            return await _repo.GetPartRelationships();
        }
        
    }
}