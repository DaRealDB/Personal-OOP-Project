﻿using System;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Reflection.Metadata;
using System.Text;
using System.Net.Http.Headers;

public class User 
{
    public string Username {  get; set; }

    public string Password { get; set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
    }

}

public class Registered_User : User
{
    public Registered_User(string username, string password) : base(username, password) { }
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
                if (parts.Length == 2)
                {
                    users.Add(new Registered_User(parts[0], parts[1]));
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
            if(keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
            {
                password.Append(keyInfo.KeyChar);
                Console.Write("*");
            }
            else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Remove(password.Length - 1, 1);
                Console.Write("\b \b");
            }
        }while (keyInfo.Key != ConsoleKey.Enter);
        return password.ToString();
    }


    static void SaveUserInfo(User user)
    {
        File.AppendAllText("user_data.txt", $"{user.Username}:{user.Password}\n");
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

        User user = users.FirstOrDefault(u => u.Username == username && u.Password == password)!;

        if (user != null)
        {
            Console.Clear();
            Console.WriteLine("Welcome To AmorCart!");
            Console.ReadKey();
            MainMenu();
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
        string u_pass= ReadPassword()!;
        Console.WriteLine("Confirm Password: ");
        string con_pass= ReadPassword()!;


        if (users.Any(u=>u.Username == u_name)) //scans through the List if the same username exist
        {
            Console.Clear();
            Console.WriteLine("Username already exists. Please try again.");
           
        }
        else
        {
            Registered_User newUser = new Registered_User(u_name, u_pass);
            users.Add(newUser);
            SaveUserInfo(newUser);
            Console.WriteLine("ACCOUNT SUCCESSFULLY REGISTERED!");
        }
    }

    static void Pause()
    {
        Console.WriteLine("Press Enter to Continue. . . .");
        Console.ReadKey();
    }

    public static void ViewProduct(List<Product>products)
    {
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
        Console.WriteLine("Add Your Product!");
        Console.WriteLine("Enter Product: ");
        string product_name = Console.ReadLine()!;
        Console.WriteLine("Enter Product Price: ");
        long product_price = int .Parse(Console.ReadLine()!);
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
                    products.Add(new Product(parts[0], price, parts[2] ));
                }
            }
        }
    }


    static void MainMenu()
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

           switch(choice)
            {
                case 1:
                    //View Product
                    ViewProduct(products);
                    break;
                case 2:
                    //Add Product to cart
                    break;
                case 3:
                    //View Cart
                    break;
                case 4:
                    //Checkout
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