using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace DiplomovaPrace
{
    public class SQLPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string heslo)
        {
            return base.HashPassword(heslo);
        }

        public override PasswordVerificationResult VerifyHashedPassword(string zasifrovaneHeslo, string zadaneHeslo)
        {
            string[] passwordProperties = zasifrovaneHeslo.Split('|');
            if (passwordProperties.Length != 3)
            {
                return base.VerifyzasifrovaneHeslo(zasifrovaneHeslo, zadaneHeslo);
            }
            else
            {
                string passwordHash = passwordProperties[0];
                int hesloFormat = 1;
                string salt = passwordProperties[2];
                if (String.Equals(EncryptPassword(zadaneHeslo, hesloFormat, salt), passwordHash, StringComparison.CurrentCultureIgnoreCase))
                {
                    return PasswordVerificationResult.SuccessRehashNeeded;
                }
                else
                {
                    return PasswordVerificationResult.Failed;
                }
            }
        }

        private string EncryptPassword(string heslo, int hesloFormat, string salt)
        {
            if (hesloFormat == 0) // MembershipPasswordFormat.Clear
                return heslo;

            byte[] bIn = System.Text.Encoding.Unicode.GetBytes(heslo);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bRet = null;

            if (hesloFormat == 1)
            { // MembershipPasswordFormat.Hashed 
                HashAlgorithm hm = HashAlgorithm.Create("SHA1");
                if (hm is KeyedHashAlgorithm)
                {
                    KeyedHashAlgorithm kha = (KeyedHashAlgorithm)hm;
                    if (kha.Key.Length == bSalt.Length)
                    {
                        kha.Key = bSalt;
                    }
                    else if (kha.Key.Length < bSalt.Length)
                    {
                        byte[] bKey = new byte[kha.Key.Length];
                        Buffer.BlockCopy(bSalt, 0, bKey, 0, bKey.Length);
                        kha.Key = bKey;
                    }
                    else
                    {
                        byte[] bKey = new byte[kha.Key.Length];
                        for (int iter = 0; iter < bKey.Length;)
                        {
                            int len = Math.Min(bSalt.Length, bKey.Length - iter);
                            Buffer.BlockCopy(bSalt, 0, bKey, iter, len);
                            iter += len;
                        }
                        kha.Key = bKey;
                    }
                    bRet = kha.ComputeHash(bIn);
                }
                else
                {
                    byte[] bAll = new byte[bSalt.Length + bIn.Length];
                    Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
                    Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
                    bRet = hm.ComputeHash(bAll);
                }
            }

            return Convert.ToBase64String(bRet);
        }
    }
}