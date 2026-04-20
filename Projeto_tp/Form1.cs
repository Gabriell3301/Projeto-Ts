using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_tp
{
    public partial class Form1 : Form
    {
        // Cliente TCP que estabelece ligação ao servidor
        private TcpClient cliente;

        // Stream usado para enviar e receber dados
        private NetworkStream stream;

        // Nome do utilizador (enviado junto com a mensagem)
        private string username = "Cliente 1";

        // Token para cancelar a thread de receção
        private CancellationTokenSource cts;

        /// Construtor do formulário
        public Form1()
        {
            InitializeComponent();
        }

        /// Evento executado quando o formulário é carregado.
        /// Responsável por estabelecer ligação ao servidor.
        private async void Form1_Load(object sender, EventArgs e)
        {
            // Desativa o botão de envio até estar ligado ao servidor
            btn_enviar.Enabled = false;

            // Garante que o utilizador tem um nome válido
            if (string.IsNullOrWhiteSpace(username))
            {
                username = "User";
            }

            // Inicializa o token de cancelamento
            cts = new CancellationTokenSource();

            try
            {
                cliente = new TcpClient();

                // Tenta ligar ao servidor (localhost:5000)
                await cliente.ConnectAsync("127.0.0.1", 5000);

                // Obtém o stream de comunicação
                stream = cliente.GetStream();

                // Ativa o botão de envio após ligação bem sucedida
                btn_enviar.Enabled = true;

                // Cria uma thread para receber mensagens em background
                Thread t = new Thread(ReceberMensagens) { IsBackground = true };
                t.Start();
            }
            catch (Exception ex)
            {
                // Mostra erro caso não consiga ligar ao servidor
                MessageBox.Show(this, "Falha ao ligar ao servidor: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// Evento do botão "Enviar".
        /// Envia uma mensagem para o servidor.
        private void btn_enviar_Click(object sender, EventArgs e)
        {
            // Obtém a mensagem da textbox
            string msg = textBox_msg.Text?.Trim();

            // Se estiver vazia, não faz nada
            if (string.IsNullOrEmpty(msg))
                return;

            // Verifica se está ligado ao servidor
            if (stream == null || cliente == null || !cliente.Connected)
            {
                MessageBox.Show(this, "Não está ligado ao servidor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Formata a mensagem com o nome do utilizador
            string msgFinal = username + ": " + msg;

            // Converte para bytes
            byte[] data = Encoding.UTF8.GetBytes(msgFinal);

            try
            {

                // Envia a mensagem
                stream.Write(data, 0, data.Length);

                // Mostra a mensagem na lista como enviada por "Eu"
                listBox1.Items.Add("Eu: " + msg);

                // Limpa a textbox
                textBox_msg.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Erro ao enviar: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// Método executado numa thread separada.
        /// Responsável por receber mensagens do servidor.
        private void ReceberMensagens()
        {
            if (stream == null)
                return;

            byte[] buffer = new byte[1024];

            try
            {
                // Continua a receber enquanto não for cancelado
                while (!cts.IsCancellationRequested)
                {
                    int bytes = 0;

                    try
                    {
                        // Lê dados do servidor
                        bytes = stream.Read(buffer, 0, buffer.Length);
                    }
                    catch (IOException)
                    {
                        // Ligação fechada pelo servidor
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }

                    // Se bytes == 0, houve desconexão
                    if (bytes == 0)
                        break;

                    // Converte bytes para string
                    string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                    // Remove "Nome: " da mensagem para exibição
                    string displayMsg = msg;
                    int idx = msg.IndexOf(": ");

                    if (idx >= 0 && idx + 2 < msg.Length)
                    {
                        displayMsg = msg.Substring(idx + 2);
                    }

                    // Atualiza a interface gráfica de forma segura (thread-safe)
                    if (listBox1.IsHandleCreated)
                    {
                        listBox1.Invoke(new Action(() =>
                        {
                            listBox1.Items.Add("Outro: " + displayMsg);
                        }));
                    }
                }
            }
            finally
            {
                // Fecha ligação e atualiza UI
                SafeClose();

                if (IsHandleCreated)
                {
                    Invoke(new Action(() =>
                    {
                        listBox1.Items.Add("Desligado do servidor.");
                        btn_enviar.Enabled = false;
                    }));
                }
            }
        }

        /// Evento chamado ao fechar o formulário.
        /// Garante que a ligação é terminada corretamente.
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Cancela a receção de mensagens
            cts?.Cancel();

            // Fecha recursos
            SafeClose();
        }

        /// Fecha o stream e o cliente de forma segura.
        private void SafeClose()
        {
            try
            {
                stream?.Close();
            }
            catch { }

            try
            {
                cliente?.Close();
            }
            catch { }

            stream = null;
            cliente = null;
        }

        /// Evento do botão "Limpar".
        /// Remove todas as mensagens da lista.
        private void btn_limpar_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
    }
}
