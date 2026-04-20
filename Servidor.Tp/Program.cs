using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Servidor.Tp
{
    internal class Program
    {
        /// Método principal que inicia o servidor TCP.
        /// O servidor fica à escuta de clientes na porta 5000 e cria uma thread
        /// para cada cliente que se liga.
        static void Main(string[] args)
        {
            // Cria o servidor TCP que aceita ligações em qualquer IP na porta 5000
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start(); // Inicia o servidor

            Console.WriteLine("Servidor ligado...");

            // Lista de clientes conectados
            List<TcpClient> clientes = new List<TcpClient>();

            // Dicionário que associa cada cliente ao seu nome
            Dictionary<TcpClient, string> nomes = new Dictionary<TcpClient, string>();

            // Objeto para garantir acesso seguro às estruturas partilhadas (thread-safe)
            object clientesLock = new object();

            // Ciclo infinito para aceitar novos clientes
            while (true)
            {
                // Espera até que um cliente se ligue
                TcpClient cliente = server.AcceptTcpClient();

                string nome;

                // Bloqueio para evitar problemas de concorrência
                lock (clientesLock)
                {
                    clientes.Add(cliente);

                    // Atribui um nome automático ao cliente (ex: cliente1, cliente2...)
                    nome = "cliente" + clientes.Count;
                    nomes[cliente] = nome;
                }

                Console.WriteLine("Cliente ligado: " + nome);

                // Cria uma nova thread para tratar este cliente
                Thread t = new Thread(() => HandleClient(cliente, clientes, nomes, clientesLock));
                t.IsBackground = true; // Thread termina quando o programa principal fecha
                t.Start();
            }
        }

        /// Método responsável por comunicar com um cliente específico.
        /// Recebe mensagens e reencaminha para os restantes clientes.
        private static void HandleClient(
            TcpClient cliente,
            List<TcpClient> clientes,
            Dictionary<TcpClient, string> nomes,
            object clientesLock)
        {
            string meuNome;

            // Obtém o nome do cliente atual
            lock (clientesLock)
            {
                nomes.TryGetValue(cliente, out meuNome);

                if (string.IsNullOrEmpty(meuNome))
                    meuNome = "cliente?";
            }

            try
            {
                // Obtém o stream de comunicação com o cliente
                using (NetworkStream stream = cliente.GetStream())
                {
                    byte[] buffer = new byte[1024]; // Buffer para armazenar dados recebidos

                    while (true)
                    {
                        int bytes;

                        try
                        {
                            // Lê dados do cliente
                            bytes = stream.Read(buffer, 0, buffer.Length);
                        }
                        catch
                        {
                            // Se ocorrer erro na leitura, assume desconexão
                            break;
                        }

                        // Se bytes == 0 significa que o cliente desligou
                        if (bytes == 0)
                            break;

                        // Converte os bytes recebidos para string
                        string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                        // Remove o prefixo "Nome: " apenas para mostrar no servidor
                        string displayMsg = msg;
                        int idx = msg.IndexOf(": ");

                        if (idx >= 0 && idx + 2 < msg.Length)
                        {
                            displayMsg = msg.Substring(idx + 2);
                        }

                        // Mostra a mensagem no servidor
                        Console.WriteLine(meuNome + " mensagem: " + displayMsg);

                        // Converte novamente a mensagem para bytes para enviar
                        byte[] data = Encoding.UTF8.GetBytes(msg);

                        // Cria uma cópia da lista de clientes para evitar erros durante iteração
                        List<TcpClient> snapshot;

                        lock (clientesLock)
                        {
                            snapshot = clientes.ToList();
                        }

                        // Envia a mensagem para todos os clientes exceto o próprio
                        foreach (var c in snapshot)
                        {
                            if (c == cliente)
                                continue;

                            try
                            {
                                NetworkStream s = c.GetStream();
                                s.Write(data, 0, data.Length);
                            }
                            catch (Exception)
                            {
                                // Se falhar o envio, remove o cliente problemático
                                try { c.Close(); } catch { }

                                lock (clientesLock)
                                {
                                    if (clientes.Contains(c))
                                        clientes.Remove(c);

                                    if (nomes.ContainsKey(c))
                                        nomes.Remove(c);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostra erro caso algo corra mal com o cliente
                Console.WriteLine("Erro no cliente " + meuNome + ": " + ex.Message);
            }
            finally
            {
                // Fecha a ligação do cliente
                try { cliente.Close(); } catch { }

                // Remove o cliente das estruturas
                lock (clientesLock)
                {
                    if (clientes.Contains(cliente))
                        clientes.Remove(cliente);

                    if (nomes.ContainsKey(cliente))
                        nomes.Remove(cliente);
                }

                Console.WriteLine(meuNome + " desligado.");
            }
        }
    }
}
