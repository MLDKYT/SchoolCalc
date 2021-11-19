using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SchoolCalc3
{
    public partial class Form1 : Form
    {
        private bool _isOnline;
        private float _onlineWattage;
        private float _offlineWattage;
        private int _onlineLessons;
        private int _offlineLessons;
        private int _homeWorkTestLessons;
        private Dictionary<string, StringBuilder> informationsAboutEachLesson;

        public Form1()
        {
            InitializeComponent();
            string path = Application.CommonAppDataPath + "/save2.xml";
            if (File.Exists(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveClass));
                FileStream stream = File.Open(path, FileMode.Open);
                SaveClass save = (SaveClass) serializer.Deserialize(stream);
                _onlineWattage = save.Wattage.OnlineWattage;
                _offlineWattage = save.Wattage.OfflineWattage;
                _onlineLessons = save.Lessons.OnlineLessons;
                _offlineLessons = save.Lessons.OfflineLessons;
                _homeWorkTestLessons = save.Lessons.HomeWorkTestLessons;
                numericUpDown1.Value = Decimal.Parse(save.Other.ComputerWattage.ToString(CultureInfo.CurrentCulture));
                stream.Close();
            }

            button1.Text = @"Offline";
            label1.Text = $@"Online: {_onlineWattage}W";
            label2.Text = $@"Offline: {_offlineWattage}W";
            label3.Text = $@"Online: {_onlineLessons}";
            label4.Text = $@"Offline: {_offlineLessons}";
            label5.Text = $@"Homework/Test instead of lesson: {_homeWorkTestLessons}";

            Timer timer = new Timer {Interval = 1000};
            timer.Tick += (sender, args) =>
            {
                if (_isOnline)
                {
                    _onlineWattage = _onlineWattage + float.Parse(numericUpDown1.Value.ToString(CultureInfo.CurrentCulture)) / 3600;
                    label1.Text = @"Online: " + _onlineWattage + @"W";
                }
                else
                {
                    _offlineWattage = _offlineWattage + float.Parse(numericUpDown1.Value.ToString(CultureInfo.CurrentCulture)) / 3600;
                    label2.Text = @"Offline: " + _offlineWattage + @"W";
                }
            };
            timer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _isOnline = !_isOnline;
            button1.Text = _isOnline ? "Online" : "Offline";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _onlineLessons += 1;
            label3.Text = @"Online: " + _onlineLessons;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _offlineLessons += 1;
            label4.Text = @"Offline: " + _offlineLessons;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _homeWorkTestLessons += 1;
            label5.Text = @"Homework/Test instead of lesson: " + _homeWorkTestLessons;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save all the shit
            WattageStuff wattageStuff = new WattageStuff(_onlineWattage, _offlineWattage);
            LessonsStuff lessonsStuff = new LessonsStuff(_onlineLessons, _offlineLessons, _homeWorkTestLessons);
            OtherStuff otherStuff = new OtherStuff(float.Parse(numericUpDown1.Value.ToString(CultureInfo.CurrentCulture)));
            SaveClass saveStuff = new SaveClass(wattageStuff, lessonsStuff, otherStuff);

            string path = Application.CommonAppDataPath + "/save2.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(SaveClass));
            if (File.Exists(path))
                File.Delete(path);
            FileStream stream = File.Create(path);
            serializer.Serialize(stream, saveStuff);
            stream.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("#############HLAVNI INFORMACE###################");
            builder.AppendLine("###informace o elektrice#########");
            builder.AppendLine("Při online                                                : " + _onlineWattage + "W");
            builder.AppendLine("Ostatní (úkoly, přestávky, neexistující online hodiny)    : " + _offlineWattage + "W");
            builder.AppendLine("");
            builder.AppendLine("###informace o online hodinách###");
            builder.AppendLine("Online hodiny                                             : " + _onlineLessons);
            builder.AppendLine("Hodiny které vůbec neexistovali                           : " + _offlineLessons);
            builder.AppendLine("Hodiny, na které se učitel vysr- dal nám úkol místo hodiny: " + _homeWorkTestLessons);
            builder.AppendLine("");
            builder.AppendLine("#############INFORMACE O HODINACH###############");
            foreach (var variable in informationsAboutEachLesson)
            {
                builder.AppendLine(variable.Key + ": ");
                builder.AppendLine(variable.Value.ToString());
            }

            builder.AppendLine("################################################");
            builder.AppendLine("//Vygenerováno pomocí programu, který je používán na sledování");
            builder.AppendLine("//online hodin. ");
        }
    }

    [Serializable]
    public class SaveClass
    {
        public SaveClass(WattageStuff wattage, LessonsStuff lessons, OtherStuff other)
        {
            Wattage = wattage;
            Lessons = lessons;
            Other = other;
        }

        public SaveClass()
        {
            // For serialization support you need to create an empty constructor
        }

        public WattageStuff Wattage;
        public LessonsStuff Lessons;
        public OtherStuff Other;
    }

    [Serializable]
    public class WattageStuff
    {
        public WattageStuff(float onlineWattage, float offlineWattage)
        {
            OnlineWattage = onlineWattage;
            OfflineWattage = offlineWattage;
        }

        public WattageStuff()
        {
            // For serialization support you need to create an empty constructor
        }

        public float OnlineWattage;
        public float OfflineWattage;

        public void SetOnlineWattages(float onlineWattage, float offlineWattage)
        {
            OnlineWattage = onlineWattage;
            OfflineWattage = offlineWattage;
        }
    }

    [Serializable]
    public class LessonsStuff
    {
        public LessonsStuff(int onlineLessons, int offlineLessons, int homeWorkTestLessons)
        {
            OnlineLessons = onlineLessons;
            OfflineLessons = offlineLessons;
            HomeWorkTestLessons = homeWorkTestLessons;
        }

        public LessonsStuff()
        {
            // For serialization support you need to create an empty constructor
        }

        public int OnlineLessons;
        public int OfflineLessons;
        public int HomeWorkTestLessons;

        public void SetLessons(int onlineLessons, int offlineLessons, int homeWorkTestLessons)
        {
            OnlineLessons = onlineLessons;
            OfflineLessons = offlineLessons;
            HomeWorkTestLessons = homeWorkTestLessons;
        }
    }

    public class OtherStuff
    {
        public OtherStuff(float computerWattage)
        {
            ComputerWattage = computerWattage;
        }

        public OtherStuff()
        {
            // For serialization support you need to create an empty constructor
        }

        public float ComputerWattage;
    }
}