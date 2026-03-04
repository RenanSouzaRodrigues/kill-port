namespace KillPort;

class Program {
    private static void Main(string[] args) {
        if (args.Length == 0) {
            Console.WriteLine("No port provided. Please provide a port");
            return;
        }

        var port = int.Parse(args[0]);

        if (!Handler.KillPort(port)) {
            Console.WriteLine($"No process found for the port {port}. Aborting!");
            return;
        }
        
        Console.WriteLine($"Process running or port {port} as killed!");
    }
}