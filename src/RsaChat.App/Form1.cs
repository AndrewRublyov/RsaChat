using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace RsaChat.App
{
    public partial class Form1 : Form
    {
        private string _publicKey = string.Empty;

        private HubConnection _connection;
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chat")
                .Build();

            _connection.On<string>("PublicKey", publicKey =>
            {
                _publicKey = publicKey;
                richTextBox1.AppendInfo("Public key received");
                SetInputState(true);
            });

            _connection.On<string, string>("Receive", (author, message) =>
            {
                richTextBox1.AppendMessage(author, message);
            });
            
            _connection.Closed += error =>
            {
                richTextBox1.AppendError($"Connection with server lost: {error.Message}");
                SetInputState(false);
                return Task.CompletedTask;
            };

            try
            {
                await _connection.StartAsync();
                await _connection.InvokeAsync("RequestPublicKey");
            }
            catch (Exception ex)
            {
                richTextBox1.AppendError($"Failed to connect: {ex.Message}");
            }
        }

        private void SetInputState(bool enabled)
        {
            textBox1.Clear();

            textBox1.Enabled = enabled;
            button1.Enabled = enabled;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                return;
            }
            
            await _connection.InvokeAsync("Send", EncryptorRSA.EncryptText($"{textBox2.Text}_|_{textBox1.Text}", _publicKey));
            textBox1.Clear();
        }
    }
}