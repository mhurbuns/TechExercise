using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exercise1;

public class Example
{
    public static void Main()
    {
        DownloadService downloadService = new DownloadService();

        Console.WriteLine("Press any key to start the download. Press 'c' to cancel.");
        Console.ReadKey();


        Task.Factory.StartNew(() =>
        {
            if (Console.ReadKey().KeyChar == 'c')
                if (downloadService.Cancel())
                {
                    Console.WriteLine("press any key to exit");
                }
        });

        IEnumerable<string> urlList = Enumerable.Repeat("https://picsum.photos/200/300/?random", 5);
        int totalLength = downloadService.DownLoad(urlList.AsEnumerable()).Result;

        Console.WriteLine($"total length: {totalLength}");

        Console.ReadKey();
    }
}