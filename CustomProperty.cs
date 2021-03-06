﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefaultSQL
{
    public class CustomProperty
    {
        public Table table { get; set; }

        public FieldPrimaryKey primaryKey { get; set; }

        public List<CustomPropertyForeingKey> listForeingKeyCustom { get; set; }

        public List<Field> listField { get; set; }

        public List<FieldOrderBy> listFieldOrderBy { get; set; }
    }
}
