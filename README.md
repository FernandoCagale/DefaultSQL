## DefaultSQL

DefaultSQL foi desenvolvido no intuito de auxiliar nas gerações dos SQL básicos, com o foco em aplicações de conexão JDBC. DefaultSQL foi criado com base em anotações.

Para utilizar o DefaultSQL, basta adicionar a referência **DefaultSQL.dll** no seu projeto, e importar **using DefaultSQL;** que já estará pronto para utilizar.

## Anotações do DefaultSQL

## Table:
  Essa anotação é a nível de classe.
  
  **Obrigatório**

  **Regras:**
    Caso anotação [Table] não seja informada, será lançada a seguinte exceção:
      "[type = Exception] - [Table] no informed"

  **Exemplo:**
  
```C#
    [Table(name = "users")]
    public class nomeDaClasse
```


  Parâmetro name: é o nome da tabela no banco de dados referente a classe.

## FieldPrimaryKey: 
  Essa anotação é para informar a chave primária de uma tabela.
  
  **Obrigatório**

  **Regras:**
    Não é possível mapear mais de um atributo com essa anotação, caso contrário, será lançada a seguinte exceção: 
      "[type = Exception] - Duplicate annotation [FieldPrimaryKey] for object {nome do objeto}"

  **Exemplo:**

```C#
    [FieldPrimaryKey(field = "id")]
    public int code { get; set; }
```

  Parâmetro field: nome da coluna chave primária referente a tabela no banco de dados.

## Field: 
  Essa anotação é para especificar a coluna da tabela.

  **Opcional:**
    Podendo ter mais de um por mapeamento.

  **Regras:**
    Não é permitido colunas com o nome duplicado, caso contrário, será lançada a seguinte exceção: 
      "[type = Exception] - Exist duplicate field to the mapping {nome do objeto}"

  **Exemplo:**

```C#
    [Field(field = "email")]
    public string email { get; set; }
```

  Parâmetro field: nome da coluna na tabela do banco de dados.

##FieldForeingKey: 
  Essa anotação é para especificar o relacionamento de chave estrangeira.

  **Opcional:**
    Podendo ter mais de um por mapeamento.

  **Regras:**
    Sempre utilizar um atributo publico e privado, para objetos relacionados, no qual o publico retorna uma instância se o mesmo não estiver instanciado.
    É necessário que o objeto relacionado esteja instanciado, caso contrário, será lançada a seguinte exceção:
      "[type = Exception] - Object relational {nome do objeto relacionado} no instance for {nome do objeto}"

  **Exemplo:**

```C#
    private User _user { get; set; }

    [FieldForeingKey(field = "\"user\"", relations = Relations.INNER)]
    public User user 
    {
        get
        {
            return _user == null ? _user = new User() : _user;
        }
        set
        {
            _user = value;
        }
    }
```

  Parâmetros:
    field (obrigatório): nome da coluna chave estrangeira na tabela do banco de dados.
    relations (opcional): tipo do relacionamento que será utilizado na consulta SQL, podendo ser INNER ou LEFT
      Valor padrão caso não informado é INNER.
      Se a relação não for INNER ou LEFT, será lançada a seguinte exceção:
        "[type = Exception] - Relations invalid, options: INNER - JOIN"

## FieldOrderBy: 
  Essa anotação é para definir ordenação padrão nas consultas SQL geradas.

  **Opcional:**
    Podendo ter mais de um por mapeamento.

  **Exemplo:**

```C#
    [FieldOrderBy(field = "description", orderBy = OrderBy.ASC)]
    [Field(field = "description")]
    public string description { get; set; }
```

  **Parâmetros:**
    field (obrigatório): nome da coluna na tabela do banco de dados.
    OrderBy (opcional): tipo da ordenação que será utilizada ASC ou DESC.
      Valor padrão caso não informado é ASC.
      Se a ordenação não for ASC ou DESC, será lançada a seguinte exceção:
      "[type = Exception] - Order by invalid, options: ASC - DESC"

## Ignore: 
  Essa anotação é para atributo que não será salvo no banco de dados.

  **Opcional:**
    Podendo ter mais de uma por mapeamento.

  **Exemplo:**

```C#
	[Ignore]
	public string observation { get; set; }
```

## Exemplos de mapeamento utilizando DefaultSQL.

```C#
  [Table(name = "users")]
  public class User
  {
      [FieldPrimaryKey(field = "id")]
      public int code { get; set; }

      [Field(field = "first_name")]
      public string firstName { get; set; }

      [Field(field = "last_name")]
      public string lastName { get; set; }

      [Field(field = "email")]
      public string email { get; set; }

      [Ignore]
      public string observation { get; set; }
  }

  [Table(name = "categories")]
  public class Category
  {
      [FieldPrimaryKey(field = "id")]
      public int code { get; set; }

      [FieldOrderBy(field = "description", orderBy = OrderBy.ASC)]
      [Field(field = "description")]
      public string description { get; set; }

      private User _user { get; set; }

      [FieldForeingKey(field = "\"user\"", relations = Relations.INNER)]
      public User user 
      {
          get
          {
              return _user == null ? _user = new User() : _user;
          }
          set
          {
              _user = value;
          }
      }
  }
```

