using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсовая_работа
{
    class Generation
    {
        public TextBox tb;
        private bool _isActive = false;
        public SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\курсач\\Курсовая работа\\Курсовая работа\\DB.mdf;Integrated Security=True");
        public Generation(TextBox t)                   
        {

            tb = t;
            conn.Open();
        }
        public void Start()
        {
            _isActive = true;
            Task.Run(() => Generate());
            Task.Run(() => GenerateClients());
            Task.Run(() => GenerateGoods());
        }
        public void Stop()
        {
            _isActive = false;
        }
        private void Generate()
        {
            while (_isActive)
            {
                Client c = PickClient();
                Order order = GenerateOrder(c);
                Checkout(order);
                Thread.Sleep(1000);
            }
        }
        public List<Client> clients = new List<Client>();
        public List<Order> orders = new List<Order>();
        public List<Goods> goods = new List<Goods>();
        public Buy buy = new Buy();

        public List<string> clientNames = new List<string> { "Покупатель"};
        public List<string> goodsTypes = new List<string> { "отечетсвенный", "импортный"};
        public List<string> goodsTransport = new List<string> { "морской", "сухопутный"};


        public void GenerateClients()
        {
            while (_isActive)
            {
                Client c = new Client();
                Random random = new Random();
                c.Name = clientNames[random.Next(0, clientNames.Count)];
                clients.Add(c);
                Thread.Sleep(1000);
            }
        }
        public void GenerateGoods()
        {
            while (_isActive)
            {
                Goods g = new Goods();
                Random random = new Random();
                g.Name = goodsTransport[random.Next(0, goodsTypes.Count)];
                g.Object = goodsTypes[random.Next(0, goodsTransport.Count)];
                if (g.Name == "морской") g.Price = random.Next(1, 9999) + 10000;
                if (g.Name == "сухопутный") g.Price = random.Next(1, 9999) + 10000;
                goods.Add(g);
                Thread.Sleep(1000);
            }
        }
        public Client PickClient()
        {
            if (clients.Count > 0)
            {
                Random random = new Random();
                return clients[random.Next(0, clients.Count)];
            }
            return null;
        }

        public Order GenerateOrder(Client c)
        {
            if (c == null) return null;
            if (goods.Count <= 0) return null;
            Random random = new Random();
            Goods g = goods[random.Next(0, goods.Count)];
            Order order = new Order() { Client = c, Goods = g, Date = DateTime.Now };
            return order;
        }

        public void Checkout(Order o)
        {
            if (o == null) return;
            orders.Add(o);
            buy.Account += o.Goods.Price;
            tb.Invoke(new Action(() => tb.Text = $"{o.Client.Name} купил {o.Goods.Name} товар за {o.Goods.Price} руб. Тип перевозки: {o.Goods.Object}.  Дата заказа: {o.Date.ToString("yyyy.MM.dd")}"));
            Insert(o);
            Thread.Sleep(500);

        }
        public void Insert(Order o)
        {
            string query = "INSERT INTO [Order] (clientName, goodsType, goodsTransport, date, price) VALUES (@clientName, @goodsType, @goodsTransport, @date, @price)";
            SqlCommand com = new SqlCommand(query, conn);
            com.Parameters.Add("@clientName", System.Data.SqlDbType.NVarChar);
            com.Parameters.Add("@goodsType", System.Data.SqlDbType.NVarChar);
            com.Parameters.Add("@goodsTransport", System.Data.SqlDbType.NVarChar);
            com.Parameters.Add("@date", System.Data.SqlDbType.Date);
            com.Parameters.Add("@price", System.Data.SqlDbType.Decimal);
            com.Parameters["@clientName"].Value = o.Client.Name;
            com.Parameters["@goodsType"].Value = o.Goods.Name;
            com.Parameters["@goodsTransport"].Value = o.Goods.Object;
            com.Parameters["@date"].Value = o.Date;
            com.Parameters["@price"].Value = o.Goods.Price;
            com.ExecuteNonQuery();
        }
    }
}
