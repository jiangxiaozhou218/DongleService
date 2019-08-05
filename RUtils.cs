using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HongshiConsoleApp
{
    class RUtils
    {
        public static string Encrypt(byte[] key, string str)
        {//加密字符串

            try
            {
                if (str == "" || str == null)
                {
                    return "";
                }
                byte[] data = Encoding.Unicode.GetBytes(str);//待加密字符串

                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();//加密、解密对象
                MemoryStream MStream = new MemoryStream();//内存流对象

                //用内存流实例化加密流对象
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);
                CStream.Write(data, 0, data.Length);//向加密流中写入数据
                CStream.FlushFinalBlock();//将数据压入基础流
                byte[] temp = MStream.ToArray();//从内存流中获取字节序列
                CStream.Close();//关闭加密流
                MStream.Close();//关闭内存流

                return Convert.ToBase64String(temp);//返回加密后的字符串
            }
            catch (Exception ex)
            {
                return str;
            }
        }


        public static string Decrypt(byte[] key, string str)
        {//解密字符串

            try
            {
                if (str == "" || str == null)
                {
                    return "";
                }
                byte[] data = Convert.FromBase64String(str);//待解密字符串

                DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();//加密、解密对象
                MemoryStream MStream = new MemoryStream();//内存流对象

                //用内存流实例化解密流对象
                CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);
                CStream.Write(data, 0, data.Length);//向加密流中写入数据
                CStream.FlushFinalBlock();//将数据压入基础流
                byte[] temp = MStream.ToArray();//从内存流中获取字节序列
                CStream.Close();//关闭加密流
                MStream.Close();//关闭内存流

                return Encoding.Unicode.GetString(temp);//返回解密后的字符串
            }
            catch (Exception ex)
            {
                return str;
            }
        }
        ///编码
        public static string EncodeBase64(byte[] code)
        {
            string encode = "";
            try
            {
                encode = Convert.ToBase64String(code);
            }
            catch
            {
            }
            return encode;
        }
        ///解码
        public static byte[] DecodeBase64(string code)
        {
            byte[] bytes = null;
            try
            {
                bytes = Convert.FromBase64String(code);
            }
            catch
            {
            }
            return bytes;
        }
        public static String ByteToString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return "";
            }
            int i = 0;
            int count = 0;
            for (; i < bytes.Length; i++)
            {
                if (bytes[i] == 0 || bytes[i] == 255)
                {
                    count++;
                } else
                {
                    count = 0;
                }
                if (count > 0)
                {
                    break;
                }

            }
            int idx = bytes.Length;
            if (i < bytes.Length-1)
            {
                idx = i;
            }
            byte[] results = new byte[idx];
            for (i = 0; i < idx; i++)
            {
                if (bytes[i] != 0)
                {
                    results[i] = bytes[i];
                }
                else
                {
                    break;
                }

            }
            return Encoding.UTF8.GetString(results);
        }
    }
}
