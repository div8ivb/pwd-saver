﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PasswordSaver
{
    public class FileManager
    {
        public static StorageFolder RoamingFolder = ApplicationData.Current.RoamingFolder;
        public static ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;

        //将对象存到jsonstring中去
        public static string GetJsonString<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        //从jsonstring中得到想要的对象
        public static T ReadFromJson<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        //将str写入RoamingData存储区中
        public async static Task WriteToRoamingDataAsync(string str)
        {
            //Debug.WriteLine(ApplicationData.Current.RoamingStorageQuota);
            StorageFile savedFile = await RoamingFolder.CreateFileAsync("dataFile", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(savedFile, str);

            //测试数据量是否超过100k
            //StorageFolder storageFolder = KnownFolders.PicturesLibrary;
            //StorageFile sfile = await storageFolder.CreateFileAsync("a", CreationCollisionOption.ReplaceExisting);
            //await FileIO.WriteTextAsync(sfile, str);
        }

        //读出RoamingData存储区中的string
        public async static Task<string> ReadRoamingDataAsync()
        {
            try
            {
                StorageFile savedFile = await RoamingFolder.GetFileAsync("dataFile");
                return await FileIO.ReadTextAsync(savedFile);
            }
            catch
            {
                return "-1";
            }
        }

        //备份当前数据，将str写到本地存储中
        public async static Task BackupAsync(string str)
        {
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile sfile = await storageFolder.CreateFileAsync("pwsv.pwsv", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(sfile, str);
        }

        //读出备份数据
        public async static Task<string> ReadBackupAsync()
        {
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            try
            {
                StorageFile sfile = await storageFolder.GetFileAsync("pwsv.pwsv");
                string str = await FileIO.ReadTextAsync(sfile);
                return str;
            }
            catch
            {
                return "-1";
            }
        }

        //将密码写入存储区
        public static void WriteCode(string pwd)
        {
            WriteSetting("Code", EncryptHelper.PwdEncrypt(pwd));
        }

        //写入设置
        private static void WriteSetting(string set, string value)
        {
            RoamingSettings.Values[set] = value;
        }

        //读出密码
        public static string GetCode()
        {
            string str = (String)RoamingSettings.Values["Code"];
            if (!String.IsNullOrEmpty(str))
                return str;
            else
                return EncryptHelper.PwdEncrypt("123");
        }
    }
    /*
        Windows.Storage.ApplicationDataContainer roamingSettings = 
            Windows.Storage.ApplicationData.Current.RoamingSettings;
        Windows.Storage.StorageFolder roamingFolder = 
            Windows.Storage.ApplicationData.Current.RoamingFolder;

        创建和检索漫游设置
        使用 ApplicationDataContainer.Values 属性访问我们在前一部分中获取的 roamingSettings 容器中的设置。此示例将创建名为 exampleSetting 的简单设置和名为 composite 的复合值。
        C#

        // Simple setting

        roamingSettings.Values["exampleSetting"] = "Hello World";
        // High Priority setting, for example, last page position in book reader app
        roamingSettings.values["HighPriority"] = "65";

        // Composite setting

        Windows.Storage.ApplicationDataCompositeValue composite = 
            new Windows.Storage.ApplicationDataCompositeValue();
        composite["intVal"] = 1;
        composite["strVal"] = "string";

        roamingSettings.Values["exampleCompositeSetting"] = composite;



        此示例将检索刚创建的设置。
        C#

        // Simple setting

        Object value = roamingSettings.Values["exampleSetting"];

        // Composite setting

        Windows.Storage.ApplicationDataCompositeValue composite = 
           (Windows.Storage.ApplicationDataCompositeValue)roamingSettings.Values["exampleCompositeSetting"];

        if (composite == null)
        {
           // No data
        }
        else
        {
           // Access data in composite["intVal"] and composite["strVal"]
        }

    */
}
