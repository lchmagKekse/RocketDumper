using RocketDumper.Engine;

namespace RocketDumper
{
    public class RocketDumper
    {
        public static string NamesPath = "GNames.txt";
        public static string ObjectsPath = "GObjects.txt";

        public static IntPtr BaseAddress;
        public static IntPtr GNamesAddress;
        public static IntPtr GObjectsAddress;
        public static IntPtr GNamesOffset;
        public static IntPtr GObjectsOffset;

        public static Dictionary<int, string> FNames = new();
        public static MemoryHelper RL = new();

        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Title = "Rocket League Dumper";
            Console.CursorVisible = false;

            File.Delete(NamesPath);
            File.Delete(ObjectsPath);

            BaseAddress = RL.BaseAddress;
            GNamesAddress = RL.FindPattern("?? ?? ?? ?? ?? ?? 00 00 ?? ?? 01 00 35 25 02 00");
            GObjectsAddress = GNamesAddress + 0x48;
            GNamesOffset = (IntPtr)(GNamesAddress.ToInt64() - BaseAddress.ToInt64());
            GObjectsOffset = (IntPtr)(GObjectsAddress.ToInt64() - BaseAddress.ToInt64());

            Console.WriteLine($"Base-Address:\t\t0x{BaseAddress.ToInt64():X}\n");
            Console.WriteLine($"GNames-Address:\t\t0x{GNamesAddress.ToInt64():X}");
            Console.WriteLine($"GNames-Offset:\t\t0x{GNamesOffset:X}\n");
            Console.WriteLine($"GObjects-Address:\t0x{GObjectsAddress.ToInt64():X}");
            Console.WriteLine($"GObjects-Offset:\t0x{GObjectsOffset:X}\n");

            DumpNames();
            DumpObjects();            

            Console.WriteLine("\n\nDone!");
        }

        private static void DumpObjects()
        {
            TArray GObjects = RL.ReadMemory<TArray>(GObjectsAddress);
            Console.WriteLine($"\n\nDumping Objects... \t[{GObjects.ArrayCount}]");

            using StreamWriter sw = File.AppendText(ObjectsPath);

            sw.WriteLine($"Base-Address:\t\t0x{BaseAddress.ToInt64():X}\n");
            sw.WriteLine($"GNames-Address:\t\t0x{GNamesAddress.ToInt64():X}");
            sw.WriteLine($"GNames-Offset:\t\t0x{GNamesOffset:X}\n");
            sw.WriteLine($"GObjects-Address:\t0x{GObjectsAddress.ToInt64():X}");
            sw.WriteLine($"GObjects-Offset:\t0x{GObjectsOffset:X}\n");

            for (int i = 0; i < GObjects.ArrayCount; i++)
            {
                ProgressBar(i, GObjects.ArrayCount);

                IntPtr ArrayData = RL.ReadMemory<IntPtr>(GObjects.ArrayData + (i * 8));

                if (ArrayData == IntPtr.Zero)
                {
                    continue;
                }

                UObject uObject = RL.ReadMemory<UObject>(ArrayData);
                sw.WriteLine($"[0x{ArrayData.ToInt64():X}] {FNames.FirstOrDefault(kv => kv.Key == uObject.FNameEntryId).Value}");
            }
        }

        private static void DumpNames()
        {
            TArray GNames = RL.ReadMemory<TArray>(GNamesAddress);
            Console.WriteLine($"Dumping Names...\t[{GNames.ArrayCount}]");

            using StreamWriter sw = File.AppendText(NamesPath);
            sw.WriteLine($"Base-Address:\t\t0x{BaseAddress.ToInt64():X}\n");
            sw.WriteLine($"GNames-Address:\t\t0x{GNamesAddress.ToInt64():X}");
            sw.WriteLine($"GNames-Offset:\t\t0x{GNamesOffset:X}\n");
            sw.WriteLine($"GObjects-Address:\t0x{GObjectsAddress.ToInt64():X}");
            sw.WriteLine($"GObjects-Offset:\t0x{GObjectsOffset:X}\n");

            for (int i = 0; i < GNames.ArrayCount; i++)
            {
                ProgressBar(i, GNames.ArrayCount);

                IntPtr ArrayData = RL.ReadMemory<IntPtr>(GNames.ArrayData + (i * 8));

                if (ArrayData == IntPtr.Zero)
                {
                    continue;
                }

                FNameEntry entry = RL.ReadMemory<FNameEntry>(ArrayData);

                if (String.IsNullOrWhiteSpace(entry.Name))
                {
                    continue;
                }

                FNames.Add(entry.Index, entry.Name);
                sw.WriteLine($"[{entry.Index.ToString().PadLeft(6, '0')}] {entry.Name}");
            }
        }

        private static void ProgressBar(int currentStep, int totalSteps)
        {
            float percentage = (float)currentStep / totalSteps;

            int filledWidth = 1 + (int)(percentage * 40);

            string progressBar = "[" + new string('=', filledWidth) + new string(' ', 40 - filledWidth) + "]";

            Console.Write("\r" + progressBar + $" {percentage:P0}");
        }
    }
}





