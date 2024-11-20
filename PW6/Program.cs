namespace PW6
{
    public abstract class AParticipant // початковий абстрактний клас
    {
        public string Name { get; set; }
        public bool AtOrder { get; set; }
        public AParticipant(string n, bool a)
        {
            Name = n;
            AtOrder = a;
        }
        public abstract void OnOrder(object sender, EventArgs e);
        public abstract void OnCycle(object sender, EventArgs e);
        public void RemoveFromsystem(OrderHandler OH)
        {
            OH.RemoveParticipant(this);
        }
    }
    public class TaxiDriver : AParticipant // водій
    {

        public TaxiDriver(string n, bool a) : base(n, a) { }

        public override void OnOrder(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Taxi driver " + this.Name + " started working for a new order" );
            AtOrder = true;
            Console.ResetColor();
        }
        public override void OnCycle(object sender, EventArgs e) // якщо у водія є заказ, він зробить його та буде вільний вже на наступній ітерації
        {
            if (this.AtOrder)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Driver " + this.Name + " done his order succesfully!");
                AtOrder = false;
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Driver " + this.Name + " is waiting for a new order!");
            }
        }
    }
    public class Passenger : AParticipant
    {
        public string Status { get; set; } // VIP чи звичайний
        public string Destination { get; set; } 
        public Passenger(string n, bool a,string st, string ds) : base(n, a) 
        { 
            Status = st; 
            Destination = ds;
        }

        public override void OnOrder(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Passenger " + this.Name + " is on his way to " + Destination.ToLower());
            AtOrder = true;
            Console.ResetColor();
        }
        public override void OnCycle(object sender, EventArgs e)
        {
            if (AtOrder)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Passenger " + this.Name + " have reached " + Destination.ToLower()); 
                RemoveFromsystem(sender as OrderHandler); // самовидалення після закінчення заказу
                Console.ResetColor();
            }
            else
            Console.WriteLine("Passenger " + this.Name + " is vaiting for the taxi!");
        }
    }
    public class OrderHandler // обробник подій
    { 
        public event EventHandler? OrderStarted;
        public event EventHandler? Cycleevent;
        public int CycleNumber = 0;
        public int Totalrides = 0;
        Random R = new Random();
        public void Cycle()
        {
            CycleNumber++;
            Console.WriteLine("Cycle number " + CycleNumber);
            Console.WriteLine("-----=====================-----");
            Cycleevent.Invoke(this, EventArgs.Empty);
            Console.WriteLine("-----=====================-----");
        }
        public void CreateOrder(TaxiDriver TD, Passenger P)
        {
            Totalrides++;
            OrderStarted += TD.OnOrder;
            OrderStarted += P.OnOrder;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Driver " + TD.Name + " is now driving " + P.Name + " to " + P.Destination.ToLower());
            Console.ResetColor();
            OrderStarted.Invoke(this, EventArgs.Empty);
            OrderStarted -= TD.OnOrder;
            OrderStarted -= P.OnOrder;
        }
        public void AddParticipant(AParticipant P)
        {
            Cycleevent += P.OnCycle;
        }
        public void RemoveParticipant(AParticipant P)
        {
            Cycleevent -= P.OnCycle;
        }
    }


    internal class Program
    {
        // делегати для більшої зручності
        public delegate void RegisterDriver(TaxiDriver D);
        public delegate void RegisterPassenger(Passenger D);
        // додаткові методи для зручності
        static void Line()
        {
            Console.WriteLine("-----=====================-----");
        }
        static void WaitInput()
        {
            Console.WriteLine("Press any button to continue");
            Console.ReadKey();
        }
        static void ErrorInput()
        {
            Console.WriteLine("Wrong data caused an unexpected error, please try again!");
            WaitInput();
        }
        static void printlist(List<Passenger> L)
        {
            foreach (Passenger o in L)
            { 
                    Console.WriteLine((L.IndexOf(o) + 1) + ". "+ o.Name + " (" + o.Status + ")");
            }
        }
        static void printlist(List<TaxiDriver> L)
        {
            foreach (TaxiDriver o in L)
            {
                Console.WriteLine((L.IndexOf(o) + 1) + ". " + o.Name);
            }
        }
        static void printlist(List<TaxiDriver> L, bool B)
        {
            foreach (TaxiDriver o in L)
            {
                string s;
                if (o.AtOrder) s = "Busy"; else s = "Free";
                Console.WriteLine((L.IndexOf(o) + 1) + ". " + o.Name + "({0})",s);
            }
        }
        static int AwaitNumber(int minvalue, int maxvalue)
        { // перевірки на помилки вбудовані в методи, де користувач може ввести некоректні дані
            while (true)
            {
                try
                {
                    int n = int.Parse(Console.ReadLine());
                    if (n >= minvalue && n <= maxvalue)
                    {
                        return n;
                    }
                    else
                    {
                        Console.WriteLine("Your number must be between {0} and {1}", minvalue, maxvalue);
                    }

                }
                catch
                {
                    ErrorInput();
                }
            }
        }
        // головний метод
        static void Main()
        {
            Random rand = new Random();
            OrderHandler OH = new OrderHandler();
            List<string> Names = ["Jack","Paul","Max","Joe","Alex","Jane","Carl","Mike","Mary","Sophia","Barbara","Zoey","Sarah"]; // випадкові імена та точки призначення
            List<string> Destinations = ["Bank","Town hall","City center","Club","Reustaraunt","Store","Train station","Airport","Park","Hospital","Home","Office"];
            List<TaxiDriver> Drivers = new List<TaxiDriver>();
            List<Passenger> Passengers = new List<Passenger>();
            RegisterDriver RegD = OH.AddParticipant;
            RegD += Drivers.Add;
            RegisterPassenger RegP = OH.AddParticipant;
            RegP += Passengers.Add;
            int num;
            Console.WriteLine("Application started");
            Line();
            while (true) 
            {
                Drivers.Clear();
                Passengers.Clear();
                try
                {
                    Console.WriteLine("Please enter the number of participants ");
                    num = AwaitNumber(1, 100);
                    break;
                }
                catch
                {
                    ErrorInput();
                    continue ;
                }
               
            }
            Line();
            Console.WriteLine("How much drivers do you want to have (enter 0 for random amount)");
            int maxDrivers = AwaitNumber(0, num);
            bool Auto = true;
            for (int i = 0; i < num; i++) // генерація учасників симуляції
            {
                string NewName = Names[rand.Next(Names.Count)];
                AParticipant NewOne;
                if (maxDrivers > 0)
                {
                    Auto = false;
                    NewOne = new TaxiDriver(NewName, false);
                    RegD(NewOne as TaxiDriver);
                    maxDrivers--;
                    continue;
                }
                if (rand.Next(0, 2) == 1 && Auto)
                {
                    NewOne = new TaxiDriver(NewName, false);
                    RegD(NewOne as TaxiDriver);
                }
                else
                {
                    string Status;
                    if (rand.Next(1, 4) == 1) Status = "VIP"; else Status = "Regular";
                    string NewDes = Destinations[rand.Next(Destinations.Count)];
                    NewOne = new Passenger(NewName,false,Status,NewDes);
                    RegP(NewOne as Passenger);
                }
            }
            for (int i = 0; i < Passengers.Count; i++) // VIP замовники ставляться на початок списку, аби їх замовлення оброблялися першими
            {
                Passenger P = Passengers[i];
                if (P.Status == "VIP")
                {
                    Passengers.Remove(P);
                    Passengers.Insert(0, P);
                }
            }
            Console.Clear(); // вивід користувачу всіх учасників
            Line();
            Console.WriteLine("All drivers: ");
            Line();
            printlist(Drivers);
            Line();
            Console.WriteLine("All passengers: ");
            Line();
            printlist(Passengers);
            Line();
            WaitInput();
            
            while (true)
            {
                Console.Clear();
                OH.Cycle();
                Console.WriteLine("Enter 1 for automatic order or 2 for manual ordering");
                num = AwaitNumber(1, 2);
                if (num == 1)
                {
                    for (int i = 0; i < Passengers.Count; i++) // автоматичне розподілення заказів (VIP коистувачі найперші у списку)
                    {
                        Passenger P = Passengers[i];  
                        foreach (TaxiDriver TD in Drivers)
                        {
                            if (TD.AtOrder == false)
                            {
                                OH.CreateOrder(TD, P);
                                Passengers.Remove(P);
                                i--;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        Console.Clear();
                        Passenger P;
                        TaxiDriver TD;
                        Console.WriteLine("Choose the passenger from the list by entering a number: ");
                        Line();
                        printlist(Passengers);
                        Line();
                        while (true)
                        {
                            num = AwaitNumber(1, Passengers.Count);
                            P = Passengers[num - 1]; // перевірка на зайнятість не потрібна, оскільки система видаляє зайнятих користувачів після створення замовлення
                            break;
                        }
                        Console.Clear();
                        Console.WriteLine("Choose a driver from the list by entering a number: ");
                        printlist(Drivers, true);
                        Line();
                        while (true)
                        {
                            num = AwaitNumber(1, Drivers.Count);
                            TD = Drivers[num - 1];
                            if (TD.AtOrder == true)
                            {
                                Console.WriteLine("{0} is already doing his order, try someone else!", TD.Name);
                                WaitInput();
                                continue;
                            }
                            break;
                        }
                        Line();
                        OH.CreateOrder(TD, P);
                        Passengers.Remove(P);
                        Line();
                        bool allbusy = true; // перевірка на те, чи всі водії зайняті
                        if (Passengers.Count == 0) { Console.WriteLine("No more passengers left, good job!"); WaitInput(); break; } // чи залишились пасажири?
                        foreach (TaxiDriver td in Drivers)
                        {
                            if (td.AtOrder == false) allbusy = false;
                        }
                        if (allbusy)
                        {
                            Console.WriteLine("All drivers are busy right now!");
                            break;
                        }
                        Console.WriteLine("Press 1 to do another order or press 0 to go to the next iteration");
                        num = AwaitNumber(0, 1);
                        if (num == 0) break;
                    }
                }
                Console.WriteLine("Going to next iteration!");
                Console.WriteLine("Enter 1 to continue or 0 to stop simulation!"); // можливість вийти з симуляції після завершення поїздок
                num = AwaitNumber(0, 1);
                Console.Clear();
                if (num == 0) break; 
                if (Passengers.Count == 0)
                {
                    Console.WriteLine("All the passengers got to their destination points!");
                    break;
                }
            }
            Console.WriteLine("Simulation ended!");
            Console.WriteLine("Total orders: " + OH.Totalrides);
            Line();
            Console.WriteLine("Press 1 to start again or press 0 to exit!"); // можливість почати симуляцію знову
            num = AwaitNumber(0, 1);
            if (num == 0)
            {
                Environment.Exit(0);
            }
            Console.Clear();
            Main();
        }
    }
}
    
