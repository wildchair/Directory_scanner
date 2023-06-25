using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

/*Недостатком данного решения является привязка к расширению файла, что может привести к некорректному
 анализу MimeType файлов, чьи расширения были изменены. Альтернативным вариантом решения был бы побитовый
 анализ файлов с составлением собственного словаря сопоставлений. Исходя из ТЗ считаю допустимым 
 использование встроенных средств из System.Web.MimeMapping*/

namespace Directory_scanner
{
    internal class Program
    {
        static void Main()
        {
            var baseDir = Directory.GetCurrentDirectory();
            var result = Scan(new DirectoryInfo(baseDir));

            //удаление из коллекции кортежей информации о файле самого приложения
            var buff = result[0];
            buff.weight -= (ulong)new FileInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).Length;
            buff.files = buff.files.Where(f => f.FullName != System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName).ToArray();
            result[0] = buff;

            Print(result);

            List<FileInfo> allFiles = new List<FileInfo>();

            foreach (var item in result)
                allFiles.AddRange(item.files);

            var mimeGrouped = allFiles.GroupBy(x => MimeMapping.GetMimeMapping(x.Name));

            PrintStat(mimeGrouped);

            Console.ReadKey(true);
        }

        static List<(DirectoryInfo dir, ulong weight, FileInfo[] files)> Scan(DirectoryInfo baseDir)
        {
            (DirectoryInfo dir, ulong weight, FileInfo[] files) currentDir = (dir: baseDir, weight: 0, files: baseDir.GetFiles());

            foreach (var item in currentDir.files)//подсчет размера файлов этой директории
                currentDir.weight += (ulong)item.Length;

            var scannedDirs = new List<(DirectoryInfo dir, ulong weight, FileInfo[] files)>() { };

            foreach (var item in baseDir.GetDirectories())//получение коллекции поддиректорий
                scannedDirs.AddRange(Scan(item));

            foreach (var dir in scannedDirs)//включение в занимаемый объем поддиректорий
                currentDir.weight += (ulong)dir.weight;

            var result = new List<(DirectoryInfo dir, ulong weight, FileInfo[] files)>() { currentDir };
            result.AddRange(scannedDirs);

            return result;
        }

        static void Print(List<(DirectoryInfo dir, ulong weight, FileInfo[] files)> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine($"---/ Каталог: {item.dir.FullName} Размер: {item.weight} байт /--- \n");
                foreach (var file in item.files)
                    Console.WriteLine($"{file.Name} Размер: {file.Length} байт -> {MimeMapping.GetMimeMapping(file.Name)}");
                Console.WriteLine();
            }
        }
        static void PrintStat(IEnumerable<IGrouping<string, FileInfo>> groupedList)
        {
            Console.WriteLine("---/ Статистика по MimeType /---\n");
            uint num = 0;
            foreach (var group in groupedList)
                num += (uint)group.Count();
            foreach (var group in groupedList)
            {
                Console.WriteLine(group.Key);
                double mediumSize = 0;
                foreach (var item in group)
                    mediumSize += item.Length;
                mediumSize /= group.Count();
                Console.WriteLine($"Средний размер = {mediumSize} байт");
                Console.WriteLine($"На данный тип приходится {group.Count()*100/num}% файлов ({group.Count()}/{num})\n");

            }
        }


    }
}