**Observação:** "user" está entre "aspas" por que ele é uma palavra reservada do banco de dados Postgres.

##Utilizando os metodos do DefaultSQL

##Gerando SQL para INSERT

```C#
GenerateSQL.getInsert(Object);
```

  **getInsert** retorna uma string com o SQL, deve ser passado por parâmetro o objeto que será utilizado como base.

  Neste caso o SQL gerado será:

```sql
INSERT INTO categories (description,"user") VALUES (@description,@user)
```

##Gerando SQL para UPDATE

```C#
GenerateSQL.getUpdate(Object);
```

  **getUpdate** retorna uma string com o SQL, deve ser passado por parâmetro o objeto que será utilizado como base.

  Neste caso o SQL gerado será:

```sql
UPDATE categories SET description=@description,"user"=@user WHERE categories.id=@code
```

##Gerando SQL para DELETE

```C#
GenerateSQL.getDelete(Object);
```

  **getDelete** retorna uma string com o SQL, deve ser passado por parâmetro o objeto que será utilizado como base.

  Neste caso o SQL gerado será:

```SQL
DELETE FROM categories WHERE categories.id=@code
```

##Gerando SQL para SELECT

```C#
GenerateSQL.getSql(Object);
```

  **getSql** retorna uma string com o SQL de todos os campos mapeados no objeto, deve ser passado por parâmetro o objeto que será utilizado como base.

  Neste caso o SQL gerado será:

```SQL
SELECT categories.id as categories_code,categories.description as categories_description,categories."user" as categories_user FROM categories  ORDER BY categories.description ASC
```

```C#
GenerateSQL.getSqlAll(Object);
```

  **getSqlAll** retorna uma string com o SQL de todos os campos mapeados no objeto, inclusive os relacionamentos de chave estrangeira, deve ser passado por parâmetro o objeto que será utilizado como base.

  Neste caso o SQL gerado será:

```SQL
SELECT categories.id as categories_code,categories.description as categories_description, users_user.id as users_user_code,users_user.first_name as users_user_firstName,users_user.last_name as users_user_lastName,users_user.email as users_user_email FROM categories INNER JOIN users users_user ON users_user.id = categories."user"  ORDER BY categories.description ASC
```

  O aliás é gerado da seguinte forma, nome da tabela relacionada, nome do atributo da classe e nome do atributo da tabela relacionada, foi necessário adotar está regra por causa de auto-relacionamento, para não ocorrer conflito.

```C#
GenerateSQL.getSqlById(Object);
```

  **getSqlById** retorna uma string com o SQL adicionando uma condição com base na chave primária do objeto, deve ser passado por parâmetro o objeto que será utilizado como base.

  Neste caso o SQL gerado será:

```SQL
SELECT categories.id as categories_code,categories.description as categories_description,categories."user" as categories_user FROM categories WHERE categories.id = @code
```

É possível habilitar o log no console do Visual Studio, e visualizar os SQL que estão sendo gerados.

```C#
GenerateSQL.showLog = true;
```

## Exemplo Prático

  Implementando um **Repositório**

