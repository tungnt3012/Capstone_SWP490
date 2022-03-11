using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Capstone_SWP490.Helper
{
    public class CommonHelper
    {
        public static string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        public static string createEncryptedPassWord(string plainText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            // Compute hash from the bytes of text
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(plainText));
            // Get hash result after compute it
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
        public static int toInt32(string s, int valueIfAbsent)
        {
            try
            {
                return Int32.Parse(s);
            }
            catch
            {
                return valueIfAbsent;
            }
        }
        public static short toShort(string s, short valueIfAbsent)
        {
            try
            {
                return short.Parse(s);
            }
            catch
            {
                return valueIfAbsent;
            }
        }

        public static DateTime toDateTime(string s)
        {
            try
            {
                return DateTime.Parse(s);
            }
            catch
            {
                return new DateTime();
            }
        }
    }
}