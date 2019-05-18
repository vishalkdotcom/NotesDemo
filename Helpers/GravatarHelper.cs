using System.Security.Cryptography;
using System.Text;

namespace NotesDemo.Helpers
{
    public static class GravatarHelper
    {
        public static string GetAvatarUrl(string email)
        {
            string hash = HashEmail(email);
            return string.Format("http://www.gravatar.com/avatar/{0}", hash);
        }

        /// Hashes an email with MD5.  Suitable for use with Gravatar profile
        /// image urls
        private static string HashEmail(string email)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.  
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));

            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string. 
        }
    }
}