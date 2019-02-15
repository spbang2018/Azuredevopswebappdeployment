﻿using System;
using System.Collections.Generic;

namespace SamLearnsAzure.Service.Models
{
    public partial class InventoryParts
    {
        public int InventoryId { get; set; }
        public string PartNum { get; set; }
        public int? ColorId { get; set; }
        public int? Quantity { get; set; }
        public bool? IsSpare { get; set; }
        public int InventoryPartId { get; set; }

        public Colors Color { get; set; }
        public Inventories Inventory { get; set; }
        public Parts Part { get; set; }
    }
}
