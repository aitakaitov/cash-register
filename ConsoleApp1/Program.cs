using System;
using System.Diagnostics;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        Register r = new Register();
        r.Start();
    }
}

public enum MessageType { 
    ERROR,
    INFO,
    NONE
}

public static class MessageConverter {
    public static string PrefixMessage(string message, MessageType type) { 
        switch (type)
        {
            case MessageType.ERROR:
                return "ERROR: " + message;
            case MessageType.INFO:
                return "INFO: " + message;
            case MessageType.NONE:
                return message;
            default:
                return message;
        }
    }
}

public class Register 
{
    private Invoice? _currentInvoice;

    private void PrintOutput(string message, MessageType type = MessageType.INFO)
    {
        Console.WriteLine(MessageConverter.PrefixMessage(message, type));
    }

    private void PrintPrompt(string prompt = "> ")
    {
        Console.Write(prompt);
    }

    private string? ReadInput()
    {
        return Console.ReadLine();
    }

    private bool YesOrNoPrompt()
    {
        PrintPrompt("Type [y/n]: ");
        while (true)
        {
            var input = ReadInput();
            if (input!.ToLower() == "y")
            {
                return true;
            }
            else if (input!.ToLower() == "n")
            {
                return false;
            }
            else
            {
                PrintOutput("Please type \"y\" or \"n\"");
            }
        }

    }

    private void PrintInvoice()
    {
        if (_currentInvoice == null)
        {
            PrintOutput("No invoice is being processed");
        }
        else 
        {
            PrintOutput(_currentInvoice.ToString(), MessageType.NONE);
        }
    }

    private void NewInvoice()
    {
        if (_currentInvoice != null)
        {
            PrintOutput("Invoice is being processed", MessageType.ERROR);
            return;
        }

        _currentInvoice = new Invoice();
        PrintOutput("Created new invoice");
    }

    private bool Exit()
    {
        if (_currentInvoice != null)
        {
            PrintOutput("You have an unfinished invoice, dou you want to quit?", MessageType.NONE);
            if (YesOrNoPrompt())
            {
                PrintOutput("The register will now exit");
                return true;
            }
            else
            {
                PrintOutput("Exit aborted");
                return false;
            }
        }

        PrintOutput("The register will now exit");
        return true;
    }

    private void Scrap()
    {
        if (_currentInvoice != null)
        {
            PrintOutput("Do you really want to scrap this invoice?");
            if (YesOrNoPrompt())
            {
                PrintOutput("Scrapping current invoice");
            }
            else
            {
                PrintOutput("Scrapping aborted");
            }
        }
        else 
        {
            PrintOutput("No invoice is being processed", MessageType.ERROR);
        }
    }

    private void Done()
    {
        if (_currentInvoice == null)
        {
            PrintOutput("No invoice is being processed", MessageType.ERROR);
        }
        else
        {
            PrintInvoice();
            PrintOutput("Invoice done");
            _currentInvoice = null;
        }
    }

    private void AddItem()
    {
        if (_currentInvoice == null)
        {
            PrintOutput("No invoice is being processed", MessageType.ERROR);
            return;
        }

        PrintOutput("To cancel item addition, write \"cancel\" as item name");
        while (true)
        {
            PrintPrompt("Item name: ");
            string? name = ReadInput();

            if (name == null || name.Length == 0)
            {
                PrintOutput("Input a valid name", MessageType.ERROR);
                continue;
            }

            if (name.ToLower() == "cancel")
            {
                PrintOutput("Cancelling add item");
                return;
            }

            PrintPrompt("Quantity: ");
            int quantity = 0;
            try
            {
                quantity = int.Parse(ReadInput());
                if (quantity <= 0) { throw new Exception(); }
            }
            catch (Exception)
            {
                PrintOutput("Input a valid quantity", MessageType.ERROR);
                continue;
            }

            PrintPrompt("Price per unit: ");
            float ppu = 0;
            try 
            {
                ppu = float.Parse(ReadInput());
                if (ppu <= 0) { throw new Exception(); }
            }
            catch (Exception)
            {
                PrintOutput("Input a valid price per unit", MessageType.ERROR);
                continue;
            }

            _currentInvoice.AddItem(new Item { Name = name, Quantity = quantity, UnitPrice = ppu });
            PrintOutput("Item added");
            break;
        }
    }

    private void RemoveItem()
    {
        if (_currentInvoice == null) 
        {
            PrintOutput("No invoice is being processed", MessageType.ERROR);
            return;
        }

        PrintInvoice();
        PrintPrompt("Please select an item to delete and write its index: ");
        while (true)
        {
            int index = 0;
            try
            {
                index = int.Parse(ReadInput());
                if (index <= 0 || index > _currentInvoice.Size)
                {
                    throw new Exception();
                }
                index -= 1;
            }
            catch (Exception)
            {
                PrintOutput("Type a valid index");
                continue;
            }

            PrintOutput("Item removed.");
            _currentInvoice.RemoveItem(index);
            break;
        }
    }

    private bool ReadCommand()
    {
        PrintPrompt();
        var input = ReadInput();
        if (input == null)
        {
            PrintOutput("Could not read command", MessageType.ERROR);
            return true;
        }

        switch (input)
        {
            case "new":
                NewInvoice();
                break;
            case "exit":
                return !Exit();
            case "scrap":
                Scrap();
                break;
            case "done":
                Done();
                break;
            case "add item":
                AddItem();
                break;
            case "remove item":
                RemoveItem();
                break;
            case "status":
                PrintInvoice();
                break;
            default:
                PrintOutput("Unknown command", MessageType.ERROR);
                break;
        }

        return true;
    }

    public void Start()
    {
        PrintOutput("Running");

        while (true)
        {
            if (!ReadCommand())
            {
                break;
            }
        }

        PrintOutput("Exiting");
    }
}

public class Invoice 
{ 
    public float PriceTotal { get { return _items.Select(i => i.PriceTotal).Sum(); } }

    private List<Item> _items = new List<Item>();
    public int Size { get { return _items.Count; } }
    public void AddItem(Item item) => _items.Add(item);
    public void RemoveItem(int index) => _items.RemoveAt(index);
    public override string ToString()
    {
        var builder = new StringBuilder();
        float total = 0;
        builder.AppendLine("------------------------------");
        builder.AppendLine($"{"Item Index"}\t{"Item Name"}\t{"Quantity"}\t{"Unit Price"}\t{"Item Total"}");
        builder.AppendLine("------------------------------");
        for (int i = 0; i < _items.Count; i++)
        {
            total += _items[i].PriceTotal;
            builder.AppendLine($"[{i + 1}] " + _items[i].ToString());
        }
        builder.AppendLine("------------------------------");
        builder.AppendLine($"Invoice Total\t{total}");
        builder.AppendLine("------------------------------");

        return builder.ToString();
    }
}

public class Item 
{
    public string Name { get; init; }
    public int Quantity { get; set; }
    public float UnitPrice { get; set; }
    public float PriceTotal { get { return Quantity * UnitPrice; } }

    public override string ToString()
    {
        return $"{Name}\t {Quantity}\t {UnitPrice}\t {PriceTotal}";
    }
}



