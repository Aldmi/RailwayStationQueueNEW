using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Logs
{
    /// <summary>
    /// Записывет список строк порциями в файл на диск.
    /// При превышении кол-ва порция файл пишется с 0 позиции.
    /// Старые данные сохраняются ниже.
    /// </summary>
    public class Log
    {
        private readonly string _path;
        private readonly int _portionString;              // порция строк для записи
        private readonly int _countPortion;               // количесво порций строк в файле. При превышении файл перезаписывается с 0 порции.


        private List<string> List { get; set; } = new List<string>();
        public int Seek { get; set; }




        public Log(string filename, int portionString, int countPortion)
        {
            _path = Path.Combine(Directory.GetCurrentDirectory() , $"Logs\\{filename}");
            _portionString = portionString;
            _countPortion = countPortion;
        }

        //public Log(string filename, XmlLogSettings settings) : this(filename, settings.PortionString, settings.CountPortion)
        //{ 
        //}




        public async Task Add(string str)
        {
            if(string.IsNullOrEmpty(str))
                return;

            List.Add(str);

            if (List.Count >= _portionString)
            {
                await WriteBufferString();
                List.Clear();
            }
        }

        private async Task WriteBufferString()
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                   
                    if (!File.Exists(_path))
                        File.Create(_path);

                    int buffSize = _portionString * _countPortion;
                    string[] buffString = new string[buffSize];

                    var readedString = File.ReadAllLines(_path);
                    Array.Copy(readedString, buffString, readedString.Length);

                    var count= readedString.Count(str => str.Length>1);//DEBUG
                    if (count < buffSize)
                    {
                        Seek = count;
                    }
                    Array.Copy(List.ToArray(), 0, buffString, Seek, _portionString);
                    if ((Seek += _portionString) > (buffSize - _portionString) || readedString.Length > buffSize)
                        Seek = 0;

                    File.WriteAllLines(_path, buffString);
                    
                });
            }
            catch (Exception ex)
            {  
                //не обработанное исключение        
            }
        }
    }
}