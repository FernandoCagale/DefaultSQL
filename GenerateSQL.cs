using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DefaultSQL
{
    public class GenerateSQL
    {
        public static bool showLog = false;

        /// <summary>
        /// Generated sql for the object (UPDATE)
        /// </summary>
        /// <param name="The mapping object"></param>
        /// <returns>Sql generated based in object</returns>
        /// <exception cref="Exception">This exception is thrown if there is error in mapping</exception>
        public static string getUpdate(object o)
        {
            ReflectionAttribute.getValidationMapping(o);

            CustomProperty custom = ReflectionAttribute.getFieldsNotRelations(o);

            string sqlFields = string.Empty;

            string sqlWhere = string.Empty;

            foreach (Field field in custom.listField)
            {
                if (string.IsNullOrWhiteSpace(sqlFields))
                {
                    sqlFields = string.Format("{1}=@{2}", custom.table.name, field.field, field.name);
                }
                else
                {
                    sqlFields += string.Format(",{1}=@{2}", custom.table.name, field.field, field.name);

                }
            }

            sqlWhere = string.Format("{0}.{1}=@{2}", custom.table.name, custom.primaryKey.field, custom.primaryKey.name);

            string sql = string.Format("UPDATE {0} SET {1} WHERE {2}", custom.table.name, sqlFields, sqlWhere);

            if (showLog)
                Console.WriteLine(sql);

            return sql;
        }

        /// <summary>
        /// Generated sql for the object (INSERT)
        /// </summary>
        /// <param name="The mapping object"></param>
        /// <returns>Sql generated based in object</returns>
        /// <exception cref="Exception">This exception is thrown if there is error in mapping</exception>
        public static string getInsert(object o) {

            ReflectionAttribute.getValidationMapping(o);

            CustomProperty custom = ReflectionAttribute.getFieldsNotRelations(o);

            string sqlFields = string.Empty;
            string sqlParams = string.Empty;

            foreach (Field field in custom.listField)
            {
                if (string.IsNullOrWhiteSpace(sqlFields))
                {
                    sqlFields = string.Format("{1}", custom.table.name, field.field);
                    sqlParams = string.Format("@{0}", field.name);
                }
                else
                {
                    sqlFields += string.Format(",{1}", custom.table.name, field.field);
                    sqlParams += string.Format(",@{0}", field.name);
                }
            }

            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", custom.table.name, sqlFields, sqlParams);

            if (showLog)
                Console.WriteLine(sql);

            return sql;

        }

        /// <summary>
        /// Generated sql for the object (DELETE)
        /// </summary>
        /// <param name="The mapping object"></param>
        /// <returns>Sql generated based in object</returns>
        /// <exception cref="Exception">This exception is thrown if there is error in mapping</exception>
        public static string getDelete(object o)
        {
            ReflectionAttribute.getValidationMapping(o);

            CustomProperty custom = ReflectionAttribute.getFieldsNotRelations(o);

            string sqlWhere = string.Empty;

            sqlWhere = string.Format("{0}.{1}=@{2}", custom.table.name, custom.primaryKey.field, custom.primaryKey.name);

            string sql = string.Format("DELETE FROM {0} WHERE {1}", custom.table.name, sqlWhere);

            if (showLog)
                Console.WriteLine(sql);

            return sql;
        }

        /// <summary>
        /// Generated sql for all fields based at the object (SELECT)
        /// </summary>
        /// <param name="The mapping object"></param>
        /// <returns>Sql generated based in object</returns>
        /// <exception cref="Exception">This exception is thrown if there is error in mapping</exception>
        public static string getSql(object o)
        {
            ReflectionAttribute.getValidationMapping(o);

            CustomProperty custom = ReflectionAttribute.getFieldsNotRelations(o);

            string sqlFields = string.Empty;

            string sqlRelations = string.Empty;

            string sqlOrderBy = string.Empty;

            sqlFields = string.Format("{0}.{1} as {0}_{2}", custom.table.name, custom.primaryKey.field, custom.primaryKey.name);

            foreach (Field field in custom.listField)
            {
                sqlFields += string.Format(",{0}.{1} as {0}_{2}", custom.table.name, field.field, field.name);
            }

            foreach (CustomPropertyForeingKey foreingKey in custom.listForeingKeyCustom)
            {
                sqlFields += string.Format(",{0}.{1} as {0}_{2}", custom.table.name, foreingKey.field, foreingKey.name);
                
            }

            foreach (FieldOrderBy field in custom.listFieldOrderBy)
            {
                if (string.IsNullOrWhiteSpace(sqlOrderBy))
                {
                    sqlOrderBy = string.Format("ORDER BY {0}.{1} {2}", custom.table.name, field.field, field.orderBy);
                }
                else
                {
                    sqlOrderBy += string.Format(", {0}.{1} {2}", custom.table.name, field.field, field.orderBy);
                }
            }

            string sql = string.Format("SELECT {0} FROM {1} {2} {3}", sqlFields, custom.table.name, sqlRelations, sqlOrderBy);

            if (showLog)
                Console.WriteLine(sql);

            return sql;
        }

        /// <summary>
        /// Generated sql for all fields based at the object and condition for primary key (SELECT)
        /// </summary>
        /// <param name="The mapping object"></param>
        /// <returns>Sql generated based in object</returns>
        /// <exception cref="Exception">This exception is thrown if there is error in mapping</exception>
        public static string getSqlById(object o)
        {
            ReflectionAttribute.getValidationMapping(o);

            CustomProperty custom = ReflectionAttribute.getFieldsNotRelations(o);

            string sqlFields = string.Empty;

            string sqlWhere = string.Empty;

            sqlFields = string.Format("{0}.{1} as {0}_{2}", custom.table.name, custom.primaryKey.field, custom.primaryKey.name);

            foreach (Field field in custom.listField)
            {
                sqlFields += string.Format(",{0}.{1} as {0}_{2}", custom.table.name, field.field, field.name);
            }

            foreach (CustomPropertyForeingKey foreingKey in custom.listForeingKeyCustom)
            {
                sqlFields += string.Format(",{0}.{1} as {0}_{2}", custom.table.name, foreingKey.field, foreingKey.name);

            }

            sqlWhere = string.Format("WHERE {0}.{1} = @{2}", custom.table.name, custom.primaryKey.field, custom.primaryKey.name);

            string sql = string.Format("SELECT {0} FROM {1} {2}", sqlFields, custom.table.name, sqlWhere);

            if (showLog)
                Console.WriteLine(sql);

            return sql;
        }

        /// <summary>
        /// Generated sql for all fields and relations based at the object (SELECT)
        /// </summary>
        /// <param name="The mapping object"></param>
        /// <returns>Sql generated based in object</returns>
        /// <exception cref="Exception">This exception is thrown if there is error in mapping</exception>
        public static string getSqlAll(object o)
        {
            ReflectionAttribute.getValidationMapping(o);

            CustomProperty custom = ReflectionAttribute.getFieldsAll(o);

            string sqlFields = string.Empty;

            string sqlRelations = string.Empty;

            string sqlOrderBy = string.Empty;

            string sqlAlias = string.Empty;

            sqlFields = string.Format("{0}.{1} as {0}_{2}", custom.table.name, custom.primaryKey.field, custom.primaryKey.name);

            foreach (Field field in custom.listField)
            {
                sqlFields += string.Format(",{0}.{1} as {0}_{2}", custom.table.name, field.field, field.name);
            }

            foreach (CustomPropertyForeingKey foreingKey in custom.listForeingKeyCustom)
            {
                sqlAlias = string.Format("{0}_{1}", foreingKey.table.name, foreingKey.field.Replace("\"",""));

                sqlFields += string.Format(", {0}.{1} as {0}_{2}", sqlAlias, foreingKey.primaryKey.field, foreingKey.primaryKey.name);

                foreach(Field field in foreingKey.listField) {
                    sqlFields += string.Format(",{0}.{1} as {0}_{2}", sqlAlias, field.field, field.name);
                }

                sqlRelations += string.Format("{0} JOIN {1} {5} ON {5}.{2} = {3}.{4} ", foreingKey.relations, foreingKey.table.name, foreingKey.primaryKey.field, custom.table.name, foreingKey.field, sqlAlias);
            }

            foreach (FieldOrderBy field in custom.listFieldOrderBy)
            {
                if (string.IsNullOrWhiteSpace(sqlOrderBy))
                {
                    sqlOrderBy = string.Format("ORDER BY {0}.{1} {2}", custom.table.name, field.field, field.orderBy);
                }
                else
                {
                    sqlOrderBy += string.Format(", {0}.{1} {2}", custom.table.name, field.field, field.orderBy);
                }
            }

            string sql = string.Format("SELECT {0} FROM {1} {2} {3}", sqlFields, custom.table.name, sqlRelations, sqlOrderBy);

            if (showLog)
                Console.WriteLine(sql);

            return sql;
        }
    }
}
