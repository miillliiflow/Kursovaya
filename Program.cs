using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp30
{
    class Program
    {
        static List<Order> orders = new List<Order>();
        static List<Client> clients = new List<Client>();
        static List<Courier> couriers = new List<Courier>();

        static void Main(string[] args)
        {
            Console.Title = "Millfood - Система доставки еды";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║           ДОСТАВКА MILLFOOD          ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();

            while (true)
            {
                Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
                Console.WriteLine("1. Добавить клиента");
                Console.WriteLine("2. Добавить курьеров");
                Console.WriteLine("3. Создать заказ");
                Console.WriteLine("4. Просмотр заказов");
                Console.WriteLine("5. Изменить заказ");
                Console.WriteLine("6. Отменить заказ");
                Console.WriteLine("7. Изменить статус заказа");
                Console.WriteLine("8. Выход");
                Console.Write("Ваш выбор: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1": AddClient(); break;
                    case "2": AddCouriers(); break;
                    case "3": CreateOrder(); break;
                    case "4": ShowOrders(); break;
                    case "5": EditOrder(); break;
                    case "6": CancelOrder(); break;
                    case "7": UpdateOrderStatus(); break;
                    case "8": return;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Неверный ввод.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        static void AddClient()
        {
            Console.Write("Введите ФИО клиента: ");
            string name = Console.ReadLine();
            Console.Write("Введите телефон: ");
            string phone = Console.ReadLine();
            clients.Add(new Client { Name = name, Phone = phone });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Клиент успешно добавлен.");
            Console.ResetColor();
        }

        static void AddCouriers()
        {
            Console.Write("Введите имена курьеров (через запятую): ");
            string input = Console.ReadLine();
            var names = input.Split(',').Select(n => n.Trim());
            foreach (var name in names)
                couriers.Add(new Courier { Name = name });
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Курьеры добавлены.");
            Console.ResetColor();
        }

        static void CreateOrder()
        {
            if (clients.Count == 0 || couriers.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Добавьте хотя бы одного клиента и курьера.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n-- Выбор клиента --");
            Console.ResetColor();
            for (int i = 0; i < clients.Count; i++)
                Console.WriteLine($"{i + 1}. {clients[i].Name}");
            int clientIndex = Convert.ToInt32(Console.ReadLine()) - 1;
            var client = clients[clientIndex];

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n-- Выбор курьера --");
            Console.ResetColor();
            for (int i = 0; i < couriers.Count; i++)
                Console.WriteLine($"{i + 1}. {couriers[i].Name}");
            int courierIndex = Convert.ToInt32(Console.ReadLine()) - 1;
            var courier = couriers[courierIndex];

            Console.Write("Введите адрес доставки: ");
            string address = Console.ReadLine();

            Console.Write("Введите количество приборов: ");
            int utensils = Convert.ToInt32(Console.ReadLine());

            Console.Write("Время доставки (1 — по готовности, 2 — к определённому времени): ");
            int timeChoice = Convert.ToInt32(Console.ReadLine());

            string deliveryTime;
            if (timeChoice == 1)
                deliveryTime = "По готовности";
            else
            {
                Console.Write("Введите желаемое время доставки (например, 18:30): ");
                deliveryTime = Console.ReadLine();
            }

            List<string> dishes = new List<string>();
            List<decimal> prices = new List<decimal>();
            Console.WriteLine("Введите блюда (введите пустую строку для завершения):");
            while (true)
            {
                Console.Write("Название блюда: ");
                string dish = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(dish)) break;
                Console.Write("Цена блюда: ");
                decimal price = Convert.ToDecimal(Console.ReadLine());
                dishes.Add(dish);
                prices.Add(price);
            }

            var order = new Order
            {
                Client = client,
                Courier = courier,
                Dishes = dishes,
                Prices = prices,
                Address = address,
                Utensils = utensils,
                DeliveryTime = deliveryTime,
                Status = OrderStatus.Принят,
                Date = DateTime.Now
            };

            orders.Add(order);
            SaveOrdersToFile();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n╔══════════════════════════════════════╗");
            Console.WriteLine("║         ЗАКАЗ УСПЕШНО ОФОРМЛЕН       ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"\nКлиент: {client.Name}");
            Console.WriteLine($"Курьер: {courier.Name}");
            Console.WriteLine($"Адрес доставки: {address}");
            Console.WriteLine($"Количество приборов: {utensils}");
            Console.WriteLine($"Время доставки: {deliveryTime}");
            Console.WriteLine("Блюда:");
            decimal total = 0;
            for (int i = 0; i < dishes.Count; i++)
            {
                Console.WriteLine($" - {dishes[i]} — {prices[i]:0.00} руб.");
                total += prices[i];
            }
            Console.WriteLine($"ИТОГО: {total:0.00} руб.");
        }

        static void ShowOrders()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("Нет заказов.");
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n-- Все заказы --");
            Console.ResetColor();

            foreach (var o in orders)
            {
                Console.WriteLine($"[{o.Date}] {o.Client.Name} заказал:");
                for (int i = 0; i < o.Dishes.Count; i++)
                    Console.WriteLine($" - {o.Dishes[i]} ({o.Prices[i]:0.00} руб.)");
                Console.WriteLine($"Курьер: {o.Courier.Name} | Приборов: {o.Utensils} | Адрес: {o.Address} | Доставить: {o.DeliveryTime}");
                Console.WriteLine($"Статус: {o.Status}");
                Console.WriteLine("--------------------------------------------------");
            }
        }

        static void EditOrder()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("Нет заказов для изменения.");
                return;
            }

            for (int i = 0; i < orders.Count; i++)
                Console.WriteLine($"{i + 1}. Заказ клиента: {orders[i].Client.Name} — {orders[i].Status}");

            Console.Write("Выберите номер заказа: ");
            int index = Convert.ToInt32(Console.ReadLine()) - 1;
            var order = orders[index];

            Console.WriteLine("\nВыберите, что изменить:");
            Console.WriteLine("1. Адрес доставки");
            Console.WriteLine("2. Кол-во приборов");
            Console.WriteLine("3. Время доставки");
            Console.WriteLine("4. Блюда");
            Console.Write("Ваш выбор: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Новый адрес: ");
                    order.Address = Console.ReadLine();
                    break;
                case "2":
                    Console.Write("Новое кол-во приборов: ");
                    order.Utensils = Convert.ToInt32(Console.ReadLine());
                    break;
                case "3":
                    Console.Write("Новое время доставки: ");
                    order.DeliveryTime = Console.ReadLine();
                    break;
                case "4":
                    order.Dishes.Clear();
                    order.Prices.Clear();
                    Console.WriteLine("Введите блюда заново (пустая строка — завершение):");
                    while (true)
                    {
                        Console.Write("Блюдо: ");
                        string d = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(d)) break;
                        Console.Write("Цена: ");
                        decimal p = Convert.ToDecimal(Console.ReadLine());
                        order.Dishes.Add(d);
                        order.Prices.Add(p);
                    }
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Изменения сохранены.");
            Console.ResetColor();
            SaveOrdersToFile();
        }

        static void CancelOrder()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("Нет заказов для отмены.");
                return;
            }

            for (int i = 0; i < orders.Count; i++)
                Console.WriteLine($"{i + 1}. Заказ клиента: {orders[i].Client.Name} — {orders[i].Status}");

            Console.Write("Выберите номер заказа: ");
            int index = Convert.ToInt32(Console.ReadLine()) - 1;
            orders.RemoveAt(index);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Заказ отменён.");
            Console.ResetColor();
            SaveOrdersToFile();
        }

        static void UpdateOrderStatus()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("Нет заказов для обновления.");
                return;
            }

            for (int i = 0; i < orders.Count; i++)
                Console.WriteLine($"{i + 1}. Заказ клиента: {orders[i].Client.Name} — {orders[i].Status}");

            Console.Write("Выберите номер заказа: ");
            int index = Convert.ToInt32(Console.ReadLine()) - 1;

            Console.WriteLine("Новый статус: 1 — Принят, 2 — Готовится, 3 — ПереданКурьеру, 4 — Доставлен");
            int status = Convert.ToInt32(Console.ReadLine());
            orders[index].Status = (OrderStatus)(status - 1);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Статус обновлён.");
            Console.ResetColor();
            SaveOrdersToFile();
        }

        static void SaveOrdersToFile()
        {
            var lines = orders.Select(o =>
                $"{o.Date}|{o.Client.Name}|{o.Courier.Name}|{string.Join(",", o.Dishes)}|" +
                $"{string.Join(",", o.Prices)}|{o.Address}|{o.Utensils}|{o.DeliveryTime}|{o.Status}");
            File.WriteAllLines("orders.txt", lines);
        }
    }

    class Client
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    class Courier
    {
        public string Name { get; set; }
    }

    class Order
    {
        public Client Client { get; set; }
        public Courier Courier { get; set; }
        public List<string> Dishes { get; set; }
        public List<decimal> Prices { get; set; }
        public string Address { get; set; }
        public int Utensils { get; set; }
        public string DeliveryTime { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Date { get; set; }
    }

    enum OrderStatus
    {
        Принят,
        Готовится,
        ПереданКурьеру,
        Доставлен
    }

}

