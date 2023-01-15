using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Collections.Specialized;

namespace laba6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Simvols
        {
            public char[] text;

            public string[] SIMVOLMASRAZ = new string[] { "+", "-", "/", "*", ",", ";", ">", "<", "!=", ">=", "=", "(", ")", "{", "}" };
            public string[] SIMVOLMASL = new string[] { "do", "int", "char", "for", "if", "while", "break", "else", "default", "double", "static", "main" };
            public string[] SIMVOLVERH = new string[] { "<программа>", "<спис_опис>", "<опис>", "<тип>", "<спис_перем>", "<опер>", "<условн.>", "<услов.>", "<блок опер.>", "<присв.>", "<знак>", "<операнд>" };
            public List<string> SIMVOLMASID = new List<string>();
            public int[] SIMVOLMASLIT = new int[] { };
            public List<MyDataType> list = new List<MyDataType>();
            public class MyDataType
            {
                public string leksema { get; set; }
                public string type { get; set; }
                public string position { get; set; }

            };

            public string simv;
            public char[] newtext;

            public List<string> Sint = new List<string>();


            int versh = 0;
            int pos_vhod = 1;
            int pos_sost = 0;
            int state = 0;
            public string[] stack;
            public int[] sostStack = new int[30];
            public string[] vhodStr;
            bool iserror = true;


            public void SDVIG()
            {
                try
                {
                    //versh += 1;
                    //stack[versh] = vhodStr[pos_vhod];
                    //pos_vhod += 1;
                    versh += 1;
                }
                catch (Exception ex) { iserror = false; }
            }

            public void GOSTATE(int st)
            {
                pos_sost += 1;
                sostStack[pos_sost] = st;
                state = st;
            }

            public void SVERTKA(int num, string neterm)
            {
                versh += num + 1;
                stack[versh] = neterm;
                pos_sost += num;
                state = sostStack[pos_sost];
                if (Math.Abs(num) > 1)
                {
                    int index = versh + 1;
                    for (int i = index; i <= stack.Length + num; i++)
                        stack[i] = stack[i + Math.Abs(num) - 1];
                    Array.Resize(ref stack, stack.Length + num + 1);
                }

            }
            public void SVERTKADOP(string dop)
            {
                Array.Resize(ref stack, stack.Length + 1);
                Array.Copy(stack, versh + 1, stack, versh + 2, stack.Length - 2 - versh);
                stack[versh + 1] = dop;
                Array.Resize(ref vhodStr, vhodStr.Length + 1);
                Array.Copy(vhodStr, versh + 1, vhodStr, versh + 2, vhodStr.Length - 2 - versh);
                vhodStr[versh + 1] = dop;
                pos_vhod -= 1;
            }


            public bool SyntaxAnalys()
            {
                bool result = true;
                string str;

                while (iserror == true)
                {
                    switch (state)
                    {
                        case 0:
                            if (stack[versh] == "<программа>")
                            {
                                iserror = false;
                                break;
                            }
                            else if (stack[versh] == " ") SDVIG();
                            else if (stack[versh] == "main") GOSTATE(1);
                            else { MessageBox.Show("Ожидался: main"); iserror = false; result = false; }
                            break;
                        case 1:
                            if (stack[versh] == "main") SDVIG();
                            else if (stack[versh] == "(") GOSTATE(2);
                            else { MessageBox.Show("Ожидался: ("); iserror = false; result = false; }
                            break;
                        case 2:
                            if (stack[versh] == "(") SDVIG();
                            else if (stack[versh] == ")") GOSTATE(3);
                            else { MessageBox.Show("Ожидался: )"); iserror = false; result = false; }
                            break;
                        case 3:
                            if (stack[versh] == ")") SDVIG();
                            else if (stack[versh] == "{") GOSTATE(4);
                            else { MessageBox.Show("Ожидался: {"); iserror = false; result = false; }
                            break;
                        case 4:
                            if (stack[versh] == "{") SDVIG();
                            else if (stack[versh] == "<спис_опер>") GOSTATE(5);
                            else if (stack[versh] == "<опер>") GOSTATE(6);
                            else if (stack[versh] == "<условн.>") GOSTATE(7);
                            else if (stack[versh] == "<присв.>") GOSTATE(8);
                            else if (stack[versh] == "while") GOSTATE(47);
                            else if (stack[versh] == "id") GOSTATE(31);
                            else if (stack[versh] == "<тип>") GOSTATE(11);
                            else if (stack[versh] == "int") GOSTATE(13);
                            else if (stack[versh] == "char") GOSTATE(14);
                            else if (stack[versh] == "double") GOSTATE(15);
                            else if (stack[versh] == "string") GOSTATE(16);
                            else
                            {
                                MessageBox.Show("Ожидался: <спис_опер>,<опер>,<условн.>,<присв.>,while,id,<тип>,int,char,double,string");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 5:
                            if (stack[versh] == "<спис_опер>") SDVIG();
                            else if (stack[versh] == "}") GOSTATE(71);
                            else if (stack[versh] == ";") GOSTATE(70);
                            else
                            {
                                MessageBox.Show("Ожидался: ;");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 70:
                            if (stack[versh] == ";") SDVIG();
                            else if (stack[versh] == "}") GOSTATE(73);
                            else if (stack[versh] == "<опер>") GOSTATE(72);
                            else if (stack[versh] == "<условн.>") GOSTATE(7);
                            else if (stack[versh] == "<присв.>") GOSTATE(8);
                            else if (stack[versh] == "while") GOSTATE(47);
                            else if (stack[versh] == "id") GOSTATE(31);
                            else if (stack[versh] == "<тип>") GOSTATE(11);
                            else if (stack[versh] == "int") GOSTATE(13);
                            else if (stack[versh] == "char") GOSTATE(14);
                            else if (stack[versh] == "double") GOSTATE(15);
                            else if (stack[versh] == "string") GOSTATE(16);
                            else
                            {
                                MessageBox.Show("Ожидался: },<опер>,<условн.>,<присв.>,while,id,<тип>,int,char,double,string");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 72:
                            SVERTKA(-3, "<спис_опер>");
                            break;
                        case 73:
                            SVERTKA(-7, "<программа>");
                            break;
                        case 71:
                            SVERTKA(-6, "<программа>");
                            break;
                        case 6:
                            SVERTKA(-1, "<спис_опер>");
                            break;
                        case 7:
                            SVERTKA(-1, "<опер>");
                            break;
                        case 8:
                            SVERTKA(-1, "<опер>");
                            break;
                        case 13:
                            SVERTKA(-1, "<тип>");
                            break;
                        case 14:
                            SVERTKA(-1, "<тип>");
                            break;
                        case 15:
                            SVERTKA(-1, "<тип>");
                            break;
                        case 16:
                            SVERTKA(-1, "<тип>");
                            break;
                        case 11:
                            if (stack[versh] == "<тип>") SDVIG();
                            else if (stack[versh] == "<спис_перем>") GOSTATE(17);
                            else if (stack[versh] == "id") GOSTATE(45);
                            else
                            {
                                MessageBox.Show("Ожидался: <спис_перем>,id");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 47:
                            if (stack[versh] == "while") SDVIG();
                            else if (stack[versh] == "(") GOSTATE(48);
                            else
                            {
                                MessageBox.Show("Ожидался: (");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 48:
                            if (stack[versh] == "(") SDVIG();
                            else if (stack[versh] == "<услов.>") GOSTATE(9);
                            else if (stack[versh] == "<операнд>") GOSTATE(10);
                            else if (stack[versh] == "id") GOSTATE(81);
                            else if (stack[versh] == "lit") GOSTATE(52);
                            else
                            {
                                MessageBox.Show("Ожидался: <услов.>,<операнд>,id,lit");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 9:
                            if (stack[versh] == "<услов.>") SDVIG();
                            else if (stack[versh] == ")") GOSTATE(49);
                            else
                            {
                                MessageBox.Show("Ожидался: )");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 49:
                            if (stack[versh] == ")") SDVIG();
                            else if (stack[versh] == "<блок опер.>") GOSTATE(51);
                            else if (stack[versh] == "<опер>") GOSTATE(61);
                            else if (stack[versh] == "{") GOSTATE(53);
                            else
                            {
                                MessageBox.Show("Ожидался: <блок опер.>,<опер>,{");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 53:
                            if (stack[versh] == "{") SDVIG();
                            else if (stack[versh] == "<спис_опер>") GOSTATE(54);
                            else if (stack[versh] == "<опер>") GOSTATE(6);
                            else if (stack[versh] == "<условн.>") GOSTATE(7);
                            else if (stack[versh] == "<присв.>") GOSTATE(8);
                            else if (stack[versh] == "while") GOSTATE(47);
                            else if (stack[versh] == "id") GOSTATE(31);
                            else if (stack[versh] == "<тип>") GOSTATE(11);
                            else if (stack[versh] == "int") GOSTATE(13);
                            else if (stack[versh] == "char") GOSTATE(14);
                            else if (stack[versh] == "double") GOSTATE(15);
                            else if (stack[versh] == "string") GOSTATE(16);
                            break;
                        case 54:
                            if (stack[versh] == "<спис_опер>") SDVIG();
                            else if (stack[versh] == ";") GOSTATE(55);
                            else
                            {
                                MessageBox.Show("Ожидался: ;");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 51:
                            SVERTKA(-5, "<условн.>");
                            break;
                        case 61:
                            SVERTKA(-1, "<блок опер.>");
                            break;
                        case 55:
                            if (stack[versh] == ";") SDVIG();
                            else if (stack[versh] == "}") GOSTATE(56);
                            else
                            {
                                MessageBox.Show("Ожидался: }");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 56:
                            SVERTKA(-4, "<блок опер.>");
                            break;
                        case 10:
                            if ((stack[versh] == "<операнд>") & (stack[versh + 1] == ")")) SVERTKA(-1, "<услов.>");
                            else { 
                                if (stack[versh] == "<операнд>") SDVIG();
                                else if (stack[versh] == "<знак>") GOSTATE(33);
                                else if (stack[versh] == ">") GOSTATE(34);
                                else if (stack[versh] == "<") GOSTATE(35);
                                else if (stack[versh] == "==") GOSTATE(36);
                                else if (stack[versh] == "!=") GOSTATE(37);
                                else if (stack[versh] == "&") GOSTATE(38);
                                else if (stack[versh] == "|") GOSTATE(39);
                                else { MessageBox.Show("Ожидался: ), <знак>, >,<,==,!=,&,|"); iserror = false; result = false; }

                            }
                            break;
                        case 33:
                            if (stack[versh] == "<знак>") SDVIG();
                            else if (stack[versh] == "<операнд>") GOSTATE(27);
                            else if (stack[versh] == "id") GOSTATE(81);
                            else if (stack[versh] == "lit") GOSTATE(52);
                            else
                            {
                                MessageBox.Show("Ожидался: <операнд>,id,lit");
                                iserror = false;
                                result = false;
                            }
                            break;
                        case 27:
                            SVERTKA(-3, "<услов.>");
                            break;
                        case 34:
                            SVERTKA(-1, "<знак>");
                            break;
                        case 35:
                            SVERTKA(-1, "<знак>");
                            break;
                        case 36:
                            SVERTKA(-1, "<знак>");
                            break;
                        case 37:
                            SVERTKA(-1, "<знак>");
                            break;
                        case 38:
                            SVERTKA(-1, "<знак>");
                            break;
                        case 39:
                            SVERTKA(-1, "<знак>");
                            break;
                        case 31:
                            if (stack[versh] == "id") SDVIG();
                            else if (stack[versh] == "=") GOSTATE(89);
                            else { MessageBox.Show("Ожидался: ="); iserror = false; result = false; }

                            break;
                        case 81:
                            SVERTKA(-1, "<операнд>");
                            break;
                        case 52:
                            SVERTKA(-1, "<операнд>");
                            break;
                        case 45:
                            if (stack[versh] == "id") SDVIG();
                            else if (stack[versh] == ",") GOSTATE(32);
                            else if (stack[versh] == "=") GOSTATE(25);
                            else if (stack[versh] == ";")
                            {
                                str = stack[versh];
                                SVERTKA(-2, "<спис_перем>");
                                SVERTKADOP(str);
                                GOSTATE(11);
                            }
                            else { MessageBox.Show("Ожидался: ;"); iserror = false; result = false; }

                            break;
                        case 89:
                            if (stack[versh] == "=") SDVIG();
                            else if (stack[versh] == "expr") GOSTATE(20);
                            else { MessageBox.Show("Ожидался: expr"); iserror = false; result = false; }
                            break;
                        case 25:
                            if (stack[versh] == "=") SDVIG();
                            else if (stack[versh] == "expr") GOSTATE(24);
                            else { MessageBox.Show("Ожидался: expr"); iserror = false; result = false; }
                            break;
                        case 20:
                            SVERTKA(-3, "<присв.>");
                            break;
                        case 24:
                            SVERTKA(-4, "<присв.>");
                            break;
                        case 32:
                            if (stack[versh] == ",") SDVIG();
                            else if (stack[versh] == "id") GOSTATE(19);
                            else { MessageBox.Show("Ожидался: <спис_перем>"); iserror = false; result = false; }
                            break;
                        case 17:
                            SVERTKA(-2, "<присв.>");
                            break;
                        case 19:
                            SVERTKA(-3, "<спис_перем>");
                            break;
                    }
                }
                return result;
            }



            public void LeksAnalys(Simvols simvols)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    //литерал

                    if (Char.IsDigit(simvols.text[i]) == true)
                    {
                        if (i > 0)
                        {


                            if (Char.IsDigit(simvols.text[i - 1]))
                            {

                                simvols.simv += simvols.text[i].ToString();
                                if (i == text.Length - 1)
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "литерал", position = Position(simvols.simv) });
                            }
                            else
                            {
                                if (Char.IsLetter(simvols.text[i - 1]) == true)
                                {
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "идентификатор", position = Position(simvols.simv) });
                                    simvols.simv = simvols.text[i].ToString();
                                }
                                else if (simvols.SIMVOLMASRAZ.Contains(simvols.text[i - 1].ToString()))
                                {
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "разделитель", position = Position(simvols.simv) });
                                    simvols.simv = simvols.text[i].ToString();
                                }
                                else
                                {
                                    simvols.simv = simvols.text[i].ToString();
                                }

                                if (i == text.Length - 1)
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "литерал", position = Position(simvols.simv) });
                            }
                        }

                        else simvols.simv = simvols.text[i].ToString();


                    }


                    //идентификатор
                    else if ((Char.IsLetter(simvols.text[i]) == true))
                    {
                        if (i > 0)
                        {
                            if (Char.IsLetter(simvols.text[i - 1]))
                            {

                                simvols.simv += simvols.text[i].ToString();
                                if (i == text.Length - 1)
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "идентификатор", position = Position(simvols.simv) });
                            }
                            else
                            {
                                if (Char.IsDigit(simvols.text[i - 1]) == true)
                                {
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "литерал", position = Position(simvols.simv) });
                                    simvols.simv = simvols.text[i].ToString();
                                }
                                else if (simvols.SIMVOLMASRAZ.Contains(simvols.text[i - 1].ToString()))
                                {
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "разделитель", position = Position(simvols.simv) });
                                    simvols.simv = simvols.text[i].ToString();
                                }
                                else
                                {
                                    simvols.simv = simvols.text[i].ToString();
                                }
                                if (i == text.Length - 1)
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "идентификатор", position = Position(simvols.simv) });


                            }
                        }
                        else simvols.simv = simvols.text[i].ToString();
                    }

                    // разделители
                    else if (simvols.SIMVOLMASRAZ.Contains(simvols.text[i].ToString()))
                    {
                        if (i > 0)
                        {
                            if (simvols.SIMVOLMASRAZ.Contains(simvols.text[i - 1].ToString()))
                            {
                                simvols.simv += simvols.text[i].ToString();
                                if (i == text.Length - 1)
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "разделитель", position = Position(simvols.simv) });
                            }

                            else
                            {
                                if (Char.IsLetter(simvols.text[i - 1]) == true)
                                {
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "идентификатор", position = Position(simvols.simv) });
                                    simvols.simv = simvols.text[i].ToString();

                                }
                                else if (Char.IsDigit(simvols.text[i - 1]) == true)
                                {
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "литерал", position = Position(simvols.simv) });
                                    simvols.simv = simvols.text[i].ToString();
                                }
                                else
                                {
                                    simvols.simv = simvols.text[i].ToString();
                                }

                                if (i == text.Length - 1)
                                    list.Add(new MyDataType() { leksema = simvols.simv, type = "разделитель", position = Position(simvols.simv) });

                            }
                        }


                    }

                    //пробел
                    else
                    {
                        if (Char.IsLetter(simvols.text[i - 1]) == true)
                        {
                            list.Add(new MyDataType() { leksema = simvols.simv, type = "идентификатор", position = Position(simvols.simv) });

                        }
                        else if (Char.IsDigit(simvols.text[i - 1]) == true)
                        {
                            list.Add(new MyDataType() { leksema = simvols.simv, type = "литерал", position = Position(simvols.simv) });

                        }

                        else if (simvols.SIMVOLMASRAZ.Contains(simvols.text[i - 1].ToString()))
                        {
                            list.Add(new MyDataType() { leksema = simvols.simv, type = "разделитель", position = Position(simvols.simv) });

                        }
                        else
                        {
                            simvols.simv = null;
                        }

                    }
                }
            }

            public string Position(string str)
            {
                int index = 0;
                int type = 0;
                if (SIMVOLMASRAZ.Contains(str))
                {
                    index = Array.IndexOf(SIMVOLMASRAZ, str) + 1;
                    type = 2;
                }
                else if (SIMVOLMASL.Contains(str))
                {
                    index = Array.IndexOf(SIMVOLMASL, str) + 1;
                    type = 1;

                }
                else if (int.TryParse(str, out int n))
                {
                    if (SIMVOLMASLIT.Length != 0)
                    {
                        if (SIMVOLMASLIT.Contains(n))
                            index = Array.IndexOf(SIMVOLMASLIT, n) + 1;
                        else
                        {
                            Array.Resize(ref SIMVOLMASLIT, SIMVOLMASLIT.Length + 1);
                            SIMVOLMASLIT[SIMVOLMASLIT.Length - 1] = n;
                            index = SIMVOLMASLIT.Length;
                        }
                    }
                    else
                    {
                        Array.Resize(ref SIMVOLMASLIT, 1);
                        SIMVOLMASLIT[0] = n;
                        index = 1;
                    }
                    type = 3;
                }
                else
                {
                    if (SIMVOLMASID.Count != 0)
                    {
                        if (SIMVOLMASID.Contains(str))
                            index = SIMVOLMASID.IndexOf(str) + 1;
                        else
                        {

                            SIMVOLMASID.Add(str);

                            index = SIMVOLMASID.Count;
                        }
                    }
                    else
                    {

                        SIMVOLMASID.Add(str);
                        index = 1;
                    }
                    type = 4;
                }

                string result = "(" + type.ToString() + "," + index.ToString() + ")";

                return result;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            Simvols simvols = new Simvols();
            char[] characters = textBox1.Text.ToCharArray();
            simvols.text = characters;
            simvols.LeksAnalys(simvols);
            dataGridView1.DataSource = simvols.list;
            dataGridView2.Columns.Clear();
            dataGridView3.Columns.Clear();
            dataGridView4.Columns.Clear();
            dataGridView5.Columns.Clear();

            dataGridView2.Columns.Add("Index", "Index");
            dataGridView3.Columns.Add("Index", "Index");
            dataGridView4.Columns.Add("Index", "Index");
            dataGridView5.Columns.Add("Index", "Index");

            dataGridView2.Columns.Add("Value", "Служебные символы");
            dataGridView3.Columns.Add("Value", "Разделители");
            dataGridView4.Columns.Add("Value", "Литералы");
            dataGridView5.Columns.Add("Value", "Идентификаторы");
            for (int i = 0; i < simvols.SIMVOLMASL.Length; i++)
            {
                dataGridView2.Rows.Add(new object[] { i + 1, simvols.SIMVOLMASL[i] });
            }
            for (int i = 0; i < simvols.SIMVOLMASRAZ.Length; i++)
            {
                dataGridView3.Rows.Add(new object[] { i + 1, simvols.SIMVOLMASRAZ[i] });
            }
            for (int i = 0; i < simvols.SIMVOLMASID.Count; i++)
            {
                if (simvols.SIMVOLMASID[i].ToString().Length > 8) { textBox2.Text = "Ошибка"; MessageBox.Show("Длина идентификатора больше 8 символов"); }

                dataGridView5.Rows.Add(new object[] { i + 1, simvols.SIMVOLMASID[i] });
            }
            for (int i = 0; i < simvols.SIMVOLMASLIT.Length; i++)
            {
                dataGridView4.Rows.Add(new object[] { i + 1, simvols.SIMVOLMASLIT[i] });
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                string str = File.ReadAllText(fileName);
                textBox1.Clear();
                textBox2.Clear();
                textBox1.Text = str;
            }
        }




        private void button3_Click(object sender, EventArgs e)
        {
            if ((textBox2.Text != "Ошибка")&&(dataGridView1.Rows.Count!=0))
            {
                int s;
                Simvols simvols = new Simvols();
                string[] column0Array = new string[dataGridView1.Rows.Count];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    column0Array[i] = dataGridView1[0, i].Value.ToString();
                for (int i = 0; i < column0Array.Length; i++)
                {
                    if (i > 1)
                    {
                        if ((int.TryParse(column0Array[i], out s)) == true)
                        {
                            if (column0Array[i - 1] == "=") column0Array[i] = "expr";
                            else column0Array[i] = "lit";
                        }
                        else if ((simvols.SIMVOLMASL.Contains(column0Array[i]) || simvols.SIMVOLMASRAZ.Contains(column0Array[i])) == false)
                        {
                            if ((column0Array[i - 1] == "=") & (((column0Array[i + 1] != "+") || (column0Array[i + 1] != "-")
                                || (column0Array[i + 1] != "/") || (column0Array[i + 1] != "*")) == true))
                                column0Array[i] = "expr";
                            else column0Array[i] = "id";

                        }
                    }

                }
                for (int i = 0; i < column0Array.Length - 1; i++)
                {
                    if (i > 1)
                    {
                        if (((column0Array[i] == "+") || (column0Array[i] == "-") || (column0Array[i] == "/") || (column0Array[i] == "*"))
                            & ((column0Array[i - 1] == "expr") || (column0Array[i - 1] == "id") || (column0Array[i - 1] == "lit"))
                            & ((column0Array[i + 1] == "id") || (column0Array[i + 1] == "lit")))
                        {
                            Array.Copy(column0Array, i + 2, column0Array, i, column0Array.Length - 2 - i);
                            column0Array[i - 1] = "expr";
                            Array.Resize(ref column0Array, column0Array.Length - 2);
                            i--;
                        }
                    }
                }

                simvols.stack = column0Array;
                simvols.vhodStr = column0Array;
                if (simvols.SyntaxAnalys() == true)
                    textBox2.Text = "Успех";
                else textBox2.Text = "Провал";
            }
            else MessageBox.Show("Синтаксический анализ не может быть запущен");
        }
    }
}