```C#
public class UserRepository
    {
        public void delete(User entity)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = Connection.getConnection;
            cmd.Transaction = Connection.getConnection.BeginTransaction();
            cmd.ExecuteNonQuery();

            string sql = GenerateSQL.getDelete(entity);

            cmd.CommandText = sql;
            cmd.Parameters.Add(new NpgsqlParameter("@code", entity.code));

            try
            {
                cmd.ExecuteNonQuery();
                cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                throw ex;
            }
        }

        public void insert(User entity)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = Connection.getConnection;
            cmd.Transaction = Connection.getConnection.BeginTransaction();
            cmd.ExecuteNonQuery();

            string sql = GenerateSQL.getInsert(entity);

            cmd.CommandText = sql;
            cmd.Parameters.Add(new NpgsqlParameter("@lastName", entity.lastName));
            cmd.Parameters.Add(new NpgsqlParameter("@firstName", entity.firstName));
            cmd.Parameters.Add(new NpgsqlParameter("@email", entity.email));

            try
            {
                cmd.ExecuteNonQuery();
                cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                throw ex;
            }
        }

        public void update(User entity)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = Connection.getConnection;
            cmd.Transaction = Connection.getConnection.BeginTransaction();
            cmd.ExecuteNonQuery();

            string sql = GenerateSQL.getUpdate(entity);

            cmd.CommandText = sql;
            cmd.Parameters.Add(new NpgsqlParameter("@code", entity.code));
            cmd.Parameters.Add(new NpgsqlParameter("@lastName", entity.lastName));
            cmd.Parameters.Add(new NpgsqlParameter("@firstName", entity.firstName));
            cmd.Parameters.Add(new NpgsqlParameter("@email", entity.email));

            try
            {
                cmd.ExecuteNonQuery();
                cmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                cmd.Transaction.Rollback();
                throw ex;
            }
        }

        public User get(int code)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = Connection.getConnection;

            string sql = GenerateSQL.getSqlById(new User());

            cmd.CommandText = sql;
            cmd.Parameters.Add(new NpgsqlParameter("@code", code));

            DataTable dt = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            da.Fill(dt);

            User entity = new User();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.Table.Columns.Contains("users_code"))
                    if (dr["users_code"] != DBNull.Value)
                        entity.code = Convert.ToInt32(dr["users_code"].ToString());

                if (dr.Table.Columns.Contains("users_lastName"))
                    if (dr["users_lastName"] != DBNull.Value)
                        entity.lastName = dr["users_lastName"].ToString();

                if (dr.Table.Columns.Contains("users_firstName"))
                    if (dr["users_firstName"] != DBNull.Value)
                        entity.firstName = dr["users_firstName"].ToString();

                if (dr.Table.Columns.Contains("users_email"))
                    if (dr["users_email"] != DBNull.Value)
                        entity.email = dr["users_email"].ToString();
            }
            return entity;
        }

        public List<User> list()
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = Connection.getConnection;

            string sql = GenerateSQL.getSql(new User());

            cmd.CommandText = sql;
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            da.Fill(dt);

            List<User> lst = new List<User>();
            foreach (DataRow dr in dt.Rows)
            {
                User entity = new User();

                if (dr.Table.Columns.Contains("users_code"))
                    if (dr["users_code"] != DBNull.Value)
                        entity.code = Convert.ToInt32(dr["users_code"].ToString());

                if (dr.Table.Columns.Contains("users_lastName"))
                    if (dr["users_lastName"] != DBNull.Value)
                        entity.lastName = dr["users_lastName"].ToString();

                if (dr.Table.Columns.Contains("users_firstName"))
                    if (dr["users_firstName"] != DBNull.Value)
                        entity.firstName = dr["users_firstName"].ToString();

                if (dr.Table.Columns.Contains("users_email"))
                    if (dr["users_email"] != DBNull.Value)
                        entity.email = dr["users_email"].ToString();
 
                lst.Add(entity);
            }
            return lst;
        }

        public List<User> listAll()
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = Connection.getConnection;

            string sql = GenerateSQL.getSqlAll(new User());

            cmd.CommandText = sql;
            DataTable dt = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            da.Fill(dt);

            List<User> lst = new List<User>();
            foreach (DataRow dr in dt.Rows)
            {
                User entity = new User();

                if (dr.Table.Columns.Contains("users_code"))
                    if (dr["users_code"] != DBNull.Value)
                        entity.code = Convert.ToInt32(dr["users_code"].ToString());

                if (dr.Table.Columns.Contains("users_lastName"))
                    if (dr["users_lastName"] != DBNull.Value)
                        entity.lastName = dr["users_lastName"].ToString();

                if (dr.Table.Columns.Contains("users_firstName"))
                    if (dr["users_firstName"] != DBNull.Value)
                        entity.firstName = dr["users_firstName"].ToString();

                if (dr.Table.Columns.Contains("users_email"))
                    if (dr["users_email"] != DBNull.Value)
                        entity.email = dr["users_email"].ToString();

                lst.Add(entity);
            }
            return lst;
        }
    }
```
  
Exemplo da classe de conexão, foi utilizado o [npgsql] para realizar a conexão com o banco de dados Postgres.

```C#
 public class Connection
    {
        private static NpgsqlConnection con;
        private static NpgsqlTransaction transaction;

        public static NpgsqlConnection getConnection
        {
            get
            {
                return con;
            }
        }
        public static void Conectar(string connString)
        {
            //Exemplo da string de conexão 
            //"string.Format("Server={0};Port={1};Database={2};User Id={3};Password={4};", server, port, database, userId, userPassword);
            con = new NpgsqlConnection(connString);
            try
            {
                con.Open();
            }
            catch (Exception exe)
            {
                throw exe; 
            }
        }

        public void OpenTransaction()
        {
            try
            {
                transaction = getConnection.BeginTransaction();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public void CommitTransaction()
        {
            try
            {
                transaction.Commit();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }

        public void RollBackTransaction()
        {
            try
            {
                transaction.Rollback();
            }
            catch (Exception exe)
            {
                throw exe;
            }
        }
    }
```

Para ter acesso a dll **DefaultSQL.dll**, faça o clone do projeto ou download, e acesse a pasta [release], adicione a referência ou se preferir adicione o projeto para sua **Solution**. DefaultSQL foi desenvolvido utilizando o Visual Studio 2013.

Qualquer sugestões de melhorias ou PR serão bem-vindas.

[npgsql]:http://www.npgsql.org/
[release]:https://github.com/FernandoCagale/DefaultSQL/tree/master/bin/Release
