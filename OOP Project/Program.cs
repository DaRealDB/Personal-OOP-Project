using System;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Reflection.Metadata;
using System.Text;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

public class User 
{
    public string Username {  get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public User(string username, string password, string email)
    {
        Username = username;
        Password = password;
        Email = email;
    }
}

public class Registered_User : User
{
    public Registered_User(string username, string password, string email) : base(username, password, email) { }
}



public class Product
{
    public string productName { get; set; }
    public decimal price { get; set; }
    public string category { get; set; }

    public Product(string p_Name, decimal Price, string Category)
    {
        productName = p_Name;
        price = Price;
        category = Category;
    }

    public void Display()
    {
        Console.WriteLine($"Product:{productName}, Price:{price}, Category:{category}");
    }

}


public class Discount
{
    public decimal Amount { get; set; }
    public string Description { get; set; }

    public Discount(decimal amount, string description) 
    { 
    Amount = amount;
    Description = description;
    }

    public virtual decimal Apply(decimal total)
    {
        return total - Amount;
    }


}



public class PercentageDiscount : Discount
{
    public decimal Percentage { get; set; }

    public PercentageDiscount(decimal percentage): base(0, $"{percentage} off!")
    {
        Percentage = percentage;
    }

    public override decimal Apply(decimal total)
    {
        return total - (total * Percentage/100);
    }
}

public class Order
{
    public Customer Customer1 { get; set; }
    public List<Product> Products { get; set; }
    public decimal TotalCost { get; set; }
    public string status { get; set; }

    public Order(Customer customer, List<Product> products)
    {
        Customer1 = customer;
        Products = products;
        TotalCost = products.Sum(p => p.price);
        status = "pending";
    }


    public void PlaceOrder()
    {
        status = "Order Confirmed";

    }

    public void DisplayOrder()
    {
        Console.WriteLine($"Order for:{Customer1.Username}");
        foreach (var product in Products) 
        {
            product.Display();
        }

        Console.WriteLine($"Total Cost: {TotalCost}"); 
        Console.WriteLine($"Status: {status}");
    }
}



    public class Customer : User
    {
        public string Name { get; set; }
        public ShoppingCart Cart { get; set; }

        public Customer(string username, string password, string email, string name) : base(username, password, email)
        {
            Name = name;
            Cart = new ShoppingCart();
        }

        public void AddtoCart(Product product, int quantity)
        {
            Cart.AddProduct(product, quantity);
        }

        public void ViewCart()
        {
            Cart.DisplayCart();
        }


    }

public class ShoppingCart
{
    private Dictionary<string, (Product product, int Quantity)> items = new Dictionary<string, (Product product, int Quantity)>();
    private List<Product>? products;
    public void AddProduct(Product product, int quantity)
    {
        if (items.ContainsKey(product.productName))
        {
            items[product.productName] = (product, items[product.productName].Quantity + quantity);
        }
        else
        {
            items.Add(product.productName, (product, quantity));
        }
    }

    public decimal CalculateTotal()
    {
        decimal total = 0;
        foreach (var item in items.Values)
        {
            total += item.product.price * item.Quantity;
        }
        return total;
    }

    public void DisplayCart()
    {
        if (items == null || items.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
            return;
        }

        foreach (var item in items.Values)
        {
            Console.WriteLine($"{item.product.productName} - {item.Quantity} x {item.product.price:C} = {item.Quantity * item.product.price:C}");
        }

        Console.WriteLine($"Total: {CalculateTotal():C}");
    }


    public List<Product> GetProducts()
    {
        return products!;
    }

    
}

    public class Program
    {

        static List<User> users = new List<User>();
        static List<Product> products = new List<Product>();

