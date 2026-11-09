using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SofiaFe_Act2
{
    public sealed class UserAccount
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
    }

    internal static class Database
    {
        private static string FindDatabasePath()
        {
            DirectoryInfo directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (directory != null)
            {
                string candidate = Path.Combine(directory.FullName, "Users.accdb");

                if (File.Exists(candidate))
                    return candidate;

                directory = directory.Parent;
            }

            throw new FileNotFoundException(
                "Users.accdb was not found.");
        }

        private static OleDbConnection OpenConnection()
        {
            string databasePath = FindDatabasePath();
            string[] providers = { "Microsoft.ACE.OLEDB.16.0", "Microsoft.ACE.OLEDB.12.0" };
            Exception lastError = null;

            foreach (string provider in providers)
            {
                OleDbConnection connection = new OleDbConnection(
                    "Provider=" + provider + ";Data Source=" + databasePath + ";Persist Security Info=False;");

                try
                {
                    connection.Open();
                    ValidateUsersTable(connection);
                    return connection;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    connection.Dispose();
                }
            }

            throw new InvalidOperationException(
                "The Access database could not be opened. " +
                (lastError == null ? "Unknown database error." : lastError.Message), lastError);
        }

        private static void ValidateUsersTable(OleDbConnection connection)
        {
            DataTable tables = connection.GetSchema("Tables");

            foreach (DataRow row in tables.Rows)
            {
                if (string.Equals(Convert.ToString(row["TABLE_NAME"]), "Users", StringComparison.OrdinalIgnoreCase))
                    return;
            }

            throw new InvalidOperationException(
                "The Users table does not exist in Users.accdb. Open the database in Microsoft Access and create the Users");
        }

        public static void CreateAccount(string fullName, string username, string password)
        {
            using (OleDbConnection connection = OpenConnection())
            {
                using (OleDbCommand check = new OleDbCommand("SELECT COUNT(*) FROM Users WHERE Username = ?", connection))
                {
                    check.Parameters.Add("?", OleDbType.VarWChar, 50).Value = username;

                    if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                        throw new InvalidOperationException("That username is already registered. Please choose another one.");
                }

                using (OleDbCommand command = new OleDbCommand(
                    "INSERT INTO Users (FullName, Username, PasswordHash, CreatedAt) VALUES (?, ?, ?, ?)", connection))
                {
                    command.Parameters.Add("?", OleDbType.VarWChar, 100).Value = fullName;
                    command.Parameters.Add("?", OleDbType.VarWChar, 50).Value = username;
                    command.Parameters.Add("?", OleDbType.VarWChar, 64).Value = HashPassword(password);
                    command.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;
                    command.ExecuteNonQuery();
                }
            }
        }

        public static UserAccount Authenticate(string username, string password)
        {
            using (OleDbConnection connection = OpenConnection())
            using (OleDbCommand command = new OleDbCommand(
                "SELECT UserId, FullName, Username FROM Users WHERE Username = ? AND PasswordHash = ?", connection))
            {
                command.Parameters.Add("?", OleDbType.VarWChar, 50).Value = username;
                command.Parameters.Add("?", OleDbType.VarWChar, 64).Value = HashPassword(password);

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return new UserAccount
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        FullName = Convert.ToString(reader["FullName"]),
                        Username = Convert.ToString(reader["Username"])
                    };
                }
            }
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder result = new StringBuilder(bytes.Length * 2);

                foreach (byte value in bytes)
                    result.Append(value.ToString("x2"));

                return result.ToString();
            }
        }
    }
}
