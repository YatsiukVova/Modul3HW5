using System;
using System.Collections.Generic;
using System.Xml;
namespace MediatorPattern
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Mediator
            ManagerMediator mediator = new ManagerMediator();
            Colleague customer = new CustomerColleague(mediator);
            Colleague programmer = new ProgrammerColleague(mediator);
            Colleague tester = new TesterColleague(mediator);
            mediator.Customer = customer;
            mediator.Programmer = programmer;
            mediator.Tester = tester;
            customer.Send("Есть заказ, надо сделать программу");
            programmer.Send("Программа готова, надо протестировать");
            tester.Send("Программа протестирована и готова к продаже");

            Console.WriteLine();

            // Visitor
            var structure = new Bank();
            structure.Add(new Person { Name = "Иван Алексеев", Number = "82184931" });
            structure.Add(new Company { Name = "Microsoft", RegNumber = "ewuir32141324", Number = "3424131445" });
            structure.Accept(new HtmlVisitor());
            structure.Accept(new XmlVisitor());

            Console.Read();
        }
        public abstract class Mediator
        {
            public abstract void Send(string msg, Colleague colleague);
        }

        abstract class Colleague
        {
            protected Mediator mediator;

            public Colleague(Mediator mediator)
            {
                this.mediator = mediator;
            }

            public virtual void Send(string message)
            {
                mediator.Send(message, this);
            }
            public abstract void Notify(string message);
        }
        // класс заказчика
        class CustomerColleague : Colleague
        {
            public CustomerColleague(Mediator mediator)
                : base(mediator)
            { }

            public override void Notify(string message)
            {
                Console.WriteLine("Сообщение заказчику: " + message);
            }
        }
        // класс программиста
        class ProgrammerColleague : Colleague
        {
            public ProgrammerColleague(Mediator mediator)
                : base(mediator)
            { }

            public override void Notify(string message)
            {
                Console.WriteLine("Сообщение программисту: " + message);
            }
        }
        // класс тестера
        class TesterColleague : Colleague
        {
            public TesterColleague(Mediator mediator)
                : base(mediator)
            { }

            public override void Notify(string message)
            {
                Console.WriteLine("Сообщение тестеру: " + message);
            }
        }

        class ManagerMediator : Mediator
        {
            public Colleague Customer { get; set; }
            public Colleague Programmer { get; set; }
            public Colleague Tester { get; set; }
            public override void Send(string msg, Colleague colleague)
            {
                // если отправитель - заказчик, значит есть новый заказ
                // отправляем сообщение программисту - выполнить заказ
                if (Customer == colleague)
                    Programmer.Notify(msg);
                // если отправитель - программист, то можно приступать к тестированию
                // отправляем сообщение тестеру
                else if (Programmer == colleague)
                    Tester.Notify(msg);
                // если отправитель - тест, значит продукт готов
                // отправляем сообщение заказчику
                else if (Tester == colleague)
                    Customer.Notify(msg);
            }
            interface IVisitor
            {
                void VisitPersonAcc(Person acc);
                void VisitCompanyAc(Company acc);
            }

            // сериализатор в HTML
            class HtmlVisitor : IVisitor
            {
                public void VisitPersonAcc(Person acc)
                {
                    string result = "<table><tr><td>Свойство<td><td>Значение</td></tr>";
                    result += "<tr><td>Name<td><td>" + acc.Name + "</td></tr>";
                    result += "<tr><td>Number<td><td>" + acc.Number + "</td></tr></table>";
                    Console.WriteLine(result);
                }

                public void VisitCompanyAc(Company acc)
                {
                    string result = "<table><tr><td>Свойство<td><td>Значение</td></tr>";
                    result += "<tr><td>Name<td><td>" + acc.Name + "</td></tr>";
                    result += "<tr><td>RegNumber<td><td>" + acc.RegNumber + "</td></tr>";
                    result += "<tr><td>Number<td><td>" + acc.Number + "</td></tr></table>";
                    Console.WriteLine(result);
                }
            }

            // сериализатор в XML
            class XmlVisitor : IVisitor
            {
                public void VisitPersonAcc(Person acc)
                {
                    string result = "<Person><Name>" + acc.Name + "</Name>" +
                        "<Number>" + acc.Number + "</Number><Person>";
                    Console.WriteLine(result);
                }

                public void VisitCompanyAc(Company acc)
                {
                    string result = "<Company><Name>" + acc.Name + "</Name>" +
                        "<RegNumber>" + acc.RegNumber + "</RegNumber>" +
                        "<Number>" + acc.Number + "</Number><Company>";
                    Console.WriteLine(result);
                }
            }

            class Bank
            {
                List<IAccount> accounts = new List<IAccount>();
                public void Add(IAccount acc)
                {
                    accounts.Add(acc);
                }
                public void Remove(IAccount acc)
                {
                    accounts.Remove(acc);
                }
                public void Accept(IVisitor visitor)
                {
                    foreach (IAccount acc in accounts)
                        acc.Accept(visitor);
                }
            }

            interface IAccount
            {
                void Accept(IVisitor visitor);
            }

            class Person : IAccount
            {
                public string Name { get; set; }
                public string Number { get; set; }

                public void Accept(IVisitor visitor)
                {
                    visitor.VisitPersonAcc(this);
                }
            }

            class Company : IAccount
            {
                public string Name { get; set; }
                public string RegNumber { get; set; }
                public string Number { get; set; }

                public void Accept(IVisitor visitor)
                {
                    visitor.VisitCompanyAc(this);
                }
            }
        }
    }
}
