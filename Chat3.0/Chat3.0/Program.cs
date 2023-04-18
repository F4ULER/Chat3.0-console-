using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

string host = "127.0.0.1";
int port = 8888;
TcpClient client = new TcpClient();
Console.Write("Введите свое имя: ");
string? userName = Console.ReadLine();
Console.WriteLine($"Добро пожаловать, {userName}");
StreamReader? Reader = null;
StreamWriter? Writer = null;

try
{
    client.Connect(host, port); //подключение клиента
    Reader = new StreamReader(client.GetStream());
    Writer = new StreamWriter(client.GetStream());
    if (Writer is null || Reader is null) return;
    // запускаем новый поток для получения данных
    Task.Run(() => ReceiveMessageAsync(Reader));
    // запускаем ввод сообщений
    await SendMessageAsync(Writer);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
Writer?.Close();
Reader?.Close();

// отправка сообщений
async Task SendMessageAsync(StreamWriter writer)
{
    //  отправляем имя
    await writer.WriteLineAsync(userName);
    await writer.FlushAsync();
    Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");

    while (true)
    {
        string? message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }
}
// получение сообщений
async Task ReceiveMessageAsync(StreamReader reader)
{
    while (true)
    {
        try
        {
            // считываем ответ в виде строки
            string? message = await reader.ReadLineAsync();
            // если пустой ответ, ничего не выводим на консоль
            if (string.IsNullOrEmpty(message)) continue;
            Console.WriteLine(message);//вывод сообщения
        }
        catch
        {
            break;
        }
    }
}
