﻿/*
 * MIT License
 *
 * Copyright(c) 2019 KeLi
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/*
             ,---------------------------------------------------,              ,---------,
        ,----------------------------------------------------------,          ,"        ,"|
      ,"                                                         ,"|        ,"        ,"  |
     +----------------------------------------------------------+  |      ,"        ,"    |
     |  .----------------------------------------------------.  |  |     +---------+      |
     |  | C:\>FILE -INFO                                     |  |  |     | -==----'|      |
     |  |                                                    |  |  |     |         |      |
     |  |                                                    |  |  |/----|`---=    |      |
     |  |              Author: KeLi                          |  |  |     |         |      |
     |  |              Email: kelistudy@163.com              |  |  |     |         |      |
     |  |              Creation Time: 10/30/2019 07:08:41 PM |  |  |     |         |      |
     |  | C:\>_                                              |  |  |     | -==----'|      |
     |  |                                                    |  |  |   ,/|==== ooo |      ;
     |  |                                                    |  |  |  // |(((( [66]|    ,"
     |  `----------------------------------------------------'  |," .;'| |((((     |  ,"
     +----------------------------------------------------------+  ;;  | |         |,"
        /_)_________________________________________________(_/  //'   | +---------+
           ___________________________/___  `,
          /  oooooooooooooooo  .o.  oooo /,   \,"-----------
         / ==ooooooooooooooo==.o.  ooo= //   ,`\--{)B     ,"
        /_==__==========__==_ooo__ooo=_/'   /___________,"
*/

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace KeLi.Power.Tool.Security
{
    /// <summary>
    ///     DES encrypt.
    /// </summary>
    public class DesEncrypt
    {
        private static readonly string Key = GenerateKey();

        /// <summary>
        ///     Encrypts the content.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string content, string key = null)
        {
            if (content is null)
                throw new ArgumentNullException(nameof(content));

            if (string.IsNullOrWhiteSpace(key))
                key = Key;

            var bytes = Encoding.UTF8.GetBytes(content);

            var dcsp = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(key), IV = Encoding.ASCII.GetBytes(key) };

            var ct = dcsp.CreateEncryptor();

            var marks = ct.TransformFinalBlock(bytes, 0, bytes.Length);

            return BitConverter.ToString(marks);
        }

        /// <summary>
        ///     Decrypts the content.
        /// </summary>
        /// <param name="ciphertext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string ciphertext, string key = null)
        {
            if (ciphertext is null)
                throw new ArgumentNullException(nameof(ciphertext));

            if (string.IsNullOrWhiteSpace(ciphertext))
                return null;

            if (string.IsNullOrWhiteSpace(Key))
                return null;

            if (string.IsNullOrWhiteSpace(key))
                key = Key;

            var marks = ciphertext.Split("-".ToCharArray());

            var bytes = new byte[marks.Length];

            for (var i = 0; i < marks.Length; i++)
                bytes[i] = byte.Parse(marks[i], NumberStyles.HexNumber);

            var dcsp = new DESCryptoServiceProvider { Key = Encoding.ASCII.GetBytes(key), IV = Encoding.ASCII.GetBytes(key) };

            bytes = dcsp.CreateDecryptor().TransformFinalBlock(bytes, 0, bytes.Length);

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        ///     Generates the secret key.
        /// </summary>
        /// <returns></returns>
        private static string GenerateKey()
        {
            var des = (DESCryptoServiceProvider)DES.Create();

            return Encoding.ASCII.GetString(des.Key);
        }
    }
}