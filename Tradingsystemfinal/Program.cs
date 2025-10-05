class Program
{
    // En simple database för users, items och requests
    static List<User> users = new List<User>();
    static List<Item> items = new List<Item>();
    static List<TradeRequest> requests = new List<TradeRequest>();


    /// om currentuser är null så visar den login/register menyn, Där finns det alternativ för att registera eller logga in. 
    // om user är inte null så viser den huvudmenyn med olika alternativ för att ladda upp items, kolla genom items, skicka trade requests, kolla genom inkommande och utgående requests, acceptera eller neka requests, och kolla genom klara requests.
    static User currentUser = null;

    static void Main()
    {
        Console.WriteLine("Welcome to Simple Trade App!");
        while (true)
        {
            if (currentUser == null)
            {
                Console.WriteLine("\n--- Main Menu ---");
                Console.WriteLine("1) Register");
                Console.WriteLine("2) Login");
                Console.WriteLine("0) Exit");
                Console.Write("Choice: ");
                string choice = Console.ReadLine();

                if (choice == "1") Register();
                else if (choice == "2") Login();
                else if (choice == "0") break;
            }
            else
            {
                Console.WriteLine($"\n--- Logged in as {currentUser.Username} ---");
                Console.WriteLine("1) Upload item");
                Console.WriteLine("2) Browse other users' items");
                Console.WriteLine("3) Request a trade");
                Console.WriteLine("4) Browse incoming requests");
                Console.WriteLine("5) Browse outgoing requests");
                Console.WriteLine("6) Accept request");
                Console.WriteLine("7) Deny request");
                Console.WriteLine("8) Browse completed requests");
                Console.WriteLine("0) Logout");
                Console.Write("Choice: ");
                string choice = Console.ReadLine();

                if (choice == "1") UploadItem();
                else if (choice == "2") BrowseItems();
                else if (choice == "3") RequestTrade();
                else if (choice == "4") BrowseIncoming();
                else if (choice == "5") BrowseOutgoing();
                else if (choice == "6") AcceptRequest();
                else if (choice == "7") DenyRequest();
                else if (choice == "0") currentUser = null;
            }
        }
    }

    // User funktioner. Här frågar den för username och password och lägger till en ny user i listan. Sedan ger varje user ett unik id 

    static void Register()
    {
        Console.Write("Username: ");
        string u = Console.ReadLine();
        Console.Write("Password: ");
        string p = Console.ReadLine();
        users.Add(new User { Id = users.Count + 1, Username = u, Password = p });
        Console.WriteLine("Registered!");
    }

    // Inloggings funktionen. Den kollar genom user listan för att se om username och password matchar. om det gör det så sätter den curentuser till den usern. 
    static void Login()
    {
        Console.Write("Username: ");
        string u = Console.ReadLine();
        Console.Write("Password: ");
        string p = Console.ReadLine();
        foreach (var user in users)
        {
            if (user.Username == u && user.Password == p)
            {
                currentUser = user;
                Console.WriteLine("Login successful!");
                return;
            }
        }
        Console.WriteLine("Login failed.");
    }

    // Lägger till en ny item i listan. varje item får ett unikt id och ägs av den inloggade usern. 
    static void UploadItem()
    {
        Console.Write("Item title: ");
        string t = Console.ReadLine();
        items.Add(new Item { Id = items.Count + 1, Title = t, OwnerId = currentUser.Id, Traded = false });
        Console.WriteLine("Item uploaded!");
    }


    // Visar alla items som inte ägs av den inloggade usern och som inte är tradeade. 
    static void BrowseItems()
    {
        Console.WriteLine("Items from other users:");
        foreach (var i in items)
        {
            if (i.OwnerId != currentUser.Id && !i.Traded)
                Console.WriteLine($"{i.Id}: {i.Title}");
        }
    }


    // Låter dig att skicka en trade request för en item som du inte äger och som inte är tradead. Den lägger till en ny request i listan med status "Pending".
    static void RequestTrade()
    {
        BrowseItems();
        Console.Write("Enter item id: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int id))
        {
            Item item = items.Find(x => x.Id == id);
            if (item != null && !item.Traded)
            {
                requests.Add(new TradeRequest
                {
                    Id = requests.Count + 1,
                    ItemId = item.Id,
                    FromUserId = currentUser.Id,
                    ToUserId = item.OwnerId,
                    Status = "Pending"
                });
                Console.WriteLine("Trade requested!");
            }
            else
            {
                Console.WriteLine("Invalid item (either it doesnt exist or its already traded).");
            }
        }
        else
        {
            Console.WriteLine("Please enter a valid number for the item id.");
        }
    }

    // Visar alla requests som är skickade till den inloggade usern.
    static void BrowseIncoming()
    {
        Console.WriteLine("Incoming requests:");
        foreach (var r in requests)
        {
            if (r.ToUserId == currentUser.Id)
                Console.WriteLine($"{r.Id}: Item {r.ItemId}, From {r.FromUserId}, Status {r.Status}");
        }
    }

    // Visar alla requets som är skickade av den inloggade usern. 
    static void BrowseOutgoing()
    {
        Console.WriteLine("Outgoing requests:");
        foreach (var r in requests)
        {
            if (r.FromUserId == currentUser.Id)
                Console.WriteLine($"{r.Id}: Item {r.ItemId}, To {r.ToUserId}, Status {r.Status}");
        }
    }

    // Den inloggade usern kollar genom requets som är inkommande och accepterar en request. Den ändrar statusen till "Accepted", markerar item som tradead och ändrar ägaren till den usern som skickade requesten.
    static void AcceptRequest()
    {
        BrowseIncoming();
        Console.Write("Enter request id: ");
        int id = int.Parse(Console.ReadLine());
        TradeRequest r = requests.Find(x => x.Id == id);
        if (r != null && r.Status == "Pending" && r.ToUserId == currentUser.Id)
        {
            r.Status = "Accepted";
            Item item = items.Find(x => x.Id == r.ItemId);
            item.Traded = true;
            item.OwnerId = r.FromUserId;
            Console.WriteLine("Trade accepted!");
        }
    }

    // Den inloggade usern kollar genom inkommande requests och kan neka en request.
    static void DenyRequest()
    {
        BrowseIncoming();
        Console.Write("Enter request id: ");
        int id = int.Parse(Console.ReadLine());
        TradeRequest r = requests.Find(x => x.Id == id);
        if (r != null && r.Status == "Pending" && r.ToUserId == currentUser.Id)
        {
            r.Status = "Denied";
            Console.WriteLine("Trade denied!");
        }
    }

    static void BrowseCompleted()
    {
        Console.WriteLine("Completed requests: ");
        foreach (var r in requests)
        {
            if ((r.FromUserId == currentUser.Id || r.ToUserId == currentUser.Id) && r.Status != "Pending")
                Console.WriteLine($"{r.Id}: Item {r.ItemId}, From {r.FromUserId}, To {r.ToUserId}, Status {r.Status}");
        }
    }

}