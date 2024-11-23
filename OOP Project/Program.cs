using System;
using System.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Reflection.Metadata;

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





public class Program
{

     static List<User> users = new List<User>();

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
    }

    static void Register()
    {
        Console.Clear();
        Console.WriteLine("REGISTER YOUR ACCOUNT!");
        Console.WriteLine("Enter Username: ");
        string u_name = Console.ReadLine()!;
        Console.WriteLine("Create Password: ");
        string u_pass= Console.ReadLine()!;
        Console.WriteLine("Confirm Password: ");
        string con_pass= Console.ReadLine()!;


        if (users.Any(u=>u.Username == u_name)) //scans through the List if the same username exist
        {
            Console.Clear();
            Console.WriteLine("Username already exists. Please try again.");
            if (con_pass != u_pass)
            {
                Console.WriteLine("Password do not match. Please try again");
            }
        }
        else
        {
            Registered_User newUser = new Registered_User(u_name, u_pass);
            users.Add(newUser);
            SaveUserInfo(newUser);
            Console.WriteLine("ACCOUNT SUCCESSFULLY REGISTERED!");
        }
    }

    static void SaveUserInfo(User user)
    {
       File.AppendAllText("user_data.txt",$"{user.Username}:{user.Password}\n");
    }

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
}