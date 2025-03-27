using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExercicioContaBancaria
{
    public class Cliente
    {
        public string Cpf { get; private set; }
        public string Nome { get; private set; }
        public List<Conta> Contas { get; private set; } = new List<Conta>();

        private static List<Cliente> clientes = new List<Cliente>();

        public Cliente(string CpfCliente, string NomeCliente)
        {
            Cpf = CpfCliente;
            Nome = NomeCliente;
        }

        public void AddCliente()
        {
            if (!ExisteCliente(Cpf))
            {
                clientes.Add(this);
            }
            else
            {
                Console.WriteLine("Este CPF já está cadastrado!");
            }
        }

        public static Cliente ObterClienteCpf(string cpf)
        {
            return clientes.FirstOrDefault(c => c.Cpf == cpf);
        }

        public static bool ExisteCliente(string Cpf)
        {
            return clientes.Any(c => c.Cpf == Cpf);
           
        }

        public void AddConta(Conta conta)
        {
            Contas.Add(conta);
        }
    }

    public class Conta
    {
        public int Numero { get; private set; }
        public double Saldo { get; private set; }
        public Cliente Cliente { get; private set; }

        private static List<Conta> contas = new List<Conta>();

        public Conta(double saldo, Cliente cliente)
        {
            Numero = GerarNumUnico();
            Saldo = 0;
            Cliente = cliente;
            contas.Add(this);
            cliente.AddConta(this);
        }

        private static int GerarNumUnico()
        {
            return contas.Count == 0 ? 1001 : contas.Max(c => c.Numero) + 1;
        }

        public static void ListarContas()
        {
            if (contas.Count == 0)
            {
                Console.WriteLine("Nenhuma conta existente");
                return;
            }

            Console.WriteLine("\nLista de Contas:");
            foreach (var conta in contas)
            {
                Console.WriteLine($"Número: {conta.Numero}, Cliente: {conta.Cliente.Nome}, CPF: {conta.Cliente.Cpf}, Saldo: R$ {conta.Saldo:F2}");
            }
        }

        public void Sacar(double amount)
        {              
            if (amount > 0 && amount <= Saldo)
            {
                Saldo -= amount;
                Console.WriteLine("Saque realizado com sucesso!");
            }
            else
            {
                Console.WriteLine("Saldo insuficiente ou inválido para saque!");
            }
        }

        public void Depositar(double amount)
        {           
            if (amount > 0)
            {
                Saldo += amount;
                Console.WriteLine($"Depósito de R$ {amount} realizado com sucesso!");
            }
            else
            {
                Console.WriteLine("Valor inválido para depósito!");
            }                           
        }

        public static Conta ObterContaNum(int numero)
        {
            return contas.FirstOrDefault(c => c.Numero == numero);
        }

        public static void Consultar(int contaNum)
        {
            Conta conta = contas.FirstOrDefault(c => c.Numero == contaNum);
            if (conta != null)
            {
                Console.WriteLine($"O saldo da conta {conta.Numero}, vinculada ao CPF {conta.Cliente.Cpf} ({conta.Cliente.Nome}), é: {conta.Saldo}");
            }
            else
            {
                Console.WriteLine("Conta não encontrada!");
            }

        }

        public void Transferir(Conta ContaRecipient, Conta ContaSender, double amount)
        {
            if(ContaSender == ContaRecipient)
            {
                Console.WriteLine("Não é possível transferir para a própria conta!");
                return;
            }

            if (ContaSender.Saldo < amount)
            {
                Console.WriteLine("Saldo insufiente para a transferência!");
                return;
            }

            ContaSender.Saldo -= amount;
            ContaRecipient.Saldo += amount;

            Console.WriteLine($"Tranferência de R$ {amount} realizada com sucesso! Sendo o envio da conta {ContaSender.Numero} para {ContaRecipient.Numero}!");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Digite o CPF do Cliente: ");
            string cpf = Console.ReadLine();

            while(cpf.Length != 11 || !cpf.All(char.IsDigit))
            {
                Console.WriteLine("CPF incorreto! Digite novamente!");
                Console.WriteLine("Digite o CPF do Cliente: ");
                cpf = Console.ReadLine();
            }

            Console.Write("Digite o Nome do Cliente: ");
            string nome = Console.ReadLine();

            while (!nome.All(c => char.IsLetter(c) || c == ' '))
            {
                Console.WriteLine("Nome incorreto! Digite novamente!");
                Console.WriteLine("Digite o Nome do Cliente: ");
                nome = Console.ReadLine();
            }

            Cliente ExisteCliente = Cliente.ObterClienteCpf(cpf);
            Cliente cliente = null;

            if (ExisteCliente != null)
            {
                Console.WriteLine("Cliente já existente");
            }
            else
            {
                cliente = new Cliente(cpf, nome);
                cliente.AddCliente();
                Console.WriteLine("Cliente criado com sucesso!");
            }

            Console.WriteLine("Criando a conta do cliente...");
            Conta novaConta = new Conta(0 , cliente);

            bool showMenu;
            do
            {
                showMenu = MainMenu(novaConta);
            } while (showMenu);
        }

        private static bool MainMenu(Conta conta)
        {
            string opcao;

            do
            {
                Console.WriteLine($"Bem-vindo, {conta.Cliente.Nome}! Selecione uma opção: ");
                Console.WriteLine("1) Saque");
                Console.WriteLine("2) Depósito");
                Console.WriteLine("3) Consulta");
                Console.WriteLine("4) Transferência");
                Console.WriteLine("5) Nova Conta");
                Console.WriteLine("6) Sair");
                Console.WriteLine("\r\nSelecione uma opção: ");

                opcao = Console.ReadLine();

                if(opcao != "1" && opcao != "2" &&  opcao != "3" && opcao != "4" && opcao != "5" && opcao != "6")
                {
                    Console.WriteLine("Opção inválida! Tente Novamente!");
                }

            } while (opcao != "1" && opcao != "2" && opcao != "3" && opcao != "4" && opcao != "5" && opcao != "6");

            switch (opcao)
            {
                case "1":
                    Console.WriteLine("Digite o valor do saque: ");
                    if (double.TryParse(Console.ReadLine(), out double saque))
                    {
                        conta.Sacar(saque);
                    }
                    else
                    {
                        Console.WriteLine("Valor inválido! Tente Novamente!");
                    }
                    return true;

                case "2":
                    Console.WriteLine("Digite o Valor do depósito: ");
                    if (double.TryParse(Console.ReadLine(), out double deposito))
                    {
                        conta.Depositar(deposito);
                    }
                    else
                    {
                        Console.WriteLine("Valor inválido! Tente Novamente!");
                    }
                    return true;

                case "3":
                    Console.WriteLine("Escreva a conta a ser consultada: ");
                    if (int.TryParse(Console.ReadLine(), out int contaConsulta))
                    {
                        Conta.Consultar(contaConsulta);
                    }
                    else
                    {
                        Console.WriteLine("Número de conta inválido!");
                    }
                    Conta.ListarContas();
                    return true;

                case "4":
                    Console.WriteLine("Digite o numero do destinatário: ");
                    if (int.TryParse(Console.ReadLine(), out int numSender))
                    {
                        Conta contaSender = Conta.ObterContaNum(numSender);

                        if (contaSender != null)
                        {
                            Console.WriteLine("Digite o valor da transferência: ");
                            if (double.TryParse(Console.ReadLine(), out double valorTransferencia))
                            {
                                conta.Transferir(contaSender, conta, valorTransferencia);
                            }
                            else
                            {
                                Console.WriteLine("Valor inválido! Tente Novamente!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Conta não encontrada!");
                        }                     
                    }
                    else
                    {
                        Console.WriteLine("Número inválido");
                    }
                    Conta.ListarContas();
                    return true;

                case "5":
                    Console.Write("Digite o CPF do Cliente: ");
                    string Novocpf = Console.ReadLine();

                    while (Novocpf.Length != 11 || !Novocpf.All(char.IsDigit))
                    {
                        Console.WriteLine("CPF incorreto! Digite novamente!");
                        Console.WriteLine("Digite o CPF do Cliente: ");
                        Novocpf = Console.ReadLine();
                    }

                    Console.Write("Digite o Nome do Cliente: ");
                    string NovoNome = Console.ReadLine();

                    while (!NovoNome.All(c => char.IsLetter(c) || c == ' '))
                    {
                        Console.WriteLine("Nome incorreto! Digite novamente!");
                        Console.WriteLine("Digite o Nome do Cliente: ");
                        NovoNome = Console.ReadLine();
                    }

                    Cliente clienteExiste = Cliente.ObterClienteCpf(Novocpf);
                    if(clienteExiste != null)
                    {
                        Console.WriteLine("Este CPF já está cadastrado!");
                    }
                    else
                    {
                        Cliente NovoCliente = new Cliente(Novocpf, NovoNome);
                        NovoCliente.AddCliente();
                        Console.WriteLine("Cliente criado com sucesso!");
                        Conta NovaConta = new Conta(0, NovoCliente);
                        Console.WriteLine("Conta criada com sucesso!");
                    }

                    return true;

                case "6":
                    return false;

                default:
                        return true;
                }
        }
    }
}
