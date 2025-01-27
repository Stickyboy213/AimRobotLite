﻿
using AimRobot.Api;
using AimRobot.Api.command;
using AimRobot.Api.events.ev;
using AimRobot.Api.plugin;
using AimRobotLite.common;
using AimRobotLite.network.packet;
using AimRobotLite.plugin;
using AimRobotLite.Properties;
using AimRobotLite.service;
using AimRobotLite.service.robotplugin;
using log4net;
using log4net.Core;
using System;

namespace AimRobotLite {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            SettingInit();

            label9.Text = Resources.version;
            button1.Visible = Program.IsDebug();
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(Form1));

        private void button1_Click(object sender, EventArgs e) {
            //Robot.GetInstance().JoinGame(long.Parse(textBox7.Text));
            //ChatEventPacket pk = new ChatEventPacket();
            //pk.ev = new PlayerChatEvent();
            //pk.ev.message = string.Empty;
            //pk.ev.speaker = textBox7.Text;
            DeathEventPacket pk = new DeathEventPacket();
            pk.ev = new PlayerDeathEvent();
            pk.ev.killerPlatoon = string.Empty;
            pk.ev.killerName = "Ultra_Robot1";
            pk.ev.killerBy = string.Empty;
            pk.ev.playerPlatoon = string.Empty;
            pk.ev.playerName = "Shallow3nk";
            ((AimRobotLite)Robot.GetInstance()).GetWebSocketConnection().SendRemote(pk);
        }

        private void SettingInit() {
            var settingData = SettingFileHelper.GetData();
            textBox1.Text = settingData["setting"]["broadcast.content1"];
            textBox2.Text = settingData["setting"]["broadcast.content2"];
            textBox3.Text = settingData["setting"]["broadcast.content3"];
            textBox4.Text = settingData["setting"]["broadcast.content4"];

            checkBox1.Checked = bool.Parse(settingData["setting"]["banplayer.type2a"]);
            checkBox2.Checked = bool.Parse(settingData["setting"]["broadcast.rocketkill"]);
            checkBox4.Checked = bool.Parse(settingData["setting"]["banplayer.floodmsg"]);

            textBox9.Text = settingData["setting"]["remoteserver.wsurl"];
            textBox10.Text = settingData["setting"]["remoteserver.serverid"];
            textBox11.Text = settingData["setting"]["remoteserver.token"];

            /*******************/
            richTextBox1.SelectionColor = Color.DarkBlue;

            richTextBox1.AppendText($"Current Version {Resources.version}\n");

            DataApi.GetNewestVersion((version) => {
                if (!string.Equals(version, Resources.version)) {
                    MessageBox.Show(this, "当前不是最新版本，建议下载并使用最新版本");
                }
            });

        }

        private void button2_Click(object sender, EventArgs e) {
            var settingData = SettingFileHelper.GetData();
            settingData["setting"]["broadcast.content1"] = textBox1.Text;
            settingData["setting"]["broadcast.content2"] = textBox2.Text;
            settingData["setting"]["broadcast.content3"] = textBox3.Text;
            settingData["setting"]["broadcast.content4"] = textBox4.Text;

            settingData["setting"]["banplayer.type2a"] = checkBox1.Checked.ToString();

            settingData["setting"]["broadcast.rocketkill"] = checkBox2.Checked.ToString();

            settingData["setting"]["banplayer.floodmsg"] = checkBox4.Checked.ToString();

            settingData["setting"]["remoteserver.wsurl"] = textBox9.Text;
            settingData["setting"]["remoteserver.serverid"] = textBox10.Text;
            settingData["setting"]["remoteserver.token"] = textBox11.Text;

            SettingFileHelper.WriteData();
            MessageBox.Show(this, "已保存设置");
        }

        private void timer1_Tick(object sender, EventArgs e) {
            var robot = Robot.GetInstance();
            label3.Text = ((AimRobotLite)robot).GetRobotConnection().GetConnectionStatus().ToString();

            if (((AimRobotLite)robot).GetRobotConnection().GetConnectionStatus()) {
                var context = ((AimRobotLite)robot).GetGameContext();
                label5.Text = context.GetCurrentGameId().ToString();
                label7.Text = context.GetCurrentPlayerName().Length == 0 ? "未获取" : context.GetCurrentPlayerName();
            }

            label15.Text = ((AimRobotLite)robot).GetWebSocketConnection().IsConnectionAlive() ? "已连接" : "未连接";

            /********************/

        }

        public void refreshPluginListBox() {
            pluginInfoShow(null, false);

            var robot = Robot.GetInstance();
            ISet<PluginBase> plugins = robot.GetPluginManager().GetPlugins();
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            foreach (var plugin in plugins) {
                if (plugin.IsEnable()) {
                    listBox1.Items.Add(plugin.GetPluginName());
                } else {
                    listBox2.Items.Add(plugin.GetPluginName());
                }
            }
        }

        private static readonly Dictionary<Level, Color> LOG_LEVEL_COLOR = new Dictionary<Level, Color>() {
            {Level.Info, Color.Black },
            {Level.Warn, Color.Orange },
            {Level.Error, Color.Red },
            {Level.Fatal, Color.Red },
            {Level.Debug, Color.Gray },
            {Level.Notice, Color.Blue }
        };

        public void ConsoleTextBoxAppend(LoggingEvent loggingEvent) {
            richTextBox1.SelectionColor = LOG_LEVEL_COLOR.ContainsKey(loggingEvent.Level) ? LOG_LEVEL_COLOR[loggingEvent.Level] : richTextBox1.ForeColor;

            string lvl = loggingEvent.Level.ToString();
            if (lvl.Length < 7) lvl = lvl.PadRight(7);

            richTextBox1.AppendText(
                $"{loggingEvent.TimeStamp} {lvl} --- [{loggingEvent.ThreadName}] {loggingEvent.RenderedMessage}\n"
                );

            richTextBox1.ScrollToCaret();
        }

        public void KillLogTextBoxAppend(string s) {
            if (textBox5.InvokeRequired) {
                textBox5.Invoke(() => textBox5.AppendText($"{s}\n"));
            } else {
                textBox5.AppendText($"{s}\n");
            }
        }

        public void ChatLogTextBoxAppend(string s) {
            if (textBox6.InvokeRequired) {
                textBox6.Invoke(() => textBox6.AppendText($"{s}\n"));
            } else {
                textBox6.AppendText($"{s}\n");
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            Statement statement = new Statement();
            statement.Show();
        }

        private void button4_Click(object sender, EventArgs e) {
            Robot.GetInstance().BanPlayer(textBox7.Text);
        }

        private void button5_Click(object sender, EventArgs e) {
            Robot.GetInstance().SendChat(textBox8.Text);
        }

        private void Form1_FormClosed(object sender, FormClosingEventArgs e) {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            ((AimRobotLite)Robot.GetInstance()).GetRobotConnection().Close();
            ((AimRobotLite)Robot.GetInstance()).GetWebSocketConnection().Close();
            SettingFileHelper.WriteData();

            ((RobotPluginManager)Robot.GetInstance().GetPluginManager())._AutoSave(null);
        }

        private void button6_Click(object sender, EventArgs e) {
            ((AimRobotLite)Robot.GetInstance()).TryConnectRemoteServer();
        }

        private void button7_Click(object sender, EventArgs e) {
            if (listBox1.SelectedItem == null) {
                MessageBox.Show(this, "请选择要停用的插件");
                return;
            }

            string pluginName = listBox1.SelectedItem.ToString();

            Robot.GetInstance().GetPluginManager().DisablePlugin(Robot.GetInstance().GetPluginManager().GetPlugin(pluginName));

            refreshPluginListBox();
        }

        private void button8_Click(object sender, EventArgs e) {
            if (listBox2.SelectedItem == null) {
                MessageBox.Show(this, "请选择要启用的插件");
                return;
            }

            string pluginName = listBox2.SelectedItem.ToString();

            Robot.GetInstance().GetPluginManager().EnablePlugin(Robot.GetInstance().GetPluginManager().GetPlugin(pluginName));

            refreshPluginListBox();
        }

        private void button9_Click(object sender, EventArgs e) {
            refreshPluginListBox();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e) {
            var select = listBox2.SelectedItem;
            if (select != null) {
                PluginBase pluginBase = Robot.GetInstance().GetPluginManager().GetPlugin(select.ToString());

                if (pluginBase != null) pluginInfoShow(pluginBase, true);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
            var select = listBox1.SelectedItem;
            if (select != null) {
                PluginBase pluginBase = Robot.GetInstance().GetPluginManager().GetPlugin(select.ToString());

                if (pluginBase != null) pluginInfoShow(pluginBase, true);
            }
        }

        private void pluginInfoShow(PluginBase pluginBase, bool show) {
            label18.Visible = show;
            label19.Visible = show;
            label20.Visible = show;
            label21.Visible = show;

            label22.Visible = show;
            label23.Visible = show;
            label24.Visible = show;
            label25.Visible = show;

            if (pluginBase != null) {
                label22.Text = pluginBase.GetPluginName();
                label23.Text = pluginBase.GetAuthor();
                label24.Text = pluginBase.GetVersion().ToString();
                label25.Text = pluginBase.GetDescription();
            }
        }

        private void button10_Click(object sender, EventArgs e) {
            richTextBox1.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }

        private void button11_Click(object sender, EventArgs e) {
            Robot.GetInstance().GetPluginManager().CheckCommand(
                null,
                textBox12.Text.StartsWith(ICommandListener.CMD_SIGN) ? textBox12.Text : ICommandListener.CMD_SIGN + textBox12.Text
                );
            textBox12.Text = "";
        }

        private void textBox12_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                Robot.GetInstance().GetPluginManager().CheckCommand(
                    null,
                    textBox12.Text.StartsWith(ICommandListener.CMD_SIGN) ? textBox12.Text : ICommandListener.CMD_SIGN + textBox12.Text
                    );
                textBox12.Text = "";
            }
        }

        private void button12_Click(object sender, EventArgs e) {
            Robot.GetInstance().KickPlayer(textBox7.Text);
        }

        private void button13_Click(object sender, EventArgs e) {
            Robot.GetInstance().UnBanPlayer(textBox7.Text);
        }
    }
}
