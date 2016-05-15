using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DefaultSQL
{
    public class ReflectionAttribute
    {
        public static Table getTableType(object o)
        {
            return Attribute.GetCustomAttribute(o.GetType(), typeof(Table)) as Table;
        }

        public static List<string> getFieldsValidation(object o)
        {
            List<string> fields = new List<string>();

            PropertyInfo[] propriedadesInfo = o.GetType().GetProperties();

            foreach (PropertyInfo info in propriedadesInfo)
            {
                foreach (Attribute attribute in info.GetCustomAttributes(false))
                {
                    if (attribute.GetType() == typeof(Ignore))
                        continue;

                    if (attribute.GetType() == typeof(FieldPrimaryKey))
                    {
                        fields.Add(((FieldPrimaryKey)attribute).field);
                    }
                    else if (attribute.GetType() == typeof(Field))
                    {
                        fields.Add(((Field)attribute).field);
                    }
                    else if (attribute.GetType() == typeof(FieldForeingKey))
                    {
                        fields.Add(((FieldForeingKey)attribute).field);
                    }
                }
            }
            return fields;
        }

        public static CustomProperty getFieldsNotRelations(object o)
        {
            if (o == null)
                throw new Exception("Object informed is null");

            CustomProperty custom = new CustomProperty();
            custom.table = getTableType(o);

            if (custom.table == null)
                throw new Exception("[Table] no informed");

            custom.listField = new List<Field>();
            custom.listFieldOrderBy = new List<FieldOrderBy>();
            custom.listForeingKeyCustom = new List<CustomPropertyForeingKey>();

            PropertyInfo[] propriedadesInfo = o.GetType().GetProperties();

            foreach (PropertyInfo info in propriedadesInfo)
            {
                foreach (Attribute attribute in info.GetCustomAttributes(false))
                {
                    if (attribute.GetType() == typeof(Ignore))
                        continue;

                    if (attribute.GetType() == typeof(FieldPrimaryKey))
                    {
                        if (custom.primaryKey != null )
                            throw new Exception(string.Format("Duplicate annotation [FieldPrimaryKey] for object {0}", o.GetType().Name));

                        ((FieldPrimaryKey)attribute).name = info.Name;
                        custom.primaryKey = ((FieldPrimaryKey)attribute);
                    }
                    else if (attribute.GetType() == typeof(Field))
                    {
                        ((Field)attribute).name = info.Name;
                        custom.listField.Add((Field)attribute);
                    }
                    else if (attribute.GetType() == typeof(FieldForeingKey))
                    {
                        CustomPropertyForeingKey foreingKey = new CustomPropertyForeingKey();
                        foreingKey.field = ((FieldForeingKey)attribute).field;
                        foreingKey.name = info.Name;
                        foreingKey.relations = ((FieldForeingKey)attribute).relations;
                        custom.listForeingKeyCustom.Add(foreingKey);
                    }
                    else if (attribute.GetType() == typeof(FieldOrderBy))
                    {
                        custom.listFieldOrderBy.Add((FieldOrderBy)attribute);
                    }
                }
            }

            validateProperty(o, custom);

            return custom;
        }

        private static void validateProperty(object o, CustomProperty custom)
        {
            if (custom.table == null)
                throw new Exception(string.Format("Not exist [Table] attribute for object {0}", o.GetType().Name));

            if (custom.primaryKey == null)
                throw new Exception(string.Format("Not exist [FieldPrimaryKey] attribute for object {0}", o.GetType().Name));

            if ((custom.listField == null || custom.listField.Count == 0) && (custom.listForeingKeyCustom == null || custom.listForeingKeyCustom.Count == 0))
                throw new Exception(string.Format("Not exist [Field or FieldForeingKey] attribute for object {0}", o.GetType().Name));
        }

        public static CustomProperty getFieldsAll(object o)
        {
            if (o == null)
                throw new Exception("Object informed is null");

            CustomProperty custom = new CustomProperty();
            custom.table = getTableType(o);

            custom.listField = new List<Field>();
            custom.listFieldOrderBy = new List<FieldOrderBy>();
            custom.listForeingKeyCustom = new List<CustomPropertyForeingKey>();

            PropertyInfo[] propriedadesInfo = o.GetType().GetProperties();

            foreach (PropertyInfo info in propriedadesInfo)
            {
                foreach (Attribute attribute in info.GetCustomAttributes(false))
                {
                    if (attribute.GetType() == typeof(Ignore))
                        continue;

                    if (attribute.GetType() == typeof(FieldPrimaryKey))
                    {
                        if (custom.primaryKey != null)
                            throw new Exception(string.Format("Duplicate annotation [FieldPrimaryKey] for object {0}", o.GetType().Name));

                        ((FieldPrimaryKey)attribute).name = info.Name;
                        custom.primaryKey = ((FieldPrimaryKey)attribute);
                    }
                    else if (attribute.GetType() == typeof(Field))
                    {
                       ((Field)attribute).name = info.Name;
                       custom.listField.Add((Field)attribute);
                    }
                    else if (attribute.GetType() == typeof(FieldForeingKey))
                    {
                        object oo = info.GetValue(o, null);

                        if (oo == null)
                            throw new Exception(string.Format("Object relational {0} no instance for {1}", ((FieldForeingKey)attribute).field, o.GetType().Name));

                        CustomProperty foreingKey = getFieldsNotRelations(oo);

                        CustomPropertyForeingKey customPropertyForeingKey = new CustomPropertyForeingKey();
                        customPropertyForeingKey.table = foreingKey.table;
                        customPropertyForeingKey.primaryKey = foreingKey.primaryKey;
                        customPropertyForeingKey.listField = foreingKey.listField;
                        customPropertyForeingKey.relations = ((FieldForeingKey)attribute).relations;
                        customPropertyForeingKey.field = ((FieldForeingKey)attribute).field;

                        custom.listForeingKeyCustom.Add(customPropertyForeingKey);
                    }
                    else if (attribute.GetType() == typeof(FieldOrderBy))
                    {
                        custom.listFieldOrderBy.Add((FieldOrderBy)attribute);
                    }
                }
            }

            validateProperty(o, custom);

            return custom;
        }

        public static void getValidationMapping(object o)
        {
            List<string> fields = getFieldsValidation(o);
            
            List<string> list = fields.Distinct().ToList();

            if (list.Count != fields.Count)
                throw new Exception(string.Format("Exist duplicate field to the mapping {0}", o.GetType().Name));
        }
    }
}
