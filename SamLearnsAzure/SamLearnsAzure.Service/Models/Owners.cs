﻿using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Service.Models
{
    public partial class Owners
    {
        public Owners()
        {
            OwnerSets = new HashSet<OwnerSets>();
        }

        public int Id { get; set; }
        public string OwnerName { get; set; }

        public ICollection<OwnerSets> OwnerSets { get; set; }
    }
}