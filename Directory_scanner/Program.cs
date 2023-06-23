using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

/*Недостатком данного решения является привязка к расширению файла, что может привести к некорректному
 анализу MimeType файлов, чьи расширения были изменены. Альтернативным вариантом решения был бы побитовый
 анализ файлов с составлением собственного словаря сопоставлений. Исходя из ТЗ считаю допустимым 
 использование встроенных средств из System.Web.MimeMapping*/

namespace Directory_scanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(MimeMapping.GetMimeMapping("C:\\ScheduleMADI\\LICENSE.txt"));
        }
    }
}
