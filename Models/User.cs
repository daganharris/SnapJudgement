using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Linq.Expressions;

namespace SnapJudgement.Models
{
    [Table("tblUsers")]
    public class User
    {
        private string username;
        public static User currentUser;
        private string passwordHash;
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }
        [NotNull, MaxLength(20), Unique]
        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
            }
        }
        [NotNull]
        public string PasswordHash
        {
            get
            {
                return passwordHash;
            }
            set
            {
                passwordHash = value;
            }
        }
        public User() { } // default constructor for SQLite
        public User(string username, string password)
        {
            Username = username;
            PasswordHash = HashPassword(password);
        }
        public static UserStatus VerifyPassword(string username, string password)
        {
            try
            {
                SQLiteConnection database = new SQLiteConnection(ContestantsViewModel.DatabasePath);
                database.CreateTable<User>();

                var dbUser = database.Table<User>().FirstOrDefault(u => u.Username == username);
                if (dbUser == null)
                {
                    return UserStatus.PasswordUserInvalid;
                }

                string inputHash = HashPassword(password);
                Debug.WriteLine($"[DEBUG] InputHash: {inputHash}");
                Debug.WriteLine($"[DEBUG] StoredHash: {dbUser.PasswordHash}");

                if (inputHash == dbUser.PasswordHash)
                {
                    currentUser = dbUser;
                    return UserStatus.PasswordCorrect;
                }
                else
                {
                    return UserStatus.PasswordIncorrect;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DBERROR: Exception thrown while verifying password:\n{ex}");
                return UserStatus.LoadFail;
            }
        }

        public static bool IsValidUsername(string username)
        {
            return !string.IsNullOrEmpty(username) && username.All(c => char.IsLetterOrDigit(c));
        }
        public static bool IsValidPassword(string password, string? confirmPassword = null)
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 8 && (password == confirmPassword || confirmPassword == null);
        }
        public static UserStatus IsUsernameTaken(string username)
        {
            try
            {
                SQLiteConnection database = new SQLiteConnection(ContestantsViewModel.DatabasePath);
                var existingUser = from user in database.Table<User>()
                                   where user.Username == username
                                   select user;
                if (existingUser.Count() == 0)
                {
                    return UserStatus.UserNotTaken;
                }
                return UserStatus.UserTaken;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DBERROR: Exception thrown while loading from database:\n{ex.ToString()}");
                return UserStatus.LoadFail;
            }
        }
        public static UserStatus CreateUser(User user)
        {
            try
            {
                SQLiteConnection database = new SQLiteConnection(ContestantsViewModel.DatabasePath);
                database.CreateTable<User>();

                User existingUser = database.Table<User>().FirstOrDefault(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    return UserStatus.UserTaken;
                }

                database.Insert(user);
                Debug.WriteLine($"[DEBUG] Inserted User: {user.Username}, Hash: {user.PasswordHash}");
                currentUser = user;
                return UserStatus.CreateSuccess;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DBERROR: Exception thrown while loading from database:\n{ex.ToString()}");
                return UserStatus.CreateFail;
            }

        }
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                string hashed = builder.ToString();
                return hashed;
            }
        }
    }
}
