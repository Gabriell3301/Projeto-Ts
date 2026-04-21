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
        private string username;



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
                this.Close();
            }
        }

        /// Evento do botão "Enviar".
        /// Envia uma mensagem para o servidor.
        private void btn_enviar_Click(object sender, EventArgs e)
        {
            // Obtém a mensagem da textbox
            string msg = textBox_msg.Text?.Trim();
            if (string.IsNullOrWhiteSpace(msg))
            {
                MessageBox.Show(this, "Por favor, insira uma mensagem.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Se estiver vazia, não faz nada
            if (string.IsNullOrEmpty(msg))
                return;

            

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
        /// <summary>
        /// Processa as mensagens recebidas do servidor e atualiza a interface do usuário. 
        /// Se a mensagem tiver o prefixo "SET_NAME|", define o nome de usuário local e exibe uma mensagem de boas-vindas.
        /// </summary>
        /// <param name="msg">A string contendo a mensagem ou comando enviado pelo servidor.</param>
        private void HandleServerMessage(string msg)
        {
            if (msg.StartsWith("SET_NAME|"))
            {
                username = msg.Split('|')[1];

                Invoke(new Action(() =>
                {
                    nameLabel.Text = "Bem-vindo, " + username + "!";
                    btn_enviar.Enabled = true;
                }));

                return;
            }

            Invoke(new Action(() =>
            {
                listBox1.Items.Add(msg);
            }));
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
                while (!cts.IsCancellationRequested)
                {
                    int bytes = 0;

                    try
                    {
                        bytes = stream.Read(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        break;
                    }

                    if (bytes == 0)
                        break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                    HandleServerMessage(msg);
                }
            }
            finally
            {
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
