﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ShipIt.Models.DataModels;

namespace ShipIt.Models.ApiModels
{
    public class Employee
    {
        public string Name { get; set; }
        public int WarehouseId { get; set; }
        public EmployeeRole role { get; set; }
        public string ext { get; set; }
        public int id { get; set; }

        public Employee(EmployeeDataModel dataModel)
        {
            Name = dataModel.Name;
            WarehouseId = dataModel.WarehouseId;
            role = MapDatabaseRoleToApiRole(dataModel.Role);
            ext = dataModel.Ext;
            id = dataModel.id;
        }

        private EmployeeRole MapDatabaseRoleToApiRole(string databaseRole)
        {
            if (databaseRole == DataBaseRoles.Cleaner) return EmployeeRole.CLEANER;
            if (databaseRole == DataBaseRoles.Manager) return EmployeeRole.MANAGER;
            if (databaseRole == DataBaseRoles.OperationsManager) return EmployeeRole.OPERATIONS_MANAGER;
            if (databaseRole == DataBaseRoles.Picker) return EmployeeRole.PICKER;
            throw new ArgumentOutOfRangeException("DatabaseRole");
        }

        //Empty constructor needed for Xml serialization
        public Employee()
        {
        }

        public override String ToString()
        {
            return new StringBuilder()
                    .AppendFormat("name: {0}, ", Name)
                    .AppendFormat("warehouseId: {0}, ", WarehouseId)
                    .AppendFormat("role: {0}, ", role)
                    .AppendFormat("ext: {0}", ext)
                    .ToString();
        }
    }
}