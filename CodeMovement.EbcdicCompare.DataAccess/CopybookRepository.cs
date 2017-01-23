using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using CodeMovement.EbcdicCompare.DataAccess.Model;
using CodeMovement.EbcdicCompare.Models;
using CodeMovement.EbcdicCompare.Models.Result;

namespace CodeMovement.EbcdicCompare.DataAccess
{
    public class CopybookRepository : ICopybookRepository
    {
        #region "SQL Statements"

        private const string InsertCopybookEbcdicFileAssociationSql =
            "INSERT INTO CopybookEbcdicFileAssociation VALUES(@copybookFilePath, @ebcdicFileName)";

        private const string DeleteCopybookEbcdicFileAssociationSql =
            "DELETE FROM CopybookEbcdicFileAssociation WHERE copybook_file_path = @copybookFilePath AND ebcdic_file_name = @ebcdicFileName";

        private const string DeleteCopybookSql =
            "DELETE FROM CopybookEbcdicFileAssociation WHERE copybook_file_path = @copybookFilePath";

        private const string SelectCopybookForEbcdicFile =
            "SELECT copybook_file_path FROM CopybookEbcdicFileAssociation WHERE ebcdic_file_name = @ebcdicFileName";

        private const string SelectAllCopybookEbcdicFileAssociationsSql =
            "SELECT copybook_file_path, ebcdic_file_name FROM CopybookEbcdicFileAssociation ORDER BY copybook_file_path";

        #endregion

        public OperationResult<bool> AddCopybookEbcdicFileAssociation(string copybookName, string ebcdicFileName)
        {
            var copybookNameParam = new SQLiteParameter("@copybookFilePath", copybookName);
            var ebcdicFileNameParam = new SQLiteParameter("@ebcdicFileName", ebcdicFileName);

            return ExecuteNonQuery(InsertCopybookEbcdicFileAssociationSql, copybookNameParam, ebcdicFileNameParam);
        }

        public OperationResult<bool> DeleteCopybookFileAssociation(string copybookName, string ebcdicFileName)
        {
            var copybookNameParam = new SQLiteParameter("@copybookFilePath", copybookName);
            var ebcdicFileNameParam = new SQLiteParameter("@ebcdicFileName", ebcdicFileName);

            return ExecuteNonQuery(DeleteCopybookEbcdicFileAssociationSql, copybookNameParam, ebcdicFileNameParam);
        }

        public OperationResult<bool> DeleteCopybook(string copybookName)
        {
            var copybookNameParameter = new SQLiteParameter("@copybookFilePath", copybookName);
            return ExecuteNonQuery(DeleteCopybookSql, copybookNameParameter);
        }

        public OperationResult<string> GetCopybookPathForEbcdicFile(string ebcdicFileName)
        {
            var ebcdicFileNameParameter = new SQLiteParameter("@ebcdicFileName", ebcdicFileName);
            var queryResult = ExecuteScalar(SelectCopybookForEbcdicFile, ebcdicFileNameParameter);

            return OperationResult<string>.CreateResult(queryResult as string);
        }

        public IEnumerable<CopybookAssociation> GetCopybooks()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = SelectAllCopybookEbcdicFileAssociationsSql;

                    var reader = cmd.ExecuteReader();
                    var associations = ReadCopybookAssociations(reader);
                    var copybooks = BuildCopybooksFromAssociations(associations);

                    return copybooks;
                }
            }
        }

        #region "Helper Methods"

        private static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["CopybookDb"].ConnectionString;
            }
        }

        private static object ExecuteScalar(string sqlStatement, params SQLiteParameter[] parameters)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = SelectCopybookForEbcdicFile;
                    cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteScalar();
                }
            }
        }

        private static OperationResult<bool> ExecuteNonQuery(string sqlStatement, params SQLiteParameter[] parameters)
        {
            var result = new OperationResult<bool>();

            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = sqlStatement;
                            cmd.Parameters.AddRange(parameters);

                            var affectedRecords = cmd.ExecuteNonQuery();
                            result.Result = affectedRecords > 0;
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Messages.Add(ex.Message);
            }

            return result;
        }

        private static IEnumerable<CopybookEbcdicFilePair> ReadCopybookAssociations(SQLiteDataReader reader)
        {
            var associations = new List<CopybookEbcdicFilePair>();

            while (reader.Read())
            {
                associations.Add(new CopybookEbcdicFilePair
                {
                    CopybookFilePath = reader["copybook_file_path"] as string,
                    EbcdicFileName = reader["ebcdic_file_name"] as string
                });
            }
            return associations;
        }

        private static IEnumerable<CopybookAssociation> BuildCopybooksFromAssociations(
            IEnumerable<CopybookEbcdicFilePair> associations)
        {
            return associations.GroupBy(association => association.CopybookFilePath,
                (key, g) => new CopybookAssociation
                {
                    FilePath = key,
                    AssociatedFiles = g.Select(a => a.EbcdicFileName).ToList()
                });
        }

        #endregion
    }
}