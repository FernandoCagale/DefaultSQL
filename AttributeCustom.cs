using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefaultSQL
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Table : Attribute
    {
        private string _name { get; set; }

        /// <summary>
        /// name for table in data base
        /// </summary>
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldPrimaryKey : Attribute
    {
        private string _field { get; set; }
        private string _name { get; set; }

        /// <summary>
        /// name in data base
        /// </summary>
        public string field 
        { 
            get { return _field; }
            set { _field = value; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Field : Attribute
    {
        private string _field { get; set; }
        private string _name { get; set; }

        /// <summary>
        /// name in data base
        /// </summary>
        public string field
        {
            get { return _field; }
            set { _field = value; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
    }


    [AttributeUsage(AttributeTargets.All)]
    public class FieldForeingKey : Attribute
    {
        private string _field { get; set; }
        private string _relations;
        private string _name { get; set; }

        /// <summary>
        /// name in data base
        /// </summary>
        public string field
        {
            get { return _field; }
            set { _field = value; }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// relations for field (INNER or JOIN)
        /// </summary>
        public string relations
        {
            get { return string.IsNullOrWhiteSpace(_relations) ? _relations = Relations.INNER : _relations; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    if (value.Equals(Relations.INNER) || value.Equals(Relations.LEFT))
                        _relations = value;
                    else
                        throw new Exception(string.Format("Relations invalid, options: {0} - {1}", Relations.INNER, Relations.LEFT));
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FieldOrderBy : Attribute
    {
        private string _field { get; set; }
        private string _orderBy;

        /// <summary>
        /// name in data base
        /// </summary>
        public string field
        {
            get { return _field; }
            set { _field = value; }
        }

        /// <summary>
        /// type order (ASC or DESC)
        /// </summary>
        public string orderBy
        {
            get { return this._orderBy; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    if (value.Equals(OrderBy.ASC) || value.Equals(OrderBy.DESC))
                        this._orderBy = value;
                    else
                        throw new Exception(string.Format("Order by invalid, options: {0} - {1}", OrderBy.ASC, OrderBy.DESC));
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Ignore : Attribute
    {

    }
}