        static void LoadUserInfo()
        {
            if (File.Exists("user_data.txt"))
            {
                string[] lines = File.ReadAllLines("user_data.txt");

                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 4) // Ensure there are four parts: username, password, name, email
                    {
                        users.Add(new Customer(parts[0], parts[1], parts[2], parts[3]));
                    }
                }
            }
        }



        public static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                {
                    password.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            } while (keyInfo.Key != ConsoleKey.Enter);
            return password.ToString();
        }


        static void SaveUserInfo(Customer customer)
        {
            string userData = $"{customer.Username}:{customer.Password}:{customer.Name}:{customer.Email}";
            File.AppendAllLines("user_data.txt", new[] { userData });
        }



        public static void Main(string[] args)
        {

            DisplayMainMenu();
        }

        static void DisplayMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("AmorCart");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");

                try
                {
                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                Login();
                                break;

                            case 2:
                                Register();
                                break;

                            case 3:
                                Environment.Exit(0);
                                break;

                            default:
                                Console.WriteLine("Invalid choice. Please try again.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

                Console.WriteLine("\n\t\t\t\t                     Press Enter to continue...");
                Console.ReadLine();
            }
        }

        static void Login()
        {
            LoadUserInfo();

            Console.Write("Enter username: ");
            string username = Console.ReadLine()!;
            Console.Write("Enter password: ");
            string password = ReadPassword()!;

            // Assuming the `users` list contains `Customer` objects
            Customer customer = (Customer)users.FirstOrDefault(u => u.Username == username && u.Password == password)!;

            if (customer != null)
            {
                Console.Clear();
                Console.WriteLine("Welcome To AmorCart!");
                Console.ReadKey();
                MainMenu(customer);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid Username or Password. Please Try Again");
                Console.ReadKey();
            }
        }


        static void Register()
        {
            Console.Clear();
            Console.WriteLine("REGISTER YOUR ACCOUNT!");
            Console.WriteLine("Enter Username: ");
            string u_name = Console.ReadLine()!;
            Console.WriteLine("Create Password: ");
            string u_pass = ReadPassword()!;
            Console.WriteLine("Enter Email: ");
            string email = Console.ReadLine()!;
            Console.WriteLine("Enter Name: ");
            string name = Console.ReadLine()!;

            if (users.Any(u => u.Username == u_name)) // Scans through the list if the same username exists
            {
                Console.Clear();
                Console.WriteLine("Username already exists. Please try again.");
            }
            else
            {
                // Here we create a new Customer object and add it to the users list
                Customer newCustomer = new Customer(u_name, u_pass, name, email);
                users.Add(newCustomer);
                SaveUserInfo(newCustomer);
                Console.WriteLine("ACCOUNT SUCCESSFULLY REGISTERED!");
            }
        }



        static void Pause()
        {
            Console.WriteLine("Press Enter to Continue. . . .");
            Console.ReadKey();
        }

        public static void ViewProduct(List<Product> products)
        {
            LoadProductData();
            Console.Clear();
            Console.WriteLine("Available Products");
            foreach (var product in products)
            {
                product.Display();
            }
            Pause();
        }

        public static void AddProduct()
        {
            Console.Clear();
            Console.WriteLine("Add Your Product!");
            Console.WriteLine("Enter Product: ");
            string product_name = Console.ReadLine()!;
            Console.WriteLine("Enter Product Price: ");
            long product_price = int.Parse(Console.ReadLine()!);
            Console.WriteLine("Enter Product Category: ");
            string product_category = Console.ReadLine()!;

            Product products1 = new Product(product_name, product_price, product_category);
            products.Add(products1);
            SaveProductData(products1);
            Console.WriteLine("PRODUCT ADDED SUCCESSFULLY");
        }


        public static void SaveProductData(Product product)
        {
            File.AppendAllText("Product.csv", $"{product.productName}, {product.price}, {product.category}");
        }

        public static void LoadProductData()
        {
            if (File.Exists("Product.csv"))
            {
                string[] lines = File.ReadAllLines("Product.csv");
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        int price = int.Parse(parts[1]);
                        products.Add(new Product(parts[0], price, parts[2]));
                    }
                }
            }
        }

        public static void AddProductsToCart(Customer customer, List<Product> products)
        {
            Console.Clear();
            Console.WriteLine("SELECT PRODUCT TO ADD!: ");
            string products2 = Console.ReadLine()!;
            var product = products.FirstOrDefault(p => p.productName.Equals(products2, StringComparison.OrdinalIgnoreCase));

            if (product != null)
            {
                Console.WriteLine("Quantity: ");
                int quantity = int.Parse(Console.ReadLine()!);
                customer.AddtoCart(product, quantity);
                Console.WriteLine("Product Added To Cart!");

            }
            else
            {
                Console.WriteLine("Product not found!");
            }


        }

    public static void Checkout(Customer customer)
    {
        // Ensure customer and cart are not null
        if (customer == null)
        {
            Console.WriteLine("Customer information is missing.");
            return;
        }

        if (customer.Cart == null)
        {
            Console.WriteLine("Shopping cart is not initialized.");
            return;
        }

        Console.Clear();
        decimal TotalCost = customer.Cart.CalculateTotal();
        Console.WriteLine($"Total Cost: {TotalCost:C}");

        Console.WriteLine("Enter discount percentage (optional): ");
        if (decimal.TryParse(Console.ReadLine(), out decimal discountPercentage) && discountPercentage > 0)
        {
            Discount discount = new PercentageDiscount(discountPercentage);
            TotalCost = discount.Apply(TotalCost);
            Console.WriteLine($"Total Cost after discount: {TotalCost:C}");
        }

        // Ensure that the list of products is not null
        var products = customer.Cart.GetProducts();
        if (products == null || products.Count == 0)
        {
            Console.WriteLine("No products in the cart to checkout.");
            return;
        }

        Order order = new Order(customer, products);
        order.DisplayOrder();
        order.PlaceOrder();
    }


    static void MainMenu(Customer customer)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("AMORCART");
                Console.WriteLine("1. View Products");
                Console.WriteLine("2. Add Products to Cart");
                Console.WriteLine("3. View Cart");
                Console.WriteLine("4. Checkout");
                Console.WriteLine("5. Add Product");
                Console.WriteLine("6. Exit");
                Console.WriteLine("Enter you choice: ");
                int choice = int.Parse(Console.ReadLine()!);


                switch (choice)
                {
                    case 1:
                        //View Product
                        ViewProduct(products);
                        break;
                    case 2:
                        //Add Product to cart
                        AddProductsToCart(customer, products);
                        break;
                    case 3:
                        //View Cart
                        customer.ViewCart();
                        Pause();
                        break;
                    case 4:
                        //Checkout
                        Checkout(customer);
                        Pause();
                        break;
                    case 5:
                        //Add Product
                        AddProduct();
                        break;
                    case 6:
                        //Exit
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid Choice. Try Again!");
                        break;
                }
            }
        }
  
}