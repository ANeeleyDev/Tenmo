using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly APIService apiService = new APIService("https://localhost:44315/");

        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            while (true)
            {
                int loginRegister = -1;
                while (loginRegister != 1 && loginRegister != 2)
                {
                    Console.WriteLine("Welcome to TEnmo!");
                    Console.WriteLine("1: Login");
                    Console.WriteLine("2: Register");
                    Console.Write("Please choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out loginRegister))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.");
                    }
                    else if (loginRegister == 1)
                    {
                        while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                        {
                            LoginUser loginUser = consoleService.PromptForLogin();
                            ApiUser user = authService.Login(loginUser);
                            if (user != null)
                            {
                                UserService.SetLogin(user);
                            }
                        }
                    }
                    else if (loginRegister == 2)
                    {
                        bool isRegistered = false;
                        while (!isRegistered) //will keep looping until user is registered
                        {
                            LoginUser registerUser = consoleService.PromptForLogin();
                            isRegistered = authService.Register(registerUser);
                            if (isRegistered)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Registration successful. You can now log in.");
                                loginRegister = -1; //reset outer loop to allow choice for login
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.");
                    }
                }

                MenuSelection();
            }
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    Console.WriteLine();
                    int userId = UserService.GetUserId();
                    decimal userBalance = apiService.GetBalance(userId);
                    Console.WriteLine($"Your current balance is: {userBalance}");
                }
                else if (menuSelection == 2)
                {
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("Tansfers");
                    Console.WriteLine("ID     From/To     Amount");
                    Console.WriteLine("--------------------------------");

                    int currentUser = UserService.GetUserId();
                    int accountId = apiService.GetAccountId(currentUser);

                    IList<TransferReceipt> transferReceipts = apiService.GetTransfersForLoggedInUser(accountId);
                    foreach (TransferReceipt item in transferReceipts)
                    {
                        if (accountId == item.AccountTo)
                        {
                            string userName = apiService.GetTransferFrom(item.TransferId);
                            Console.WriteLine($"{item.TransferId}     From: {userName}     {item.Amount}");
                        }
                        else
                        {
                            string userName = apiService.GetTransferTo(item.TransferId);
                            Console.WriteLine($"{item.TransferId}     To: {userName}     {item.Amount}");
                        }

                    }
                    Console.WriteLine("Please enter transfer ID to view details (0 to cancel): ");
                    Console.WriteLine();

                    string userInputForDetailsAsString = Console.ReadLine();
                    int userInputForDetailsAsInt = Convert.ToInt32(userInputForDetailsAsString);

                    string userFrom = apiService.GetTransferFrom(userInputForDetailsAsInt);
                    string userTo = apiService.GetTransferTo(userInputForDetailsAsInt);
                    TransferRequest transfer = new TransferRequest();
                    foreach (TransferReceipt item in transferReceipts)
                    {

                        if (item.TransferId == userInputForDetailsAsInt)
                        {
                            transfer.Amount = item.Amount;

                        }
                        else
                        {
                            return;
                        }



                    }

                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("Transfer Details");
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine($"ID: {userInputForDetailsAsInt}");
                    Console.WriteLine($"From: {userFrom}");
                    Console.WriteLine($"To: {userTo}");
                    Console.WriteLine($"Type: Send");
                    Console.WriteLine($"Status: Approved");
                    Console.WriteLine($"Amount: {transfer.Amount}");

                }
                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)
                {
                    int userId = UserService.GetUserId();

                    Console.WriteLine();
                    IList<User> usersList = apiService.GetUsers();

                    Console.WriteLine("--------------------------------");
                    Console.WriteLine("Users");
                    Console.WriteLine("ID        Name");
                    Console.WriteLine("--------------------------------");

                    foreach (User user in usersList)
                    {
                        Console.WriteLine($"{user.UserId}      {user.Username}");
                    }
                    Console.WriteLine("----------");
                    Console.WriteLine();
                    Console.WriteLine("Enter ID of user you are sending (0 to cancel):");

                    string usersChoiceString = Console.ReadLine();
                    int usersChoiceInt = int.Parse(usersChoiceString);

                    foreach (User user1 in usersList)
                    {
                        if (user1.UserId == usersChoiceInt)
                        {
                            Console.WriteLine("Enter amount:");
                            //do whatever
                        }
                    }
                    string userChoiceAmountAsString = Console.ReadLine();
                    int userChoiceAmountAsInt = int.Parse(userChoiceAmountAsString);

                    TransferRequest transferRequest = new TransferRequest();

                    transferRequest.Amount = userChoiceAmountAsInt;
                    transferRequest.UserFrom = userId;
                    transferRequest.UserTo = usersChoiceInt;
                    transferRequest.AccountFrom = apiService.GetAccountId(userId);
                    transferRequest.AccountTo = apiService.GetAccountId(usersChoiceInt);
                    if (transferRequest.Amount < 0)
                    {
                        Console.WriteLine("Can't tranfer negative moneies");

                    }
                    else if (apiService.GetBalance(transferRequest.UserFrom) < transferRequest.Amount)
                    {
                        Console.WriteLine("You are too poor");
                    }
                    else
                    {
                        apiService.CreateTransaction(transferRequest);
                    }

                }
                else if (menuSelection == 5)
                {

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new ApiUser()); //wipe out previous login info
                    Console.Clear();
                    menuSelection = 0;
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
